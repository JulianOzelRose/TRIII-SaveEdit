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

#####       Weapons Config Table             #####
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

####       Jungle level offsets             ####
| **Variable**            | **File offset**   |
| :---                    | :---              |
| Level Name              | 0x0000            |
| Save Number             | 0x004B            |
| Small Medipack          | 0x00E6            |
| Large Medipack          | 0x00E7            |
| Flares                  | 0x00E9            |
| Weapons Config Number   | 0x00ED            |
| Harpoon gun             | 0x00EE            |
| Shotgun ammo 1          | 0x00DC            |
| Shotgun ammo 2          | 0x164B            |
| Deagle ammo	1           | 0x00D8            |
| Deagle ammo 2	          | 0x1643            |
| Grenade launcher ammo 1 | 0x00E4            |
| Grenade launcher ammo 2 | 0x1657            |
| Rocket launcher ammo 1  | 0x1657            |
| Rocket launcher ammo 2  | 0x1653            |
| Harpoon ammo 1          | 0x00E2            |
| Harpoon ammo 2          | 0x164F            |
| MP5 ammo 1              | 0x00DE            |
| MP5 ammo 2              | 0x165B            |
| Uzi ammo 1              | 0x00DA            |
| Uzi ammo 2              | 0x1647            |
