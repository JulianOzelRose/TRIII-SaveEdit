# TRIII-SaveEdit
This is a save game editor for Tomb Raider III. It has been tested to work with the Steam version of the game, but it should work on
the original and multi-patched version as well. Be sure to back up your save game files anyway as a precaution.
This editor can enable any weapon on any level, including the bonus level. No setup is necessary, just download and run.

## Installation and usage
1. Download **TRIII-SaveEdit.exe** from the **TRIII-SaveEdit** folder in this repository.
2. Launch the editor and select "Browse" to locate your game directory.
3. If you're using the Steam version of the game, your directory should be:<br>
```\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\```<br>
4. Once your save game is selected, make the desired modifications.
5. Click "Save" to apply your changes, and enjoy.

#### Screenshot of TRIII-SaveEdit
![TRIII-SaveEdit](https://github.com/JulianOzelRose/TRIII-SaveEdit/assets/95890436/9cc84f2c-299e-4265-8aa0-f30bd6beaf54)

## How weapons information is stored
Unlike Tomb Raider: Chronicles, the save file offsets in Tomb Raider III are stored differently in each level. Another interesting difference is that
instead of storing weapons on individual offsets, all weapons information is stored on a single offset, which I call ```weaponsConfigNum```.
The only exception is the harpoon gun, which is stored on its own offset; 1 for enabled, 0 for disabled. The weapons configuration variable has a
base number of 1, which indicates no weapons present in inventory. Each weapon adds a unique number to the variable.

###       ```weaponsConfigNum```             ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistol                  | 2                 |
| Deagle                  | 4                 |
| Uzi                     | 8                 |
| Shotgun                 | 16                |
| MP5                     | 32                |
| Rocket Launcher         | 64                |
| Grenade Launcher        | 128               |

When the game loads a save file, it checks this number to determine which weapons are present in inventory. When reverse
engineering this part of the game, we need to choose our methods wisely. Since there are 128 possible combinations, using conditional
operations would be extremely inefficient. Bitwise operations are perfect for this scenario. We account for the base case of no weapons,
and then we use our else block to check if the unique bit of a weapon is present in the config variable, and then set our values to display accordingly.

```
const int Pistol = 2;
const int Deagle = 4;
const int Uzi = 8;
const int Shotgun = 16;
const int MP5 = 32;
const int RocketLauncher = 64;
const int GrenadeLauncher = 128;

if (weaponsConfigNum == 1)
{
    pistolsCheckBox.Checked = false;
    shotgunCheckBox.Checked = false;
    deagleCheckBox.Checked = false;
    uziCheckBox.Checked = false;
    mp5CheckBox.Checked = false;
    rocketLauncherCheckBox.Checked = false;
    grenadeLauncherCheckBox.Checked = false;
}
else
{
    pistolsCheckBox.Checked = (weaponsConfigNum & Pistol) != 0;
    shotgunCheckBox.Checked = (weaponsConfigNum & Shotgun) != 0;
    deagleCheckBox.Checked = (weaponsConfigNum & Deagle) != 0;
    uziCheckBox.Checked = (weaponsConfigNum & Uzi) != 0;
    mp5CheckBox.Checked = (weaponsConfigNum & MP5) != 0;
    rocketLauncherCheckBox.Checked = (weaponsConfigNum & RocketLauncher) != 0;
    grenadeLauncherCheckBox.Checked = (weaponsConfigNum & GrenadeLauncher) != 0;
}
```

## How ammunition info is stored
Ammunition is stored on two different offsets. It is always stored on a lower offset, which I call the base ammo offset, and then it is
stored on an additional offset, which I call the secondary ammo offset. The secondary offsets vary throughout the levels. Some levels have as little as 1
secondary offset, while others have up to 7 secondary ammo offsets. The "correct" secondary ammo offset changes as you progress through a level.
Writing to incorrect or multiple secondary offsets typically results in the game crashing upon loading. I was not able to discern any useful large-scale
patterns from the ammo offsets at first, so this editor takes a somewhat brute-force approach to handling ammunition.
To determine which secondary offset is the correct one to write to, we loop through them and check for equivalency with the base offset.

```
int[] GetValidAmmoOffsets(int baseOffset, params int[] offsets)
{
    List<int> validOffsets = new List<int>();
    int baseAmmoValue = GetAmmoValue(baseOffset);

    for (int i = 0; i < offsets.Length; i++)
    {
        int ammoValue = GetAmmoValue(offsets[i]);

        if (baseAmmoValue == ammoValue && offsets[i] != 0)
        {
            validOffsets.Add(offsets[i]);
        }
    }

    if (validOffsets.Count == 0)
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            if (offsets[i] != 0 && GetSaveFileData(offsets[i] - 1) == 0 && GetSaveFileData(offsets[i] + 1) == 0)
            {
                validOffsets.Add(offsets[i]);
            }
        }
    }

    validOffsets.Add(baseOffset);
    return validOffsets.ToArray();
}
```

If no secondary offset matches the base offset, we take a heuristic approach. We loop through the secondary offsets,
and we check the surrounding data. If the next offset over is non-zero, we know we cannot write to it. If the preceding
offset is written to, the game will crash. I have not figured out a method that determines the secondary offset
with 100% accuracy when a non-zero equivalent cannot be found, but this comes quite close. There are only 3 levels that
require this heuristic. Here is the heuristic code block, which is nested in the previous function.

```
if (validOffsets.Count == 0)
{
    for (int i = 0; i < offsets.Length; i++)
    {
        if (offsets[i] != 0 && GetSaveFileData(offsets[i] - 1) == 0 && GetSaveFileData(offsets[i] + 1) == 0)
        {
            validOffsets.Add(offsets[i]);
        }
    }
}
```

## Calculating the secondary ammo offsets algorithmically
It is worth noting that it would probably be more efficient to simply find a base secondary ammo offset (not to
be confused with the base ammo offset) and calculate the remaining secondary offsets using a for loop. This is
very doable, since the secondary ammo offsets seem to have a consistent difference of 0x12. So you use 0x12 as an iterator.

```
int baseSecondaryAmmoOffset = 0x210D;
int maxIterations = 10;

