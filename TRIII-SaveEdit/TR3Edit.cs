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

            saveNumBox.Maximum = 65535;
            rocketLauncherAmmoNumBox.Maximum = 65535;
            harpoonGunAmmoNumBox.Maximum = 65535;
            grenadeLauncherAmmoNumBox.Maximum = 65535;
            uziAmmoNumBox.Maximum = 65535;
            flaresNumBox.Maximum = 255;
            mp5AmmoNumBox.Maximum = 65535;
            deagleAmmoNumBox.Maximum = 65535;
            shotgunAmmoNumBox.Maximum = 10922;
            lrgMedipacksNumBox.Maximum = 255;
            smallMedipacksNumBox.Maximum = 255;
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
            int saveNum = 0;
            
            if (GetSaveFileData(saveNumOffset + 1) == 0)
            {
                saveNum = GetSaveFileData(saveNumOffset);
            }
            else
            {
                byte firstHalf = GetSaveFileData(saveNumOffset + 1);
                byte secondHalf = GetSaveFileData(saveNumOffset);
                saveNum = firstHalf * 256 + secondHalf;
            }

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
                shotgunAmmoOffset2 = 0x165D;
                shotgunAmmoOffset3 = 0x166F;
                shotgunAmmoOffset4 = 0x16A5;
                deagleAmmoOffset = 0xD8;
                deagleAmmoOffset2 = 0x1655;
                deagleAmmoOffset3 = 0x1667;
                deagleAmmoOffset4 = 0x169D;
                grenadeLauncherAmmoOffset = 0xE4;
                grenadeLauncherAmmoOffset2 = 0x1669;
                grenadeLauncherAmmoOffset3 = 0x167B;
                grenadeLauncherAmmoOffset4 = 0x16B1;
                rocketLauncherAmmoOffset = 0xE0;
                rocketLauncherAmmoOffset2 = 0x1665;
                rocketLauncherAmmoOffset3 = 0x1677;
                rocketLauncherAmmoOffset4 = 0x16AD;
                harpoonAmmoOffset = 0xE2;
                harpoonAmmoOffset2 = 0x1661;
                harpoonAmmoOffset3 = 0x1673;
                harpoonAmmoOffset4 = 0x16A9;
                mp5AmmoOffset = 0xDE;
                mp5AmmoOffset2 = 0x166D;
                mp5AmmoOffset3 = 0x167F;
                mp5AmmoOffset4 = 0x16B5;
                uziAmmoOffset = 0xDA;
                uziAmmoOffset2 = 0x1659;
                uziAmmoOffset3 = 0x166B;
                uziAmmoOffset4 = 0x16A1;
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
                shotgunAmmoOffset3 = 0x23CD;
                deagleAmmoOffset = 0x10B;
                deagleAmmoOffset2 = 0x23B3;
                deagleAmmoOffset3 = 0x23C5;
                grenadeLauncherAmmoOffset = 0x117;
                grenadeLauncherAmmoOffset2 = 0x23C7;
                grenadeLauncherAmmoOffset3 = 0x23D9;
                rocketLauncherAmmoOffset = 0x113;
                rocketLauncherAmmoOffset2 = 0x23C3;
                rocketLauncherAmmoOffset3 = 0x23D5;
                harpoonAmmoOffset = 0x115;
                harpoonAmmoOffset2 = 0x23BF;
                harpoonAmmoOffset3 = 0x23D1;
                mp5AmmoOffset = 0x111;
                mp5AmmoOffset2 = 0x23CB;
                mp5AmmoOffset3 = 0x23DD;
                uziAmmoOffset = 0x10D;
                uziAmmoOffset2 = 0x23B7;
                uziAmmoOffset3 = 0x23C9;
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
                shotgunAmmoOffset3 = 0x1816;
                deagleAmmoOffset = 0x13E;
                deagleAmmoOffset2 = 0x17FC;
                deagleAmmoOffset3 = 0x180E;
                grenadeLauncherAmmoOffset = 0x14A;
                grenadeLauncherAmmoOffset2 = 0x1810;
                grenadeLauncherAmmoOffset3 = 0x1822;
                rocketLauncherAmmoOffset = 0x146;
                rocketLauncherAmmoOffset2 = 0x180C;
                rocketLauncherAmmoOffset3 = 0x181E;
                harpoonAmmoOffset = 0x148;
                harpoonAmmoOffset2 = 0x1808;
                harpoonAmmoOffset3 = 0x181A;
                mp5AmmoOffset = 0x144;
                mp5AmmoOffset2 = 0x1814;
                mp5AmmoOffset3 = 0x1826;
                uziAmmoOffset = 0x140;
                uziAmmoOffset2 = 0x1800;
                uziAmmoOffset3 = 0x1812;
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
                shotgunAmmoOffset2 = 0x17DA;
                shotgunAmmoOffset3 = 0x17A4;
                shotgunAmmoOffset4 = 0x17B6;
                shotgunAmmoOffset5 = 0x17C8;
                deagleAmmoOffset = 0x33C;
                deagleAmmoOffset2 = 0x17D2;
                deagleAmmoOffset3 = 0x179C;
                deagleAmmoOffset4 = 0x17AE;
                deagleAmmoOffset5 = 0x17C0;
                grenadeLauncherAmmoOffset = 0x348;
                grenadeLauncherAmmoOffset2 = 0x17E6;
                grenadeLauncherAmmoOffset3 = 0x17B0;
                grenadeLauncherAmmoOffset4 = 0x17C2;
                grenadeLauncherAmmoOffset5 = 0x17D4;
                rocketLauncherAmmoOffset = 0x344;
                rocketLauncherAmmoOffset2 = 0x17E2;
                rocketLauncherAmmoOffset3 = 0x17AC;
                rocketLauncherAmmoOffset4 = 0x17BE;
                rocketLauncherAmmoOffset5 = 0x17D0;
                harpoonAmmoOffset = 0x346;
                harpoonAmmoOffset2 = 0x17DE;
                harpoonAmmoOffset3 = 0x17A8;
                harpoonAmmoOffset4 = 0x17BA;
                harpoonAmmoOffset5 = 0x17CC;
                mp5AmmoOffset = 0x342;
                mp5AmmoOffset2 = 0x17EA;
                mp5AmmoOffset3 = 0x17B4;
                mp5AmmoOffset4 = 0x17C6;
                mp5AmmoOffset5 = 0x17D8;
                uziAmmoOffset = 0x33E;
                uziAmmoOffset2 = 0x17D6;
                uziAmmoOffset3 = 0x17A0;
                uziAmmoOffset4 = 0x17B2;
                uziAmmoOffset5 = 0x17C4;
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
                shotgunAmmoOffset3 = 0x1EA5;
                shotgunAmmoOffset4 = 0x1EB7;
                deagleAmmoOffset = 0x36F;
                deagleAmmoOffset2 = 0x1E43;
                deagleAmmoOffset3 = 0x1E9D;
                deagleAmmoOffset4 = 0x1EAF;
                grenadeLauncherAmmoOffset = 0x37B;
                grenadeLauncherAmmoOffset2 = 0x1E57;
                grenadeLauncherAmmoOffset3 = 0x1EB1;
                grenadeLauncherAmmoOffset4 = 0x1EC3;
                rocketLauncherAmmoOffset = 0x377;
                rocketLauncherAmmoOffset2 = 0x1E53;
                rocketLauncherAmmoOffset3 = 0x1EAD;
                rocketLauncherAmmoOffset4 = 0x1EBF;
                harpoonAmmoOffset = 0x379;
                harpoonAmmoOffset2 = 0x1E4F;
                harpoonAmmoOffset3 = 0x1EA9;
                harpoonAmmoOffset4 = 0x1EBB;
                mp5AmmoOffset = 0x375;
                mp5AmmoOffset2 = 0x1E5B;
                mp5AmmoOffset3 = 0x1EB5;
                mp5AmmoOffset4 = 0x1EC7;
                uziAmmoOffset = 0x371;
                uziAmmoOffset2 = 0x1E47;
                uziAmmoOffset3 = 0x1EA1;
                uziAmmoOffset4 = 0x1EB3;
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
                shotgunAmmoOffset2 = 0x211F;
                shotgunAmmoOffset3 = 0x210D;
                shotgunAmmoOffset4 = 0x2143;
                shotgunAmmoOffset5 = 0x2155;
                shotgunAmmoOffset6 = 0x2167;
                shotgunAmmoOffset7 = 0x218B;
                shotgunAmmoOffset8 = 0x21AF;
                deagleAmmoOffset = 0x3A2;
                deagleAmmoOffset2 = 0x2117;
                deagleAmmoOffset3 = 0x2105;
                deagleAmmoOffset4 = 0x213B;
                deagleAmmoOffset5 = 0x214D;
                deagleAmmoOffset6 = 0x215F;
                deagleAmmoOffset7 = 0x2183;
                deagleAmmoOffset8 = 0x21A7;
                grenadeLauncherAmmoOffset = 0x3AE;
                grenadeLauncherAmmoOffset2 = 0x212B;
                grenadeLauncherAmmoOffset3 = 0x2119;
                grenadeLauncherAmmoOffset4 = 0x214F;
                grenadeLauncherAmmoOffset5 = 0x2161;
                grenadeLauncherAmmoOffset6 = 0x2173;
                grenadeLauncherAmmoOffset7 = 0x2197;
                grenadeLauncherAmmoOffset8 = 0x21BB;
                rocketLauncherAmmoOffset = 0x3AA;
                rocketLauncherAmmoOffset2 = 0x2127;
                rocketLauncherAmmoOffset3 = 0x2115;
                rocketLauncherAmmoOffset4 = 0x214B;
                rocketLauncherAmmoOffset5 = 0x215D;
                rocketLauncherAmmoOffset6 = 0x216F;
                rocketLauncherAmmoOffset7 = 0x2193;
                rocketLauncherAmmoOffset8 = 0x21B7;
                harpoonAmmoOffset = 0x3AC;
                harpoonAmmoOffset2 = 0x2123;
                harpoonAmmoOffset3 = 0x2111;
                harpoonAmmoOffset4 = 0x2147;
                harpoonAmmoOffset5 = 0x2159;
                harpoonAmmoOffset6 = 0x216B;
                harpoonAmmoOffset7 = 0x218F;
                harpoonAmmoOffset8 = 0x21B3;
                mp5AmmoOffset = 0x3A8;
                mp5AmmoOffset2 = 0x212F;
                mp5AmmoOffset3 = 0x211D;
                mp5AmmoOffset4 = 0x2153;
                mp5AmmoOffset5 = 0x2165;
                mp5AmmoOffset6 = 0x2177;
                mp5AmmoOffset7 = 0x219B;
                mp5AmmoOffset8 = 0x21BF;
                uziAmmoOffset = 0x3A4;
                uziAmmoOffset2 = 0x211B;
                uziAmmoOffset3 = 0x2109;
                uziAmmoOffset4 = 0x213F;
                uziAmmoOffset5 = 0x2151;
                uziAmmoOffset6 = 0x2163;
                uziAmmoOffset7 = 0x2187;
                uziAmmoOffset8 = 0x21AB;
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
                shotgunAmmoOffset3 = 0x17D5;
                shotgunAmmoOffset4 = 0x17E7;
                deagleAmmoOffset = 0x1A4;
                deagleAmmoOffset2 = 0x17A9;
                deagleAmmoOffset3 = 0x17CD;
                deagleAmmoOffset4 = 0x17DF;
                grenadeLauncherAmmoOffset = 0x1B0;
                grenadeLauncherAmmoOffset2 = 0x17BD;
                grenadeLauncherAmmoOffset3 = 0x17E1;
                grenadeLauncherAmmoOffset4 = 0x17F3;
                rocketLauncherAmmoOffset = 0x1AC;
                rocketLauncherAmmoOffset2 = 0x17B9;
                rocketLauncherAmmoOffset3 = 0x17DD;
                rocketLauncherAmmoOffset4 = 0x17EF;
                harpoonAmmoOffset = 0x1AE;
                harpoonAmmoOffset2 = 0x17B5;
                harpoonAmmoOffset3 = 0x17D9;
                harpoonAmmoOffset4 = 0x17EB;
                mp5AmmoOffset = 0x1AA;
                mp5AmmoOffset2 = 0x17C1;
                mp5AmmoOffset3 = 0x17E5;
                mp5AmmoOffset4 = 0x17F7;
                uziAmmoOffset = 0x1A6;
                uziAmmoOffset2 = 0x17AD;
                uziAmmoOffset3 = 0x17D1;
                uziAmmoOffset4 = 0x17E3;
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
                shotgunAmmoOffset3 = 0x18F7;
                shotgunAmmoOffset4 = 0x1909;
                deagleAmmoOffset = 0x1D7;
                deagleAmmoOffset2 = 0x18CB;
                deagleAmmoOffset3 = 0x18EF;
                deagleAmmoOffset4 = 0x1901;
                grenadeLauncherAmmoOffset = 0x1E3;
                grenadeLauncherAmmoOffset2 = 0x18DF;
                grenadeLauncherAmmoOffset3 = 0x1915;
                grenadeLauncherAmmoOffset4 = 0x1903;
                rocketLauncherAmmoOffset = 0x1DF;
                rocketLauncherAmmoOffset2 = 0x18DB;
                rocketLauncherAmmoOffset3 = 0x1911;
                rocketLauncherAmmoOffset4 = 0x18FF;
                harpoonAmmoOffset = 0x1E1;
                harpoonAmmoOffset2 = 0x18D7;
                harpoonAmmoOffset3 = 0x18FB;
                harpoonAmmoOffset4 = 0x190D;
                mp5AmmoOffset = 0x1DD;
                mp5AmmoOffset2 = 0x18E3;
                mp5AmmoOffset3 = 0x1907;
                mp5AmmoOffset4 = 0x1919;
                uziAmmoOffset = 0x1D9;
                uziAmmoOffset2 = 0x18CF;
                uziAmmoOffset3 = 0x18F3;
                uziAmmoOffset4 = 0x1905;
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
                shotgunAmmoOffset3 = 0x1441;
                shotgunAmmoOffset4 = 0x1453;
                shotgunAmmoOffset5 = 0x1477;
                deagleAmmoOffset = 0x20A;
                deagleAmmoOffset2 = 0x1415;
                deagleAmmoOffset3 = 0x1439;
                deagleAmmoOffset4 = 0x144B;
                deagleAmmoOffset5 = 0x146F;
                grenadeLauncherAmmoOffset = 0x216;
                grenadeLauncherAmmoOffset2 = 0x1429;
                grenadeLauncherAmmoOffset3 = 0x144D;
                grenadeLauncherAmmoOffset4 = 0x145F;
                grenadeLauncherAmmoOffset5 = 0x1483;
                rocketLauncherAmmoOffset = 0x212;
                rocketLauncherAmmoOffset2 = 0x1425;
                rocketLauncherAmmoOffset3 = 0x1449;
                rocketLauncherAmmoOffset4 = 0x145B;
                rocketLauncherAmmoOffset5 = 0x147F;
                harpoonAmmoOffset = 0x214;
                harpoonAmmoOffset2 = 0x1421;
                harpoonAmmoOffset3 = 0x1445;
                harpoonAmmoOffset4 = 0x1457;
                harpoonAmmoOffset5 = 0x147B;
                mp5AmmoOffset = 0x210;
                mp5AmmoOffset2 = 0x142D;
                mp5AmmoOffset3 = 0x1451;
                mp5AmmoOffset4 = 0x1463;
                mp5AmmoOffset5 = 0x1487;
                uziAmmoOffset = 0x20C;
                uziAmmoOffset2 = 0x1419;
                uziAmmoOffset3 = 0x143D;
                uziAmmoOffset4 = 0x144F;
                uziAmmoOffset5 = 0x1473;
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
                shotgunAmmoOffset3 = 0x1107;
                deagleAmmoOffset = 0x23D;
                deagleAmmoOffset2 = 0x10ED;
                deagleAmmoOffset3 = 0x10FF;
                grenadeLauncherAmmoOffset = 0x249;
                grenadeLauncherAmmoOffset2 = 0x1101;
                grenadeLauncherAmmoOffset3 = 0x1113;
                rocketLauncherAmmoOffset = 0x245;
                rocketLauncherAmmoOffset2 = 0x10FD;
                rocketLauncherAmmoOffset3 = 0x110F;
                harpoonAmmoOffset = 0x247;
                harpoonAmmoOffset2 = 0x10F9;
                harpoonAmmoOffset3 = 0x110B;
                mp5AmmoOffset = 0x243;
                mp5AmmoOffset2 = 0x1105;
                mp5AmmoOffset3 = 0x1117;
                uziAmmoOffset = 0x23F;
                uziAmmoOffset2 = 0x10F1;
                uziAmmoOffset3 = 0x1103;
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
                shotgunAmmoOffset2 = 0x1897;
                shotgunAmmoOffset3 = 0x1873;
                shotgunAmmoOffset4 = 0x1885;
                deagleAmmoOffset = 0x270;
                deagleAmmoOffset2 = 0x188F;
                deagleAmmoOffset3 = 0x186B;
                deagleAmmoOffset4 = 0x187D;
                grenadeLauncherAmmoOffset = 0x27C;
                grenadeLauncherAmmoOffset2 = 0x18A3;
                grenadeLauncherAmmoOffset3 = 0x187F;
                grenadeLauncherAmmoOffset4 = 0x1891;
                rocketLauncherAmmoOffset = 0x278;
                rocketLauncherAmmoOffset2 = 0x189F;
                rocketLauncherAmmoOffset3 = 0x187B;
                rocketLauncherAmmoOffset4 = 0x188D;
                harpoonAmmoOffset = 0x27A;
                harpoonAmmoOffset2 = 0x189B;
                harpoonAmmoOffset3 = 0x1877;
                harpoonAmmoOffset4 = 0x1889;
                mp5AmmoOffset = 0x276;
                mp5AmmoOffset2 = 0x18A7;
                mp5AmmoOffset3 = 0x1883;
                mp5AmmoOffset4 = 0x1895;
                uziAmmoOffset = 0x272;
                uziAmmoOffset2 = 0x1893;
                uziAmmoOffset3 = 0x186F;
                uziAmmoOffset4 = 0x1881;
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
                shotgunAmmoOffset3 = 0x1DAD;
                deagleAmmoOffset = 0x2D6;
                deagleAmmoOffset2 = 0x1D81;
                deagleAmmoOffset3 = 0x1DA5;
                grenadeLauncherAmmoOffset = 0x2E2;
                grenadeLauncherAmmoOffset2 = 0x1D95;
                grenadeLauncherAmmoOffset3 = 0x1DB9;
                rocketLauncherAmmoOffset = 0x2DE;
                rocketLauncherAmmoOffset2 = 0x1D91;
                rocketLauncherAmmoOffset3 = 0x1DB5;
                rocketLauncherAmmoOffset4 = 0x1DB9;
                harpoonAmmoOffset = 0x2E0;
                harpoonAmmoOffset2 = 0x1D8D;
                harpoonAmmoOffset3 = 0x1DB1;
                mp5AmmoOffset = 0x2DC;
                mp5AmmoOffset2 = 0x1D99;
                mp5AmmoOffset3 = 0x1DBD;
                uziAmmoOffset = 0x2D8;
                uziAmmoOffset2 = 0x1D85;
                uziAmmoOffset3 = 0x1DA9;
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
                shotgunAmmoOffset3 = 0xB05;
                deagleAmmoOffset = 0x309;
                deagleAmmoOffset2 = 0xAEB;
                deagleAmmoOffset3 = 0xAFD;
                grenadeLauncherAmmoOffset = 0x315;
                grenadeLauncherAmmoOffset2 = 0xAFF;
                grenadeLauncherAmmoOffset3 = 0xB11;
                rocketLauncherAmmoOffset = 0x311;
                rocketLauncherAmmoOffset2 = 0xAFB;
                rocketLauncherAmmoOffset3 = 0xB0D;
                harpoonAmmoOffset = 0x313;
                harpoonAmmoOffset2 = 0xAF7;
                harpoonAmmoOffset3 = 0xB09;
                mp5AmmoOffset = 0x30F;
                mp5AmmoOffset2 = 0xB03;
                mp5AmmoOffset3 = 0xB15;
                uziAmmoOffset = 0x30B;
                uziAmmoOffset2 = 0xAEF;
                uziAmmoOffset3 = 0xB01;
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
                shotgunAmmoOffset3 = 0x19A7;
                deagleAmmoOffset = 0x3D5;
                deagleAmmoOffset2 = 0x198D;
                deagleAmmoOffset3 = 0x199F;
                grenadeLauncherAmmoOffset = 0x3E1;
                grenadeLauncherAmmoOffset2 = 0x19A1;
                grenadeLauncherAmmoOffset3 = 0x19B3;
                rocketLauncherAmmoOffset = 0x3DD;
                rocketLauncherAmmoOffset2 = 0x199D;
                rocketLauncherAmmoOffset3 = 0x19AF;
                harpoonAmmoOffset = 0x3DF;
                harpoonAmmoOffset2 = 0x1999;
                harpoonAmmoOffset3 = 0x19AB;
                mp5AmmoOffset = 0x3DB;
                mp5AmmoOffset2 = 0x19A5;
                mp5AmmoOffset3 = 0x19B7;
                uziAmmoOffset = 0x3D7;
                uziAmmoOffset2 = 0x1991;
                uziAmmoOffset3 = 0x19A3;
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
                shotgunAmmoOffset3 = 0x197B;
                shotgunAmmoOffset4 = 0x198D;
                deagleAmmoOffset = 0x408;
                deagleAmmoOffset2 = 0x194F;
                deagleAmmoOffset3 = 0x1973;
                deagleAmmoOffset4 = 0x1985;
                grenadeLauncherAmmoOffset = 0x414;
                grenadeLauncherAmmoOffset2 = 0x1963;
                grenadeLauncherAmmoOffset3 = 0x1987;
                grenadeLauncherAmmoOffset4 = 0x1999;
                rocketLauncherAmmoOffset = 0x410;
                rocketLauncherAmmoOffset2 = 0x195F;
                rocketLauncherAmmoOffset3 = 0x1983;
                rocketLauncherAmmoOffset4 = 0x1995;
                harpoonAmmoOffset = 0x412;
                harpoonAmmoOffset2 = 0x195B;
                harpoonAmmoOffset3 = 0x197F;
                harpoonAmmoOffset4 = 0x1991;
                mp5AmmoOffset = 0x40E;
                mp5AmmoOffset2 = 0x1967;
                mp5AmmoOffset3 = 0x198B;
                mp5AmmoOffset4 = 0x199D;
                uziAmmoOffset = 0x40A;
                uziAmmoOffset2 = 0x1953;
                uziAmmoOffset3 = 0x1977;
                uziAmmoOffset4 = 0x1989;
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
                shotgunAmmoOffset3 = 0x1DCD;
                deagleAmmoOffset = 0x43B;
                deagleAmmoOffset2 = 0x1D8F;
                deagleAmmoOffset3 = 0x1DC5;
                grenadeLauncherAmmoOffset = 0x447;
                grenadeLauncherAmmoOffset2 = 0x1DA3;
                grenadeLauncherAmmoOffset3 = 0x1DD9;
                rocketLauncherAmmoOffset = 0x443;
                rocketLauncherAmmoOffset2 = 0x1D9F;
                rocketLauncherAmmoOffset3 = 0x1DD5;
                harpoonAmmoOffset = 0x445;
                harpoonAmmoOffset2 = 0x1D9B;
                harpoonAmmoOffset3 = 0x1DD1;
                mp5AmmoOffset = 0x441;
                mp5AmmoOffset2 = 0x1DA7;
                mp5AmmoOffset3 = 0x1DDD;
                uziAmmoOffset = 0x43D;
                uziAmmoOffset2 = 0x1D93;
                uziAmmoOffset3 = 0x1DC9;
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
                shotgunAmmoOffset2 = 0xAFB;
                shotgunAmmoOffset3 = 0xAE9;
                deagleAmmoOffset = 0x46E;
                deagleAmmoOffset2 = 0xAF3;
                deagleAmmoOffset3 = 0xAE1;
                grenadeLauncherAmmoOffset = 0x47A;
                grenadeLauncherAmmoOffset2 = 0xB07;
                grenadeLauncherAmmoOffset3 = 0xAF5;
                rocketLauncherAmmoOffset = 0x476;
                rocketLauncherAmmoOffset2 = 0xB03;
                rocketLauncherAmmoOffset3 = 0xAF1;
                harpoonAmmoOffset = 0x478;
                harpoonAmmoOffset2 = 0xAFF;
                harpoonAmmoOffset3 = 0xAED;
                mp5AmmoOffset = 0x474;
                mp5AmmoOffset2 = 0xB0B;
                mp5AmmoOffset3 = 0xAF9;
                uziAmmoOffset = 0x470;
                uziAmmoOffset2 = 0xAF7;
                uziAmmoOffset3 = 0xAE5;
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
            const int Shotgun = 16;
            const int Deagle = 4;
            const int Uzi = 8;
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
        private int shotgunAmmoOffset3 = 0;
        private int shotgunAmmoOffset4 = 0;
        private int shotgunAmmoOffset5 = 0;
        private int shotgunAmmoOffset6 = 0;
        private int shotgunAmmoOffset7 = 0;
        private int shotgunAmmoOffset8 = 0;
        private int deagleAmmoOffset = 0;
        private int deagleAmmoOffset2 = 0;
        private int deagleAmmoOffset3 = 0;
        private int deagleAmmoOffset4 = 0;
        private int deagleAmmoOffset5 = 0;
        private int deagleAmmoOffset6 = 0;
        private int deagleAmmoOffset7 = 0;
        private int deagleAmmoOffset8 = 0;
        private int grenadeLauncherAmmoOffset = 0;
        private int grenadeLauncherAmmoOffset2 = 0;
        private int grenadeLauncherAmmoOffset3 = 0;
        private int grenadeLauncherAmmoOffset4 = 0;
        private int grenadeLauncherAmmoOffset5 = 0;
        private int grenadeLauncherAmmoOffset6 = 0;
        private int grenadeLauncherAmmoOffset7 = 0;
        private int grenadeLauncherAmmoOffset8 = 0;
        private int rocketLauncherAmmoOffset = 0;
        private int rocketLauncherAmmoOffset2 = 0;
        private int rocketLauncherAmmoOffset3 = 0;
        private int rocketLauncherAmmoOffset4 = 0;
        private int rocketLauncherAmmoOffset5 = 0;
        private int rocketLauncherAmmoOffset6 = 0;
        private int rocketLauncherAmmoOffset7 = 0;
        private int rocketLauncherAmmoOffset8 = 0;
        private int harpoonAmmoOffset = 0;
        private int harpoonAmmoOffset2 = 0;
        private int harpoonAmmoOffset3 = 0;
        private int harpoonAmmoOffset4 = 0;
        private int harpoonAmmoOffset5 = 0;
        private int harpoonAmmoOffset6 = 0;
        private int harpoonAmmoOffset7 = 0;
        private int harpoonAmmoOffset8 = 0;
        private int mp5AmmoOffset = 0;
        private int mp5AmmoOffset2 = 0;
        private int mp5AmmoOffset3 = 0;
        private int mp5AmmoOffset4 = 0;
        private int mp5AmmoOffset5 = 0;
        private int mp5AmmoOffset6 = 0;
        private int mp5AmmoOffset7 = 0;
        private int mp5AmmoOffset8 = 0;
        private int uziAmmoOffset = 0;
        private int uziAmmoOffset2 = 0;
        private int uziAmmoOffset3 = 0;
        private int uziAmmoOffset4 = 0;
        private int uziAmmoOffset5 = 0;
        private int uziAmmoOffset6 = 0;
        private int uziAmmoOffset7 = 0;
        private int uziAmmoOffset8 = 0;

        private string sSaveFilePath;

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2, shotgunAmmoOffset3, shotgunAmmoOffset4, shotgunAmmoOffset5, shotgunAmmoOffset6, shotgunAmmoOffset7, shotgunAmmoOffset8);
            int[] validDeagleAmmoOffsets = GetValidAmmoOffsets(deagleAmmoOffset, deagleAmmoOffset2, deagleAmmoOffset3, deagleAmmoOffset4, deagleAmmoOffset5, deagleAmmoOffset6, deagleAmmoOffset7, deagleAmmoOffset8);
            int[] validGrenadeLauncherAmmoOffsets = GetValidAmmoOffsets(grenadeLauncherAmmoOffset, grenadeLauncherAmmoOffset2, grenadeLauncherAmmoOffset3, grenadeLauncherAmmoOffset4, grenadeLauncherAmmoOffset5, grenadeLauncherAmmoOffset6, grenadeLauncherAmmoOffset7, grenadeLauncherAmmoOffset8);
            int[] validRocketLauncherAmmoOffsets = GetValidAmmoOffsets(rocketLauncherAmmoOffset, rocketLauncherAmmoOffset2, rocketLauncherAmmoOffset3, rocketLauncherAmmoOffset4, rocketLauncherAmmoOffset5, rocketLauncherAmmoOffset6, rocketLauncherAmmoOffset7, rocketLauncherAmmoOffset8);
            int[] validHarpoonAmmoOffsets = GetValidAmmoOffsets(harpoonAmmoOffset, harpoonAmmoOffset2, harpoonAmmoOffset3, harpoonAmmoOffset4, harpoonAmmoOffset5, harpoonAmmoOffset6, harpoonAmmoOffset7, harpoonAmmoOffset8);
            int[] validMp5AmmoOffsets = GetValidAmmoOffsets(mp5AmmoOffset, mp5AmmoOffset2, mp5AmmoOffset3, mp5AmmoOffset4, mp5AmmoOffset5, mp5AmmoOffset6, mp5AmmoOffset7, mp5AmmoOffset8);
            int[] validUziAmmoOffsets = GetValidAmmoOffsets(uziAmmoOffset, uziAmmoOffset2, uziAmmoOffset3, uziAmmoOffset4, uziAmmoOffset5, uziAmmoOffset6, uziAmmoOffset7, uziAmmoOffset8);

            WriteToSaveFile(smallMedipackOffset, Decimal.ToInt32(smallMedipacksNumBox.Value));
            WriteToSaveFile(largeMedipackOffset, Decimal.ToInt32(lrgMedipacksNumBox.Value));
            WriteToSaveFile(numFlaresOffset, Decimal.ToInt32(flaresNumBox.Value));

            if ((int)saveNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)saveNumBox.Value / 256);
                byte secondHalf = (byte)((int)saveNumBox.Value % 256);

                WriteToSaveFile(saveNumOffset + 1, firstHalf);
                WriteToSaveFile(saveNumOffset, secondHalf);
            }
            else
            {
                WriteToSaveFile(saveNumOffset, (byte)Decimal.ToInt32(saveNumBox.Value));
                WriteToSaveFile(saveNumOffset + 1, 0);
            }

            int shotgunAmmo = (int)(shotgunAmmoNumBox.Value) * 6;
            if (shotgunAmmo > 255)
            {
                byte firstHalf = (byte)(shotgunAmmo / 256);
                byte secondHalf = (byte)(shotgunAmmo % 256);

                for (int i = 0; i < validShotgunAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validShotgunAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validShotgunAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validShotgunAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validShotgunAmmoOffsets[i], (byte)Decimal.ToInt32(shotgunAmmoNumBox.Value));
                    WriteToSaveFile(validShotgunAmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)deagleAmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)deagleAmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)deagleAmmoNumBox.Value % 256);

                for (int i = 0; i < validDeagleAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validDeagleAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validDeagleAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validDeagleAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validDeagleAmmoOffsets[i], (byte)Decimal.ToInt32(deagleAmmoNumBox.Value));
                    WriteToSaveFile(validDeagleAmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)grenadeLauncherAmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)grenadeLauncherAmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)grenadeLauncherAmmoNumBox.Value % 256);

                for (int i = 0; i < validGrenadeLauncherAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validGrenadeLauncherAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validGrenadeLauncherAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validGrenadeLauncherAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validGrenadeLauncherAmmoOffsets[i], (byte)Decimal.ToInt32(grenadeLauncherAmmoNumBox.Value));
                    WriteToSaveFile(validGrenadeLauncherAmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)rocketLauncherAmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)rocketLauncherAmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)rocketLauncherAmmoNumBox.Value % 256);

                for (int i = 0; i < validRocketLauncherAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validRocketLauncherAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validRocketLauncherAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validRocketLauncherAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validRocketLauncherAmmoOffsets[i], (byte)Decimal.ToInt32(rocketLauncherAmmoNumBox.Value));
                    WriteToSaveFile(validRocketLauncherAmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)harpoonGunAmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)harpoonGunAmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)harpoonGunAmmoNumBox.Value % 256);

                for (int i = 0; i < validHarpoonAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validHarpoonAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validHarpoonAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validHarpoonAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validHarpoonAmmoOffsets[i], (byte)Decimal.ToInt32(harpoonGunAmmoNumBox.Value));
                    WriteToSaveFile(validHarpoonAmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)mp5AmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)mp5AmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)mp5AmmoNumBox.Value % 256);

                for (int i = 0; i < validMp5AmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validMp5AmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validMp5AmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validMp5AmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validMp5AmmoOffsets[i], (byte)Decimal.ToInt32(mp5AmmoNumBox.Value));
                    WriteToSaveFile(validMp5AmmoOffsets[i] + 1, 0);
                }
            }

            if ((int)uziAmmoNumBox.Value > 255)
            {
                byte firstHalf = (byte)((int)uziAmmoNumBox.Value / 256);
                byte secondHalf = (byte)((int)uziAmmoNumBox.Value % 256);

                for (int i = 0; i < validUziAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validUziAmmoOffsets[i] + 1, firstHalf);
                    WriteToSaveFile(validUziAmmoOffsets[i], secondHalf);
                }
            }
            else
            {
                for (int i = 0; i < validUziAmmoOffsets.Length; i++)
                {
                    WriteToSaveFile(validUziAmmoOffsets[i], (byte)Decimal.ToInt32(uziAmmoNumBox.Value));
                    WriteToSaveFile(validUziAmmoOffsets[i] + 1, 0);
                }
            }

            // Calculate new weapons config number
            int newWeaponsConfigNum = 1;
            if (pistolsCheckBox.Checked) newWeaponsConfigNum += 2;
            if (shotgunCheckBox.Checked) newWeaponsConfigNum += 16;
            if (deagleCheckBox.Checked) newWeaponsConfigNum += 4;
            if (uziCheckBox.Checked) newWeaponsConfigNum += 8;
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
