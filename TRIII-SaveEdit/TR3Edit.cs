/*
    Julian O. Rose
    TR3Edit.cs
    8-8-2023
*/

using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace TRIII_SaveEdit
{
    public partial class TR3Edit : Form
    {
        public TR3Edit()
        {
            InitializeComponent();

            SaveButton.Enabled = false;
            pistolsCheckBox.Enabled = false;
            deagleCheckBox.Enabled = false;
            deagleAmmoNumBox.Enabled = false;
            grenadeLauncherAmmoNumBox.Enabled = false;
            grenadeLauncherCheckBox.Enabled = false;
            mp5AmmoNumBox.Enabled = false;
            mp5CheckBox.Enabled = false;
            harpoonGunCheckBox.Enabled = false;
            harpoonGunAmmoNumBox.Enabled = false;
            uziAmmoNumBox.Enabled = false;
            uziCheckBox.Enabled = false;
            rocketLauncherAmmoNumBox.Enabled = false;
            rocketLauncherCheckBox.Enabled = false;
            shotgunAmmoNumBox.Enabled = false;
            shotgunCheckBox.Enabled = false;
            lrgMedipacksNumBox.Enabled = false;
            smallMedipacksNumBox.Enabled = false;
            flaresNumBox.Enabled = false;
            saveNumBox.Enabled = false;

            smallMedipacksNumBox.Maximum = 255;
            lrgMedipacksNumBox.Maximum = 255;
            flaresNumBox.Maximum = 255;
            saveNumBox.Maximum = 65535;
            shotgunAmmoNumBox.Maximum = 10922;
            rocketLauncherAmmoNumBox.Maximum = 65535;
            harpoonGunAmmoNumBox.Maximum = 65535;
            grenadeLauncherAmmoNumBox.Maximum = 65535;
            uziAmmoNumBox.Maximum = 65535;
            mp5AmmoNumBox.Maximum = 65535;
            deagleAmmoNumBox.Maximum = 65535;
        }

        void SetSaveFilePath(string filePath)
        {
            sSaveFilePath = filePath;
        }

        string GetSaveFilePath()
        {
            return sSaveFilePath;
        }

        string GetLvlName()
        {
            string saveFilePath = GetSaveFilePath();

            using (FileStream saveFileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (saveFileStream.CanRead)
                {
                    using (StreamReader saveFileReader = new StreamReader(saveFileStream))
                    {
                        return saveFileReader.ReadLine().Trim();
                    }
                }
            }

            return null;
        }

        string GetCleanLvlName()
        {
            string lvlName = GetLvlName();
            lvlName = lvlName.Trim();

            if (lvlName.StartsWith("Jungle")) return "Jungle";
            else if (lvlName.StartsWith("Temple Ruins")) return "Temple Ruins";
            else if (lvlName.StartsWith("The River Ganges")) return "The River Ganges";
            else if (lvlName.StartsWith("Caves Of Kaliya")) return "Caves Of Kaliya";
            else if (lvlName.StartsWith("Nevada Desert")) return "Nevada Desert";
            else if (lvlName.StartsWith("High Security Compound")) return "High Security Compound";
            else if (lvlName.StartsWith("Area 51")) return "Area 51";
            else if (lvlName.StartsWith("Coastal Village")) return "Coastal Village";
            else if (lvlName.StartsWith("Crash Site")) return "Crash Site";
            else if (lvlName.StartsWith("Madubu Gorge")) return "Madubu Gorge";
            else if (lvlName.StartsWith("Temple Of Puna")) return "Temple Of Puna";
            else if (lvlName.StartsWith("Thames Wharf")) return "Thames Wharf";
            else if (lvlName.StartsWith("Aldwych")) return "Aldwych";
            else if (lvlName.StartsWith("Lud's Gate")) return "Lud's Gate";
            else if (lvlName.StartsWith("City")) return "City";
            else if (lvlName.StartsWith("Antarctica")) return "Antarctica";
            else if (lvlName.StartsWith("RX-Tech Mines")) return "RX-Tech Mines";
            else if (lvlName.StartsWith("Lost City Of Tinnos")) return "Lost City Of Tinnos";
            else if (lvlName.StartsWith("Meteorite Cavern")) return "Meteorite Cavern";
            else if (lvlName.StartsWith("All Hallows")) return "All Hallows";

            return null;
        }

        byte GetSaveFileData(long offset)
        {
            string saveFilePath = GetSaveFilePath();

            using (FileStream saveFile = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                return (byte)saveFile.ReadByte();
            }
        }

        void WriteToSaveFile(long offset, int value)
        {
            string saveFilePath = GetSaveFilePath();

            using (FileStream saveFile = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                byte[] byteData = { (byte)value };
                saveFile.Write(byteData, 0, byteData.Length);
            }    
        }

        int GetAmmoValue(long offset)
        {
            int firstHalf = GetSaveFileData(offset);
            int secondHalf = GetSaveFileData(offset + 1);

            int result = firstHalf + (secondHalf << 8);

            return result;
        }

        void WriteAmmoValue(long offset, int value)
        {
            if (value > 255)
            {
                byte firstHalf = (byte)(value / 256);
                byte secondHalf = (byte)(value % 256);

                WriteToSaveFile(offset + 1, firstHalf);
                WriteToSaveFile(offset, secondHalf);
            }
            else
            {
                WriteToSaveFile(offset + 1, 0);
                WriteToSaveFile(offset, (byte)value);
            }
        }

        int[] GetValidAmmoOffsets(int primaryOffset, int baseSecondaryOffset)
        {
            List<int> secondaryOffsets = new List<int>();
            List<int> validOffsets = new List<int>();

            int primaryAmmoValue = GetAmmoValue(primaryOffset);

            for (int i = 0; i < 10; i++)
            {
                secondaryOffsets.Add(baseSecondaryOffset + i * 0x12);
            }

            for (int i = 0; i < secondaryOffsets.Count; i++)
            {
                int ammoValue = GetAmmoValue(secondaryOffsets[i]);

                if (primaryAmmoValue == ammoValue && ammoValue != 0)
                {
                    validOffsets.Add(secondaryOffsets[i]);
                }
            }
            
            if (validOffsets.Count == 0)
            {
                for (int i = 0; i < secondaryOffsets.Count; i++)
                {
                    if (GetAmmoValue(secondaryOffsets[i] - 2) == 0 && GetAmmoValue(secondaryOffsets[i] + 1) == 0)
                    {
                        validOffsets.Add(secondaryOffsets[i]);
                    }
                }
            }

            validOffsets.Add(primaryOffset);
            return validOffsets.ToArray();
        }

        void GetLvlInfo()
        {
            string lvlName = GetLvlName();
            lvlNameTxtBox.Clear();
            lvlNameTxtBox.AppendText(lvlName);
        }

        void GetNumSmallMedipacks()
        {
            int numSmallMedipacks = GetSaveFileData(smallMedipackOffset);
            smallMedipacksNumBox.Value = numSmallMedipacks;
        }

        void GetNumLargeMedipacks()
        {
            int numLargeMedipacks = GetSaveFileData(largeMedipackOffset);
            lrgMedipacksNumBox.Value = numLargeMedipacks;
        }

        void GetShotgunAmmo()
        {
            int shotgunAmmo = GetAmmoValue(shotgunAmmoOffset);
            shotgunAmmoNumBox.Value = shotgunAmmo / 6;
        }

        void GetDeagleAmmo()
        {
            int deagleAmmo = GetAmmoValue(deagleAmmoOffset);
            deagleAmmoNumBox.Value = deagleAmmo;
        }

        void GetMP5Ammo()
        {
            int mp5Ammo = GetAmmoValue(mp5AmmoOffset);
            mp5AmmoNumBox.Value = mp5Ammo;
        }

        void GetNumFlares()
        {
            int numFlares = GetSaveFileData(numFlaresOffset);
            flaresNumBox.Value = numFlares;
        }

        void GetUziAmmo()
        {
            int uziAmmo = GetAmmoValue(uziAmmoOffset);
            uziAmmoNumBox.Value = uziAmmo;
        }

        void GetGrenadeLauncherAmmo()
        {
            int grenadeLauncherAmmo = GetAmmoValue(grenadeLauncherAmmoOffset);
            grenadeLauncherAmmoNumBox.Value = grenadeLauncherAmmo;
        }

        void GetHarpoonAmmo()
        {
            int harpoonAmmo = GetAmmoValue(harpoonAmmoOffset);
            harpoonGunAmmoNumBox.Value = harpoonAmmo;
        }

        void GetRocketLauncherAmmo()
        {
            int rocketLauncherAmmo = GetAmmoValue(rocketLauncherAmmoOffset);
            rocketLauncherAmmoNumBox.Value = rocketLauncherAmmo;
        }

        void GetSaveNum()
        {
            int saveNum = GetAmmoValue(saveNumOffset);
            saveNumBox.Value = saveNum;
        }

        void GetWeaponsInfo()
        {
            if (GetCleanLvlName() == "Jungle")
            {
                // Health offsets
                smallMedipackOffset = 0xE6;
                largeMedipackOffset = 0xE7;

                // Misc offsets
                numFlaresOffset = 0xE9;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0xED;
                harpoonGunOffset = 0xEE;

                // Ammo offsets
                shotgunAmmoOffset = 0xDC;
                shotgunAmmoOffset2 = 0x1639;
                deagleAmmoOffset = 0xD8;
                deagleAmmoOffset2 = 0x1631;
                grenadeLauncherAmmoOffset = 0xE4;
                grenadeLauncherAmmoOffset2 = 0x1657;
                rocketLauncherAmmoOffset = 0xE0;
                rocketLauncherAmmoOffset2 = 0x1653;
                harpoonAmmoOffset = 0xE2;
                harpoonAmmoOffset2 = 0x1619;
                mp5AmmoOffset = 0xDE;
                mp5AmmoOffset2 = 0x1649;
                uziAmmoOffset = 0xDA;
                uziAmmoOffset2 = 0x1647;
            }

            else if (GetCleanLvlName() == "Temple Ruins")
            {
                // Health offsets
                smallMedipackOffset = 0x119;
                largeMedipackOffset = 0x11A;

                // Misc offsets
                numFlaresOffset = 0x11C;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x120;
                harpoonGunOffset = 0x121;

                // Ammo offsets
                shotgunAmmoOffset = 0x10F;
                shotgunAmmoOffset2 = 0x23BB;
                deagleAmmoOffset = 0x10B;
                deagleAmmoOffset2 = 0x23B3;
                grenadeLauncherAmmoOffset = 0x117;
                grenadeLauncherAmmoOffset2 = 0x23C7;
                rocketLauncherAmmoOffset = 0x113;
                rocketLauncherAmmoOffset2 = 0x23C3;
                harpoonAmmoOffset = 0x115;
                harpoonAmmoOffset2 = 0x23BF;
                mp5AmmoOffset = 0x111;
                mp5AmmoOffset2 = 0x23CB;
                uziAmmoOffset = 0x10D;
                uziAmmoOffset2 = 0x23B7;
            }

            else if (GetCleanLvlName() == "The River Ganges")
            {
                // Health offsets
                smallMedipackOffset = 0x14C;
                largeMedipackOffset = 0x14D;

                // Misc offsets
                numFlaresOffset = 0x14F;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x153;
                harpoonGunOffset = 0x154;

                // Ammo offsets
                shotgunAmmoOffset = 0x142;
                shotgunAmmoOffset2 = 0x1804;
                deagleAmmoOffset = 0x13E;
                deagleAmmoOffset2 = 0x17FC;
                grenadeLauncherAmmoOffset = 0x14A;
                grenadeLauncherAmmoOffset2 = 0x1810;
                rocketLauncherAmmoOffset = 0x146;
                rocketLauncherAmmoOffset2 = 0x180C;
                harpoonAmmoOffset = 0x148;
                harpoonAmmoOffset2 = 0x1808;
                mp5AmmoOffset = 0x144;
                mp5AmmoOffset2 = 0x1814;
                uziAmmoOffset = 0x140;
                uziAmmoOffset2 = 0x1800;
            }

            else if (GetCleanLvlName() == "Caves Of Kaliya")
            {
                // Health offsets
                smallMedipackOffset = 0x17F;
                largeMedipackOffset = 0x180;

                // Misc offsets
                numFlaresOffset = 0x182;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x186;
                harpoonGunOffset = 0x187;

                // Ammo offsets
                shotgunAmmoOffset = 0x175;
                shotgunAmmoOffset2 = 0xD1F;
                deagleAmmoOffset = 0x171;
                deagleAmmoOffset2 = 0xD17;
                grenadeLauncherAmmoOffset = 0x17D;
                grenadeLauncherAmmoOffset2 = 0xD2B;
                rocketLauncherAmmoOffset = 0x179;
                rocketLauncherAmmoOffset2 = 0xD27;
                harpoonAmmoOffset = 0x17B;
                harpoonAmmoOffset2 = 0xD23;
                mp5AmmoOffset = 0x177;
                mp5AmmoOffset2 = 0xD2F;
                uziAmmoOffset = 0x173;
                uziAmmoOffset2 = 0xD1B;
            }

            else if (GetCleanLvlName() == "Nevada Desert")
            {
                // Health offsets
                smallMedipackOffset = 0x34A;
                largeMedipackOffset = 0x34B;

                // Misc offsets
                numFlaresOffset = 0x34A;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x351;
                harpoonGunOffset = 0x352;

                // Ammo offsets
                shotgunAmmoOffset = 0x340;
                shotgunAmmoOffset2 = 0x17A4;
                deagleAmmoOffset = 0x33C;
                deagleAmmoOffset2 = 0x179C;
                grenadeLauncherAmmoOffset = 0x348;
                grenadeLauncherAmmoOffset2 = 0x17B0;
                rocketLauncherAmmoOffset = 0x344;
                rocketLauncherAmmoOffset2 = 0x17AC;
                harpoonAmmoOffset = 0x346;
                harpoonAmmoOffset2 = 0x17A8;
                mp5AmmoOffset = 0x342;
                mp5AmmoOffset2 = 0x17B4;
                uziAmmoOffset = 0x33E;
                uziAmmoOffset2 = 0x17A0;
            }

            else if (GetCleanLvlName() == "High Security Compound")
            {
                // Health offsets
                smallMedipackOffset = 0x37D;
                largeMedipackOffset = 0x37E;

                // Misc offsets
                numFlaresOffset = 0x380;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x384;
                harpoonGunOffset = 0x385;

                // Ammo offsets
                shotgunAmmoOffset = 0x373;
                shotgunAmmoOffset2 = 0x1E4B;
                deagleAmmoOffset = 0x36F;
                deagleAmmoOffset2 = 0x1E43;
                grenadeLauncherAmmoOffset = 0x37B;
                grenadeLauncherAmmoOffset2 = 0x1E57;
                rocketLauncherAmmoOffset = 0x377;
                rocketLauncherAmmoOffset2 = 0x1E53;
                harpoonAmmoOffset = 0x379;
                harpoonAmmoOffset2 = 0x1E4F;
                mp5AmmoOffset = 0x375;
                mp5AmmoOffset2 = 0x1E5B;
                uziAmmoOffset = 0x371;
                uziAmmoOffset2 = 0x1E47;
            }

            else if (GetCleanLvlName() == "Area 51")
            {
                // Health offsets
                smallMedipackOffset = 0x3B0;
                largeMedipackOffset = 0x3B1;

                // Misc offsets
                numFlaresOffset = 0x3B3;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x3B7;
                harpoonGunOffset = 0x3B8;

                // Ammo offsets
                shotgunAmmoOffset = 0x3A6;
                shotgunAmmoOffset2 = 0x210D;
                deagleAmmoOffset = 0x3A2;
                deagleAmmoOffset2 = 0x2105;
                grenadeLauncherAmmoOffset = 0x3AE;
                grenadeLauncherAmmoOffset2 = 0x2119;
                rocketLauncherAmmoOffset = 0x3AA;
                rocketLauncherAmmoOffset2 = 0x2115;
                harpoonAmmoOffset = 0x3AC;
                harpoonAmmoOffset2 = 0x2111;
                mp5AmmoOffset = 0x3A8;
                mp5AmmoOffset2 = 0x211D;
                uziAmmoOffset = 0x3A4;
                uziAmmoOffset2 = 0x2109;
            }

            else if (GetCleanLvlName() == "Coastal Village")
            {
                // Health offsets
                smallMedipackOffset = 0x1B2;
                largeMedipackOffset = 0x1B3;

                // Misc offsets
                numFlaresOffset = 0x1B5;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x1B9;
                harpoonGunOffset = 0x1BA;

                // Ammo offsets
                shotgunAmmoOffset = 0x1A8;
                shotgunAmmoOffset2 = 0x17B1;
                deagleAmmoOffset = 0x1A4;
                deagleAmmoOffset2 = 0x17A9;
                grenadeLauncherAmmoOffset = 0x1B0;
                grenadeLauncherAmmoOffset2 = 0x17BD;
                rocketLauncherAmmoOffset = 0x1AC;
                rocketLauncherAmmoOffset2 = 0x17B9;
                harpoonAmmoOffset = 0x1AE;
                harpoonAmmoOffset2 = 0x17B5;
                mp5AmmoOffset = 0x1AA;
                mp5AmmoOffset2 = 0x17C1;
                uziAmmoOffset = 0x1A6;
                uziAmmoOffset2 = 0x17AD;
            }

            else if (GetCleanLvlName() == "Crash Site")
            {
                // Health offsets
                smallMedipackOffset = 0x1E5;
                largeMedipackOffset = 0x1E6;

                // Misc offsets
                numFlaresOffset = 0x1E8;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x1EC;
                harpoonGunOffset = 0x1ED;

                // Ammo offsets
                shotgunAmmoOffset = 0x1DB;
                shotgunAmmoOffset2 = 0x18D3;
                deagleAmmoOffset = 0x1D7;
                deagleAmmoOffset2 = 0x18CB;
                grenadeLauncherAmmoOffset = 0x1E3;
                grenadeLauncherAmmoOffset2 = 0x18DF;
                rocketLauncherAmmoOffset = 0x1DF;
                rocketLauncherAmmoOffset2 = 0x18DB;
                harpoonAmmoOffset = 0x1E1;
                harpoonAmmoOffset2 = 0x18D7;
                mp5AmmoOffset = 0x1DD;
                mp5AmmoOffset2 = 0x18E3;
                uziAmmoOffset = 0x1D9;
                uziAmmoOffset2 = 0x18CF;
            }

            else if (GetCleanLvlName() == "Madubu Gorge")
            {
                // Health offsets
                smallMedipackOffset = 0x218;
                largeMedipackOffset = 0x219;

                // Misc offsets
                numFlaresOffset = 0x21B;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x21F;
                harpoonGunOffset = 0x220;

                // Ammo offsets
                shotgunAmmoOffset = 0x20E;
                shotgunAmmoOffset2 = 0x141D;
                deagleAmmoOffset = 0x20A;
                deagleAmmoOffset2 = 0x1415;
                grenadeLauncherAmmoOffset = 0x216;
                grenadeLauncherAmmoOffset2 = 0x1429;
                rocketLauncherAmmoOffset = 0x212;
                rocketLauncherAmmoOffset2 = 0x1425;
                harpoonAmmoOffset = 0x214;
                harpoonAmmoOffset2 = 0x1421;
                mp5AmmoOffset = 0x210;
                mp5AmmoOffset2 = 0x142D;
                uziAmmoOffset = 0x20C;
                uziAmmoOffset2 = 0x1419;
            }

            else if (GetCleanLvlName() == "Temple Of Puna")
            {
                // Health offsets
                smallMedipackOffset = 0x24B;
                largeMedipackOffset = 0x24C;

                // Misc offsets
                numFlaresOffset = 0x24E;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x252;
                harpoonGunOffset = 0x253;

                // Ammo offsets
                shotgunAmmoOffset = 0x241;
                shotgunAmmoOffset2 = 0x10F5;
                deagleAmmoOffset = 0x23D;
                deagleAmmoOffset2 = 0x10ED;
                grenadeLauncherAmmoOffset = 0x249;
                grenadeLauncherAmmoOffset2 = 0x1101;
                rocketLauncherAmmoOffset = 0x245;
                rocketLauncherAmmoOffset2 = 0x10FD;
                harpoonAmmoOffset = 0x247;
                harpoonAmmoOffset2 = 0x10F9;
                mp5AmmoOffset = 0x243;
                mp5AmmoOffset2 = 0x1105;
                uziAmmoOffset = 0x23F;
                uziAmmoOffset2 = 0x10F1;
            }

            else if (GetCleanLvlName() == "Thames Wharf")
            {
                // Health offsets
                smallMedipackOffset = 0x27E;
                largeMedipackOffset = 0x27F;

                // Misc offsets
                numFlaresOffset = 0x281;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x285;
                harpoonGunOffset = 0x286;

                // Ammo offsets
                shotgunAmmoOffset = 0x274;
                shotgunAmmoOffset2 = 0x1873;
                deagleAmmoOffset = 0x270;
                deagleAmmoOffset2 = 0x186B;
                grenadeLauncherAmmoOffset = 0x27C;
                grenadeLauncherAmmoOffset2 = 0x187F;
                rocketLauncherAmmoOffset = 0x278;
                rocketLauncherAmmoOffset2 = 0x187B;
                harpoonAmmoOffset = 0x27A;
                harpoonAmmoOffset2 = 0x1877;
                mp5AmmoOffset = 0x276;
                mp5AmmoOffset2 = 0x1883;
                uziAmmoOffset = 0x272;
                uziAmmoOffset2 = 0x186F;
            }

            else if (GetCleanLvlName() == "Aldwych")
            {
                // Health offsets
                smallMedipackOffset = 0x2B1;
                largeMedipackOffset = 0x2B2;

                // Misc offsets
                numFlaresOffset = 0x2B4;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x2B8;
                harpoonGunOffset = 0x2B9;

                // Ammo offsets
                shotgunAmmoOffset = 0x2A7;
                shotgunAmmoOffset2 = 0x22FF;
                deagleAmmoOffset = 0x2A3;
                deagleAmmoOffset2 = 0x22F7;
                grenadeLauncherAmmoOffset = 0x2AF;
                grenadeLauncherAmmoOffset2 = 0x230B;
                rocketLauncherAmmoOffset = 0x2AB;
                rocketLauncherAmmoOffset2 = 0x2307;
                harpoonAmmoOffset = 0x2AD;
                harpoonAmmoOffset2 = 0x2303;
                mp5AmmoOffset = 0x2A9;
                mp5AmmoOffset2 = 0x230F;
                uziAmmoOffset = 0x2A5;
                uziAmmoOffset2 = 0x22FB;
            }

            else if (GetCleanLvlName() == "Lud's Gate")
            {
                // Health offsets
                smallMedipackOffset = 0x2E4;
                largeMedipackOffset = 0x2E5;

                // Misc offsets
                numFlaresOffset = 0x2E7;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x2EB;
                harpoonGunOffset = 0x2EC;

                // Ammo offsets
                shotgunAmmoOffset = 0x2DA;
                shotgunAmmoOffset2 = 0x1D89;
                deagleAmmoOffset = 0x2D6;
                deagleAmmoOffset2 = 0x1D81;
                grenadeLauncherAmmoOffset = 0x2E2;
                grenadeLauncherAmmoOffset2 = 0x1D95;
                rocketLauncherAmmoOffset = 0x2DE;
                rocketLauncherAmmoOffset2 = 0x1D91;
                harpoonAmmoOffset = 0x2E0;
                harpoonAmmoOffset2 = 0x1D8D;
                mp5AmmoOffset = 0x2DC;
                mp5AmmoOffset2 = 0x1D99;
                uziAmmoOffset = 0x2D8;
                uziAmmoOffset2 = 0x1D85;
            }

            else if (GetCleanLvlName() == "City")
            {
                // Health offsets
                smallMedipackOffset = 0x317;
                largeMedipackOffset = 0x318;

                // Misc offsets
                numFlaresOffset = 0x31A;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x31E;
                harpoonGunOffset = 0x31F;

                // Ammo offsets
                shotgunAmmoOffset = 0x30D;
                shotgunAmmoOffset2 = 0xAF3;
                deagleAmmoOffset = 0x309;
                deagleAmmoOffset2 = 0xAEB;
                grenadeLauncherAmmoOffset = 0x315;
                grenadeLauncherAmmoOffset2 = 0xAFF;
                rocketLauncherAmmoOffset = 0x311;
                rocketLauncherAmmoOffset2 = 0xAFB;
                harpoonAmmoOffset = 0x313;
                harpoonAmmoOffset2 = 0xAF7;
                mp5AmmoOffset = 0x30F;
                mp5AmmoOffset2 = 0xB03;
                uziAmmoOffset = 0x30B;
                uziAmmoOffset2 = 0xAEF;
            }

            else if (GetCleanLvlName() == "Antarctica")
            {
                // Health offsets
                smallMedipackOffset = 0x3E3;
                largeMedipackOffset = 0x3E4;

                // Misc offsets
                numFlaresOffset = 0x3E6;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x3EA;
                harpoonGunOffset = 0x3EB;

                // Ammo offsets
                shotgunAmmoOffset = 0x3D9;
                shotgunAmmoOffset2 = 0x1995;
                deagleAmmoOffset = 0x3D5;
                deagleAmmoOffset2 = 0x198D;
                grenadeLauncherAmmoOffset = 0x3E1;
                grenadeLauncherAmmoOffset2 = 0x19A1;
                rocketLauncherAmmoOffset = 0x3DD;
                rocketLauncherAmmoOffset2 = 0x199D;
                harpoonAmmoOffset = 0x3DF;
                harpoonAmmoOffset2 = 0x1999;
                mp5AmmoOffset = 0x3DB;
                mp5AmmoOffset2 = 0x19A5;
                uziAmmoOffset = 0x3D7;
                uziAmmoOffset2 = 0x1991;
            }

            else if (GetCleanLvlName() == "RX-Tech Mines")
            {
                // Health offsets
                smallMedipackOffset = 0x416;
                largeMedipackOffset = 0x417;

                // Misc offsets
                numFlaresOffset = 0x419;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x41D;
                harpoonGunOffset = 0x41E;

                // Ammo offsets
                shotgunAmmoOffset = 0x40C;
                shotgunAmmoOffset2 = 0x1957;
                deagleAmmoOffset = 0x408;
                deagleAmmoOffset2 = 0x194F;
                grenadeLauncherAmmoOffset = 0x414;
                grenadeLauncherAmmoOffset2 = 0x1963;
                rocketLauncherAmmoOffset = 0x410;
                rocketLauncherAmmoOffset2 = 0x195F;
                harpoonAmmoOffset = 0x412;
                harpoonAmmoOffset2 = 0x195B;
                mp5AmmoOffset = 0x40E;
                mp5AmmoOffset2 = 0x1967;
                uziAmmoOffset = 0x40A;
                uziAmmoOffset2 = 0x1953;
            }

            else if (GetCleanLvlName() == "Lost City Of Tinnos")
            {
                // Health offsets
                smallMedipackOffset = 0x449;
                largeMedipackOffset = 0x44A;

                // Misc offsets
                numFlaresOffset = 0x44C;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x450;
                harpoonGunOffset = 0x451;

                // Ammo offsets
                shotgunAmmoOffset = 0x43F;
                shotgunAmmoOffset2 = 0x1D97;
                deagleAmmoOffset = 0x43B;
                deagleAmmoOffset2 = 0x1D8F;
                grenadeLauncherAmmoOffset = 0x447;
                grenadeLauncherAmmoOffset2 = 0x1DA3;
                rocketLauncherAmmoOffset = 0x443;
                rocketLauncherAmmoOffset2 = 0x1D9F;
                harpoonAmmoOffset = 0x445;
                harpoonAmmoOffset2 = 0x1D9B;
                mp5AmmoOffset = 0x441;
                mp5AmmoOffset2 = 0x1DA7;
                uziAmmoOffset = 0x43D;
                uziAmmoOffset2 = 0x1D93;
            }

            else if (GetCleanLvlName() == "Meteorite Cavern")
            {
                // Health offsets
                smallMedipackOffset = 0x47C;
                largeMedipackOffset = 0x47D;

                // Misc offsets
                numFlaresOffset = 0x47F;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x483;
                harpoonGunOffset = 0x484;

                // Ammo offsets
                shotgunAmmoOffset = 0x472;
                shotgunAmmoOffset2 = 0xAE9;
                deagleAmmoOffset = 0x46E;
                deagleAmmoOffset2 = 0xAE1;
                grenadeLauncherAmmoOffset = 0x47A;
                grenadeLauncherAmmoOffset2 = 0xAF5;
                rocketLauncherAmmoOffset = 0x476;
                rocketLauncherAmmoOffset2 = 0xAF1;
                harpoonAmmoOffset = 0x478;
                harpoonAmmoOffset2 = 0xAED;
                mp5AmmoOffset = 0x474;
                mp5AmmoOffset2 = 0xAF9;
                uziAmmoOffset = 0x470;
                uziAmmoOffset2 = 0xAE5;
            }

            else if (GetCleanLvlName() == "All Hallows")
            {
                // Health offsets
                smallMedipackOffset = 0x4AF;
                largeMedipackOffset = 0x4B0;

                // Misc offsets
                numFlaresOffset = 0x4B2;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x4B6;
                harpoonGunOffset = 0x4B7;

                // Ammo offsets
                shotgunAmmoOffset = 0x472;
                shotgunAmmoOffset2 = 0x102D;
                deagleAmmoOffset = 0x46E;
                deagleAmmoOffset2 = 0x1025;
                grenadeLauncherAmmoOffset = 0x47A;
                grenadeLauncherAmmoOffset2 = 0x1039;
                rocketLauncherAmmoOffset = 0x476;
                rocketLauncherAmmoOffset2 = 0x1035;
                harpoonAmmoOffset = 0x478;
                harpoonAmmoOffset2 = 0x1031;
                mp5AmmoOffset = 0x4A7;
                mp5AmmoOffset2 = 0x103D;
                uziAmmoOffset = 0x4A3;
                uziAmmoOffset2 = 0x1029;
            }

            // Update weapons vars
            int weaponsConfigNum = GetSaveFileData(weaponsConfigNumOffset);
            int harpoonGunVal = GetSaveFileData(harpoonGunOffset);

            // Define weapons config constants
            const int Pistol = 2;
            const int Deagle = 4;
            const int Uzi = 8;
            const int Shotgun = 16;
            const int MP5 = 32;
            const int RocketLauncher = 64;
            const int GrenadeLauncher = 128;

            // Update weapons checkboxes
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

            // Update harpoon gun checkbox
            if (harpoonGunVal == 1) harpoonGunCheckBox.Checked = true;
            else harpoonGunCheckBox.Checked = false;

            // Get remaining values
            GetNumSmallMedipacks();
            GetNumLargeMedipacks();
            GetLvlInfo();
            GetShotgunAmmo();
            GetDeagleAmmo();
            GetNumFlares();
            GetMP5Ammo();
            GetSaveNum();
            GetUziAmmo();
            GetGrenadeLauncherAmmo();
            GetHarpoonAmmo();
            GetRocketLauncherAmmo();
        }

        // Offsets
        private int smallMedipackOffset = 0;
        private int largeMedipackOffset = 0;
        private int numFlaresOffset = 0;
        private int saveNumOffset = 0;
        private int weaponsConfigNumOffset = 0;
        private int harpoonGunOffset = 0;
        private int shotgunAmmoOffset = 0;
        private int shotgunAmmoOffset2 = 0;
        private int deagleAmmoOffset = 0;
        private int deagleAmmoOffset2 = 0;
        private int grenadeLauncherAmmoOffset = 0;
        private int grenadeLauncherAmmoOffset2 = 0;
        private int rocketLauncherAmmoOffset = 0;
        private int rocketLauncherAmmoOffset2 = 0;
        private int harpoonAmmoOffset = 0;
        private int harpoonAmmoOffset2 = 0;
        private int mp5AmmoOffset = 0;
        private int mp5AmmoOffset2 = 0;
        private int uziAmmoOffset = 0;
        private int uziAmmoOffset2 = 0;

        private string sSaveFilePath;

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2);
            int[] validDeagleAmmoOffsets = GetValidAmmoOffsets(deagleAmmoOffset, deagleAmmoOffset2);
            int[] validGrenadeLauncherAmmoOffsets = GetValidAmmoOffsets(grenadeLauncherAmmoOffset, grenadeLauncherAmmoOffset2);
            int[] validRocketLauncherAmmoOffsets = GetValidAmmoOffsets(rocketLauncherAmmoOffset, rocketLauncherAmmoOffset2);
            int[] validHarpoonAmmoOffsets = GetValidAmmoOffsets(harpoonAmmoOffset, harpoonAmmoOffset2);
            int[] validMp5AmmoOffsets = GetValidAmmoOffsets(mp5AmmoOffset, mp5AmmoOffset2);
            int[] validUziAmmoOffsets = GetValidAmmoOffsets(uziAmmoOffset, uziAmmoOffset2);

            WriteToSaveFile(smallMedipackOffset, Decimal.ToInt32(smallMedipacksNumBox.Value));
            WriteToSaveFile(largeMedipackOffset, Decimal.ToInt32(lrgMedipacksNumBox.Value));
            WriteToSaveFile(numFlaresOffset, Decimal.ToInt32(flaresNumBox.Value));
            WriteAmmoValue(saveNumOffset, Decimal.ToInt32(saveNumBox.Value));

            if (!shotgunCheckBox.Checked)
            {
                WriteAmmoValue(shotgunAmmoOffset, Decimal.ToInt32(shotgunAmmoNumBox.Value) * 6);
            }
            else
            {
                for (int i = 0; i < validShotgunAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validShotgunAmmoOffsets[i], Decimal.ToInt32(shotgunAmmoNumBox.Value) * 6);
                }
            }

            if (!deagleCheckBox.Checked)
            {
                WriteAmmoValue(deagleAmmoOffset, Decimal.ToInt32(shotgunAmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validDeagleAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validDeagleAmmoOffsets[i], Decimal.ToInt32(shotgunAmmoNumBox.Value));
                }
            }

            if (!grenadeLauncherCheckBox.Checked)
            {
                WriteAmmoValue(grenadeLauncherAmmoOffset, Decimal.ToInt32(grenadeLauncherAmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validGrenadeLauncherAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validGrenadeLauncherAmmoOffsets[i], Decimal.ToInt32(grenadeLauncherAmmoNumBox.Value));
                }
            }

            if (!rocketLauncherCheckBox.Checked)
            {
                WriteAmmoValue(rocketLauncherAmmoOffset, Decimal.ToInt32(rocketLauncherAmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validRocketLauncherAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validRocketLauncherAmmoOffsets[i], Decimal.ToInt32(rocketLauncherAmmoNumBox.Value));
                }
            }

            if (!harpoonGunCheckBox.Checked)
            {
                WriteAmmoValue(harpoonAmmoOffset, Decimal.ToInt32(harpoonGunAmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validHarpoonAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validHarpoonAmmoOffsets[i], Decimal.ToInt32(harpoonGunAmmoNumBox.Value));
                }
            }

            if (!mp5CheckBox.Checked)
            {
                WriteAmmoValue(mp5AmmoOffset, Decimal.ToInt32(mp5AmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validMp5AmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validMp5AmmoOffsets[i], Decimal.ToInt32(mp5AmmoNumBox.Value));
                }
            }

            if (!uziCheckBox.Checked)
            {
                WriteAmmoValue(uziAmmoOffset, Decimal.ToInt32(uziAmmoNumBox.Value));
            }
            else
            {
                for (int i = 0; i < validUziAmmoOffsets.Length; i++)
                {
                    WriteAmmoValue(validUziAmmoOffsets[i], Decimal.ToInt32(uziAmmoNumBox.Value));
                }
            }

            // Calculate new weapons config number
            int newWeaponsConfigNum = 1;
            if (pistolsCheckBox.Checked) newWeaponsConfigNum += 2;
            if (deagleCheckBox.Checked) newWeaponsConfigNum += 4;
            if (uziCheckBox.Checked) newWeaponsConfigNum += 8;
            if (shotgunCheckBox.Checked) newWeaponsConfigNum += 16;
            if (mp5CheckBox.Checked) newWeaponsConfigNum += 32;
            if (rocketLauncherCheckBox.Checked) newWeaponsConfigNum += 64;
            if (grenadeLauncherCheckBox.Checked) newWeaponsConfigNum += 128;

            // Write new weapons config num to save file
            WriteToSaveFile(weaponsConfigNumOffset, newWeaponsConfigNum);

            // Write harpoon gun value to save file
            if (harpoonGunCheckBox.Checked) WriteToSaveFile(harpoonGunOffset, 1);
            else WriteToSaveFile(harpoonGunOffset, 0);

            helperTxtBox.Clear();
            helperTxtBox.AppendText("File patched!");

            MessageBox.Show("Save file patched!", "SUCCESS");
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    SetSaveFilePath(filePath);

                    fileTxtBox.Clear();
                    fileTxtBox.AppendText(filePath);

                    helperTxtBox.Clear();
                    helperTxtBox.AppendText("Loaded save file: " + Path.GetFileName(openFileDialog.FileName));

                    SaveButton.Enabled = true;
                    pistolsCheckBox.Enabled = true;
                    deagleCheckBox.Enabled = true;
                    deagleAmmoNumBox.Enabled = true;
                    grenadeLauncherAmmoNumBox.Enabled = true;
                    grenadeLauncherCheckBox.Enabled = true;
                    mp5AmmoNumBox.Enabled = true;
                    mp5CheckBox.Enabled = true;
                    harpoonGunCheckBox.Enabled = true;
                    harpoonGunAmmoNumBox.Enabled = true;
                    uziAmmoNumBox.Enabled = true;
                    uziCheckBox.Enabled = true;
                    rocketLauncherAmmoNumBox.Enabled = true;
                    rocketLauncherCheckBox.Enabled = true;
                    shotgunAmmoNumBox.Enabled = true;
                    shotgunCheckBox.Enabled = true;
                    lrgMedipacksNumBox.Enabled = true;
                    smallMedipacksNumBox.Enabled = true;
                    flaresNumBox.Enabled = true;
                    saveNumBox.Enabled = true;

                    GetWeaponsInfo();
                }
            }
        }
    }
}
