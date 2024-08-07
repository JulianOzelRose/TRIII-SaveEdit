﻿/*
    Julian O. Rose
    TR3Edit.cs
    8-8-2023
*/

using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TRIII_SaveEdit
{
    public partial class TR3Edit : Form
    {
        public TR3Edit()
        {
            InitializeComponent();
        }

        public void SetSaveFilePath(string filePath)
        {
            strSaveFilePath = filePath;
        }

        public string GetSaveFilePath()
        {
            return strSaveFilePath;
        }

        public byte ReadByte(long offset)
        {
            string saveFilePath = GetSaveFilePath();

            using (FileStream saveFile = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                return (byte)saveFile.ReadByte();
            }
        }

        public int ReadUInt16(long offset)
        {
            int lowerByte = ReadByte(offset);
            int upperByte = ReadByte(offset + 1);

            int value = lowerByte + (upperByte << 8);

            return value;
        }

        public void WriteByte(long offset, int value)
        {
            string saveFilePath = GetSaveFilePath();

            using (FileStream saveFile = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                saveFile.Seek(offset, SeekOrigin.Begin);
                byte[] byteData = { (byte)value };
                saveFile.Write(byteData, 0, byteData.Length);
            }
        }

        public void WriteUInt16(long offset, int value)
        {
            if (value > 255)
            {
                byte upperByte = (byte)(value / 256);
                byte lowerByte = (byte)(value % 256);

                WriteByte(offset + 1, upperByte);
                WriteByte(offset, lowerByte);
            }
            else
            {
                WriteByte(offset + 1, 0);
                WriteByte(offset, (byte)value);
            }
        }

        public string GetLvlName()
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

        public string GetCleanLvlName()
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

        private readonly Dictionary<string, Dictionary<int, int[]>> ammoIndexData = new Dictionary<string, Dictionary<int, int[]>>
        {
            ["Jungle"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1663, 0x1664, 0x1665, 0x1666 },
                [1] = new int[] { 0x1675, 0x1676, 0x1677, 0x1678 },
                [2] = new int[] { 0x1687, 0x1688, 0x1689, 0x168A },
                [3] = new int[] { 0x1699, 0x169A, 0x169B, 0x169C },
                [4] = new int[] { 0x16AB, 0x16AC, 0x16AD, 0x16AE },
                [5] = new int[] { 0x16BD, 0x16BE, 0x16BF, 0x16C0 }
            },

            ["Temple Ruins"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x23D3, 0x23D4, 0x23D5, 0x23D6 },
                [1] = new int[] { 0x23E5, 0x23E6, 0x23E7, 0x23E8 },
            },

            ["The River Ganges"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x181C, 0x181D, 0x181E, 0x181F },
                [1] = new int[] { 0x182E, 0x182F, 0x1830, 0x1831 },
            },

            ["Caves Of Kaliya"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0xD37, 0xD38, 0xD39, 0xD3A },
                [1] = new int[] { 0xD53, 0xD54, 0xD55, 0xD56 }
            },

            ["Nevada Desert"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x17BC, 0x17BD, 0x17BE, 0x17BF },
                [1] = new int[] { 0x17CE, 0x17CF, 0x17D0, 0x17D1 },
                [2] = new int[] { 0x17E0, 0x17E1, 0x17E2, 0x17E3 },
                [3] = new int[] { 0x17F2, 0x17F3, 0x17F4, 0x17F5 }
            },

            ["High Security Compound"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1E63, 0x1E64, 0x1E65, 0x1E66 },
                [1] = new int[] { 0x1E75, 0x1E76, 0x1E77, 0x1E78 },
                [2] = new int[] { 0x1E91, 0x1E92, 0x1E93, 0x1E94 },
                [3] = new int[] { 0x1EA3, 0x1EA4, 0x1EA5, 0x1EA6 },
                [4] = new int[] { 0x1EB5, 0x1EB6, 0x1EB7, 0x1EB8 },
                [5] = new int[] { 0x1EBD, 0x1EBE, 0x1EBF, 0x1EC0 },
                [6] = new int[] { 0x1ECF, 0x1ED0, 0x1ED1, 0x1ED2 }
            },

            ["Area 51"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x2125, 0x2126, 0x2127, 0x2128 },
                [1] = new int[] { 0x2137, 0x2138, 0x2139, 0x213A },
                [2] = new int[] { 0x2149, 0x214A, 0x214B, 0x214C },
                [3] = new int[] { 0x215B, 0x215C, 0x215D, 0x215E },
                [4] = new int[] { 0x216D, 0x216E, 0x216F, 0x2170 },
                [5] = new int[] { 0x217F, 0x2180, 0x2181, 0x2182 },
                [6] = new int[] { 0x219B, 0x219C, 0x219D, 0x219E },
                [7] = new int[] { 0x21A3, 0x21A4, 0x21A5, 0x21A6 },
                [8] = new int[] { 0x21BF, 0x21C0, 0x21C1, 0x21C2 },
                [9] = new int[] { 0x21C7, 0x21C8, 0x21C9, 0x21CA }
            },

            ["Coastal Village"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x17C9, 0x17CA, 0x17CB, 0x17CC },
                [1] = new int[] { 0x17DB, 0x17DC, 0x17DD, 0x17DE },
                [2] = new int[] { 0x17ED, 0x17EE, 0x17EF, 0x17F0 }
            },

            ["Crash Site"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x18EB, 0x18EC, 0x18ED, 0x18EE },
                [1] = new int[] { 0x18FD, 0x18FE, 0x18FF, 0x1900 },
                [2] = new int[] { 0x190F, 0x1910, 0x1911, 0x1912 },
                [3] = new int[] { 0x1921, 0x1922, 0x1923, 0x1924 },
                [4] = new int[] { 0x1933, 0x1934, 0x1935, 0x1936 },
                [5] = new int[] { 0x1945, 0x1946, 0x1947, 0x1948 }
            },

            ["Madubu Gorge"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1435, 0x1436, 0x1437, 0x1438 },
                [1] = new int[] { 0x1447, 0x1448, 0x1449, 0x144A },
                [2] = new int[] { 0x1459, 0x145A, 0x145B, 0x145C },
                [3] = new int[] { 0x146B, 0x146C, 0x146D, 0x146E },
                [4] = new int[] { 0x1487, 0x1488, 0x1489, 0x148A },
                [5] = new int[] { 0x148F, 0x1490, 0x1491, 0x1492 }
            },

            ["Temple Of Puna"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x110D, 0x110E, 0x110F, 0x1110 },
                [1] = new int[] { 0x105D, 0x105E, 0x105F, 0x1060 }
            },

            ["Thames Wharf"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x188B, 0x188C, 0x188D, 0x188E },
                [1] = new int[] { 0x189D, 0x189E, 0x189F, 0x18A0 },
                [2] = new int[] { 0x18AF, 0x18B0, 0x18B1, 0x18B2 }
            },

            ["Aldwych"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x2317, 0x2318, 0x2319, 0x231A },
                [1] = new int[] { 0x2329, 0x232A, 0x232B, 0x232C }
            },

            ["Lud's Gate"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1D8F, 0x1D90, 0x1D91, 0x1D92 },
                [1] = new int[] { 0x1DA1, 0x1DA2, 0x1DA3, 0x1DA4 },
                [2] = new int[] { 0x1DB3, 0x1DB4, 0x1DB5, 0x1DB6 },
                [3] = new int[] { 0x1D03, 0x1D04, 0x1D05, 0x1D06 }
            },

            ["City"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0xB0B, 0xB0C, 0xB0D, 0xB0E },
                [1] = new int[] { 0xB1D, 0xB1E, 0xB1F, 0xB20 }
            },

            ["Antarctica"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x19AD, 0x19AE, 0x19AF, 0x19B0 },
                [1] = new int[] { 0x19BF, 0x19C0, 0x19C1, 0x19C2 }
            },

            ["RX-Tech Mines"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x196F, 0x1970, 0x1971, 0x1972 },
                [1] = new int[] { 0x1981, 0x1982, 0x1983, 0x1984 },
                [2] = new int[] { 0x1993, 0x1994, 0x1995, 0x1996 },
                [3] = new int[] { 0x19A5, 0x19A6, 0x19A7, 0x19A8 }
            },

            ["Lost City Of Tinnos"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2 },
                [1] = new int[] { 0x1DC1, 0x1DC2, 0x1DC3, 0x1DC4 },
                [2] = new int[] { 0x1DD3, 0x1DD4, 0x1DD5, 0x1DD6 },
            },

            ["Meteorite Cavern"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0xB01, 0xB02, 0xB03, 0xB04 },
                [1] = new int[] { 0xB13, 0xB14, 0xB15, 0xB16 },
            },

            ["All Hallows"] = new Dictionary<int, int[]>
            {
                [0] = new int[] { 0x1045, 0x1046, 0x1047, 0x1048 },
                [1] = new int[] { 0x1057, 0x1058, 0x1059, 0x105A },
                [2] = new int[] { 0x1609, 0x106A, 0x106B, 0x106C }
            }
        };

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

                    if (offsets.All(offset => ReadByte(offset) == 0xFF))
                    {
                        ammoIndex = key;
                        break;
                    }
                }
            }

            return ammoIndex;
        }

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

        public void DisplayLvlName()
        {
            string lvlName = GetLvlName();
            txtLvlName.Clear();
            txtLvlName.AppendText(lvlName);
        }

        public void DisplayNumSmallMedipacks()
        {
            byte numSmallMedipacks = ReadByte(smallMedipackOffset);
            nudSmallMedipacks.Value = numSmallMedipacks;
        }

        public void DisplayNumLargeMedipacks()
        {
            byte numLargeMedipacks = ReadByte(largeMedipackOffset);
            nudLargeMedipacks.Value = numLargeMedipacks;
        }

        public void DisplayShotgunAmmo()
        {
            int shotgunAmmo = ReadUInt16(shotgunAmmoOffset);
            nudShotgunAmmo.Value = shotgunAmmo / 6;
        }

        public void DisplayDeagleAmmo()
        {
            int deagleAmmo = ReadUInt16(deagleAmmoOffset);
            nudDesertEagleAmmo.Value = deagleAmmo;
        }

        public void DisplayMP5Ammo()
        {
            int mp5Ammo = ReadUInt16(mp5AmmoOffset);
            nudMp5Ammo.Value = mp5Ammo;
        }

        public void DisplayNumFlares()
        {
            byte numFlares = ReadByte(numFlaresOffset);
            nudFlares.Value = numFlares;
        }

        public void DisplayUziAmmo()
        {
            int uziAmmo = ReadUInt16(uziAmmoOffset);
            nudUziAmmo.Value = uziAmmo;
        }

        public void DisplayGrenadeLauncherAmmo()
        {
            int grenadeLauncherAmmo = ReadUInt16(grenadeLauncherAmmoOffset);
            nudGrenadeLauncher.Value = grenadeLauncherAmmo;
        }

        public void DisplayHarpoonAmmo()
        {
            int harpoonAmmo = ReadUInt16(harpoonAmmoOffset);
            nudHarpoonGunAmmo.Value = harpoonAmmo;
        }

        public void DisplayRocketLauncherAmmo()
        {
            int rocketLauncherAmmo = ReadUInt16(rocketLauncherAmmoOffset);
            nudRocketLauncher.Value = rocketLauncherAmmo;
        }

        public void DisplaySaveNum()
        {
            int saveNum = ReadUInt16(saveNumOffset);
            nudSaveNum.Value = saveNum;
        }

        public void DisplayWeaponsInfo()
        {
            // Update weapons vars
            byte weaponsConfigNum = ReadByte(weaponsConfigNumOffset);
            byte harpoonGunVal = ReadByte(harpoonGunOffset);

            // Define weapons config constants
            const byte Pistols = 2;
            const byte Deagle = 4;
            const byte Uzis = 8;
            const byte Shotgun = 16;
            const byte MP5 = 32;
            const byte RocketLauncher = 64;
            const byte GrenadeLauncher = 128;

            // Update weapons checkboxes
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

            chkHarpoonGun.Checked = (harpoonGunVal == 1) ? true : false;
        }

        public void DisplayHealthValue()
        {
            int healthOffset = GetHealthOffset();

            if (healthOffset == -1)
            {
                trbHealth.Enabled = false;
                lblHealthError.Visible = true;
                lblHealth.Visible = false;
                trbHealth.Value = 0;
            }
            else
            {
                int health = ReadUInt16(healthOffset);
                trbHealth.Enabled = true;
                lblHealthError.Visible = false;
                double healthPercentage = (double)health / MAX_HEALTH_VALUE * 100.0;
                trbHealth.Value = (int)Math.Round(healthPercentage, 1);
                lblHealth.Visible = true;
                lblHealth.Text = healthPercentage.ToString("0.0") + "%";
            }
        }

        public void SetHealthOffsets(params int[] offsets)
        {
            healthOffsets.Clear();

            for (int i = 0; i < offsets.Length; i++)
            {
                healthOffsets.Add(offsets[i]);
            }
        }

        public bool IsKnownByteFlagPattern(byte byteFlag1, byte byteFlag2, byte byteFlag3)
        {
            if (byteFlag1 == 0x02 && byteFlag2 == 0x00 && byteFlag3 == 0x02) return true;       // Standing
            if (byteFlag1 == 0x13 && byteFlag2 == 0x00 && byteFlag3 == 0x13) return true;       // Climbing
            if (byteFlag1 == 0x21 && byteFlag2 == 0x00 && byteFlag3 == 0x21) return true;       // On water
            if (byteFlag1 == 0x0D && byteFlag2 == 0x00 && byteFlag3 == 0x12) return true;       // Underwater
            if (byteFlag1 == 0x57 && byteFlag2 == 0x00 && byteFlag3 == 0x57) return true;       // Climbing 2

            return false;
        }

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

        public void SetLvlParams()
        {
            if (GetCleanLvlName() == "Jungle")
            {
                // Health offsets
                smallMedipackOffset = 0xE6;
                largeMedipackOffset = 0xE7;
                SetHealthOffsets(0x6D3);

                // Misc offsets
                numFlaresOffset = 0xE9;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0xED;
                harpoonGunOffset = 0xEE;

                // Ammo offsets
                shotgunAmmoOffset = 0xDC;
                shotgunAmmoOffset2 = 0x164B;
                deagleAmmoOffset = 0xD8;
                deagleAmmoOffset2 = 0x1643;
                grenadeLauncherAmmoOffset = 0xE4;
                grenadeLauncherAmmoOffset2 = 0x1657;
                rocketLauncherAmmoOffset = 0xE0;
                rocketLauncherAmmoOffset2 = 0x1653;
                harpoonAmmoOffset = 0xE2;
                harpoonAmmoOffset2 = 0x164F;
                mp5AmmoOffset = 0xDE;
                mp5AmmoOffset2 = 0x165B;
                uziAmmoOffset = 0xDA;
                uziAmmoOffset2 = 0x1647;
            }
            else if (GetCleanLvlName() == "Temple Ruins")
            {
                // Health offsets
                smallMedipackOffset = 0x119;
                largeMedipackOffset = 0x11A;
                SetHealthOffsets(0x8F7, 0x909);

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
                SetHealthOffsets(0x6B9, 0xB05);

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
                SetHealthOffsets(0xB05);

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
                SetHealthOffsets(0x6B5);

                // Misc offsets
                numFlaresOffset = 0x34D;
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
                SetHealthOffsets(0x6F7, 0x709);

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
                SetHealthOffsets(0xC45, 0xC57);

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
                SetHealthOffsets(0x7BB);

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
                SetHealthOffsets(0x1797, 0x1785, 0x17A9, 0x17BB);

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
                SetHealthOffsets(0xBE3, 0xBF5);

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
                SetHealthOffsets(0x68F);

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
                SetHealthOffsets(0xB15);

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
                SetHealthOffsets(0x2135);

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
                SetHealthOffsets(0xAB1, 0xAD5);

                // Misc offsets
                numFlaresOffset = 0x2E7;
                saveNumOffset = 0x4B;

                // Weapon offsets
                weaponsConfigNumOffset = 0x2EB;
                harpoonGunOffset = 0x2EC;

                // Ammo offsets
                shotgunAmmoOffset = 0x2DA;
                shotgunAmmoOffset2 = 0x1D77;
                deagleAmmoOffset = 0x2D6;
                deagleAmmoOffset2 = 0x1D6F;
                grenadeLauncherAmmoOffset = 0x2E2;
                grenadeLauncherAmmoOffset2 = 0x1D83;
                rocketLauncherAmmoOffset = 0x2DE;
                rocketLauncherAmmoOffset2 = 0x1D7F;
                harpoonAmmoOffset = 0x2E0;
                harpoonAmmoOffset2 = 0x1D7B;
                mp5AmmoOffset = 0x2DC;
                mp5AmmoOffset2 = 0x1D87;
                uziAmmoOffset = 0x2D8;
                uziAmmoOffset2 = 0x1D73;
            }
            else if (GetCleanLvlName() == "City")
            {
                // Health offsets
                smallMedipackOffset = 0x317;
                largeMedipackOffset = 0x318;
                SetHealthOffsets(0x737);

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
                SetHealthOffsets(0x6C5);

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
                SetHealthOffsets(0xA65, 0xA77);

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
                SetHealthOffsets(0x711);

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
                SetHealthOffsets(0x68D);

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
                SetHealthOffsets(0xAF5);

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
        }

        // Offsets
        private int healthOffset = -1;
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

        // Health
        private const int MAX_HEALTH_VALUE = 1000;
        private const int MIN_HEALTH_VALUE = 0;
        private List<int> healthOffsets = new List<int>();

        // Strings
        private string strSaveFilePath;
        private string strSavegameDirectory = "c:\\";

        private void btnSave_Click(object sender, EventArgs e)
        {
            int[] validShotgunAmmoOffsets = GetValidAmmoOffsets(shotgunAmmoOffset, shotgunAmmoOffset2);
            int[] validDeagleAmmoOffsets = GetValidAmmoOffsets(deagleAmmoOffset, deagleAmmoOffset2);
            int[] validGrenadeLauncherAmmoOffsets = GetValidAmmoOffsets(grenadeLauncherAmmoOffset, grenadeLauncherAmmoOffset2);
            int[] validRocketLauncherAmmoOffsets = GetValidAmmoOffsets(rocketLauncherAmmoOffset, rocketLauncherAmmoOffset2);
            int[] validHarpoonAmmoOffsets = GetValidAmmoOffsets(harpoonAmmoOffset, harpoonAmmoOffset2);
            int[] validMp5AmmoOffsets = GetValidAmmoOffsets(mp5AmmoOffset, mp5AmmoOffset2);
            int[] validUziAmmoOffsets = GetValidAmmoOffsets(uziAmmoOffset, uziAmmoOffset2);

            WriteByte(smallMedipackOffset, Decimal.ToInt32(nudSmallMedipacks.Value));
            WriteByte(largeMedipackOffset, Decimal.ToInt32(nudLargeMedipacks.Value));
            WriteByte(numFlaresOffset, Decimal.ToInt32(nudFlares.Value));
            WriteUInt16(saveNumOffset, Decimal.ToInt32(nudSaveNum.Value));

            if (!chkShotgun.Checked)
            {
                WriteUInt16(validShotgunAmmoOffsets[1], 0);
                WriteUInt16(shotgunAmmoOffset, Decimal.ToInt32(nudShotgunAmmo.Value) * 6);
            }
            else
            {
                for (int i = 0; i < validShotgunAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validShotgunAmmoOffsets[i], Decimal.ToInt32(nudShotgunAmmo.Value) * 6);
                }
            }

            if (!chkDesertEagle.Checked)
            {
                WriteUInt16(validDeagleAmmoOffsets[1], 0);
                WriteUInt16(deagleAmmoOffset, Decimal.ToInt32(nudShotgunAmmo.Value));
            }
            else
            {
                for (int i = 0; i < validDeagleAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validDeagleAmmoOffsets[i], Decimal.ToInt32(nudShotgunAmmo.Value));
                }
            }

            if (!chkGrenadeLauncher.Checked)
            {
                WriteUInt16(validGrenadeLauncherAmmoOffsets[1], 0);
                WriteUInt16(grenadeLauncherAmmoOffset, Decimal.ToInt32(nudGrenadeLauncher.Value));
            }
            else
            {
                for (int i = 0; i < validGrenadeLauncherAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validGrenadeLauncherAmmoOffsets[i], Decimal.ToInt32(nudGrenadeLauncher.Value));
                }
            }

            if (!chkRocketLauncher.Checked)
            {
                WriteUInt16(validRocketLauncherAmmoOffsets[1], 0);
                WriteUInt16(rocketLauncherAmmoOffset, Decimal.ToInt32(nudRocketLauncher.Value));
            }
            else
            {
                for (int i = 0; i < validRocketLauncherAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validRocketLauncherAmmoOffsets[i], Decimal.ToInt32(nudRocketLauncher.Value));
                }
            }

            if (!chkHarpoonGun.Checked)
            {
                WriteUInt16(validHarpoonAmmoOffsets[1], 0);
                WriteUInt16(harpoonAmmoOffset, Decimal.ToInt32(nudHarpoonGunAmmo.Value));
            }
            else
            {
                for (int i = 0; i < validHarpoonAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validHarpoonAmmoOffsets[i], Decimal.ToInt32(nudHarpoonGunAmmo.Value));
                }
            }

            if (!chkMp5.Checked)
            {
                WriteUInt16(validMp5AmmoOffsets[1], 0);
                WriteUInt16(mp5AmmoOffset, Decimal.ToInt32(nudMp5Ammo.Value));
            }
            else
            {
                for (int i = 0; i < validMp5AmmoOffsets.Length; i++)
                {
                    WriteUInt16(validMp5AmmoOffsets[i], Decimal.ToInt32(nudMp5Ammo.Value));
                }
            }

            if (!chkUzis.Checked)
            {
                WriteUInt16(validUziAmmoOffsets[1], 0);
                WriteUInt16(uziAmmoOffset, Decimal.ToInt32(nudUziAmmo.Value));
            }
            else
            {
                for (int i = 0; i < validUziAmmoOffsets.Length; i++)
                {
                    WriteUInt16(validUziAmmoOffsets[i], Decimal.ToInt32(nudUziAmmo.Value));
                }
            }

            // Calculate new weapons config number
            byte newWeaponsConfigNum = 1;
            if (chkPistols.Checked) newWeaponsConfigNum += 2;
            if (chkDesertEagle.Checked) newWeaponsConfigNum += 4;
            if (chkUzis.Checked) newWeaponsConfigNum += 8;
            if (chkShotgun.Checked) newWeaponsConfigNum += 16;
            if (chkMp5.Checked) newWeaponsConfigNum += 32;
            if (chkRocketLauncher.Checked) newWeaponsConfigNum += 64;
            if (chkGrenadeLauncher.Checked) newWeaponsConfigNum += 128;

            // Write new weapons config num to save file
            WriteByte(weaponsConfigNumOffset, newWeaponsConfigNum);

            // Write harpoon gun value to save file
            if (chkHarpoonGun.Checked) WriteByte(harpoonGunOffset, 1);
            else WriteByte(harpoonGunOffset, 0);

            // Write new health value to save file
            double newHealthPercentage = (double)trbHealth.Value;
            int newHealth = (int)(newHealthPercentage / 100.0 * MAX_HEALTH_VALUE);
            healthOffset = GetHealthOffset();

            if (healthOffset != -1) WriteUInt16(healthOffset, newHealth);

            slblStatus.Text = "File patched!";

            MessageBox.Show("Save file patched!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = strSavegameDirectory;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    SetSaveFilePath(filePath);
                    strSavegameDirectory = Path.GetDirectoryName(filePath);

                    txtFilePath.Clear();
                    txtFilePath.AppendText(filePath);

                    btnSave.Enabled = true;

                    slblStatus.Text = "Loaded save file: " + Path.GetFileName(openFileDialog.FileName);

                    SetLvlParams();
                    DisplayWeaponsInfo();
                    DisplayNumSmallMedipacks();
                    DisplayNumLargeMedipacks();
                    DisplayLvlName();
                    DisplayShotgunAmmo();
                    DisplayDeagleAmmo();
                    DisplayNumFlares();
                    DisplayMP5Ammo();
                    DisplaySaveNum();
                    DisplayUziAmmo();
                    DisplayGrenadeLauncherAmmo();
                    DisplayHarpoonAmmo();
                    DisplayRocketLauncherAmmo();
                    DisplayHealthValue();
                }
            }
        }

        private void trbHealth_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealth.Value;
            lblHealth.Text = healthPercentage.ToString("0.0") + "%";
        }
    }
}