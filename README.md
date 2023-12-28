# Tomb Raider III - Savegame Editor
This is a standalone savegame editor for Tomb Raider III. It has been rigorously tested to work with the Steam version of the game, but it should work on
the original and multi-patched versions as well. Be sure to back up your save game files as a precaution.
This editor can enable any weapon on any level, including the bonus level. No setup is necessary, simply download and run.

![TRIII-SaveEdit-UI](https://github.com/JulianOzelRose/TRIII-SaveEdit/assets/95890436/d609241f-9d27-417a-8b26-f04033c0b34d)


## Installation and usage
1. Navigate to the [Release](https://github.com/JulianOzelRose/TRIII-SaveEdit/tree/master/TRIII-SaveEdit/bin/x64/Release) folder, then download ```TRIII-SaveEdit.exe```.
2. Launch the editor and click ```Browse``` to locate your game directory.
3. If you're using the Steam version of the game, your directory should be:<br>
```\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\```<br>
4. Once your save game is selected, make the desired modifications.
5. Click ```Save``` to apply your changes, and enjoy.

## Reading weapons information
Unlike Tomb Raider: Chronicles, the save file offsets in Tomb Raider III are stored differently on each level. Another interesting difference is that
instead of storing weapons on individual offsets, all weapons information is stored on a single offset, which I call ```weaponsConfigNum```.
The only exception is the harpoon gun, which is stored on its own offset, and is of boolean type. The weapons configuration variable has a
base number of 1, which indicates no weapons present in inventory. Each weapon adds a unique number to the variable.

###       ```weaponsConfigNum```             ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistols                 | 2                 |
| Deagle                  | 4                 |
| Uzis                    | 8                 |
| Shotgun                 | 16                |
| MP5                     | 32                |
| Rocket Launcher         | 64                |
| Grenade Launcher        | 128               |

When the game loads a save file, it checks this number to determine which weapons are present in inventory. When reverse
engineering this part of the game, we need to choose our methods wisely. Since there are 128 possible combinations, using conditional
operations would be extremely inefficient. Bitwise operations are perfect for this scenario. We account for the base case of no weapons,
and then we use our else block to check if the unique bit of a weapon is present in the config variable, and then set our values to display accordingly.

```
const byte Pistols = 2;
const byte Deagle = 4;
const byte Uzis = 8;
const byte Shotgun = 16;
const byte MP5 = 32;
const byte RocketLauncher = 64;
const byte GrenadeLauncher = 128;

if (weaponsConfigNum == 1)
{
    chkPistols.Checked = false;
    chkShotgun.Checked = false;
    chkDesertEagle.Checked = false;
    chkUzis.Checked = false;
    chkMp5.Checked = false;
    chkRocketLauncher.Checked = false;
    chkGrenadeLauncher.Checked = false;
}
else
{
    chkPistols.Checked = (weaponsConfigNum & Pistols) != 0;
    chkShotgun.Checked = (weaponsConfigNum & Shotgun) != 0;
    chkDesertEagle.Checked = (weaponsConfigNum & Deagle) != 0;
    chkUzis.Checked = (weaponsConfigNum & Uzis) != 0;
    chkMp5.Checked = (weaponsConfigNum & MP5) != 0;
    chkRocketLauncher.Checked = (weaponsConfigNum & RocketLauncher) != 0;
    chkGrenadeLauncher.Checked = (weaponsConfigNum & GrenadeLauncher) != 0;
}
```

## Writing weapons information
When storing our new weapons configuration, we just perform the same operations in reverse.
We start with the base number of 1, and then increment based on the checked weapons.
Lastly, we write the calculated number to the save file.

```
byte newWeaponsConfigNum = 1;

if (chkPistols.Checked) newWeaponsConfigNum += 2;
if (chkDesertEagle.Checked) newWeaponsConfigNum += 4;
if (chkUzis.Checked) newWeaponsConfigNum += 8;
if (chkShotgun.Checked) newWeaponsConfigNum += 16;
if (chkMp5.Checked) newWeaponsConfigNum += 32;
if (chkRocketLauncher.Checked) newWeaponsConfigNum += 64;
if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 128;

WriteByte(weaponsConfigNumOffset, newWeaponsConfigNum);
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
public int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
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
public int GetAmmoIndex()
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

            if (offsets.All(offset = > ReadByte(offset) == 0xFF))
            {
                ammoIndex = key;
                break;
            }
        }
    }

    return ammoIndex;
}
```

## Determining the correct health offset
Health is stored on anywhere from 1 to 3 different offsets per level. The exact offset it is written to changes
as you progress through a level. Writing to the incorrect offset may crash the game or lead to other glitches.
To get around this issue, this savegame editor stores the known health offsets for each level on an array.
When pulling health information, it loops through the known health offsets, and does a couple of heuristic
checks to see which is the correct health offset. First, it checks for impossible health values (0 or greater than 1000).
Then, it checks the surrounding data. Since health is always stored 8 bytes away from the character animation data,
it checks those addresses for character animation byte flags. If a match is found, it returns the correct health offset. 

```
public int GetHealthOffset()
{
    for (int i = 0; i < healthOffsets.Count; i++)
    {
        int healthValue = ReadUInt16(healthOffsets[i]);

        if (healthValue > MIN_HEALTH_VALUE && healthValue <= MAX_HEALTH_VALUE)
        {
            byte byteFlag1 = ReadByte(healthOffsets[i] - 10);
            byte byteFlag2 = ReadByte(healthOffsets[i] - 9);
            byte byteFlag3 = ReadByte(healthOffsets[i] - 8);

            if (IsKnownByteFlagPattern(byteFlag1, byteFlag2, byteFlag3))
            {
                return healthOffsets[i];
            }
        }
    }

    return -1;
}
```

## Offset tables
Aside from the level name and save number, the save file offsets differ on every level. The offsets for small medipack
and large medipack are 1 byte away. Flares is 2 bytes away from large medipack, and the weapons config number is 4 bytes away
from flares. Next to the weapons config number is the harpoon gun offset. The first ammo offset listed on each
table is the primary ammo offset, and the next one is the secondary base offset.

#### Jungle ####
| **File offset** | **Type** | **Variable**            |
| :---            | :---     | :---                    |
| 0x0000          | String   | Level Name              |
| 0x004B          | UInt16   | Save Number             |
| 0x00D8          | UInt16   | Deagle Ammo 1           |
| 0x00DA          | UInt16   | Uzi Ammo 1              |
| 0x00DC          | UInt16   | Shotgun Ammo 1          |
| 0x00DE          | UInt16   | MP5 Ammo 1              |
| 0x00E0          | UInt16   | Rocket Launcher Ammo 1  |
| 0x00E2          | UInt16   | Harpoon Ammo 1          |
| 0x00E4          | UInt16   | Grenade Launcher Ammo 1 |
| 0x00E6          | BYTE     | Small Medipack          |
| 0x00E7          | BYTE     | Large Medipack          |
| 0x00E9          | BYTE     | Flares                  |
| 0x00ED          | BYTE     | Weapons Config Number   |
| 0x00EE          | BYTE     | Harpoon Gun             |
| 0x1643          | UInt16   | Deagle Ammo 2	       |
| 0x1647          | UInt16   | Uzi Ammo 2              |
| 0x164B          | UInt16   | Shotgun Ammo 2          |
| 0x164F          | UInt16   | Harpoon Ammo 2          |
| 0x1653          | UInt16   | Rocket Launcher Ammo 2  |
| 0x1657          | UInt16   | Grenade Launcher Ammo 2 |
| 0x165B          | UInt16   | MP5 Ammo 2              |

#### Temple Ruins ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x010B            | UInt16   | Deagle Ammo 1           |
| 0x010D            | UInt16   | Uzi Ammo 1              |
| 0x010F            | UInt16   | Shotgun Ammo 1          |
| 0x0111            | UInt16   | MP5 Ammo 1              |
| 0x0113            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0115            | UInt16   | Harpoon Ammo 1          |
| 0x0117            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0119            | BYTE     | Small Medipack          |
| 0x011A            | BYTE     | Large Medipack          |
| 0x011C            | BYTE     | Flares                  |
| 0x0120            | BYTE     | Weapons Config Number   |
| 0x0121            | BYTE     | Harpoon Gun             |
| 0x23B3            | UInt16   | Deagle Ammo 2           |
| 0x23B7            | UInt16   | Uzi Ammo 2              |
| 0x23BB            | UInt16   | Shotgun Ammo 2          |
| 0x23BF            | UInt16   | Harpoon Ammo 2          |
| 0x23C3            | UInt16   | Rocket Launcher Ammo 2  |
| 0x23C7            | UInt16   | Grenade Launcher Ammo 2 |
| 0x23CB            | UInt16   | MP5 Ammo 2              |

#### The River Ganges ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x013E            | UInt16   | Deagle Ammo 1           |
| 0x0140            | UInt16   | Uzi Ammo 1              |
| 0x0142            | UInt16   | Shotgun Ammo 1          |
| 0x0144            | UInt16   | MP5 Ammo 1              |
| 0x0146            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0148            | UInt16   | Harpoon Ammo 1          |
| 0x014A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x014C            | BYTE     | Small Medipack          |
| 0x014D            | BYTE     | Large Medipack          |
| 0x014F            | BYTE     | Flares                  |
| 0x0153            | BYTE     | Weapons Config Number   |
| 0x0154            | BYTE     | Harpoon Gun             |
| 0x17FC            | UInt16   | Deagle Ammo 2           |
| 0x1800            | UInt16   | Uzi Ammo 2              |
| 0x1804            | UInt16   | Shotgun Ammo 2          |
| 0x1808            | UInt16   | Harpoon Ammo 2          |
| 0x180C            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1810            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1814            | UInt16   | MP5 Ammo 2              |

#### Caves of Kaliya ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0171            | UInt16   | Deagle Ammo 1           |
| 0x0173            | UInt16   | Uzi Ammo 1              |
| 0x0175            | UInt16   | Shotgun Ammo 1          |
| 0x0177            | UInt16   | MP5 Ammo 1              |
| 0x0179            | UInt16   | Rocket Launcher Ammo 1  |
| 0x017B            | UInt16   | Harpoon Ammo 1          |
| 0x017D            | UInt16   | Grenade Launcher Ammo 1 |
| 0x017F            | BYTE     | Small Medipack          |
| 0x0180            | BYTE     | Large Medipack          |
| 0x0182            | BYTE     | Flares                  |
| 0x0186            | BYTE     | Weapons Config Number   |
| 0x0187            | BYTE     | Harpoon Gun             |
| 0x0D17            | UInt16   | Deagle Ammo 2           |
| 0x0D1B            | UInt16   | Uzi Ammo 2              |
| 0x0D1F            | UInt16   | Shotgun Ammo 2          |
| 0x0D23            | UInt16   | Harpoon Ammo 2          |
| 0x0D27            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0D2B            | UInt16   | Grenade Launcher Ammo 2 |
| 0x0D2F            | UInt16   | MP5 Ammo 2              |

#### Nevada Desert ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x033C            | UInt16   | Deagle Ammo 1           |
| 0x033E            | UInt16   | Uzi Ammo 1              |
| 0x0340            | UInt16   | Shotgun Ammo 1          |
| 0x0342            | UInt16   | MP5 Ammo 1              |
| 0x0344            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0346            | UInt16   | Harpoon Ammo 1          |
| 0x0348            | UInt16   | Grenade Launcher Ammo 1 |
| 0x034A            | BYTE     | Small Medipack          |
| 0x034B            | BYTE     | Large Medipack          |
| 0x034A            | BYTE     | Flares                  |
| 0x0351            | BYTE     | Weapons Config Number   |
| 0x0352            | BYTE     | Harpoon Gun             |
| 0x179C            | UInt16   | Deagle Ammo 2           |
| 0x17A0            | UInt16   | Uzi Ammo 2              |
| 0x17A4            | UInt16   | Shotgun Ammo 2          |
| 0x17A8            | UInt16   | Harpoon Ammo 2          |
| 0x17AC            | UInt16   | Rocket Launcher Ammo 2  |
| 0x17B0            | UInt16   | Grenade Launcher Ammo 2 |
| 0x17B4            | UInt16   | MP5 Ammo 2              |

#### High Security Compound ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x036F            | UInt16   | Deagle Ammo 1           |
| 0x0371            | UInt16   | Uzi Ammo 1              |
| 0x0373            | UInt16   | Shotgun Ammo 1          |
| 0x0375            | UInt16   | MP5 Ammo 1              |
| 0x0377            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0379            | UInt16   | Harpoon Ammo 1          |
| 0x037B            | UInt16   | Grenade Launcher Ammo 1 |
| 0x037D            | BYTE     | Small Medipack          |
| 0x037E            | BYTE     | Large Medipack          |
| 0x0380            | BYTE     | Flares                  |
| 0x0384            | BYTE     | Weapons Config Number   |
| 0x0385            | BYTE     | Harpoon Gun             |
| 0x1E43            | UInt16   | Deagle Ammo 2           |
| 0x1E47            | UInt16   | Uzi Ammo 2              |
| 0x1E4B            | UInt16   | Shotgun Ammo 2          |
| 0x1E4F            | UInt16   | Harpoon Ammo 2          |
| 0x1E53            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1E57            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1E5B            | UInt16   | MP5 Ammo 2              |

#### Area 51 ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x03A2            | UInt16   | Deagle Ammo 1           |
| 0x03A4            | UInt16   | Uzi Ammo 1              |
| 0x03A6            | UInt16   | Shotgun Ammo 1          |
| 0x03A8            | UInt16   | MP5 Ammo 1              |
| 0x03AA            | UInt16   | Rocket Launcher Ammo 1  |
| 0x03AC            | UInt16   | Harpoon Ammo 1          |
| 0x03AE            | UInt16   | Grenade Launcher Ammo 1 |
| 0x03B0            | BYTE     | Small Medipack          |
| 0x03B1            | BYTE     | Large Medipack          |
| 0x03B3            | BYTE     | Flares                  |
| 0x03B7            | BYTE     | Weapons Config Number   |
| 0x03B8            | BYTE     | Harpoon Gun             |
| 0x2105            | UInt16   | Deagle Ammo 2           |
| 0x2109            | UInt16   | Uzi Ammo 2              |
| 0x210D            | UInt16   | Shotgun Ammo 2          |
| 0x2111            | UInt16   | Harpoon Ammo 2          |
| 0x2115            | UInt16   | Rocket Launcher Ammo 2  |
| 0x2119            | UInt16   | Grenade Launcher Ammo 2 |
| 0x211D            | UInt16   | MP5 Ammo 2              |

#### Coastal Village ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x01A4            | UInt16   | Deagle Ammo 1           |
| 0x01A6            | UInt16   | Uzi Ammo 1              |
| 0x01A8            | UInt16   | Shotgun Ammo 1          |
| 0x01AA            | UInt16   | MP5 Ammo 1              |
| 0x01AC            | UInt16   | Rocket Launcher Ammo 1  |
| 0x01AE            | UInt16   | Harpoon Ammo 1          |
| 0x01B0            | UInt16   | Grenade Launcher Ammo 1 |
| 0x01B2            | BYTE     | Small Medipack          |
| 0x01B3            | BYTE     | Large Medipack          |
| 0x01B5            | BYTE     | Flares                  |
| 0x01B9            | BYTE     | Weapons Config Number   |
| 0x01BA            | BYTE     | Harpoon Gun             |
| 0x17A9            | UInt16   | Deagle Ammo 2           |
| 0x17AD            | UInt16   | Uzi Ammo 2              |
| 0x17B1            | UInt16   | Shotgun Ammo 2          |
| 0x17B5            | UInt16   | Harpoon Ammo 2          |
| 0x17B9            | UInt16   | Rocket Launcher Ammo 2  |
| 0x17BD            | UInt16   | Grenade Launcher Ammo 2 |
| 0x17C1            | UInt16   | MP5 Ammo 2              |

#### Crash Site ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x01D7            | UInt16   | Deagle Ammo 1           |
| 0x01D9            | UInt16   | Uzi Ammo 1              |
| 0x01DB            | UInt16   | Shotgun Ammo 1          |
| 0x01DD            | UInt16   | MP5 Ammo 1              |
| 0x01DF            | UInt16   | Rocket Launcher Ammo 1  |
| 0x01E1            | UInt16   | Harpoon Ammo 1          |
| 0x01E3            | UInt16   | Grenade Launcher Ammo 1 |
| 0x01E5            | BYTE     | Small Medipack          |
| 0x01E6            | BYTE     | Large Medipack          |
| 0x01EC            | BYTE     | Weapons Config Number   |
| 0x01ED            | BYTE     | Harpoon Gun             |
| 0x18CB            | UInt16   | Deagle Ammo 2           |
| 0x18CF            | UInt16   | Uzi Ammo 2              |
| 0x18D3            | UInt16   | Shotgun Ammo 2          |
| 0x18D7            | UInt16   | Harpoon Ammo 2          |
| 0x18DB            | UInt16   | Rocket Launcher Ammo 2  |
| 0x18DF            | UInt16   | Grenade Launcher Ammo 2 |
| 0x18E3            | UInt16   | MP5 Ammo 2              |

#### Madubu Gorge ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x020A            | UInt16   | Deagle Ammo 1           |
| 0x020C            | UInt16   | Uzi Ammo 1              |
| 0x020E            | UInt16   | Shotgun Ammo 1          |
| 0x0210            | UInt16   | MP5 Ammo 1              |
| 0x0212            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0214            | UInt16   | Harpoon Ammo 1          |
| 0x0216            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0218            | BYTE     | Small Medipack          |
| 0x0219            | BYTE     | Large Medipack          |
| 0x021F            | BYTE     | Weapons Config Number   |
| 0x0220            | BYTE     | Harpoon Gun             |
| 0x1415            | UInt16   | Deagle Ammo 2           |
| 0x1419            | UInt16   | Uzi Ammo 2              |
| 0x141D            | UInt16   | Shotgun Ammo 2          |
| 0x1421            | UInt16   | Harpoon Ammo 2          |
| 0x1425            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1429            | UInt16   | Grenade Launcher Ammo 2 |
| 0x142D            | UInt16   | MP5 Ammo 2              |

#### Temple of Puna ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x023D            | UInt16   | Deagle Ammo 1           |
| 0x023F            | UInt16   | Uzi Ammo 1              |
| 0x0241            | UInt16   | Shotgun Ammo 1          |
| 0x0243            | UInt16   | MP5 Ammo 1              |
| 0x0245            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0247            | UInt16   | Harpoon Ammo 1          |
| 0x0249            | UInt16   | Grenade Launcher Ammo 1 |
| 0x024B            | BYTE     | Small Medipack          |
| 0x024C            | BYTE     | Large Medipack          |
| 0x024E            | BYTE     | Flares                  |
| 0x0252            | BYTE     | Weapons Config Number   |
| 0x0253            | BYTE     | Harpoon Gun             |
| 0x10ED            | UInt16   | Deagle Ammo 2           |
| 0x10F1            | UInt16   | Uzi Ammo 2              |
| 0x10F5            | UInt16   | Shotgun Ammo 2          |
| 0x10F9            | UInt16   | Harpoon Ammo 2          |
| 0x10FD            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1101            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1105            | UInt16   | MP5 Ammo 2              |

#### Thames Wharf ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0270            | UInt16   | Deagle Ammo 1           |
| 0x0272            | UInt16   | Uzi Ammo 1              |
| 0x0274            | UInt16   | Shotgun Ammo 1          |
| 0x0276            | UInt16   | MP5 Ammo 1              |
| 0x0278            | UInt16   | Rocket Launcher Ammo 1  |
| 0x027A            | UInt16   | Harpoon Ammo 1          |
| 0x027C            | UInt16   | Grenade Launcher Ammo 1 |
| 0x027E            | BYTE     | Small Medipack          |
| 0x027F            | BYTE     | Large Medipack          |
| 0x0281            | BYTE     | Flares                  |
| 0x0285            | BYTE     | Weapons Config Number   |
| 0x0286            | BYTE     | Harpoon Gun             |
| 0x186B            | UInt16   | Deagle Ammo 2           |
| 0x186F            | UInt16   | Uzi Ammo 2              |
| 0x1873            | UInt16   | Shotgun Ammo 2          |
| 0x1877            | UInt16   | Harpoon Ammo 2          |
| 0x187B            | UInt16   | Rocket Launcher Ammo 2  |
| 0x187F            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1883            | UInt16   | MP5 Ammo 2              |

#### Aldwych ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x02A3            | UInt16   | Deagle Ammo 1           |
| 0x02A5            | UInt16   | Uzi Ammo 1              |
| 0x02A7            | UInt16   | Shotgun Ammo 1          |
| 0x02A9            | UInt16   | MP5 Ammo 1              |
| 0x02AB            | UInt16   | Rocket Launcher Ammo 1  |
| 0x02AD            | UInt16   | Harpoon Ammo 1          |
| 0x02AF            | UInt16   | Grenade Launcher Ammo 1 |
| 0x02B1            | BYTE     | Small Medipack          |
| 0x02B2            | BYTE     | Large Medipack          |
| 0x02B4            | BYTE     | Flares                  |
| 0x02B8            | BYTE     | Weapons Config Number   |
| 0x02B9            | BYTE     | Harpoon Gun             |
| 0x22F7            | UInt16   | Deagle Ammo 2           |
| 0x22FB            | UInt16   | Uzi Ammo 2              |
| 0x22FF            | UInt16   | Shotgun Ammo 2          |
| 0x2303            | UInt16   | Harpoon Ammo 2          |
| 0x2307            | UInt16   | Rocket Launcher Ammo 2  |
| 0x230B            | UInt16   | Grenade Launcher Ammo 2 |
| 0x230F            | UInt16   | MP5 Ammo 2              |

#### Lud's Gate ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x02D6            | UInt16   | Deagle Ammo 1           |
| 0x02D8            | UInt16   | Uzi Ammo 1              |
| 0x02DA            | UInt16   | Shotgun Ammo 1          |
| 0x02DC            | UInt16   | MP5 Ammo 1              |
| 0x02DE            | UInt16   | Rocket Launcher Ammo 1  |
| 0x02E0            | UInt16   | Harpoon Ammo 1          |
| 0x02E2            | UInt16   | Grenade Launcher Ammo 1 |
| 0x02E4            | BYTE     | Small Medipack          |
| 0x02E5            | BYTE     | Large Medipack          |
| 0x02E7            | BYTE     | Flares                  |
| 0x02EB            | BYTE     | Weapons Config Number   |
| 0x02EC            | BYTE     | Harpoon Gun             |
| 0x1D6F            | UInt16   | Deagle Ammo 2           |
| 0x1D73            | UInt16   | Uzi Ammo 2              |
| 0x1D77            | UInt16   | Shotgun Ammo 2          |
| 0x1D7B            | UInt16   | Harpoon Ammo 2          |
| 0x1D7F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1D83            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1D87            | UInt16   | MP5 Ammo 2              |

#### City ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0309            | UInt16   | Deagle Ammo 1           |
| 0x030B            | UInt16   | Uzi Ammo 1              |
| 0x030D            | UInt16   | Shotgun Ammo 1          |
| 0x030F            | UInt16   | MP5 Ammo 1              |
| 0x0311            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0313            | UInt16   | Harpoon Ammo 1          |
| 0x0315            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0317            | BYTE     | Small Medipack          |
| 0x0318            | BYTE     | Large Medipack          |
| 0x031A            | BYTE     | Flares                  |
| 0x031E            | BYTE     | Weapons Config Number   |
| 0x031F            | BYTE     | Harpoon Gun             |
| 0x0AEB            | UInt16   | Deagle Ammo 2           |
| 0x0AEF            | UInt16   | Uzi Ammo 2              |
| 0x0AF3            | UInt16   | Shotgun Ammo 2          |
| 0x0AF7            | UInt16   | Harpoon Ammo 2          |
| 0x0AFB            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0AFF            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1B03            | UInt16   | MP5 Ammo 2              |

#### Antarctica ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x03D5            | UInt16   | Deagle Ammo 1           |
| 0x03D7            | UInt16   | Uzi Ammo 1              |
| 0x03D9            | UInt16   | Shotgun Ammo 1          |
| 0x03DB            | UInt16   | MP5 Ammo 1              |
| 0x03DD            | UInt16   | Rocket Launcher Ammo 1  |
| 0x03DF            | UInt16   | Harpoon Ammo 1          |
| 0x03E1            | UInt16   | Grenade Launcher Ammo 1 |
| 0x03E3            | BYTE     | Small Medipack          |
| 0x03E4            | BYTE     | Large Medipack          |
| 0x03E6            | BYTE     | Flares                  |
| 0x03EA            | BYTE     | Weapons Config Number   |
| 0x03EB            | BYTE     | Harpoon Gun             |
| 0x198D            | UInt16   | Deagle Ammo 2           |
| 0x1991            | UInt16   | Uzi Ammo 2              |
| 0x1995            | UInt16   | Shotgun Ammo 2          |
| 0x1999            | UInt16   | Harpoon Ammo 2          |
| 0x199D            | UInt16   | Rocket Launcher Ammo 2  |
| 0x19A1            | UInt16   | Grenade Launcher Ammo 2 |
| 0x19A5            | UInt16   | MP5 Ammo 2              |

#### RX-Tech Mines ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x0408            | UInt16   | Deagle Ammo 1           |
| 0x040A            | UInt16   | Uzi Ammo 1              |
| 0x040C            | UInt16   | Shotgun Ammo 1          |
| 0x040E            | UInt16   | MP5 Ammo 1              |
| 0x0410            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0412            | UInt16   | Harpoon Ammo 1          |
| 0x0414            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0416            | BYTE     | Small Medipack          |
| 0x0417            | BYTE     | Large Medipack          |
| 0x0419            | BYTE     | Flares                  |
| 0x041D            | BYTE     | Weapons Config Number   |
| 0x041E            | BYTE     | Harpoon Gun             |
| 0x194F            | UInt16   | Deagle Ammo 2           |
| 0x1953            | UInt16   | Uzi Ammo 2              |
| 0x1957            | UInt16   | Shotgun Ammo 2          |
| 0x195B            | UInt16   | Harpoon Ammo 2          |
| 0x195F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1963            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1967            | UInt16   | MP5 Ammo 2              |

#### Lost City of Tinnos ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x043B            | UInt16   | Deagle Ammo 1           |
| 0x043D            | UInt16   | Uzi Ammo 1              |
| 0x043F            | UInt16   | Shotgun Ammo 1          |
| 0x0441            | UInt16   | MP5 Ammo 1              |
| 0x0443            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0445            | UInt16   | Harpoon Ammo 1          |
| 0x0447            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0449            | BYTE     | Small Medipack          |
| 0x044A            | BYTE     | Large Medipack          |
| 0x044C            | BYTE     | Flares                  |
| 0x0450            | BYTE     | Weapons Config Number   |
| 0x0451            | BYTE     | Harpoon Gun             |
| 0x1D8F            | UInt16   | Deagle Ammo 2           |
| 0x1D93            | UInt16   | Uzi Ammo 2              |
| 0x1D97            | UInt16   | Shotgun Ammo 2          |
| 0x1D9B            | UInt16   | Harpoon Ammo 2          |
| 0x1D9F            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1DA3            | UInt16   | Grenade Launcher Ammo 2 |
| 0x1DA7            | UInt16   | MP5 Ammo 2              |

#### Meteorite Cavern ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x046E            | UInt16   | Deagle Ammo 1           |
| 0x0470            | UInt16   | Uzi Ammo 1              |
| 0x0472            | UInt16   | Shotgun Ammo 1          |
| 0x0474            | UInt16   | MP5 Ammo 1              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0478            | UInt16   | Harpoon Ammo 1          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x047C            | BYTE     | Small Medipack          |
| 0x047D            | BYTE     | Large Medipack          |
| 0x047F            | BYTE     | Flares                  |
| 0x0483            | BYTE     | Weapons Config Number   |
| 0x0484            | BYTE     | Harpoon Gun             |
| 0x0AE1            | UInt16   | Deagle Ammo 2           |
| 0x0AE5            | UInt16   | Uzi Ammo 2              |
| 0x0AE9            | UInt16   | Shotgun Ammo 2          |
| 0x0AED            | UInt16   | Harpoon Ammo 2          |
| 0x0AF1            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0AF5            | UInt16   | Grenade Launcher Ammo 2 |
| 0x0AF9            | UInt16   | MP5 Ammo 2              |

#### All Hallows ####
| **File offset**   | **Type** | **Variable**            |
| :---              | :---     | :---                    |
| 0x0000            | String   | Level Name              |
| 0x004B            | UInt16   | Save Number             |
| 0x046E            | UInt16   | Deagle Ammo 1           |
| 0x0470            | UInt16   | Uzi Ammo 1              |
| 0x0472            | UInt16   | Shotgun Ammo 1          |
| 0x0474            | UInt16   | MP5 Ammo 1              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 1  |
| 0x0478            | UInt16   | Harpoon Ammo 1          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 1 |
| 0x0472            | UInt16   | Shotgun Ammo 2          |
| 0x0474            | UInt16   | MP5 Ammo 2              |
| 0x0476            | UInt16   | Rocket Launcher Ammo 2  |
| 0x0478            | UInt16   | Harpoon Ammo 2          |
| 0x047A            | UInt16   | Grenade Launcher Ammo 2 |
| 0x047C            | BYTE     | Small Medipack          |
| 0x047D            | BYTE     | Large Medipack          |
| 0x047F            | BYTE     | Flares                  |
| 0x0483            | BYTE     | Weapons Config Number   |
| 0x0484            | BYTE     | Harpoon Gun             |
| 0x1025            | UInt16   | Deagle Ammo 2           |
| 0x1029            | UInt16   | Uzi Ammo 2              |
| 0x102D            | UInt16   | Shotgun Ammo 2          |
| 0x1031            | UInt16   | Harpoon Ammo 2          |
| 0x1035            | UInt16   | Rocket Launcher Ammo 2  |
| 0x1039            | UInt16   | Grenade Launcher Ammo 2 |
| 0x103D            | UInt16   | MP5 Ammo 2              |