List<int> secondaryOffsets = new List<int>();

for (int i = 0; i < maxIterations; i++)
{
    secondaryOffsets.Add(baseSecondaryAmmoOffset + i * 0x12);
}
```

We plug in shotgun base secondary ammo 0x210D for from the Area 51 level, and we get the same offsets as we did from our brute force method, plus more.
In conclusion, go with the more efficient method to save yourself some headache.

```
0x210D
0x211F
0x2131
0x2143
0x2155
0x2167
0x2179
0x218B
0x219D
0x21AF
```


## Offset tables
Aside from the level name and save number, the save file offsets differ on every level. The offsets for small medipack
and large medipack are 1 byte away. Flares is 2 bytes away from large medipack, and the weapons config number is 4 bytes away
from flares. Next to the weapons config number is the harpoon gun offset. The first ammo offset listed on each
table is the base ammo offset, and the subsequent ones are the secondary ammo offsets.

#### Jungle ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x00E6            |
| Large Medipack          | 0x00E7            |
| Flares                  | 0x00E9            |
| Weapons Config Number   | 0x00ED            |
| Harpoon Gun             | 0x00EE            |
| Shotgun Ammo 1          | 0x00DC            |
| Shotgun Ammo 2          | 0x165B            |
| Shotgun Ammo 3          | 0x166F            |
| Shotgun Ammo 4          | 0x16A5            |
| Deagle Ammo 1           | 0x00D8            |
| Deagle Ammo 2	          | 0x1655            |
| Deagle Ammo 3	          | 0x1667            |
| Deagle Ammo 4	          | 0x169D            |
| Grenade Launcher Ammo 1 | 0x00E4            |
| Grenade Launcher Ammo 2 | 0x1669            |
| Grenade Launcher Ammo 3 | 0x167B            |
| Grenade Launcher Ammo 4 | 0x169D            |
| Rocket Launcher Ammo 1  | 0x00E0            |
| Rocket Launcher Ammo 2  | 0x1665            |
| Rocket Launcher Ammo 3  | 0x1677            |
| Rocket Launcher Ammo 4  | 0x16AD            |
| Harpoon Ammo 1          | 0x00E2            |
| Harpoon Ammo 2          | 0x1661            |
| Harpoon Ammo 3          | 0x1673            |
| Harpoon Ammo 4          | 0x16A9            |
| MP5 Ammo 1              | 0x00DE            |
| MP5 Ammo 2              | 0x166D            |
| MP5 Ammo 3              | 0x167F            |
| MP5 Ammo 4              | 0x16B5            |
| Uzi Ammo 1              | 0x00DA            |
| Uzi Ammo 2              | 0x1659            |
| Uzi Ammo 3              | 0x166B            |
| Uzi Ammo 4              | 0x16A1            |

#### Temple Ruins ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x0119            |
| Large Medipack          | 0x011A            |
| Flares                  | 0x011C            |
| Weapons Config Number   | 0x0120            |
| Harpoon Gun             | 0x0121            |
| Shotgun Ammo 1          | 0x010F            |
| Shotgun Ammo 2          | 0x23BB            |
| Shotgun Ammo 3          | 0x23CD            |
| Deagle Ammo 1           | 0x010B            |
| Deagle Ammo 2	          | 0x23B3            |
| Deagle Ammo 3	          | 0x23C5            |
| Grenade Launcher Ammo 1 | 0x0117            |
| Grenade Launcher Ammo 2 | 0x23C7            |
| Grenade Launcher Ammo 3 | 0x23D9            |
| Rocket Launcher Ammo 1  | 0x0113            |
| Rocket Launcher Ammo 2  | 0x23C3            |
| Rocket Launcher Ammo 3  | 0x23D5            |
| Harpoon Ammo 1          | 0x0115            |
| Harpoon Ammo 2          | 0x23BF            |
| Harpoon Ammo 3          | 0x23D1            |
| MP5 Ammo 1              | 0x0111            |
| MP5 Ammo 2              | 0x23CB            |
| MP5 Ammo 3              | 0x23DD            |
| Uzi Ammo 1              | 0x010D            |
| Uzi Ammo 2              | 0x23B7            |
| Uzi Ammo 3              | 0x23C9            |

#### The River Ganges ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x014C            |
| Large Medipack          | 0x014D            |
| Flares                  | 0x014F            |
| Weapons Config Number   | 0x0153            |
| Harpoon Gun             | 0x0154            |
| Shotgun Ammo 1          | 0x0142            |
| Shotgun Ammo 2          | 0x1804            |
| Shotgun Ammo 3          | 0x1816            |
| Deagle Ammo 1           | 0x013E            |
| Deagle Ammo 2	          | 0x17FC            |
| Deagle Ammo 3	          | 0x180E            |
| Grenade Launcher Ammo 1 | 0x014A            |
| Grenade Launcher Ammo 2 | 0x1810            |
| Grenade Launcher Ammo 3 | 0x1822            |
| Rocket Launcher Ammo 1  | 0x0146            |
| Rocket Launcher Ammo 2  | 0x180C            |
| Rocket Launcher Ammo 3  | 0x181E            |
| Harpoon Ammo 1          | 0x0148            |
| Harpoon Ammo 2          | 0x1814            |
| Harpoon Ammo 3          | 0x1826            |
| MP5 Ammo 1              | 0x0144            |
| MP5 Ammo 2              | 0x1814            |
| MP5 Ammo 3              | 0x1826            |
| Uzi Ammo 1              | 0x0140            |
| Uzi Ammo 2              | 0x1800            |
| Uzi Ammo 3              | 0x1812            |

#### Caves of Kaliya ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x017F            |
| Large Medipack          | 0x0180            |
| Flares                  | 0x0182            |
| Weapons Config Number   | 0x0186            |
| Harpoon Gun             | 0x0187            |
| Shotgun Ammo 1          | 0x0175            |
| Shotgun Ammo 2          | 0x0D1F            |
| Deagle Ammo 1           | 0x0171            |
| Deagle Ammo 2	          | 0x0D17            |
| Grenade Launcher Ammo 1 | 0x017D            |
| Grenade Launcher Ammo 2 | 0x0D2B            |
| Rocket Launcher Ammo 1  | 0x0179            |
| Rocket Launcher Ammo 2  | 0x0D27            |
| Harpoon Ammo 1          | 0x017B            |
| Harpoon Ammo 2          | 0x0D23            |
| MP5 Ammo 1              | 0x0177            |
| MP5 Ammo 2              | 0x0D2F            |
| Uzi Ammo 1              | 0x0173            |
| Uzi Ammo 2              | 0x0D1B            |

#### Nevada Desert ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x034A            |
| Large Medipack          | 0x034B            |
| Flares                  | 0x034A            |
| Weapons Config Number   | 0x0351            |
| Harpoon Gun             | 0x0352            |
| Shotgun Ammo 1          | 0x0340            |
| Shotgun Ammo 2          | 0x17A4            |
| Shotgun Ammo 3          | 0x17B6            |
| Shotgun Ammo 4          | 0x17C8            |
| Shotgun Ammo 5          | 0x17DA            |
| Deagle Ammo 1           | 0x033C            |
| Deagle Ammo 2	          | 0x179C            |
| Deagle Ammo 3           | 0x17AC            |
| Deagle Ammo 4	          | 0x17C0            |
| Deagle Ammo 5	          | 0x17D2            |
| Grenade Launcher Ammo 1 | 0x0348            |
| Grenade Launcher Ammo 2 | 0x17B0            |
| Grenade Launcher Ammo 3 | 0x17C2            |
| Grenade Launcher Ammo 4 | 0x17D4            |
| Grenade Launcher Ammo 5 | 0x17E6            |
| Rocket Launcher Ammo 1  | 0x0344            |
| Rocket Launcher Ammo 2  | 0x17AC            |
| Rocket Launcher Ammo 3  | 0x17BE            |
| Rocket Launcher Ammo 4  | 0x17D0            |
| Rocket Launcher Ammo 5  | 0x17E2            |
| Harpoon Ammo 1          | 0x0346            |
| Harpoon Ammo 2          | 0x17A8            |
| Harpoon Ammo 3          | 0x17BA            |
| Harpoon Ammo 4          | 0x17CC            |
| Harpoon Ammo 5          | 0x17DE            |
| MP5 Ammo 1              | 0x0342            |
| MP5 Ammo 2              | 0x17B4            |
| MP5 Ammo 3              | 0x17C6            |
| MP5 Ammo 4              | 0x17D8            |
| MP5 Ammo 5              | 0x17EA            |
| Uzi Ammo 1              | 0x033E            |
| Uzi Ammo 2              | 0x17A0            |
| Uzi Ammo 3              | 0x17B2            |
| Uzi Ammo 4              | 0x17C4            |
| Uzi Ammo 5              | 0x17D6            |

#### High Security Compound ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x037D            |
| Large Medipack          | 0x037E            |
| Flares                  | 0x0380            |
| Weapons Config Number   | 0x0384            |
| Harpoon Gun             | 0x0385            |
| Shotgun Ammo 1          | 0x0373            |
| Shotgun Ammo 2          | 0x1EA5            |
| Shotgun Ammo 3          | 0x1E4B            |
| Shotgun Ammo 4          | 0x1EB7            |
| Deagle Ammo 1           | 0x036F            |
| Deagle Ammo 2	          | 0x1E43            |
| Deagle Ammo 3	          | 0x1E9D            |
| Deagle Ammo 4	          | 0x1EAF            |
| Grenade Launcher Ammo 1 | 0x037B            |
| Grenade Launcher Ammo 2 | 0x1E57            |
| Grenade Launcher Ammo 3 | 0x1EB1            |
| Grenade Launcher Ammo 4 | 0x1EC3            |
| Rocket Launcher Ammo 1  | 0x0377            |
| Rocket Launcher Ammo 2  | 0x1E53            |
| Rocket Launcher Ammo 3  | 0x1EAD            |
| Rocket Launcher Ammo 4  | 0x1EBF            |
| Harpoon Ammo 1          | 0x0379            |
| Harpoon Ammo 2          | 0x1E4F            |
| Harpoon Ammo 3          | 0x1EA9            |
| Harpoon Ammo 4          | 0x1EBB            |
| MP5 Ammo 1              | 0x0375            |
| MP5 Ammo 2              | 0x1E5B            |
| MP5 Ammo 3              | 0x1EB5            |
| MP5 Ammo 4              | 0x1EC7            |
| Uzi Ammo 1              | 0x0371            |
| Uzi Ammo 2              | 0x1E47            |
| Uzi Ammo 3              | 0x1EA1            |
| Uzi Ammo 4              | 0x1EB3            |

#### Area 51 ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x03B0            |
| Large Medipack          | 0x03B1            |
| Flares                  | 0x03B3            |
| Weapons Config Number   | 0x03B7            |
| Harpoon Gun             | 0x03B8            |
| Shotgun Ammo 1          | 0x03A6            |
| Shotgun Ammo 2          | 0x210D            |
| Shotgun Ammo 3          | 0x211F            |
| Shotgun Ammo 4          | 0x2143            |
| Shotgun Ammo 5          | 0x2155            |
| Shotgun Ammo 6          | 0x2167            |
| Shotgun Ammo 7          | 0x218B            |
| Shotgun Ammo 8          | 0x21AF            |
| Deagle Ammo 1           | 0x03A2            |
| Deagle Ammo 2	          | 0x2105            |
| Deagle Ammo 3	          | 0x2117            |
| Deagle Ammo 4	          | 0x213B            |
| Deagle Ammo 5	          | 0x214D            |
| Deagle Ammo 6	          | 0x215F            |
| Deagle Ammo 7	          | 0x2183            |
| Deagle Ammo 8	          | 0x21A7            |
| Grenade Launcher Ammo 1 | 0x03AE            |
| Grenade Launcher Ammo 2 | 0x2119            |
| Grenade Launcher Ammo 3 | 0x212B            |
| Grenade Launcher Ammo 4 | 0x214F            |
| Grenade Launcher Ammo 5 | 0x2161            |
| Grenade Launcher Ammo 6 | 0x2173            |
| Grenade Launcher Ammo 7 | 0x2197            |
| Grenade Launcher Ammo 8 | 0x21BB            |
| Rocket Launcher Ammo 1  | 0x03AA            |
| Rocket Launcher Ammo 2  | 0x2115            |
| Rocket Launcher Ammo 3  | 0x2127            |
| Rocket Launcher Ammo 4  | 0x214B            |
| Rocket Launcher Ammo 5  | 0x215D            |
| Rocket Launcher Ammo 6  | 0x216F            |
| Rocket Launcher Ammo 7  | 0x2193            |
| Rocket Launcher Ammo 8  | 0x21B7            |
| Harpoon Ammo 1          | 0x03AC            |
| Harpoon Ammo 2          | 0x2111            |
| Harpoon Ammo 3          | 0x2123            |
| Harpoon Ammo 4          | 0x2147            |
| Harpoon Ammo 5          | 0x2159            |
| Harpoon Ammo 6          | 0x216B            |
| Harpoon Ammo 7          | 0x218F            |
| Harpoon Ammo 8          | 0x21B3            |
| MP5 Ammo 1              | 0x03A8            |
| MP5 Ammo 2              | 0x211D            |
| MP5 Ammo 3              | 0x212F            |
| MP5 Ammo 4              | 0x2153            |
| MP5 Ammo 5              | 0x2165            |
| MP5 Ammo 6              | 0x2177            |
| MP5 Ammo 7              | 0x219B            |
| MP5 Ammo 8              | 0x21BF            |
| Uzi Ammo 1              | 0x03A4            |
| Uzi Ammo 2              | 0x2109            |
| Uzi Ammo 3              | 0x211B            |
| Uzi Ammo 4              | 0x213F            |
| Uzi Ammo 5              | 0x2151            |
| Uzi Ammo 6              | 0x2163            |
| Uzi Ammo 7              | 0x2187            |
| Uzi Ammo 8              | 0x21AB            |

#### Coastal Village ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x01B2            |
| Large Medipack          | 0x01B3            |
| Flares                  | 0x01B5            |
| Weapons Config Number   | 0x01B9            |
| Harpoon Gun             | 0x01BA            |
| Shotgun Ammo 1          | 0x01A8            |
| Shotgun Ammo 2          | 0x17B1            |
| Shotgun Ammo 3          | 0x17D5            |
| Shotgun Ammo 4          | 0x17E7            |
| Deagle Ammo 1           | 0x01A4            |
| Deagle Ammo 2	          | 0x17A9            |
| Deagle Ammo 3	          | 0x17CD            |
| Deagle Ammo 4	          | 0x17DF            |
| Grenade Launcher Ammo 1 | 0x01B0            |
| Grenade Launcher Ammo 2 | 0x17BD            |
| Grenade Launcher Ammo 3 | 0x17E1            |
| Grenade Launcher Ammo 4 | 0x17F3            |
| Rocket Launcher Ammo 1  | 0x01AC            |
| Rocket Launcher Ammo 2  | 0x17B9            |
| Rocket Launcher Ammo 3  | 0x17DD            |
| Rocket Launcher Ammo 4  | 0x17EF            |
| Harpoon Ammo 1          | 0x01AE            |
| Harpoon Ammo 2          | 0x17B5            |
| Harpoon Ammo 3          | 0x17D9            |
| Harpoon Ammo 4          | 0x17EB            |
| MP5 Ammo 1              | 0x01AA            |
| MP5 Ammo 2              | 0x17C1            |
| MP5 Ammo 3              | 0x17E5            |
| MP5 Ammo 4              | 0x17F7            |
| Uzi Ammo 1              | 0x01A6            |
| Uzi Ammo 2              | 0x17AD            |
| Uzi Ammo 3              | 0x17D1            |
| Uzi Ammo 4              | 0x17E3            |

#### Crash Site ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x01E5            |
| Large Medipack          | 0x01E6            |
| Flares                  | 0x01E8            |
| Weapons Config Number   | 0x01EC            |
| Harpoon Gun             | 0x01ED            |
| Shotgun Ammo 1          | 0x01DB            |
| Shotgun Ammo 2          | 0x18D3            |
| Shotgun Ammo 3          | 0x18F7            |
| Shotgun Ammo 4          | 0x1909            |
| Deagle Ammo 1           | 0x01D7            |
| Deagle Ammo 2	          | 0x18CB            |
| Deagle Ammo 3	          | 0x18EF            |
| Deagle Ammo 4	          | 0x1901            |
| Grenade Launcher Ammo 1 | 0x01E3            |
| Grenade Launcher Ammo 2 | 0x18DF            |
| Grenade Launcher Ammo 3 | 0x1903            |
| Grenade Launcher Ammo 4 | 0x1915            |
| Rocket Launcher Ammo 1  | 0x01DF            |
| Rocket Launcher Ammo 2  | 0x18DB            |
| Rocket Launcher Ammo 3  | 0x18FF            |
| Rocket Launcher Ammo 4  | 0x1911            |
| Harpoon Ammo 1          | 0x01E1            |
| Harpoon Ammo 2          | 0x18D7            |
| Harpoon Ammo 3          | 0x18FB            |
| Harpoon Ammo 4          | 0x190D            |
| MP5 Ammo 1              | 0x01DD            |
| MP5 Ammo 2              | 0x18E3            |
| MP5 Ammo 3              | 0x1907            |
| MP5 Ammo 4              | 0x1919            |
| Uzi Ammo 1              | 0x01D9            |
| Uzi Ammo 2              | 0x18CF            |
| Uzi Ammo 3              | 0x18F3            |
| Uzi Ammo 4              | 0x1905            |

#### Madubu Gorge ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x0218            |
| Large Medipack          | 0x0219            |
| Flares                  | 0x021B            |
| Weapons Config Number   | 0x021F            |
| Harpoon Gun             | 0x0220            |
| Shotgun Ammo 1          | 0x020E            |
| Shotgun Ammo 2          | 0x141D            |
| Shotgun Ammo 3          | 0x1441            |
| Shotgun Ammo 4          | 0x1453            |
| Shotgun Ammo 5          | 0x1477            |
| Deagle Ammo 1           | 0x020A            |
| Deagle Ammo 2	          | 0x1415            |
| Deagle Ammo 3	          | 0x1439            |
| Deagle Ammo 4	          | 0x144B            |
| Deagle Ammo 5	          | 0x146F            |
| Grenade Launcher Ammo 1 | 0x0216            |
| Grenade Launcher Ammo 2 | 0x1429            |
| Grenade Launcher Ammo 3 | 0x144D            |
| Grenade Launcher Ammo 4 | 0x145F            |
| Grenade Launcher Ammo 5 | 0x1483            |
| Rocket Launcher Ammo 1  | 0x0212            |
| Rocket Launcher Ammo 2  | 0x1425            |
| Rocket Launcher Ammo 3  | 0x1449            |
| Rocket Launcher Ammo 4  | 0x145B            |
| Rocket Launcher Ammo 5  | 0x147F            |
| Harpoon Ammo 1          | 0x0214            |
| Harpoon Ammo 2          | 0x1421            |
| Harpoon Ammo 3          | 0x1445            |
| Harpoon Ammo 4          | 0x1457            |
| Harpoon Ammo 5          | 0x147B            |
| MP5 Ammo 1              | 0x0210            |
| MP5 Ammo 2              | 0x142D            |
| MP5 Ammo 3              | 0x1451            |
| MP5 Ammo 4              | 0x1463            |
| MP5 Ammo 5              | 0x1487            |
| Uzi Ammo 1              | 0x020C            |
| Uzi Ammo 2              | 0x1419            |
| Uzi Ammo 3              | 0x143D            |
| Uzi Ammo 4              | 0x144F            |
| Uzi Ammo 5              | 0x1473            |

#### Temple of Puna ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x024B            |
| Large Medipack          | 0x024C            |
| Flares                  | 0x024E            |
| Weapons Config Number   | 0x0252            |
| Harpoon Gun             | 0x0253            |
| Shotgun Ammo 1          | 0x0241            |
| Shotgun Ammo 2          | 0x10F5            |
| Shotgun Ammo 3          | 0x1107            |
| Deagle Ammo 1           | 0x023D            |
| Deagle Ammo 2	          | 0x10ED            |
| Deagle Ammo 3	          | 0x10FF            |
| Grenade Launcher Ammo 1 | 0x0249            |
| Grenade Launcher Ammo 2 | 0x1101            |
| Grenade Launcher Ammo 3 | 0x1113            |
| Rocket Launcher Ammo 1  | 0x0245            |
| Rocket Launcher Ammo 2  | 0x10FD            |
| Rocket Launcher Ammo 3  | 0x110F            |
| Harpoon Ammo 1          | 0x0247            |
| Harpoon Ammo 2          | 0x10F9            |
| Harpoon Ammo 3          | 0x110B            |
| MP5 Ammo 1              | 0x0243            |
| MP5 Ammo 2              | 0x1105            |
| MP5 Ammo 3              | 0x1117            |
| Uzi Ammo 1              | 0x023F            |
| Uzi Ammo 2              | 0x10F1            |
| Uzi Ammo 3              | 0x1103            |

#### Thames Wharf ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x027E            |
| Large Medipack          | 0x027F            |
| Flares                  | 0x0281            |
| Weapons Config Number   | 0x0285            |
| Harpoon Gun             | 0x0286            |
| Shotgun Ammo 1          | 0x0274            |
| Shotgun Ammo 2          | 0x1873            |
| Shotgun Ammo 3          | 0x1885            |
| Shotgun Ammo 4          | 0x1897            |
| Deagle Ammo 1           | 0x0270            |
| Deagle Ammo 2	          | 0x186B            |
| Deagle Ammo 3	          | 0x187D            |
| Deagle Ammo 4	          | 0x188F            |
| Grenade Launcher Ammo 1 | 0x027C            |
| Grenade Launcher Ammo 2 | 0x187F            |
| Grenade Launcher Ammo 3 | 0x1891            |
| Grenade Launcher Ammo 4 | 0x18A3            |
| Rocket Launcher Ammo 1  | 0x0278            |
| Rocket Launcher Ammo 2  | 0x187B            |
| Rocket Launcher Ammo 3  | 0x188D            |
| Rocket Launcher Ammo 4  | 0x189F            |
| Harpoon Ammo 1          | 0x027A            |
| Harpoon Ammo 2          | 0x1877            |
| Harpoon Ammo 3          | 0x1889            |
| Harpoon Ammo 4          | 0x189B            |
| MP5 Ammo 1              | 0x0276            |
| MP5 Ammo 2              | 0x1883            |
| MP5 Ammo 3              | 0x1895            |
| MP5 Ammo 4              | 0x18A7            |
| Uzi Ammo 1              | 0x0272            |
| Uzi Ammo 2              | 0x186F            |
| Uzi Ammo 3              | 0x1881            |
| Uzi Ammo 4              | 0x1893            |

#### Aldwych ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x02B1            |
| Large Medipack          | 0x02B2            |
| Flares                  | 0x02B4            |
| Weapons Config Number   | 0x02B8            |
| Harpoon Gun             | 0x02B9            |
| Shotgun Ammo 1          | 0x02A7            |
| Shotgun Ammo 2          | 0x22FF            |
| Deagle Ammo 1           | 0x02A3            |
| Deagle Ammo 2	          | 0x22F7            |
| Grenade Launcher Ammo 1 | 0x02AF            |
| Grenade Launcher Ammo 2 | 0x230B            |
| Rocket Launcher Ammo 1  | 0x02AB            |
| Rocket Launcher Ammo 2  | 0x2307            |
| Harpoon Ammo 1          | 0x02AD            |
| Harpoon Ammo 2          | 0x2303            |
| MP5 Ammo 1              | 0x02A9            |
| MP5 Ammo 2              | 0x230F            |
| Uzi Ammo 1              | 0x02A5            |
| Uzi Ammo 2              | 0x22FB            |

#### Lud's Gate ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x02E4            |
| Large Medipack          | 0x02E5            |
| Flares                  | 0x02E7            |
| Weapons Config Number   | 0x02EB            |
| Harpoon Gun             | 0x02EC            |
| Shotgun Ammo 1          | 0x02DA            |
| Shotgun Ammo 2          | 0x1D89            |
| Shotgun Ammo 3          | 0x1DAD            |
| Deagle Ammo 1           | 0x02D6            |
| Deagle Ammo 2	          | 0x1D81            |
| Deagle Ammo 3	          | 0x1DA5            |
| Grenade Launcher Ammo 1 | 0x02E2            |
| Grenade Launcher Ammo 2 | 0x1D95            |
| Grenade Launcher Ammo 3 | 0x1DB9            |
| Rocket Launcher Ammo 1  | 0x02DE            |
| Rocket Launcher Ammo 2  | 0x1D91            |
| Rocket Launcher Ammo 3  | 0x1DB5            |
| Rocket Launcher Ammo 4  | 0x1DB9            |
| Harpoon Ammo 1          | 0x02E0            |
| Harpoon Ammo 2          | 0x1D8D            |
| Harpoon Ammo 3          | 0x1DB1            |
| MP5 Ammo 1              | 0x02DC            |
| MP5 Ammo 2              | 0x1D99            |
| MP5 Ammo 3              | 0x1DBD            |
| Uzi Ammo 1              | 0x02D8            |
| Uzi Ammo 2              | 0x1D85            |
| Uzi Ammo 3              | 0x1DA9            |

#### City ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x0317            |
| Large Medipack          | 0x0318            |
| Flares                  | 0x031A            |
| Weapons Config Number   | 0x031E            |
| Harpoon Gun             | 0x031F            |
| Shotgun Ammo 1          | 0x030D            |
| Shotgun Ammo 2          | 0x0AF3            |
| Shotgun Ammo 3          | 0x0B05            |
| Deagle Ammo 1           | 0x0309            |
| Deagle Ammo 2	          | 0x0AEB            |
| Deagle Ammo 3	          | 0x0AFB            |
| Grenade Launcher Ammo 1 | 0x0315            |
| Grenade Launcher Ammo 2 | 0x0AFF            |
| Grenade Launcher Ammo 3 | 0x0B11            |
| Rocket Launcher Ammo 1  | 0x0311            |
| Rocket Launcher Ammo 2  | 0x0AFB            |
| Rocket Launcher Ammo 3  | 0x0B0D            |
| Harpoon Ammo 1          | 0x0313            |
| Harpoon Ammo 2          | 0x0AF7            |
| Harpoon Ammo 3          | 0x0B09            |
| MP5 Ammo 1              | 0x030F            |
| MP5 Ammo 2              | 0x1B03            |
| MP5 Ammo 3              | 0x1B15            |
| Uzi Ammo 1              | 0x030B            |
| Uzi Ammo 2              | 0x0AEF            |
| Uzi Ammo 3              | 0x0B01            |

#### Antarctica ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x03E3            |
| Large Medipack          | 0x03E4            |
| Flares                  | 0x03E6            |
| Weapons Config Number   | 0x03EA            |
| Harpoon Gun             | 0x03EB            |
| Shotgun Ammo 1          | 0x03D9            |
| Shotgun Ammo 2          | 0x1995            |
| Shotgun Ammo 2          | 0x19A7            |
| Deagle Ammo 1           | 0x03D5            |
| Deagle Ammo 2	          | 0x198D            |
| Deagle Ammo 3	          | 0x199F            |
| Grenade Launcher Ammo 1 | 0x0315            |
| Grenade Launcher Ammo 2 | 0x19A1            |
| Grenade Launcher Ammo 3 | 0x19B3            |
| Rocket Launcher Ammo 1  | 0x0311            |
| Rocket Launcher Ammo 2  | 0x199D            |
| Rocket Launcher Ammo 3  | 0x19AF            |
| Harpoon Ammo 1          | 0x03DF            |
| Harpoon Ammo 2          | 0x1999            |
| Harpoon Ammo 3          | 0x19AB            |
| MP5 Ammo 1              | 0x03DB            |
| MP5 Ammo 2              | 0x19A5            |
| MP5 Ammo 3              | 0x19B7            |
| Uzi Ammo 1              | 0x03D7            |
| Uzi Ammo 2              | 0x1991            |
| Uzi Ammo 2              | 0x19A3            |

#### RX-Tech Mines ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x0416            |
| Large Medipack          | 0x0417            |
| Flares                  | 0x0419            |
| Weapons Config Number   | 0x041D            |
| Harpoon Gun             | 0x041E            |
| Shotgun Ammo 1          | 0x040C            |
| Shotgun Ammo 2          | 0x1957            |
| Shotgun Ammo 3          | 0x197B            |
| Shotgun Ammo 4          | 0x198D            |
| Deagle Ammo 1           | 0x0408            |
| Deagle Ammo 2	          | 0x194F            |
| Deagle Ammo 3	          | 0x1973            |
| Deagle Ammo 4	          | 0x1985            |
| Grenade Launcher Ammo 1 | 0x0414            |
| Grenade Launcher Ammo 2 | 0x1963            |
| Rocket Launcher Ammo 1  | 0x0410            |
| Rocket Launcher Ammo 2  | 0x1963            |
| Rocket Launcher Ammo 2  | 0x1987            |
| Rocket Launcher Ammo 2  | 0x1999            |
| Harpoon Ammo 1          | 0x0412            |
| Harpoon Ammo 2          | 0x195B            |
| Harpoon Ammo 3          | 0x197F            |
| Harpoon Ammo 4          | 0x1991            |
| MP5 Ammo 1              | 0x040E            |
| MP5 Ammo 2              | 0x1967            |
| MP5 Ammo 3              | 0x198B            |
| MP5 Ammo 4              | 0x199D            |
| Uzi Ammo 1              | 0x040A            |
| Uzi Ammo 2              | 0x1953            |
| Uzi Ammo 3              | 0x1977            |
| Uzi Ammo 4              | 0x1989            |

#### Lost City of Tinnos ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x0449            |
| Large Medipack          | 0x044A            |
| Flares                  | 0x044C            |
| Weapons Config Number   | 0x0450            |
| Harpoon Gun             | 0x0451            |
| Shotgun Ammo 1          | 0x043F            |
| Shotgun Ammo 2          | 0x1D97            |
| Shotgun Ammo 3          | 0x1DCD            |
| Deagle Ammo 1           | 0x043B            |
| Deagle Ammo 2	          | 0x1D8F            |
| Deagle Ammo 3	          | 0x1DC5            |
| Grenade Launcher Ammo 1 | 0x0447            |
| Grenade Launcher Ammo 2 | 0x1DA3            |
| Grenade Launcher Ammo 3 | 0x1DD9            |
| Rocket Launcher Ammo 1  | 0x0443            |
| Rocket Launcher Ammo 2  | 0x1D9F            |
| Rocket Launcher Ammo 3  | 0x1DD5            |
| Harpoon Ammo 1          | 0x0445            |
| Harpoon Ammo 2          | 0x1D9B            |
| Harpoon Ammo 3          | 0x1DD1            |
| MP5 Ammo 1              | 0x0441            |
| MP5 Ammo 2              | 0x1DA7            |
| MP5 Ammo 3              | 0x1DDD            |
| Uzi Ammo 1              | 0x043D            |
| Uzi Ammo 2              | 0x1D93            |
| Uzi Ammo 3              | 0x1DC9            |

#### Meteorite Cavern ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x047C            |
| Large Medipack          | 0x047D            |
| Flares                  | 0x047F            |
| Weapons Config Number   | 0x0483            |
| Harpoon Gun             | 0x0484            |
| Shotgun Ammo 1          | 0x0472            |
| Shotgun Ammo 2          | 0x0AE9            |
| Shotgun Ammo 3          | 0x0AFB            |
| Deagle Ammo 1           | 0x046E            |
| Deagle Ammo 2	          | 0x0AE1            |
| Deagle Ammo 3	          | 0x0AF3            |
| Grenade Launcher Ammo 1 | 0x047A            |
| Grenade Launcher Ammo 2 | 0x0AF5            |
| Grenade Launcher Ammo 3 | 0x0B07            |
| Rocket Launcher Ammo 1  | 0x0476            |
| Rocket Launcher Ammo 2  | 0x0AF1            |
| Rocket Launcher Ammo 3  | 0x0B03            |
| Harpoon Ammo 1          | 0x0478            |
| Harpoon Ammo 2          | 0x0AED            |
| Harpoon Ammo 3          | 0x0AFF            |
| MP5 Ammo 1              | 0x0474            |
| MP5 Ammo 2              | 0x0AF9            |
| MP5 Ammo 3              | 0x0B0B            |
| Uzi Ammo 1              | 0x0470            |
| Uzi Ammo 2              | 0x0AE5            |
| Uzi Ammo 3              | 0x0AF7            |

#### All Hallows ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x04AF            |
| Large Medipack          | 0x04B0            |
| Flares                  | 0x04B2            |
| Weapons Config Number   | 0x04B6            |
| Harpoon Gun             | 0x04B7            |
| Shotgun Ammo 1          | 0x0472            |
| Shotgun Ammo 2          | 0x102D            |
| Deagle Ammo 1           | 0x046E            |
| Deagle Ammo 2	          | 0x1025            |
| Grenade Launcher Ammo 1 | 0x047A            |
| Grenade Launcher Ammo 2 | 0x1039            |
| Rocket Launcher Ammo 1  | 0x0476            |
| Rocket Launcher Ammo 2  | 0x1035            |
| Harpoon Ammo 1          | 0x0478            |
| Harpoon Ammo 2          | 0x1031            |
| MP5 Ammo 1              | 0x04A7            |
| MP5 Ammo 2              | 0x103D            |
| Uzi Ammo 1              | 0x04A3            |
| Uzi Ammo 2              | 0x1029            |
