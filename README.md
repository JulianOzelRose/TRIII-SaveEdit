# Tomb Raider III - Savegame Editor
This is a savegame editor for Tomb Raider III. It has been rigorously tested to work with the Steam version of the game, but it should work on
the original and multi-patched version as well. Be sure to back up your save game files anyway as a precaution.
This editor can enable any weapon on any level, including the bonus level. No setup is necessary, just download and run.

## Installation and usage
1. Navigate to the [Release](https://github.com/JulianOzelRose/TRIII-SaveEdit/tree/master/TRIII-SaveEdit/bin/x64/Release) folder, then download ```TRIII-SaveEdit.exe```.
2. Launch the editor and click ```Browse``` to locate your game directory.
3. If you're using the Steam version of the game, your directory should be:<br>
```\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\```<br>
4. Once your save game is selected, make the desired modifications.
5. Click ```Save``` to apply your changes, and enjoy.

#### Screenshot of TRIII-SaveEdit
![TRIII-SaveEdit-UI](https://github.com/JulianOzelRose/TRIII-SaveEdit/assets/95890436/2bde47ca-a4ac-471e-9bdc-30473dbe4e68)

## Reading weapons information
Unlike Tomb Raider: Chronicles, the save file offsets in Tomb Raider III are stored differently on each level. Another interesting difference is that
instead of storing weapons on individual offsets, all weapons information is stored on a single offset, which I call ```weaponsConfigNum```.
The only exception is the harpoon gun, which is stored on its own offset, and is of boolean type. The weapons configuration variable has a
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

## Writing weapons information
When storing our new weapons configuration, we just perform the same operations in reverse.
We start with the base number of 1, and then increment based on the checked weapons.
Lastly, we write the calculated number to the save file.

```
int newWeaponsConfigNum = 1;
if (pistolsCheckBox.Checked) newWeaponsConfigNum += 2;
if (deagleCheckBox.Checked) newWeaponsConfigNum += 4;
if (uziCheckBox.Checked) newWeaponsConfigNum += 8;
if (shotgunCheckBox.Checked) newWeaponsConfigNum += 16;
if (mp5CheckBox.Checked) newWeaponsConfigNum += 32;
if (rocketLauncherCheckBox.Checked) newWeaponsConfigNum += 64;
if (grenadeLauncherCheckBox.Checked) newWeaponsConfigNum += 128;

WriteToSaveFile(weaponsConfigNumOffset, newWeaponsConfigNum);
```

## Calculating ammunition offsets
Ammunition is stored on up to two different offsets. It is always stored on a lower offset, which I call the primary ammo offset, and then it is
stored on an additional offset, which I call the secondary ammo offset. If the respective weapon is not in inventory, then the ammo is only stored on
the primary offset. If the weapon is equipped, then the ammo is stored on both offsets. There can be anywhere from 1-9 different secondary ammo offsets
per level.

The "correct" secondary ammo offset changes throughout the level, and seems to depend on the number of active entities in the game.
Writing to incorrect or multiple secondary offsets typically results in the game crashing upon loading. To determine which secondary offset is the correct
one to write to, we take the base secondary offset and loop through the potential secondary offsets, using 0x12 as an iterator.
We then check the current ammo index with ```GetAmmoIndex()``` and add both the primary and secondary offsets to our list, and return as an array.

```
int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
{
    List<int> secondaryOffsets = new List<int>();
    List<int> validOffsets = new List<int>();

    int currentAmmoIndex = GetAmmoIndex();

    for (int i = 0; i < 10; i++)
    {
        secondaryOffsets.Add(baseSecondaryOffset + i * 0x12);
    }

    validOffsets.Add(primaryOffset);
    validOffsets.Add(secondaryOffsets[currentAmmoIndex]);

    return validOffsets.ToArray();
}
```

## Determining the current ammo index
The ammo index is based on the number of active entities in the game. If there are no active entities,
the correct ammo index is 0. If there is 1 entity, the index is 1, and so on. Ammo indices are also uniform,
meaning if the current ammo index for MP5 is 2, then the ammo index for the shotgun and other weapons is also 2.
One possible way to determine the current ammo index would be to reverse the entity data structures, and find
common byte flags which may identify them as active entities. Since there are a total of 32 entities in the game,
this would be an extremely daunting task.

Fortunately, I was able to identify certain bytes in the save files that changed consistently along with the ammo index.
I am not entirely sure what this data represents, but it seems to correlate with entity data. It's a 4-byte line
consisting of 0xFF, 4 times. Here's what that data looks like on the same level with an ammo index of 0 (left) versus an ammo index of 1 (right).

![TR3-Index-Diffs](https://github.com/JulianOzelRose/TRIII-SaveEdit/assets/95890436/e858081d-604e-4c6b-a1b4-4fd866094d86)

So we take note of the byte patterns for the different ammo indices for each level, then store that data as a dictionary. There are 20
levels and these bytes are stored differently on each one. We then call the dictionary in ```GetAmmoIndex()``` to check which lines
the 0xFF bytes are located on. We will then know what the current ammo index is.

```
int GetAmmoIndex()
{
    string lvlName = GetCleanLvlName();
    int ammoIndex = 0;

    if (ammoIndexData.ContainsKey(lvlName))
    {
        Dictionary<int, int[]> indexData = ammoIndexData[lvlName];

        for (int i = 0; i < indexData.Count; i++)
        {
            int key = indexData.ElementAt(i).Key;
            int[] offsets = indexData.ElementAt(i).Value;

            if (offsets.All(offset => GetSaveFileData(offset) == 0xFF))
            {
                ammoIndex = key;
                break;
            }
        }
    }

    return ammoIndex;
}
```

## Offset tables
Aside from the level name and save number, the save file offsets differ on every level. The offsets for small medipack
and large medipack are 1 byte away. Flares is 2 bytes away from large medipack, and the weapons config number is 4 bytes away
from flares. Next to the weapons config number is the harpoon gun offset. The first ammo offset listed on each
table is the primary ammo offset, and the next one is the secondary base offset.

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
| Shotgun Ammo 2          | 0x164B            |
| Deagle Ammo 1           | 0x00D8            |
| Deagle Ammo 2	          | 0x1643            |
| Grenade Launcher Ammo 1 | 0x00E4            |
| Grenade Launcher Ammo 2 | 0x1657            |
| Rocket Launcher Ammo 1  | 0x00E0            |
| Rocket Launcher Ammo 2  | 0x1653            |
| Harpoon Ammo 1          | 0x00E2            |
| Harpoon Ammo 2          | 0x164F            |
| MP5 Ammo 1              | 0x00DE            |
| MP5 Ammo 2              | 0x165B            |
| Uzi Ammo 1              | 0x00DA            |
| Uzi Ammo 2              | 0x1647            |

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
| Deagle Ammo 1           | 0x010B            |
| Deagle Ammo 2	          | 0x23B3            |
| Grenade Launcher Ammo 1 | 0x0117            |
| Grenade Launcher Ammo 2 | 0x23C7            |
| Rocket Launcher Ammo 1  | 0x0113            |
| Rocket Launcher Ammo 2  | 0x23C3            |
| Harpoon Ammo 1          | 0x0115            |
| Harpoon Ammo 2          | 0x23BF            |
| MP5 Ammo 1              | 0x0111            |
| MP5 Ammo 2              | 0x23CB            |
| Uzi Ammo 1              | 0x010D            |
| Uzi Ammo 2              | 0x23B7            |

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
| Deagle Ammo 1           | 0x013E            |
| Deagle Ammo 2	          | 0x17FC            |
| Grenade Launcher Ammo 1 | 0x014A            |
| Grenade Launcher Ammo 2 | 0x1810            |
| Rocket Launcher Ammo 1  | 0x0146            |
| Rocket Launcher Ammo 2  | 0x180C            |
| Harpoon Ammo 1          | 0x0148            |
| Harpoon Ammo 2          | 0x1808            |
| MP5 Ammo 1              | 0x0144            |
| MP5 Ammo 2              | 0x1814            |
| Uzi Ammo 1              | 0x0140            |
| Uzi Ammo 2              | 0x1800            |

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
| Deagle Ammo 1           | 0x033C            |
| Deagle Ammo 2	          | 0x179C            |
| Grenade Launcher Ammo 1 | 0x0348            |
| Grenade Launcher Ammo 2 | 0x17B0            |
| Rocket Launcher Ammo 1  | 0x0344            |
| Rocket Launcher Ammo 2  | 0x17AC            |
| Harpoon Ammo 1          | 0x0346            |
| Harpoon Ammo 2          | 0x17A8            |
| MP5 Ammo 1              | 0x0342            |
| MP5 Ammo 2              | 0x17B4            |
| Uzi Ammo 1              | 0x033E            |
| Uzi Ammo 2              | 0x17A0            |

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
| Shotgun Ammo 2          | 0x1E4B            |
| Deagle Ammo 1           | 0x036F            |
| Deagle Ammo 2	          | 0x1E43            |
| Grenade Launcher Ammo 1 | 0x037B            |
| Grenade Launcher Ammo 2 | 0x1E57            |
| Rocket Launcher Ammo 1  | 0x0377            |
| Rocket Launcher Ammo 2  | 0x1E53            |
| Harpoon Ammo 1          | 0x0379            |
| Harpoon Ammo 2          | 0x1E4F            |
| MP5 Ammo 1              | 0x0375            |
| MP5 Ammo 2              | 0x1E5B            |
| Uzi Ammo 1              | 0x0371            |
| Uzi Ammo 2              | 0x1E47            |

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
| Deagle Ammo 1           | 0x03A2            |
| Deagle Ammo 2	          | 0x2105            |
| Grenade Launcher Ammo 1 | 0x03AE            |
| Grenade Launcher Ammo 2 | 0x2119            |
| Rocket Launcher Ammo 1  | 0x03AA            |
| Rocket Launcher Ammo 2  | 0x2115            |
| Harpoon Ammo 1          | 0x03AC            |
| Harpoon Ammo 2          | 0x2111            |
| MP5 Ammo 1              | 0x03A8            |
| MP5 Ammo 2              | 0x211D            |
| Uzi Ammo 1              | 0x03A4            |
| Uzi Ammo 2              | 0x2109            |

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
| Deagle Ammo 1           | 0x01A4            |
| Deagle Ammo 2	          | 0x17A9            |
| Grenade Launcher Ammo 1 | 0x01B0            |
| Grenade Launcher Ammo 2 | 0x17BD            |
| Rocket Launcher Ammo 1  | 0x01AC            |
| Rocket Launcher Ammo 2  | 0x17B9            |
| Harpoon Ammo 1          | 0x01AE            |
| Harpoon Ammo 2          | 0x17B5            |
| MP5 Ammo 1              | 0x01AA            |
| MP5 Ammo 2              | 0x17C1            |
| Uzi Ammo 1              | 0x01A6            |
| Uzi Ammo 2              | 0x17AD            |

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
| Deagle Ammo 1           | 0x01D7            |
| Deagle Ammo 2	          | 0x18CB            |
| Grenade Launcher Ammo 1 | 0x01E3            |
| Grenade Launcher Ammo 2 | 0x18DF            |
| Rocket Launcher Ammo 1  | 0x01DF            |
| Rocket Launcher Ammo 2  | 0x18DB            |
| Harpoon Ammo 1          | 0x01E1            |
| Harpoon Ammo 2          | 0x18D7            |
| MP5 Ammo 1              | 0x01DD            |
| MP5 Ammo 2              | 0x18E3            |
| Uzi Ammo 1              | 0x01D9            |
| Uzi Ammo 2              | 0x18CF            |

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
| Deagle Ammo 1           | 0x020A            |
| Deagle Ammo 2	          | 0x1415            |
| Grenade Launcher Ammo 1 | 0x0216            |
| Grenade Launcher Ammo 2 | 0x1429            |
| Rocket Launcher Ammo 1  | 0x0212            |
| Rocket Launcher Ammo 2  | 0x1425            |
| Harpoon Ammo 1          | 0x0214            |
| Harpoon Ammo 2          | 0x1421            |
| MP5 Ammo 1              | 0x0210            |
| MP5 Ammo 2              | 0x142D            |
| Uzi Ammo 1              | 0x020C            |
| Uzi Ammo 2              | 0x1419            |

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
| Deagle Ammo 1           | 0x023D            |
| Deagle Ammo 2	          | 0x10ED            |
| Grenade Launcher Ammo 1 | 0x0249            |
| Grenade Launcher Ammo 2 | 0x1101            |
| Rocket Launcher Ammo 1  | 0x0245            |
| Rocket Launcher Ammo 2  | 0x10FD            |
| Harpoon Ammo 1          | 0x0247            |
| Harpoon Ammo 2          | 0x10F9            |
| MP5 Ammo 1              | 0x0243            |
| MP5 Ammo 2              | 0x1105            |
| Uzi Ammo 1              | 0x023F            |
| Uzi Ammo 2              | 0x10F1            |

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
| Deagle Ammo 1           | 0x0270            |
| Deagle Ammo 2	          | 0x186B            |
| Grenade Launcher Ammo 1 | 0x027C            |
| Grenade Launcher Ammo 2 | 0x187F            |
| Rocket Launcher Ammo 1  | 0x0278            |
| Rocket Launcher Ammo 2  | 0x187B            |
| Harpoon Ammo 1          | 0x027A            |
| Harpoon Ammo 2          | 0x1877            |
| MP5 Ammo 1              | 0x0276            |
| MP5 Ammo 2              | 0x1883            |
| Uzi Ammo 1              | 0x0272            |
| Uzi Ammo 2              | 0x186F            |

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
| Shotgun Ammo 2          | 0x1D77            |
| Deagle Ammo 1           | 0x02D6            |
| Deagle Ammo 2	          | 0x1D6F            |
| Grenade Launcher Ammo 1 | 0x02E2            |
| Grenade Launcher Ammo 2 | 0x1D83            |
| Rocket Launcher Ammo 1  | 0x02DE            |
| Rocket Launcher Ammo 2  | 0x1D7F            |
| Harpoon Ammo 1          | 0x02E0            |
| Harpoon Ammo 2          | 0x1D7B            |
| MP5 Ammo 1              | 0x02DC            |
| MP5 Ammo 2              | 0x1D87            |
| Uzi Ammo 1              | 0x02D8            |
| Uzi Ammo 2              | 0x1D73            |

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
| Deagle Ammo 1           | 0x0309            |
| Deagle Ammo 2	          | 0x0AEB            |
| Grenade Launcher Ammo 1 | 0x0315            |
| Grenade Launcher Ammo 2 | 0x0AFF            |
| Rocket Launcher Ammo 1  | 0x0311            |
| Rocket Launcher Ammo 2  | 0x0AFB            |
| Harpoon Ammo 1          | 0x0313            |
| Harpoon Ammo 2          | 0x0AF7            |
| MP5 Ammo 1              | 0x030F            |
| MP5 Ammo 2              | 0x1B03            |
| Uzi Ammo 1              | 0x030B            |
| Uzi Ammo 2              | 0x0AEF            |

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
| Deagle Ammo 1           | 0x03D5            |
| Deagle Ammo 2	          | 0x198D            |
| Grenade Launcher Ammo 1 | 0x03E1            |
| Grenade Launcher Ammo 2 | 0x19A1            |
| Rocket Launcher Ammo 1  | 0x03DD            |
| Rocket Launcher Ammo 2  | 0x199D            |
| Harpoon Ammo 1          | 0x03DF            |
| Harpoon Ammo 2          | 0x1999            |
| MP5 Ammo 1              | 0x03DB            |
| MP5 Ammo 2              | 0x19A5            |
| Uzi Ammo 1              | 0x03D7            |
| Uzi Ammo 2              | 0x1991            |

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
| Deagle Ammo 1           | 0x0408            |
| Deagle Ammo 2	          | 0x194F            |
| Grenade Launcher Ammo 1 | 0x0414            |
| Grenade Launcher Ammo 2 | 0x1963            |
| Rocket Launcher Ammo 1  | 0x0410            |
| Rocket Launcher Ammo 2  | 0x195F            |
| Harpoon Ammo 1          | 0x0412            |
| Harpoon Ammo 2          | 0x195B            |
| MP5 Ammo 1              | 0x040E            |
| MP5 Ammo 2              | 0x1967            |
| Uzi Ammo 1              | 0x040A            |
| Uzi Ammo 2              | 0x1953            |

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
| Deagle Ammo 1           | 0x043B            |
| Deagle Ammo 2	          | 0x1D8F            |
| Grenade Launcher Ammo 1 | 0x0447            |
| Grenade Launcher Ammo 2 | 0x1DA3            |
| Rocket Launcher Ammo 1  | 0x0443            |
| Rocket Launcher Ammo 2  | 0x1D9F            |
| Harpoon Ammo 1          | 0x0445            |
| Harpoon Ammo 2          | 0x1D9B            |
| MP5 Ammo 1              | 0x0441            |
| MP5 Ammo 2              | 0x1DA7            |
| Uzi Ammo 1              | 0x043D            |
| Uzi Ammo 2              | 0x1D93            |

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
| Deagle Ammo 1           | 0x046E            |
| Deagle Ammo 2	          | 0x0AE1            |
| Grenade Launcher Ammo 1 | 0x047A            |
| Grenade Launcher Ammo 2 | 0x0AF5            |
| Rocket Launcher Ammo 1  | 0x0476            |
| Rocket Launcher Ammo 2  | 0x0AF1            |
| Harpoon Ammo 1          | 0x0478            |
| Harpoon Ammo 2          | 0x0AED            |
| MP5 Ammo 1              | 0x0474            |
| MP5 Ammo 2              | 0x0AF9            |
| Uzi Ammo 1              | 0x0470            |
| Uzi Ammo 2              | 0x0AE5            |

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
