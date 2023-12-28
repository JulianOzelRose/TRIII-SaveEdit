
namespace TRIII_SaveEdit
{
    partial class TR3Edit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TR3Edit));
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.grpLevel = new System.Windows.Forms.GroupBox();
            this.lblSaveNumber = new System.Windows.Forms.Label();
            this.nudSaveNum = new System.Windows.Forms.NumericUpDown();
            this.lblLvlName = new System.Windows.Forms.Label();
            this.txtLvlName = new System.Windows.Forms.TextBox();
            this.grpItems = new System.Windows.Forms.GroupBox();
            this.lblFlares = new System.Windows.Forms.Label();
            this.nudFlares = new System.Windows.Forms.NumericUpDown();
            this.lblLargeMedipacks = new System.Windows.Forms.Label();
            this.lblSmallMedipacks = new System.Windows.Forms.Label();
            this.nudLargeMedipacks = new System.Windows.Forms.NumericUpDown();
            this.nudSmallMedipacks = new System.Windows.Forms.NumericUpDown();
            this.lblHealthError = new System.Windows.Forms.Label();
            this.lblHealth = new System.Windows.Forms.Label();
            this.trbHealth = new System.Windows.Forms.TrackBar();
            this.grpWeapons = new System.Windows.Forms.GroupBox();
            this.lblPistolAmmo = new System.Windows.Forms.Label();
            this.nudMp5Ammo = new System.Windows.Forms.NumericUpDown();
            this.chkMp5 = new System.Windows.Forms.CheckBox();
            this.nudRocketLauncher = new System.Windows.Forms.NumericUpDown();
            this.chkRocketLauncher = new System.Windows.Forms.CheckBox();
            this.nudUziAmmo = new System.Windows.Forms.NumericUpDown();
            this.chkUzis = new System.Windows.Forms.CheckBox();
            this.nudHarpoonGunAmmo = new System.Windows.Forms.NumericUpDown();
            this.chkHarpoonGun = new System.Windows.Forms.CheckBox();
            this.chkPistols = new System.Windows.Forms.CheckBox();
            this.nudGrenadeLauncher = new System.Windows.Forms.NumericUpDown();
            this.chkGrenadeLauncher = new System.Windows.Forms.CheckBox();
            this.nudDesertEagleAmmo = new System.Windows.Forms.NumericUpDown();
            this.chkDesertEagle = new System.Windows.Forms.CheckBox();
            this.nudShotgunAmmo = new System.Windows.Forms.NumericUpDown();
            this.chkShotgun = new System.Windows.Forms.CheckBox();
            this.grpHealth = new System.Windows.Forms.GroupBox();
            this.ssStatusStrip = new System.Windows.Forms.StatusStrip();
            this.slblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaveNum)).BeginInit();
            this.grpItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlares)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLargeMedipacks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSmallMedipacks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHealth)).BeginInit();
            this.grpWeapons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMp5Ammo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRocketLauncher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUziAmmo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHarpoonGunAmmo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrenadeLauncher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDesertEagleAmmo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShotgunAmmo)).BeginInit();
            this.grpHealth.SuspendLayout();
            this.ssStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(39, 15);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(398, 20);
            this.txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(441, 14);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(520, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(12, 18);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(26, 13);
            this.lblFile.TabIndex = 3;
            this.lblFile.Text = "File:";
            // 
            // grpLevel
            // 
            this.grpLevel.Controls.Add(this.lblSaveNumber);
            this.grpLevel.Controls.Add(this.nudSaveNum);
            this.grpLevel.Controls.Add(this.lblLvlName);
            this.grpLevel.Controls.Add(this.txtLvlName);
            this.grpLevel.Location = new System.Drawing.Point(8, 41);
            this.grpLevel.Name = "grpLevel";
            this.grpLevel.Size = new System.Drawing.Size(589, 56);
            this.grpLevel.TabIndex = 4;
            this.grpLevel.TabStop = false;
            this.grpLevel.Text = "Level";
            // 
            // lblSaveNumber
            // 
            this.lblSaveNumber.AutoSize = true;
            this.lblSaveNumber.Location = new System.Drawing.Point(424, 23);
            this.lblSaveNumber.Name = "lblSaveNumber";
            this.lblSaveNumber.Size = new System.Drawing.Size(75, 13);
            this.lblSaveNumber.TabIndex = 5;
            this.lblSaveNumber.Text = "Save Number:";
            // 
            // nudSaveNum
            // 
            this.nudSaveNum.Location = new System.Drawing.Point(511, 21);
            this.nudSaveNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudSaveNum.Name = "nudSaveNum";
            this.nudSaveNum.Size = new System.Drawing.Size(55, 20);
            this.nudSaveNum.TabIndex = 4;
            // 
            // lblLvlName
            // 
            this.lblLvlName.AutoSize = true;
            this.lblLvlName.Location = new System.Drawing.Point(31, 24);
            this.lblLvlName.Name = "lblLvlName";
            this.lblLvlName.Size = new System.Drawing.Size(38, 13);
            this.lblLvlName.TabIndex = 1;
            this.lblLvlName.Text = "Name:";
            // 
            // txtLvlName
            // 
            this.txtLvlName.Location = new System.Drawing.Point(75, 21);
            this.txtLvlName.Name = "txtLvlName";
            this.txtLvlName.ReadOnly = true;
            this.txtLvlName.Size = new System.Drawing.Size(224, 20);
            this.txtLvlName.TabIndex = 0;
            this.txtLvlName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpItems
            // 
            this.grpItems.Controls.Add(this.lblFlares);
            this.grpItems.Controls.Add(this.nudFlares);
            this.grpItems.Controls.Add(this.lblLargeMedipacks);
            this.grpItems.Controls.Add(this.lblSmallMedipacks);
            this.grpItems.Controls.Add(this.nudLargeMedipacks);
            this.grpItems.Controls.Add(this.nudSmallMedipacks);
            this.grpItems.Location = new System.Drawing.Point(8, 103);
            this.grpItems.Name = "grpItems";
            this.grpItems.Size = new System.Drawing.Size(299, 136);
            this.grpItems.TabIndex = 5;
            this.grpItems.TabStop = false;
            this.grpItems.Text = "Items";
            // 
            // lblFlares
            // 
            this.lblFlares.AutoSize = true;
            this.lblFlares.Location = new System.Drawing.Point(31, 101);
            this.lblFlares.Name = "lblFlares";
            this.lblFlares.Size = new System.Drawing.Size(38, 13);
            this.lblFlares.TabIndex = 4;
            this.lblFlares.Text = "Flares:";
            // 
            // nudFlares
            // 
            this.nudFlares.Location = new System.Drawing.Point(225, 96);
            this.nudFlares.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudFlares.Name = "nudFlares";
            this.nudFlares.Size = new System.Drawing.Size(55, 20);
            this.nudFlares.TabIndex = 18;
            // 
            // lblLargeMedipacks
            // 
            this.lblLargeMedipacks.AutoSize = true;
            this.lblLargeMedipacks.Location = new System.Drawing.Point(31, 63);
            this.lblLargeMedipacks.Name = "lblLargeMedipacks";
            this.lblLargeMedipacks.Size = new System.Drawing.Size(92, 13);
            this.lblLargeMedipacks.TabIndex = 3;
            this.lblLargeMedipacks.Text = "Large Medipacks:";
            // 
            // lblSmallMedipacks
            // 
            this.lblSmallMedipacks.AutoSize = true;
            this.lblSmallMedipacks.Location = new System.Drawing.Point(31, 30);
            this.lblSmallMedipacks.Name = "lblSmallMedipacks";
            this.lblSmallMedipacks.Size = new System.Drawing.Size(90, 13);
            this.lblSmallMedipacks.TabIndex = 2;
            this.lblSmallMedipacks.Text = "Small Medipacks:";
            // 
            // nudLargeMedipacks
            // 
            this.nudLargeMedipacks.Location = new System.Drawing.Point(225, 62);
            this.nudLargeMedipacks.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudLargeMedipacks.Name = "nudLargeMedipacks";
            this.nudLargeMedipacks.Size = new System.Drawing.Size(55, 20);
            this.nudLargeMedipacks.TabIndex = 1;
            // 
            // nudSmallMedipacks
            // 
            this.nudSmallMedipacks.Location = new System.Drawing.Point(225, 27);
            this.nudSmallMedipacks.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSmallMedipacks.Name = "nudSmallMedipacks";
            this.nudSmallMedipacks.Size = new System.Drawing.Size(55, 20);
            this.nudSmallMedipacks.TabIndex = 0;
            // 
            // lblHealthError
            // 
            this.lblHealthError.AutoSize = true;
            this.lblHealthError.Location = new System.Drawing.Point(64, 53);
            this.lblHealthError.Name = "lblHealthError";
            this.lblHealthError.Size = new System.Drawing.Size(136, 13);
            this.lblHealthError.TabIndex = 7;
            this.lblHealthError.Text = "Unable to find health bytes.";
            this.lblHealthError.Visible = false;
            // 
            // lblHealth
            // 
            this.lblHealth.AutoSize = true;
            this.lblHealth.Location = new System.Drawing.Point(251, 27);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(30, 13);
            this.lblHealth.TabIndex = 6;
            this.lblHealth.Text = "0.0%";
            // 
            // trbHealth
            // 
            this.trbHealth.Location = new System.Drawing.Point(12, 21);
            this.trbHealth.Maximum = 100;
            this.trbHealth.Name = "trbHealth";
            this.trbHealth.Size = new System.Drawing.Size(241, 45);
            this.trbHealth.TabIndex = 4;
            this.trbHealth.Scroll += new System.EventHandler(this.trbHealth_Scroll);
            // 
            // grpWeapons
            // 
            this.grpWeapons.Controls.Add(this.lblPistolAmmo);
            this.grpWeapons.Controls.Add(this.nudMp5Ammo);
            this.grpWeapons.Controls.Add(this.chkMp5);
            this.grpWeapons.Controls.Add(this.nudRocketLauncher);
            this.grpWeapons.Controls.Add(this.chkRocketLauncher);
            this.grpWeapons.Controls.Add(this.nudUziAmmo);
            this.grpWeapons.Controls.Add(this.chkUzis);
            this.grpWeapons.Controls.Add(this.nudHarpoonGunAmmo);
            this.grpWeapons.Controls.Add(this.chkHarpoonGun);
            this.grpWeapons.Controls.Add(this.chkPistols);
            this.grpWeapons.Controls.Add(this.nudGrenadeLauncher);
            this.grpWeapons.Controls.Add(this.chkGrenadeLauncher);
            this.grpWeapons.Controls.Add(this.nudDesertEagleAmmo);
            this.grpWeapons.Controls.Add(this.chkDesertEagle);
            this.grpWeapons.Controls.Add(this.nudShotgunAmmo);
            this.grpWeapons.Controls.Add(this.chkShotgun);
            this.grpWeapons.Location = new System.Drawing.Point(313, 103);
            this.grpWeapons.Name = "grpWeapons";
            this.grpWeapons.Size = new System.Drawing.Size(284, 216);
            this.grpWeapons.TabIndex = 7;
            this.grpWeapons.TabStop = false;
            this.grpWeapons.Text = "Weapons";
            // 
            // lblPistolAmmo
            // 
            this.lblPistolAmmo.AutoSize = true;
            this.lblPistolAmmo.Location = new System.Drawing.Point(205, 26);
            this.lblPistolAmmo.Name = "lblPistolAmmo";
            this.lblPistolAmmo.Size = new System.Drawing.Size(50, 13);
            this.lblPistolAmmo.TabIndex = 4;
            this.lblPistolAmmo.Text = "Unlimited";
            // 
            // nudMp5Ammo
            // 
            this.nudMp5Ammo.Location = new System.Drawing.Point(207, 162);
            this.nudMp5Ammo.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudMp5Ammo.Name = "nudMp5Ammo";
            this.nudMp5Ammo.Size = new System.Drawing.Size(55, 20);
            this.nudMp5Ammo.TabIndex = 21;
            // 
            // chkMp5
            // 
            this.chkMp5.AutoSize = true;
            this.chkMp5.Location = new System.Drawing.Point(19, 162);
            this.chkMp5.Name = "chkMp5";
            this.chkMp5.Size = new System.Drawing.Size(51, 17);
            this.chkMp5.TabIndex = 20;
            this.chkMp5.Text = "MP5:";
            this.chkMp5.UseVisualStyleBackColor = true;
            // 
            // nudRocketLauncher
            // 
            this.nudRocketLauncher.Location = new System.Drawing.Point(207, 114);
            this.nudRocketLauncher.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudRocketLauncher.Name = "nudRocketLauncher";
            this.nudRocketLauncher.Size = new System.Drawing.Size(55, 20);
            this.nudRocketLauncher.TabIndex = 19;
            // 
            // chkRocketLauncher
            // 
            this.chkRocketLauncher.AutoSize = true;
            this.chkRocketLauncher.Location = new System.Drawing.Point(19, 116);
            this.chkRocketLauncher.Name = "chkRocketLauncher";
            this.chkRocketLauncher.Size = new System.Drawing.Size(112, 17);
            this.chkRocketLauncher.TabIndex = 18;
            this.chkRocketLauncher.Text = "Rocket Launcher:";
            this.chkRocketLauncher.UseVisualStyleBackColor = true;
            // 
            // nudUziAmmo
            // 
            this.nudUziAmmo.Location = new System.Drawing.Point(207, 186);
            this.nudUziAmmo.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudUziAmmo.Name = "nudUziAmmo";
            this.nudUziAmmo.Size = new System.Drawing.Size(55, 20);
            this.nudUziAmmo.TabIndex = 15;
            // 
            // chkUzis
            // 
            this.chkUzis.AutoSize = true;
            this.chkUzis.Location = new System.Drawing.Point(19, 184);
            this.chkUzis.Name = "chkUzis";
            this.chkUzis.Size = new System.Drawing.Size(49, 17);
            this.chkUzis.TabIndex = 14;
            this.chkUzis.Text = "Uzis:";
            this.chkUzis.UseVisualStyleBackColor = true;
            // 
            // nudHarpoonGunAmmo
            // 
            this.nudHarpoonGunAmmo.Location = new System.Drawing.Point(207, 138);
            this.nudHarpoonGunAmmo.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudHarpoonGunAmmo.Name = "nudHarpoonGunAmmo";
            this.nudHarpoonGunAmmo.Size = new System.Drawing.Size(55, 20);
            this.nudHarpoonGunAmmo.TabIndex = 13;
            // 
            // chkHarpoonGun
            // 
            this.chkHarpoonGun.AutoSize = true;
            this.chkHarpoonGun.Location = new System.Drawing.Point(19, 139);
            this.chkHarpoonGun.Name = "chkHarpoonGun";
            this.chkHarpoonGun.Size = new System.Drawing.Size(93, 17);
            this.chkHarpoonGun.TabIndex = 12;
            this.chkHarpoonGun.Text = "Harpoon Gun:";
            this.chkHarpoonGun.UseVisualStyleBackColor = true;
            // 
            // chkPistols
            // 
            this.chkPistols.AutoSize = true;
            this.chkPistols.Location = new System.Drawing.Point(19, 29);
            this.chkPistols.Name = "chkPistols";
            this.chkPistols.Size = new System.Drawing.Size(56, 17);
            this.chkPistols.TabIndex = 11;
            this.chkPistols.Text = "Pistols";
            this.chkPistols.UseVisualStyleBackColor = true;
            // 
            // nudGrenadeLauncher
            // 
            this.nudGrenadeLauncher.Location = new System.Drawing.Point(207, 90);
            this.nudGrenadeLauncher.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudGrenadeLauncher.Name = "nudGrenadeLauncher";
            this.nudGrenadeLauncher.Size = new System.Drawing.Size(55, 20);
            this.nudGrenadeLauncher.TabIndex = 8;
            // 
            // chkGrenadeLauncher
            // 
            this.chkGrenadeLauncher.AutoSize = true;
            this.chkGrenadeLauncher.Location = new System.Drawing.Point(19, 94);
            this.chkGrenadeLauncher.Name = "chkGrenadeLauncher";
            this.chkGrenadeLauncher.Size = new System.Drawing.Size(118, 17);
            this.chkGrenadeLauncher.TabIndex = 7;
            this.chkGrenadeLauncher.Text = "Grenade Launcher:";
            this.chkGrenadeLauncher.UseVisualStyleBackColor = true;
            // 
            // nudDesertEagleAmmo
            // 
            this.nudDesertEagleAmmo.Location = new System.Drawing.Point(207, 67);
            this.nudDesertEagleAmmo.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudDesertEagleAmmo.Name = "nudDesertEagleAmmo";
            this.nudDesertEagleAmmo.Size = new System.Drawing.Size(55, 20);
            this.nudDesertEagleAmmo.TabIndex = 6;
            // 
            // chkDesertEagle
            // 
            this.chkDesertEagle.AutoSize = true;
            this.chkDesertEagle.Location = new System.Drawing.Point(19, 72);
            this.chkDesertEagle.Name = "chkDesertEagle";
            this.chkDesertEagle.Size = new System.Drawing.Size(90, 17);
            this.chkDesertEagle.TabIndex = 5;
            this.chkDesertEagle.Text = "Desert Eagle:";
            this.chkDesertEagle.UseVisualStyleBackColor = true;
            // 
            // nudShotgunAmmo
            // 
            this.nudShotgunAmmo.Location = new System.Drawing.Point(207, 43);
            this.nudShotgunAmmo.Maximum = new decimal(new int[] {
            10922,
            0,
            0,
            0});
            this.nudShotgunAmmo.Name = "nudShotgunAmmo";
            this.nudShotgunAmmo.Size = new System.Drawing.Size(55, 20);
            this.nudShotgunAmmo.TabIndex = 4;
            // 
            // chkShotgun
            // 
            this.chkShotgun.AutoSize = true;
            this.chkShotgun.Location = new System.Drawing.Point(19, 51);
            this.chkShotgun.Name = "chkShotgun";
            this.chkShotgun.Size = new System.Drawing.Size(69, 17);
            this.chkShotgun.TabIndex = 0;
            this.chkShotgun.Text = "Shotgun:";
            this.chkShotgun.UseVisualStyleBackColor = true;
            // 
            // grpHealth
            // 
            this.grpHealth.Controls.Add(this.lblHealthError);
            this.grpHealth.Controls.Add(this.trbHealth);
            this.grpHealth.Controls.Add(this.lblHealth);
            this.grpHealth.Location = new System.Drawing.Point(8, 245);
            this.grpHealth.Name = "grpHealth";
            this.grpHealth.Size = new System.Drawing.Size(299, 74);
            this.grpHealth.TabIndex = 18;
            this.grpHealth.TabStop = false;
            this.grpHealth.Text = "Health";
            // 
            // ssStatusStrip
            // 
            this.ssStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblStatus});
            this.ssStatusStrip.Location = new System.Drawing.Point(0, 329);
            this.ssStatusStrip.Name = "ssStatusStrip";
            this.ssStatusStrip.Size = new System.Drawing.Size(604, 22);
            this.ssStatusStrip.SizingGrip = false;
            this.ssStatusStrip.TabIndex = 19;
            this.ssStatusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.slblStatus.Name = "toolStripStatusLabel";
            this.slblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // TR3Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 351);
            this.Controls.Add(this.ssStatusStrip);
            this.Controls.Add(this.grpHealth);
            this.Controls.Add(this.grpWeapons);
            this.Controls.Add(this.grpItems);
            this.Controls.Add(this.grpLevel);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TR3Edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tomb Raider III - Savegame Editor";
            this.grpLevel.ResumeLayout(false);
            this.grpLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaveNum)).EndInit();
            this.grpItems.ResumeLayout(false);
            this.grpItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlares)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLargeMedipacks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSmallMedipacks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHealth)).EndInit();
            this.grpWeapons.ResumeLayout(false);
            this.grpWeapons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMp5Ammo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRocketLauncher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUziAmmo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHarpoonGunAmmo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrenadeLauncher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDesertEagleAmmo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShotgunAmmo)).EndInit();
            this.grpHealth.ResumeLayout(false);
            this.grpHealth.PerformLayout();
            this.ssStatusStrip.ResumeLayout(false);
            this.ssStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.GroupBox grpLevel;
        private System.Windows.Forms.Label lblLvlName;
        private System.Windows.Forms.TextBox txtLvlName;
        private System.Windows.Forms.GroupBox grpItems;
        private System.Windows.Forms.Label lblLargeMedipacks;
        private System.Windows.Forms.Label lblSmallMedipacks;
        private System.Windows.Forms.NumericUpDown nudLargeMedipacks;
        private System.Windows.Forms.NumericUpDown nudSmallMedipacks;
        private System.Windows.Forms.GroupBox grpWeapons;
        private System.Windows.Forms.NumericUpDown nudShotgunAmmo;
        private System.Windows.Forms.CheckBox chkShotgun;
        private System.Windows.Forms.CheckBox chkDesertEagle;
        private System.Windows.Forms.NumericUpDown nudDesertEagleAmmo;
        private System.Windows.Forms.NumericUpDown nudGrenadeLauncher;
        private System.Windows.Forms.CheckBox chkGrenadeLauncher;
        private System.Windows.Forms.CheckBox chkPistols;
        private System.Windows.Forms.NumericUpDown nudHarpoonGunAmmo;
        private System.Windows.Forms.CheckBox chkHarpoonGun;
        private System.Windows.Forms.NumericUpDown nudUziAmmo;
        private System.Windows.Forms.CheckBox chkUzis;
        private System.Windows.Forms.GroupBox grpHealth;
        private System.Windows.Forms.Label lblFlares;
        private System.Windows.Forms.NumericUpDown nudFlares;
        private System.Windows.Forms.Label lblSaveNumber;
        private System.Windows.Forms.NumericUpDown nudSaveNum;
        private System.Windows.Forms.NumericUpDown nudRocketLauncher;
        private System.Windows.Forms.CheckBox chkRocketLauncher;
        private System.Windows.Forms.NumericUpDown nudMp5Ammo;
        private System.Windows.Forms.CheckBox chkMp5;
        private System.Windows.Forms.Label lblPistolAmmo;
        private System.Windows.Forms.TrackBar trbHealth;
        private System.Windows.Forms.Label lblHealth;
        private System.Windows.Forms.Label lblHealthError;
        private System.Windows.Forms.StatusStrip ssStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel slblStatus;
    }
}

