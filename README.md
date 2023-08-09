# TRIII-SaveEdit
This is a save game editor for Tomb Raider III. It has been tested to work with the Steam version of the game, but it should work on
the original and multi-patched version as well. To use this program, download **TRIII-SaveEdit.exe** from the **TRIII-SaveEdit**
folder on this repo. No installation is necessary. Once the editor is open, click **Browse**, then navigate to your game directory.
If you are using the Steam version of the game, your folder should be:<br>

```\Program Files (x86)\Steam\steamapps\common\TombRaider (III)\```<br>

Once your save game is selected, you can modify it to your desire. This editor can enable any gun on any level, including the bonus level.
Just click **Save** when you are done, and enjoy.

#### Screenshot of TRIII-SaveEdit
![TRIII-SaveEdit](https://github.com/JulianOzelRose/TRIII-SaveEdit/assets/95890436/9cc84f2c-299e-4265-8aa0-f30bd6beaf54)

## Reversing the save files
Unlike Tomb Raider: Chronicles, the offsets in Tomb Raider III are different in each level. Another interesting difference is that
instead of storing weapons on individual offsets, all weapons information are stored on a single offset, which I call ```weaponsConfigNum```.
The only exception is the harpoon gun, which is stored on its own offset. The weapons configuration variable has a base number of 1, which indicates
no weapons present in inventory. Each weapon adds a unique number to the variable.

###       ```weaponsConfigNum```             ###
| **Weapon**              | **Unique number** |
| :---                    | :---              |
| Pistol                  | 2                 |
| Deagle                  | 4                 |
| Uzi                     | 8                 |
| MP5                     | 32                |
| Rocket Launcher         | 64                |
| Grenade Launcher        | 128               |

When the game loads a save file, it checks this number to determine which weapons are present in inventory. When reverse
engineering this part of the game, we need to choose our methods wisely. Since there are 128 possible combinations, using conditional
operations would be extremely inefficient. Bitwise operations are perfect for this scenario. We account for the base case of no weapons,
and then we use our else block to check if the unique bit of a wepaon is present in the config variable, and set our variables to display accordingly.

```
const int Pistol = 2;
const int Shotgun = 16;
const int Deagle = 4;
const int Uzi = 8;
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
## Save Game Hex Tables
Below are the save game offset tables. Note that the offsets differ on every level, except for the level name and save number variables.
Ammunition for each weapon is stored in two separate offsets. It appears that one is for ammunition when the weapon is not equipped,
and the other is for when the weapon is equipped, showing up as ammunition.

