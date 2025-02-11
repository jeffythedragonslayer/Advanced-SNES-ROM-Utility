﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advanced_SNES_ROM_Utility
{
    public partial class frmMain : Form
    {
        // Create empty ROM
        SNESROM sourceROM;

        Crc32 savedFileCRC32 = new Crc32();
        string savedFileHash;

        bool saveWithHeader;

        // Create combo box for selecting country and region
        List<ComboBoxCountryRegionList> listCountryRegion = new List<ComboBoxCountryRegionList>();
        List<ComboBoxExpandROMList> listExpandROM = new List<ComboBoxExpandROMList>();
        List<ComboBoxSplitROMList> listSplitROM = new List<ComboBoxSplitROMList>();

        public frmMain()
        {
            InitializeComponent();

            // Fill combo box for country and region
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 0, Name = "Japan | NTSC" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 1, Name = "USA | NTSC" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 2, Name = "Europe/Oceania/Asia | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 3, Name = "Sweden/Scandinavia | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 4, Name = "Finland | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 5, Name = "Denmark | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 6, Name = "France | SECAM (PAL-like, 50 Hz)" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 7, Name = "Netherlands | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 8, Name = "Spain | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 9, Name = "Germany/Austria/Switzerland | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 10, Name = "China/Hong Kong | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 11, Name = "Indonesia | PAL" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 12, Name = "South Korea | NTSC" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 14, Name = "Canada | NTSC" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 15, Name = "Brazil | PAL-M (NTSC-like, 60 Hz)" });
            listCountryRegion.Add(new ComboBoxCountryRegionList { Id = 16, Name = "Australia | PAL" });
        }

        private void ButtonSelectROM_Click(object sender, EventArgs e)
        {
            // Select ROM file dialogue
            OpenFileDialog selectROMDialog = new OpenFileDialog();

            selectROMDialog.Filter = "SNES/SFC ROMs (*.smc;*.swc;*;*.sfc;*.fig)|*.smc;*.swc*;*.sfc;*.fig|" +
                                     "All Files (*.*)|*.*";

            // If successfully selected a ROM file...
            if (selectROMDialog.ShowDialog() == DialogResult.OK)
            {
                // Create new ROM
                sourceROM = new SNESROM(@selectROMDialog.FileName);
                if (sourceROM.SourceROM == null) { return; }

                // Store CRC32 for dirty tracking
                savedFileHash = GetCRC32FromFile(@selectROMDialog.FileName);

                // Check if ROM contains region locks
                if (sourceROM.UnlockRegion(false)) 
                {
                    buttonFixRegion.Enabled = true;
                }
                
                else
                {
                    buttonFixRegion.Enabled = false;
                }

                // Check if FastROM contains SlowROM checks
                if (sourceROM.ByteROMSpeed == 0x30)
                {
                    if (sourceROM.RemoveSlowROMChecks(false))
                    {
                        buttonFixSlowROMChecks.Enabled = true;
                    }
                }

                else
                {
                    buttonFixSlowROMChecks.Enabled = false;
                }

                // Check if ROM contains SRAM checks
                if (sourceROM.RemoveSRAMChecks(false))
                {
                    buttonFixSRAMChecks.Enabled = true;
                }

                else
                {
                    buttonFixSRAMChecks.Enabled = false;
                }

                // Initialize combo box for country and region
                comboBoxCountryRegion.DataSource = listCountryRegion;
                comboBoxCountryRegion.DisplayMember = "Name";
                comboBoxCountryRegion.ValueMember = "Id";

                // Enable / disable text and combo boxes
                if (!textBoxTitle.Enabled) { textBoxTitle.Enabled = true; }
                if (sourceROM.IsBSROM) { comboBoxCountryRegion.Enabled = false; } else { comboBoxCountryRegion.Enabled = true; }
                textBoxTitle.MaxLength = sourceROM.StringTitle.Length;
                if (!textBoxVersion.Enabled) { textBoxVersion.Enabled = true; }

                // Load values into labels and enable / disable buttons
                RefreshLabelsAndButtons();
            }
        }

        private void ButtonAddHeader_Click(object sender, EventArgs e)
        {
            sourceROM.AddHeader();
            RefreshLabelsAndButtons();
        }

        private void ButtonRemoveHeader_Click(object sender, EventArgs e)
        {
            sourceROM.RemoveHeader();
            RefreshLabelsAndButtons();
        }

        private void ButtonSwapBinROM_Click(object sender, EventArgs e)
        {
            sourceROM.SwapBin();
            MessageBox.Show("ROM successfully swapped!\n\nFile(s) saved to: '" + sourceROM.ROMFolder + "\n\nIn case there was a header, it has been removed!");
        }

        private void ButtonExpandROM_Click(object sender, EventArgs e)
        {
            if ((int)comboBoxExpandROM.SelectedValue < 1) { return; }

            // Create new ROM for expanding
            int sizeExpandedROM = (int)comboBoxExpandROM.SelectedValue - ((int)comboBoxExpandROM.SelectedValue % 2);
            byte[] expandedROM = new byte[sizeExpandedROM * 131072];

            foreach (byte singleByte in expandedROM) { expandedROM[singleByte] = 0x00; }

            Buffer.BlockCopy(sourceROM.SourceROM, 0, expandedROM, 0, sourceROM.SourceROM.Length);

            // If expanding a non ExROM to ExROM
            if (sizeExpandedROM >= 48 && sourceROM.IntCalcFileSize <= 32 && (int)comboBoxExpandROM.SelectedValue % 2 == 0)
            {
                if (sourceROM.StringMapMode.StartsWith("LoROM"))
                {
                  //if (sourceROM.ByteMapMode >= 0x30)
                  //{

                    if (sourceROM.ByteMapMode >= 0x30)          // <-- remove this if block when using uncommented way!
                    {
                        byte[] newMapMode = new byte[1] { 0x32 };
                        Buffer.BlockCopy(newMapMode, 0, expandedROM, (int)sourceROM.UIntROMHeaderOffset + 0x25, 1);
                    }
                    
                    Buffer.BlockCopy(expandedROM, 0, expandedROM, 0x400000, 0x8000);

                  //}
                    /*
                    else
                    {
                        int size = 0x400000;
                        if (sizeExpandedROM == 48) { size = 0x200000; }
                        Buffer.BlockCopy(expandedROM, 0, expandedROM, 0x400000, size);

                        int fillZeroLength = 0x3F8000;
                        if (sizeExpandedROM == 48) { fillZeroLength = 0x1F8000; }
                        byte[] fillZeroByte = new byte[fillZeroLength];
                        foreach (byte singleByte in fillZeroByte) { fillZeroByte[singleByte] = 0x00; }

                        Buffer.BlockCopy(fillZeroByte, 0, expandedROM, 0x8000, fillZeroByte.Length);
                    }*/
                }

                else if (sourceROM.StringMapMode.StartsWith("HiROM"))
                {
                    byte[] newMapMode = new byte[1];
                    newMapMode[0] = (byte)(sourceROM.ByteROMSpeed | 0x05);
                    Buffer.BlockCopy(newMapMode, 0, expandedROM, (int)sourceROM.UIntROMHeaderOffset + 0x25, 1);
                    Buffer.BlockCopy(expandedROM, 0x8000, expandedROM, 0x408000, 0x8000);
                }
            }

            // If expanding a non ExROM to ExROM using mirroring
            if (sizeExpandedROM >= 48 && sourceROM.IntCalcFileSize <= 32 && (int)comboBoxExpandROM.SelectedValue % 2 == 1)
            {
                if (sourceROM.StringMapMode.Contains("LoROM"))
                {
                    if (sourceROM.ByteMapMode >= 0x30)
                    {
                        byte[] newMapMode = new byte[1] { 0x32 };
                        Buffer.BlockCopy(newMapMode, 0, expandedROM, (int)sourceROM.UIntROMHeaderOffset + 0x25, 1);
                    }

                    int size = 0x400000;
                    if (sizeExpandedROM == 48) { size = 0x200000; }
                    Buffer.BlockCopy(expandedROM, 0, expandedROM, 0x400000, size);
                }

                else if (sourceROM.StringMapMode.Contains("HiROM"))
                {
                    byte[] newMapMode = new byte[1];
                    newMapMode[0] = (byte)(sourceROM.ByteROMSpeed | 0x05);
                    Buffer.BlockCopy(newMapMode, 0, expandedROM, (int)sourceROM.UIntROMHeaderOffset + 0x25, 1);

                    int offset = 0x408000;

                    while (offset < expandedROM.Length)
                    {
                        int firstHalfLocation = offset - 0x400000;
                        Buffer.BlockCopy(expandedROM, firstHalfLocation, expandedROM, offset, 0x8000);
                        offset += 0x10000;
                    }
                }
            }

            sourceROM.SourceROM = expandedROM;

            sourceROM.Initialize();
            RefreshLabelsAndButtons();
        }

        private void ButtonSplitROM_Click(object sender, EventArgs e)
        {
            int splitROMSize = (int)comboBoxSplitROM.SelectedValue;
            int romChunks = sourceROM.SourceROM.Length / (splitROMSize * 131072);

            for (int index = 0; index < romChunks; index++)
            {
                string romChunkName = sourceROM.ROMName + "_[" + index + "]";
                byte[] splitROM = new byte[splitROMSize * 131072];

                Buffer.BlockCopy(sourceROM.SourceROM, index * (splitROMSize * 131072), splitROM, 0, splitROMSize * 131072);

                // Save file split
                File.WriteAllBytes(@sourceROM.ROMFolder + @"\" + romChunkName + "_[split]" + ".bin", splitROM);
                MessageBox.Show("ROM successfully splittet!\n\nFile saved to: '" + @sourceROM.ROMFolder + @"\" + romChunkName + "_[split]" + ".bin'\n\nIn case there was a header, it has been removed!");
            }
        }

        private void ButtonDeinterleave_Click(object sender, EventArgs e)
        {
            sourceROM.Deinterlave();
            RefreshLabelsAndButtons();
        }

        private void ButtonFixChksm_Click(object sender, EventArgs e)
        {
            sourceROM.FixChecksum();
            RefreshLabelsAndButtons();
        }

        private void ButtonFixRegion_Click(object sender, EventArgs e)
        {
            sourceROM.UnlockRegion(true);
            RefreshLabelsAndButtons();
            // Set button manually, because RefreshLabelsAndButtons doesn't do that for performace reasons
            buttonFixRegion.Enabled = false;
        }

        private void ButtonSlowROMFix_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Removing SlowROM checks from a FastROM will force this ROM into SlowROM mode.\n\n" +
                                                        "This might cause some slow downs while playing.\n\n" +
                                                        "It isn't really recommended or necessary to do this anymore.\n\n" +
                                                        "This only makes sense, if you want to play this ROM on a device slower than 120ns.\n\n\n" +
                                                        "Do you want to proceed anyway?", "Attention!", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                sourceROM.RemoveSlowROMChecks(true);
                RefreshLabelsAndButtons();
                // Set button manually, because RefreshLabelsAndButtons doesn't do that for performace reasons
                buttonFixSlowROMChecks.Enabled = false;
            }

            else
            {
                return;
            }
        }

        private void ButtonFixSRAMChecks_Click(object sender, EventArgs e)
        {
            sourceROM.RemoveSRAMChecks(true);
            RefreshLabelsAndButtons();
            // Set button manually, because RefreshLabelsAndButtons doesn't do that for performace reasons
            buttonFixSRAMChecks.Enabled = false;
        }

        private void ButtonFixROMSize_Click(object sender, EventArgs e)
        {
            sourceROM.FixInternalROMSize();
            RefreshLabelsAndButtons();
        }

        private void ButtonPatch_Click(object sender, EventArgs e)
        {
            // Select patch file dialogue
            OpenFileDialog selectPatchDialog = new OpenFileDialog();

            selectPatchDialog.Filter = "Patch File (*.ips;*.ups;*.bps;*.bdf;*.xdelta)|*.ips;*.ups;*.bps;*.bdf;*.xdelta";

            // If successfully selected a patch file...
            if (selectPatchDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] patchedSourceROM = null;
                byte[] mergedSourceROM = new byte[sourceROM.SourceROM.Length + sourceROM.UIntSMCHeader];

                if (sourceROM.SourceROMSMCHeader != null && sourceROM.UIntSMCHeader > 0)
                {
                    // Merge header with ROM if header exists
                    Buffer.BlockCopy(sourceROM.SourceROMSMCHeader, 0, mergedSourceROM, 0, sourceROM.SourceROMSMCHeader.Length);
                    Buffer.BlockCopy(sourceROM.SourceROM, 0, mergedSourceROM, sourceROM.SourceROMSMCHeader.Length, sourceROM.SourceROM.Length);
                }

                else
                {
                    // Just copy source ROM if no header exists
                    Buffer.BlockCopy(sourceROM.SourceROM, 0, mergedSourceROM, 0, sourceROM.SourceROM.Length);
                }

                switch (Path.GetExtension(selectPatchDialog.FileName))
                {
                    case ".ips": patchedSourceROM = IPSPatch.Apply(mergedSourceROM, selectPatchDialog.FileName); break;
                    case ".ups": patchedSourceROM = UPSPatch.Apply(mergedSourceROM, sourceROM.CRC32Hash, selectPatchDialog.FileName); break;
                    case ".bps": patchedSourceROM = BPSPatch.Apply(mergedSourceROM, sourceROM.CRC32Hash, selectPatchDialog.FileName); break;
                    case ".bdf": patchedSourceROM = BDFPatch.Apply(mergedSourceROM, selectPatchDialog.FileName); break;
                    case ".xdelta": patchedSourceROM = XDELTAPatch.Apply(mergedSourceROM, selectPatchDialog.FileName); break;
                }
                
                if (patchedSourceROM != null)
                {
                    sourceROM.SourceROM = patchedSourceROM;
                    sourceROM.UIntSMCHeader = 0;
                    sourceROM.SourceROMSMCHeader = null;
                    sourceROM.Initialize();
                    RefreshLabelsAndButtons();
                    MessageBox.Show("ROM has successfully been patched!");
                }

                else
                {
                    MessageBox.Show("Could not apply patch! Please check if your patch is valid for your ROM.");
                }
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Save(sourceROM.ROMFullPath, "ROM file has successfully been saved!", saveWithHeader);
        }

        private void ButtonSaveAs_Click(object sender, EventArgs e)
        {
            // Save ROM file dialogue
            SaveFileDialog saveROMDialog = new SaveFileDialog();
            saveROMDialog.InitialDirectory = Path.GetFullPath(sourceROM.ROMFolder);
            saveROMDialog.FileName = Path.GetFileName(sourceROM.ROMFullPath);
            saveROMDialog.DefaultExt = Path.GetExtension(sourceROM.ROMFullPath);

            saveROMDialog.Filter = "SNES/SFC ROMs (*.smc;*.swc;*;*.sfc;*.fig)|*.smc;*.swc*;*.sfc;*.fig|" +
                                   "All Files (*.*)|*.*";

            if (saveROMDialog.ShowDialog() == DialogResult.OK)
            {
                // Use Save function for writing file
                Save(@saveROMDialog.FileName, "ROM file has successfully been saved to: " + @saveROMDialog.FileName, saveWithHeader);

                // Reload ROM
                sourceROM = new SNESROM(@saveROMDialog.FileName);
                RefreshLabelsAndButtons();
            }
        }

        private void Save(string filepath, string message, bool saveWithHeader)
        {
            if (saveWithHeader)
            {
                // Merge header with ROM
                byte[] haderedROM = new byte[sourceROM.SourceROMSMCHeader.Length + sourceROM.SourceROM.Length];

                Buffer.BlockCopy(sourceROM.SourceROMSMCHeader, 0, haderedROM, 0, sourceROM.SourceROMSMCHeader.Length);
                Buffer.BlockCopy(sourceROM.SourceROM, 0, haderedROM, sourceROM.SourceROMSMCHeader.Length, sourceROM.SourceROM.Length);

                // Write to file
                File.WriteAllBytes(filepath, haderedROM);
            }

            else
            {
                // Just write ROM to file
                File.WriteAllBytes(filepath, sourceROM.SourceROM);
            }

            // Store CRC32 for dirty tracking
            savedFileHash = GetCRC32FromFile(filepath);

            // Show message
            MessageBox.Show(message);
        }

        private void TextBoxGetTitle_TextChanged(object sender, EventArgs e)
        {
            if (!sourceROM.StringTitle.Trim().Equals(textBoxTitle.Text.Trim()))
            {
                sourceROM.SetTitle(textBoxTitle.Text, textBoxTitle.MaxLength);
                RefreshLabelsAndButtons();
            }
        }

        private void TextBoxGetVersion_Leave(object sender, EventArgs e)
        {
            if (sourceROM.StringVersion != "" && sourceROM.StringVersion.Trim() != textBoxVersion.Text.Trim())
            {
                string versionPattern = @"^([1]\.)([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|2[5][0-5])$";

                if (Regex.IsMatch(textBoxVersion.Text.Trim(), versionPattern))
                {
                    byte[] byteArrayVersion = new byte[1];
                    string[] splitVersion = textBoxVersion.Text.Split('.');
                    int intVersion = Int16.Parse(splitVersion[1]);
                    byteArrayVersion = BitConverter.GetBytes(intVersion);

                    sourceROM.SetVersion(byteArrayVersion[0]);
                    RefreshLabelsAndButtons();
                }

                else
                {
                    textBoxVersion.Text = sourceROM.StringVersion;
                    MessageBox.Show("Enter version number between 1.0 and 1.255");
                }
            }
        }

        private void ComboBoxCountryRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBoxCountryRegion.Enabled || comboBoxCountryRegion.SelectedIndex < 0) { return; }

            int selectedCountryRegion = (int)comboBoxCountryRegion.SelectedValue;
            byte byteCountryRegion = Convert.ToByte(selectedCountryRegion);
            byte[] byteArrayCountryRegion = new byte[1];
            byteArrayCountryRegion[0] = byteCountryRegion;

            if (sourceROM.ByteCountry != byteArrayCountryRegion[0])
            {
                sourceROM.SetCountryRegion(byteArrayCountryRegion[0]);
                RefreshLabelsAndButtons();
            }
        }

        private void RefreshLabelsAndButtons()
        {
            // Set combo boxes for expanding and splitting, if file size has changed
            if (labelGetFileSize.Text.Split(' ')[0] != sourceROM.IntCalcFileSize.ToString())
            {
                comboBoxExpandROM.DataSource = null;
                comboBoxSplitROM.DataSource = null;
                comboBoxExpandROM.Items.Clear();
                comboBoxSplitROM.Items.Clear();
                comboBoxExpandROM.Enabled = false;
                comboBoxSplitROM.Enabled = false;

                if ((!sourceROM.IsBSROM && sourceROM.IntCalcFileSize < 64) || (sourceROM.IsBSROM && sourceROM.IntCalcFileSize < 32))
                {
                    List<ComboBoxExpandROMList> list = new List<ComboBoxExpandROMList>();

                    if (sourceROM.IntCalcFileSize < 1) { list.Add(new ComboBoxExpandROMList { Id = 1, Name = "1 Mbit (128 kByte) | 27C1001" }); };
                    if (sourceROM.IntCalcFileSize < 2) { list.Add(new ComboBoxExpandROMList { Id = 2, Name = "2 Mbit (256 kByte) | 27C2001" }); };
                    if (sourceROM.IntCalcFileSize < 4) { list.Add(new ComboBoxExpandROMList { Id = 4, Name = "4 Mbit (512 kByte) | 274001" }); };
                    if (sourceROM.IntCalcFileSize < 8) { list.Add(new ComboBoxExpandROMList { Id = 8, Name = "8 Mbit (1 MByte) | 27C801" }); };
                    if (sourceROM.IntCalcFileSize < 12) { list.Add(new ComboBoxExpandROMList { Id = 12, Name = "12 Mbit (1,5 MByte)" }); };
                    if (sourceROM.IntCalcFileSize < 16) { list.Add(new ComboBoxExpandROMList { Id = 16, Name = "16 Mbit (2 MByte) | 27C160" }); };
                    if (sourceROM.IntCalcFileSize < 20) { list.Add(new ComboBoxExpandROMList { Id = 20, Name = "20 Mbit (2,5 MByte)" }); };
                    if (sourceROM.IntCalcFileSize < 24) { list.Add(new ComboBoxExpandROMList { Id = 24, Name = "24 Mbit (3 MByte)" }); };
                    if (sourceROM.IntCalcFileSize < 28) { list.Add(new ComboBoxExpandROMList { Id = 28, Name = "28 Mbit (3,5 MByte)" }); };
                    if (sourceROM.IntCalcFileSize < 32) { list.Add(new ComboBoxExpandROMList { Id = 32, Name = "32 Mbit (4 MByte) | 27C322" }); };
                    if (sourceROM.IntCalcFileSize < 48 && !sourceROM.IsBSROM)
                    {
                        list.Add(new ComboBoxExpandROMList { Id = 48, Name = "48 Mbit (6 MByte)" });
                        if (sourceROM.IntCalcFileSize <= 32) { list.Add(new ComboBoxExpandROMList { Id = 49, Name = "48 Mbit (6 MByte) [Mirror]" }); }
                    };
                    if (sourceROM.IntCalcFileSize < 64 && !sourceROM.IsBSROM)
                    {
                        list.Add(new ComboBoxExpandROMList { Id = 64, Name = "64 Mbit (8 MByte)" });
                        if (sourceROM.IntCalcFileSize <= 32) { list.Add(new ComboBoxExpandROMList { Id = 65, Name = "64 Mbit (8 MByte) [Mirror]" }); }
                    };

                    comboBoxExpandROM.DataSource = list;
                    comboBoxExpandROM.DisplayMember = "Name";
                    comboBoxExpandROM.ValueMember = "Id";

                    buttonExpandROM.Enabled = true;
                    comboBoxExpandROM.Enabled = true;
                }

                else
                {
                    buttonExpandROM.Enabled = false;
                    comboBoxExpandROM.Enabled = false;
                }

                if (sourceROM.IntCalcFileSize > 1)
                {
                    List<ComboBoxSplitROMList> list = new List<ComboBoxSplitROMList>();

                    if (sourceROM.IntCalcFileSize % 64 == 0 && sourceROM.IntCalcFileSize > 64) { list.Add(new ComboBoxSplitROMList { Id = 64, Name = "64 Mbit (8 MByte)" }); };
                    if (sourceROM.IntCalcFileSize % 32 == 0 && sourceROM.IntCalcFileSize > 32) { list.Add(new ComboBoxSplitROMList { Id = 32, Name = "32 Mbit (4 MByte) | 27C322" }); };
                    if (sourceROM.IntCalcFileSize % 16 == 0 && sourceROM.IntCalcFileSize > 16) { list.Add(new ComboBoxSplitROMList { Id = 16, Name = "16 Mbit (2 MByte) | 27C160" }); };
                    if (sourceROM.IntCalcFileSize % 8 == 0 && sourceROM.IntCalcFileSize > 8) { list.Add(new ComboBoxSplitROMList { Id = 8, Name = "8 Mbit (1 MByte) | 27C801" }); };
                    if (sourceROM.IntCalcFileSize % 4 == 0 && sourceROM.IntCalcFileSize > 4) { list.Add(new ComboBoxSplitROMList { Id = 4, Name = "4 Mbit (512 kByte) | 27C4001" }); };
                    if (sourceROM.IntCalcFileSize % 2 == 0 && sourceROM.IntCalcFileSize > 2) { list.Add(new ComboBoxSplitROMList { Id = 2, Name = "2 Mbit (256 kByte) | 27C2001" }); };
                    list.Add(new ComboBoxSplitROMList { Id = 1, Name = "1 Mbit (128 kByte) | 27C1001" });

                    comboBoxSplitROM.DataSource = list;
                    comboBoxSplitROM.DisplayMember = "Name";
                    comboBoxSplitROM.ValueMember = "Id";

                    buttonSplitROM.Enabled = true;
                    comboBoxSplitROM.Enabled = true;
                }

                else
                {
                    buttonSplitROM.Enabled = false;
                    comboBoxSplitROM.Enabled = false;
                }
            }

            // Set text boxes
            textBoxROMName.Text = sourceROM.ROMFullPath;
            textBoxTitle.Text = sourceROM.StringTitle.Trim();
            textBoxVersion.Text = sourceROM.StringVersion;

            // Set labels
            labelGetMapMode.Text = sourceROM.StringMapMode;
            labelGetROMType.Text = sourceROM.StringROMType;
            labelGetROMSize.Text = sourceROM.StringROMSize;
            labelGetSRAM.Text = sourceROM.StringRAMSize;
            labelSRAM.Text = "(S)RAM"; if (sourceROM.ByteSRAMSize > 0x00) { labelSRAM.Text = "SRAM"; } else if (sourceROM.ByteExRAMSize > 0x00) { labelSRAM.Text = "RAM"; }
            labelGetFileSize.Text = sourceROM.IntCalcFileSize.ToString() + " Mbit (" + ((float)sourceROM.IntCalcFileSize / 8) + " MByte)";
            labelGetSMCHeader.Text = sourceROM.StringSMCHeader;
            labelGetROMSpeed.Text = sourceROM.StringROMSpeed;
            labelGetCompany.Text = sourceROM.StringCompany;
            labelGetIntChksm.Text = BitConverter.ToString(sourceROM.ByteArrayChecksum).Replace("-", "");
            labelGetIntInvChksm.Text = BitConverter.ToString(sourceROM.ByteArrayInvChecksum).Replace("-", "");
            labelGetCalcChksm.Text = BitConverter.ToString(sourceROM.ByteArrayCalcChecksum).Replace("-", "");
            labelGetCalcInvChksm.Text = BitConverter.ToString(sourceROM.ByteArrayCalcInvChecksum).Replace("-", "");
            labelGetCRC32Chksm.Text = sourceROM.CRC32Hash;

            // Set combo boxes
            int selectedCompanyRegion = sourceROM.ByteCountry;
            if (selectedCompanyRegion > 12) { selectedCompanyRegion--; }
            comboBoxCountryRegion.SelectedIndex = selectedCompanyRegion;

            // Set buttons
            if (!buttonSave.Enabled) { buttonSave.Enabled = true; }
            if (!buttonSaveAs.Enabled) { buttonSaveAs.Enabled = true; }
            if (!buttonPatch.Enabled) { buttonPatch.Enabled = true; }
            if (sourceROM.SourceROMSMCHeader == null) { buttonAddHeader.Enabled = true; buttonRemoveHeader.Enabled = false; saveWithHeader = false; } else { buttonAddHeader.Enabled = false; buttonRemoveHeader.Enabled = true; saveWithHeader = true; }
            if (sourceROM.SourceROM.Length % 1048576 == 0) { buttonSwapBinROM.Enabled = true; } else { buttonSwapBinROM.Enabled = false; }
            if (sourceROM.IntROMSize < sourceROM.IntCalcFileSize) { buttonFixROMSize.Enabled = true; } else { buttonFixROMSize.Enabled = false; }
            if (sourceROM.IsInterleaved) { buttonDeinterleave.Enabled = true; buttonFixChksm.Enabled = false; return; } else { buttonDeinterleave.Enabled = false; }
            if (!sourceROM.ByteArrayChecksum.SequenceEqual(sourceROM.ByteArrayCalcChecksum)) { buttonFixChksm.Enabled = true; } else { buttonFixChksm.Enabled = false; }
        }

        private string GetCRC32FromFile(string filepath)
        {
            string tempFileHash = null;

            using (FileStream fs = File.Open(filepath, FileMode.Open))
            {
                foreach (byte b in savedFileCRC32.ComputeHash(fs))
                {
                    tempFileHash += b.ToString("x2").ToUpper();
                }
            }

            return tempFileHash;
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            frmManual helpForm = new frmManual();
            helpForm.Show();
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            frmAbout aboutForm = new frmAbout();
            aboutForm.Show();
        }

        class ComboBoxExpandROMList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void CopyToClipboard_Click(object sender, EventArgs e)
        {
            Label clipboardLabel = (Label)sender;
            ToolTip clipboardToolTipp = new ToolTip();

            Clipboard.SetText(clipboardLabel.Text);

            clipboardToolTipp.Show("Checksum has been copied to clipboard!", this, PointToClient(Cursor.Position), 1500);
        }

        class ComboBoxSplitROMList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        class ComboBoxCountryRegionList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (savedFileHash != null && sourceROM.SourceROM != null)
            {
                // Generate actual ROM
                byte[] trackingROM;

                if (saveWithHeader)
                {
                    // Merge header with ROM
                    trackingROM = new byte[sourceROM.SourceROMSMCHeader.Length + sourceROM.SourceROM.Length];

                    Buffer.BlockCopy(sourceROM.SourceROMSMCHeader, 0, trackingROM, 0, sourceROM.SourceROMSMCHeader.Length);
                    Buffer.BlockCopy(sourceROM.SourceROM, 0, trackingROM, sourceROM.SourceROMSMCHeader.Length, sourceROM.SourceROM.Length);
                }

                else
                {
                    trackingROM = new byte[sourceROM.SourceROM.Length];
                    Buffer.BlockCopy(sourceROM.SourceROM, 0, trackingROM, 0, sourceROM.SourceROM.Length);
                }

                Crc32 trackingROMCRC32 = new Crc32();
                string trackingROMHash = null;

                foreach (byte singleByte in trackingROMCRC32.ComputeHash(trackingROM))
                {
                    trackingROMHash += singleByte.ToString("X2").ToUpper();
                }

                if (savedFileHash != trackingROMHash)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to save your progress before closing?", "Attention!", MessageBoxButtons.YesNoCancel);

                    if (dialogResult == DialogResult.Yes)
                    {
                        // Generate ROM, write to file and store copy for dirty tracking
                        Save(sourceROM.ROMFullPath, "ROM file has successfully been saved to: " + sourceROM.ROMFullPath, saveWithHeader);
                        e.Cancel = false;
                    }

                    else if (dialogResult == DialogResult.No)
                    {
                        e.Cancel = false;
                    }

                    else if (dialogResult == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}