#### Jungle level ####
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
| Rocket Launcher Ammo 1  | 0x1657            |
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
| Shotgun Ammo 2          | 0x23CD            |
| Deagle Ammo 1           | 0x010B            |
| Deagle Ammo 2	          | 0x23C5            |
| Grenade Launcher Ammo 1 | 0x0117            |
| Grenade Launcher Ammo 2 | 0x23D9            |
| Rocket Launcher Ammo 1  | 0x0113            |
| Rocket Launcher Ammo 2  | 0x23D5            |
| Harpoon Ammo 1          | 0x0115            |
| Harpoon Ammo 2          | 0x23D1            |
| MP5 Ammo 1              | 0x0111            |
| MP5 Ammo 2              | 0x23DD            |
| Uzi Ammo 1              | 0x010D            |
| Uzi Ammo 2              | 0x23C9            |

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
| Harpoon Ammo 2          | 0x1814            |
| MP5 Ammo 1              | 0x0144            |
| MP5 Ammo 2              | 0x1808            |
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
| Harpoon Ammo 2          | 0x0D2F            |
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
| Shotgun Ammo 2          | 0x17DA            |
| Deagle Ammo 1           | 0x033C            |
| Deagle Ammo 2	          | 0x17D2            |
| Grenade Launcher Ammo 1 | 0x0344            |
| Grenade Launcher Ammo 2 | 0x17E6            |
| Rocket Launcher Ammo 1  | 0x0344            |
| Rocket Launcher Ammo 2  | 0x17E2            |
| Harpoon Ammo 1          | 0x0346            |
| Harpoon Ammo 2          | 0x17E2            |
| MP5 Ammo 1              | 0x0346            |
| MP5 Ammo 2              | 0x17EA            |
| Uzi Ammo 1              | 0x033E            |
| Uzi Ammo 2              | 0x17D6            |

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
| Harpoon Ammo 2          | 0x1E5B            |
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
| Shotgun Ammo 2          | 0x211F            |
| Deagle Ammo 1           | 0x03A2            |
| Deagle Ammo 2	          | 0x2117            |
| Grenade Launcher Ammo 1 | 0x03AE            |
| Grenade Launcher Ammo 2 | 0x212B            |
| Rocket Launcher Ammo 1  | 0x03AA            |
| Rocket Launcher Ammo 2  | 0x2127            |
| Harpoon Ammo 1          | 0x03AC            |
| Harpoon Ammo 2          | 0x2123            |
| MP5 Ammo 1              | 0x03A8            |
| MP5 Ammo 2              | 0x212F            |
| Uzi Ammo 1              | 0x03A4            |
| Uzi Ammo 2              | 0x211B            |

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
| Rocket Launcher Ammo 1  | 0x01B0            |
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
| Rocket Launcher Ammo 1  | 0x01E3            |
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
| Shotgun Ammo 2          | 0x1897            |
| Deagle Ammo 1           | 0x0270            |
| Deagle Ammo 2	          | 0x188F            |
| Grenade Launcher Ammo 1 | 0x027C            |
| Grenade Launcher Ammo 2 | 0x18A3            |
| Rocket Launcher Ammo 1  | 0x0278            |
| Rocket Launcher Ammo 2  | 0x189F            |
| Harpoon Ammo 1          | 0x027A            |
| Harpoon Ammo 2          | 0x189B            |
| MP5 Ammo 1              | 0x0276            |
| MP5 Ammo 2              | 0x18A7            |
| Uzi Ammo 1              | 0x0272            |
| Uzi Ammo 2              | 0x1893            |

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
| Deagle Ammo 1           | 0x02D6            |
| Deagle Ammo 2	          | 0x1D81            |
| Grenade Launcher Ammo 1 | 0x02E2            |
| Grenade Launcher Ammo 2 | 0x1D95            |
| Rocket Launcher Ammo 1  | 0x02D2            |
| Rocket Launcher Ammo 2  | 0x1D91            |
| Harpoon Ammo 1          | 0x02E0            |
| Harpoon Ammo 2          | 0x1D8D            |
| MP5 Ammo 1              | 0x02DC            |
| MP5 Ammo 2              | 0x1D99            |
| Uzi Ammo 1              | 0x02D8            |
| Uzi Ammo 2              | 0x1D85            |

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
| Shotgun Ammo 1          | 0x030D            |
| Shotgun Ammo 2          | 0x199D            |
| Deagle Ammo 1           | 0x03D5            |
| Deagle Ammo 2	          | 0x198D            |
| Grenade Launcher Ammo 1 | 0x0315            |
| Grenade Launcher Ammo 2 | 0x19A1            |
| Rocket Launcher Ammo 1  | 0x0311            |
| Rocket Launcher Ammo 2  | 0x199D            |
| Harpoon Ammo 1          | 0x0313            |
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
| Rocket Launcher Ammo 2  | 0x1963            |
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
| Shotgun Ammo 2          | 0x0AFB            |
| Deagle Ammo 1           | 0x046E            |
| Deagle Ammo 2	          | 0x0AF3            |
| Grenade Launcher Ammo 1 | 0x047A            |
| Grenade Launcher Ammo 2 | 0x0B07            |
| Rocket Launcher Ammo 1  | 0x0476            |
| Rocket Launcher Ammo 2  | 0x0B03            |
| Harpoon Ammo 1          | 0x0478            |
| Harpoon Ammo 2          | 0x0AFF            |
| MP5 Ammo 1              | 0x0474            |
| MP5 Ammo 2              | 0x0B0B            |
| Uzi Ammo 1              | 0x0470            |
| Uzi Ammo 2              | 0x0AF7            |

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
