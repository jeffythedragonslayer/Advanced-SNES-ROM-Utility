﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Advanced_SNES_ROM_Utility
{
    public partial class Form3 : Form
    {
        private byte[] sourceROMCopy = null;
        private byte[] sourceROMSMCHeaderCopy = null;
        private uint romHeaderOffsetCopy = 0;
        private string romSavePathCopy = null;
        private string romNameCopy = null;
        private string romTitleCopy = null;
        private byte[] romByteTitleCopy = null;
        private byte romCountryRegionCopy;
        private string romVersionCopy = null;
        private int intROMSizeCopy = 0;
        private int calcFileSizeCopy = 0;
        private bool isBROMCopy = false;

        public Form3(byte[] sourceROM, byte[] sourceROMSMCHeader, uint romHeaderOffset, string title, byte[] byteTitle, string version, byte country, string company, int intROMSize, int calcFileSize, string romSavePath, string romName, bool isBSROM)
        {
            InitializeComponent();

            if (isBSROM) { comboBoxChangeCountryRegion.Enabled = false; textBoxChangeTitle.MaxLength = 16; }
            comboBoxChangeCompany.Enabled = false;
            textBoxChangeVersion.Enabled = true;
            checkBoxFixSize.Enabled = false;
            checkBoxFixSize.Checked = false;

            sourceROMCopy = sourceROM;
            sourceROMSMCHeaderCopy = sourceROMSMCHeader;
            romHeaderOffsetCopy = romHeaderOffset;
            romSavePathCopy = romSavePath;
            romNameCopy = romName;
            romTitleCopy = title;
            romByteTitleCopy = byteTitle;
            romCountryRegionCopy = country;
            romVersionCopy = version;
            intROMSizeCopy = intROMSize;
            calcFileSizeCopy = calcFileSize;
            isBROMCopy = isBSROM;

            // Set original values for editing
            textBoxChangeTitle.Text = title.Trim();
            textBoxChangeVersion.Text = version;

            // Load combo box for selecting country & region
            List<comboBoxChangeCountryRegionList> listCountryRegion = new List<comboBoxChangeCountryRegionList>();

            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 0, Name = "Japan | NTSC" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 1, Name = "USA | NTSC" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 2, Name = "Europe/Oceania/Asia | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 3, Name = "Sweden/Scandinavia | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 4, Name = "Finland | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 5, Name = "Denmark | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 6, Name = "France | SECAM (PAL-like, 50 Hz)" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 7, Name = "Netherlands | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 8, Name = "Spain | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 9, Name = "Germany/Austria/Switzerland | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 10, Name = "China/Hong Kong | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 11, Name = "Indonesia | PAL" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 12, Name = "South Korea | NTSC" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 14, Name = "Canada | NTSC" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 15, Name = "Brazil | PAL-M (NTSC-like, 60 Hz)" });
            listCountryRegion.Add(new comboBoxChangeCountryRegionList { Id = 16, Name = "Australia | PAL" });

            comboBoxChangeCountryRegion.DataSource = listCountryRegion;
            comboBoxChangeCountryRegion.DisplayMember = "Name";
            comboBoxChangeCountryRegion.ValueMember = "Id";

            // Don't know why, but SelectedValue doesn't work here, so we have to use a little trick
            int selectedCompanyRegion = country;

            if (selectedCompanyRegion > 12)
            {
                selectedCompanyRegion--;
            }

            // Pre-select country & region information
            comboBoxChangeCountryRegion.SelectedIndex = selectedCompanyRegion;

            // Load combo box for selecting company
            List<comboBoxChangeCompanyList> listCompany = new List<comboBoxChangeCompanyList>();

            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C5", Name = "3DO" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x021C", Name = "A Wave" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B4", Name = "Absolute Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x018C", Name = "Acclaim Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B5", Name = "Acclaim" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DF", Name = "Acclaim/LJN Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0105", Name = "Accolade Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B6", Name = "Activision" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00AE", Name = "Advanced Productions" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x020F", Name = "Alphadream Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01E3", Name = "Altron" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B7", Name = "American Sammy" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C3", Name = "American Softworks Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BF", Name = "Angel/Sotsu Agency/Sunrise" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0077", Name = "Arcade Zone Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0266", Name = "Aruze" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x018D", Name = "ASCII Co./Nexoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01D8", Name = "Ask Kodansha" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0112", Name = "Asmik Ace Entertainment Inc./AIA" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0200", Name = "Asmik" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0232", Name = "Aspyr" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01FF", Name = "Athena" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0203", Name = "Atlus" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0248", Name = "Avex" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C4", Name = "Axela/Crea-Tech" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00ED", Name = "BAM! Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0024", Name = "Banarex" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x018E", Name = "Bandai" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DD", Name = "Banpresto" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E9", Name = "BBC" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0147", Name = "BEC" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00AC", Name = "Black Pearl" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C3", Name = "Boss" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0155", Name = "Bottom Up" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0247", Name = "Broccoli" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x012B", Name = "Bullet-Proof Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DC", Name = "Capcom Co., Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0008", Name = "Capcom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x006D", Name = "Carrozzeria" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x012E", Name = "Character Soft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0208", Name = "Chatnoir" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0145", Name = "Chun Soft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F2", Name = "Classified Games" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0029", Name = "Cobra Team" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000B", Name = "Coconuts Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000C", Name = "Coconuts Japan/G.X.Media" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0072", Name = "Codemasters" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x019F", Name = "Compile" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D7", Name = "Conspiracy/Swing" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DB", Name = "Copya System" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B3", Name = "Crave Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x003D", Name = "Creatures Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D1", Name = "Cryo Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0052", Name = "Culture Brain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0196", Name = "Culture Brain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0225", Name = "Cybersoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0249", Name = "D3 Publisher" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0037", Name = "Daikokudenki" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0239", Name = "Daiwon" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B5", Name = "Data East" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0128", Name = "DATAM-Polystar" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x022A", Name = "Davidson/Western Tech." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0154", Name = "Den'Z" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0048", Name = "Destination Software/KSS" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0237", Name = "Digital Tainment Pool" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00AA", Name = "Disney Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F6", Name = "DreamCatcher" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x006E", Name = "Dynamic" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x009F", Name = "Eidos/U.S. Gold" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E7", Name = "Electro Brain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0027", Name = "Electronic Arts Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E1", Name = "Electronic Arts" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01EB", Name = "Elf" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E6", Name = "Elite Systems" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0113", Name = "Empire Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x010D", Name = "Encore" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x004E", Name = "Enix" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0190", Name = "Enix" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C9", Name = "Enterbrain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0078", Name = "Entertainment International/Empire Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0204", Name = "Epic/Sony Records (Japan)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01FD", Name = "Epoch Co., Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0267", Name = "Ertain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x021F", Name = "Extreme Entertainment Group" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0006", Name = "Falcom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0032", Name = "Forum/OpenSystem" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00A3", Name = "Fox Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01A6", Name = "Fuuki" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x005E", Name = "Gajin/Jordan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0019", Name = "Game Village" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01E5", Name = "Gaps Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0133", Name = "General Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01A5", Name = "Global A Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0004", Name = "Gray Matter" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x007A", Name = "Gremlin Graphics" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B1", Name = "GT Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0192", Name = "HAL Laboratory/Halken" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x015E", Name = "Hands-On Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0159", Name = "Hasbro Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C8", Name = "Hasbro" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0071", Name = "Hect" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B9", Name = "Hi Tech" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0231", Name = "Hip Games" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0169", Name = "Hori" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0009", Name = "Hot B Co." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x002C", Name = "Hudson Soft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01E2", Name = "Human Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x002A", Name = "Human/Field" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x001A", Name = "IE Institute" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x011A", Name = "Ignition Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0206", Name = "IGS (Information Global Service)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0150", Name = "Imagineer" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0003", Name = "Imagineer-Zoom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x012F", Name = "I'Max" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0136", Name = "I'Max" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0026", Name = "Infocom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0181", Name = "Infogrames Hudson" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FC", Name = "Infogrames" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0056", Name = "Intec" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FD", Name = "Interplay" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0236", Name = "iQue" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0053", Name = "Irem Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0099", Name = "Irem" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FB", Name = "ITE Media" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000A", Name = "Jaleco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01E4", Name = "Jaleco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01F8", Name = "Jaleco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0031", Name = "Japan Glary Business" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0116", Name = "Jester Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F7", Name = "JoWood Produtions" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0172", Name = "JVC (Europe/Japan)/Victor Musical Industries" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FE", Name = "JVC (US)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0179", Name = "J-Wing" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x016D", Name = "K.Amusement Leasing Co." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x014B", Name = "Kaneko" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x016E", Name = "Kawada" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0050", Name = "Kemco Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x010B", Name = "Kemco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B2", Name = "Kemco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x015D", Name = "Keynet Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x017C", Name = "KID" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0110", Name = "Kiddinx" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0244", Name = "KiKi Co. Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0202", Name = "King Records" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x002B", Name = "KOEI" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0124", Name = "KOEI" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B8", Name = "KOEI" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x024B", Name = "Konami Computer Entertainment Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C6", Name = "Konami Computer Entertainment Osaka" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x020E", Name = "Konami Computer Entertainment Tokyo" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x016C", Name = "Konami" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BA", Name = "Konami/Ultra/Palcom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x024E", Name = "KSG" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0074", Name = "Laguna" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E3", Name = "Laser Beam" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x021E", Name = "Left Field Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00CE", Name = "LEGO Media" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0094", Name = "Life Fitness" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00BA", Name = "LJN Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0079", Name = "Loriciel" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x004F", Name = "Loriciel/Electro Brain" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0123", Name = "LOZC" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FA", Name = "LSP (Light & Shadow Prod.)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00DC", Name = "LucasArts Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x019C", Name = "Magical" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0070", Name = "Magifact" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C4", Name = "Majesco Sales Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x009D", Name = "Malibu Games" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x015B", Name = "Marvelous Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0235", Name = "Mastiff" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00BC", Name = "Mattel" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00DA", Name = "Maxis" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000F", Name = "Mebio Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0178", Name = "Media Rings Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x005B", Name = "Media Works" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x017D", Name = "Mediafactory" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0062", Name = "Mediakite" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BD", Name = "Meldac" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00CB", Name = "Metro3D" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x024F", Name = "Micott & Basara Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x004B", Name = "Micro World" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D5", Name = "Microids" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000D", Name = "Micronet" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0108", Name = "Microprose Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C1", Name = "Midway/Tradewest" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00BE", Name = "Mindscape/Red Orb Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0120", Name = "Misawa" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0013", Name = "Mitsui Fudosan/Dentsu" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x021D", Name = "Motown Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01A1", Name = "MTO Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0122", Name = "Namco Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0177", Name = "Namco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0201", Name = "Natsume" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DA", Name = "Naxat" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01E1", Name = "NCS" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x005C", Name = "NEC InterChannel" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C9", Name = "NewKidCo" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x014E", Name = "Nichibutsu/Nihon Bussan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0001", Name = "Nintendo" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0153", Name = "Nova" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0017", Name = "Nowpro" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x020B", Name = "NTT COMWARE" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BB", Name = "NTVIC/VAP" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00DF", Name = "Ocean" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0245", Name = "Open Sesame Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0251", Name = "Orbital Media" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0054", Name = "Palsoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00FF", Name = "Parker Brothers" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0241", Name = "PCCW Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x017B", Name = "Pioneer LDC" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00A0", Name = "Playmates Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BE", Name = "Pony Canyon (Japan)/FCI (US)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0195", Name = "Pony Canyon Hanbai" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0058", Name = "Poppo" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x004A", Name = "POW (Planning Office Wada)/VR 1 Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0227", Name = "Psygnosis" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01D6", Name = "Quest Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x010C", Name = "Rage Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B2", Name = "RARE" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x009B", Name = "Raya Systems" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D4", Name = "Red Storm Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x009C", Name = "Renovation Products" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0209", Name = "Right Stuff" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0002", Name = "Rocket Games/Ajinomoto" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0118", Name = "Rockstar Games" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00BF", Name = "Romstar" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x002D", Name = "S.C.P./Game Village" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x019B", Name = "Sammy" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x004D", Name = "San-X" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0130", Name = "Saurus" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0119", Name = "Scholastic" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0101", Name = "SCI (Sales Curve Interactive)/Storm" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0139", Name = "SEGA Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F8", Name = "SEGA" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0090", Name = "Seika Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C5", Name = "Sekaibunka-Sha/Sumire kobo/Marigul Management Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0051", Name = "Seta Co.,Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0010", Name = "Shouei System" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01D7", Name = "Sigma" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0111", Name = "Simon & Schuster Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0246", Name = "Sims" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0034", Name = "SMDE" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x005F", Name = "Smilesoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0268", Name = "SNK Playmore" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0193", Name = "SNK" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01D5", Name = "Sofel" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00EB", Name = "Software 2000" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0199", Name = "Sony Imagesoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0097", Name = "Spectrum Holobyte" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x020D", Name = "Spike" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B3", Name = "Square" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x024D", Name = "Square-Enix" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x011C", Name = "Stadlbauer" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0011", Name = "Starfish" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0025", Name = "Starfish" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0211", Name = "Sting" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00EE", Name = "Studio 3" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0137", Name = "Success" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x011B", Name = "Summitsoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01A3", Name = "Sunrise Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0092", Name = "Sunsoft US" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0197", Name = "Sunsoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0049", Name = "Sunsoft/Tokai Engineering" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0096", Name = "System 3" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0057", Name = "System Sacom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01FC", Name = "T&ESoft" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B0", Name = "Taito" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01D4", Name = "Taito/Disco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0073", Name = "Taito/GAGA Communications" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0144", Name = "Takara" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x016F", Name = "Takara" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00B8", Name = "Take 2/GameTek" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x005D", Name = "Tam" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00C0", Name = "Taxan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x003E", Name = "TDK Deep Impresion" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F4", Name = "TDK Mediactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0171", Name = "Technos Japan Corp." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x000E", Name = "Technos" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0220", Name = "TecMagik" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0030", Name = "Tecmo Products" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x014F", Name = "Tecmo" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0121", Name = "Teichiku" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00CA", Name = "Telegames" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0168", Name = "Telenet" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0075", Name = "Telstar Fun & Games/Event/Taito" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0157", Name = "TGL (Technical Group Laboratory)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0230", Name = "The Game Factory Europe" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0262", Name = "The Game Factory USA" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00E8", Name = "The Learning Company" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0104", Name = "THQ Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00A4", Name = "Time Warner Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D8", Name = "Titus" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0174", Name = "Toei Animation" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0175", Name = "Toho" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0126", Name = "Tokuma Shoten Intermedia" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B4", Name = "Tokuma Shoten" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01DE", Name = "TOMY" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01B6", Name = "Tonkin House" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0198", Name = "Toshiba EMI" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0265", Name = "Treasure" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0106", Name = "Triffix Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0127", Name = "Tsukuda Original" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0091", Name = "UBI SOFT Entertainment Software" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0059", Name = "Ubisoft Japan" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0109", Name = "Universal Interactive/Sierra/Simon & Schuster" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x022B", Name = "Unlicensed" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01BC", Name = "Use Co., Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0149", Name = "Varie" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01FB", Name = "Varie" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00CD", Name = "Vatical Entertainment" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x006C", Name = "Viacom" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x012C", Name = "Vic Tokai Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x014D", Name = "Victor Interactive Software/Pack-in-Video" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0146", Name = "Video System Co., Ltd./McO'River" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0033", Name = "Virgin Games (Japan)" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D9", Name = "Virgin Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x019D", Name = "Visco" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0055", Name = "Visit Co., Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00F9", Name = "Wannado Edition" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0015", Name = "Warashi Inc." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x00D0", Name = "Xicat Interactive" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0238", Name = "XS Games" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x002E", Name = "Yanoman" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0035", Name = "Yojigen" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0299", Name = "Yojigen" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x014A", Name = "Yonezawa/S'pal" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01C0", Name = "Yumedia/Aroma Co., Ltd." });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x01FA", Name = "Yutaka" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x0005", Name = "Zamuse" });
            listCompany.Add(new comboBoxChangeCompanyList { Id = "0x010F", Name = "Zoo" });

            comboBoxChangeCompany.DataSource = listCompany;
            comboBoxChangeCompany.DisplayMember = "Name";
            comboBoxChangeCompany.ValueMember = "Id";

            // Pre-select company information
            comboBoxChangeCompany.SelectedValue = company;

            // Recalculate company code value

            // Debugging
            //MessageBox.Show("Selected display member: " + comboBoxChangeCompany.Text + "\n\nSelected item value: " + comboBoxChangeCompany.SelectedValue);

            // Some Hacks may have an odd size in their header, so we should fix that by taking the right value
            if ((intROMSizeCopy < calcFileSizeCopy) && !isBSROM)
            {
                intROMSizeCopy = 1;

                while (intROMSizeCopy < calcFileSizeCopy)
                {
                    intROMSizeCopy *= 2;
                }

                checkBoxFixSize.Enabled = true;
                checkBoxFixSize.Checked = true;
            }
        }
        
        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            // Make a copy for editing
            byte[] editedROM = sourceROMCopy;

            // Reset values for version input checking
            bool madeChange = false;
            bool versionIsValid = false;

            // Get selected country & region
            int selectedCountryRegion = (int)comboBoxChangeCountryRegion.SelectedValue;
            byte byteCountryRegion = Convert.ToByte(selectedCountryRegion);
            byte[] byteArrayCountryRegion = new byte[1];
            byteArrayCountryRegion[0] = byteCountryRegion;

            if (romCountryRegionCopy != byteArrayCountryRegion[0])
            {
                Buffer.BlockCopy(byteArrayCountryRegion, 0, editedROM, (int)romHeaderOffsetCopy + 0x29, 1);             // Set new country & region
                madeChange = true;
            }
         
            // Get new title
            byte[] byteArrayTitle = new byte[21] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            if (isBROMCopy) { byteArrayTitle = new byte[16] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; }

            Encoding newEncodedTitle = Encoding.GetEncoding(932);                                   //ASCIIEncoding newASCIITitle = new System.Text.ASCIIEncoding();
            byte[] newByteTitleTemp = newEncodedTitle.GetBytes(textBoxChangeTitle.Text);            //newASCIITitle.GetBytes(textBoxChangeTitle.Text);
            
            int newByteTitleTempLenght = newByteTitleTemp.Length;

            if (isBROMCopy && newByteTitleTempLenght > 16) { newByteTitleTempLenght = 16; }
            if (!isBROMCopy && newByteTitleTempLenght > 21) { newByteTitleTempLenght = 21; }

            Buffer.BlockCopy(newByteTitleTemp, 0, byteArrayTitle, 0, newByteTitleTempLenght);

            if (!romByteTitleCopy.SequenceEqual(byteArrayTitle))
            {
                if (isBROMCopy) { Buffer.BlockCopy(byteArrayTitle, 0, editedROM, (int)romHeaderOffsetCopy + 0x10, 16); } else { Buffer.BlockCopy(byteArrayTitle, 0, editedROM, (int)romHeaderOffsetCopy + 0x10, 21); }  // Set new title
                madeChange = true;
            }

            // Get new version
            byte[] byteArrayVersion = new byte[1];
            string versionPattern = @"^([1]\.)([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|2[5][0-5])$";
            versionIsValid = Regex.IsMatch(textBoxChangeVersion.Text, versionPattern);

            if (!romVersionCopy.Equals(textBoxChangeVersion.Text) && versionIsValid)
            {
                string[] splitVersion = textBoxChangeVersion.Text.Split('.');
                int intVersion = Int16.Parse(splitVersion[1]);
                byteArrayVersion = BitConverter.GetBytes(intVersion);

                Buffer.BlockCopy(byteArrayVersion, 0, editedROM, (int)romHeaderOffsetCopy + 0x2B, 1);               // Set new version
                madeChange = true;
            }

            // Fix wrong ROM size value
            if (checkBoxFixSize.Checked)
            {
                byte byteROMSizeValue = Convert.ToByte(intROMSizeCopy);
                byte[] byteArrayROMSizeValue = new byte[1];
                byteArrayROMSizeValue[0] = byteROMSizeValue;

                switch (byteArrayROMSizeValue[0])
                {
                    case 0x01: byteArrayROMSizeValue[0] = 0x07; break;
                    case 0x02: byteArrayROMSizeValue[0] = 0x08; break;
                    case 0x04: byteArrayROMSizeValue[0] = 0x09; break;
                    case 0x08: byteArrayROMSizeValue[0] = 0x0A; break;
                    case 0x10: byteArrayROMSizeValue[0] = 0x0B; break;
                    case 0x20: byteArrayROMSizeValue[0] = 0x0C; break;
                    case 0x40: byteArrayROMSizeValue[0] = 0x0D; break;
                }

                Buffer.BlockCopy(byteArrayROMSizeValue, 0, editedROM, (int)romHeaderOffsetCopy + 0x27, 1);              // Set new ROM size value
                madeChange = true;
            }

            if (madeChange && versionIsValid)
            {
                // Save edited file with header
                if (sourceROMSMCHeaderCopy != null)
                {
                    byte[] editedHeaderedROM = new byte[sourceROMSMCHeaderCopy.Length + editedROM.Length];

                    Buffer.BlockCopy(sourceROMSMCHeaderCopy, 0, editedHeaderedROM, 0, sourceROMSMCHeaderCopy.Length);
                    Buffer.BlockCopy(editedROM, 0, editedHeaderedROM, sourceROMSMCHeaderCopy.Length, editedROM.Length);

                    File.WriteAllBytes(@romSavePathCopy + @"\" + romNameCopy + "_[edited]" + ".sfc", editedHeaderedROM);
                    MessageBox.Show("ROM successfully edited!\n\nFile saved to: '" + @romSavePathCopy + @"\" + romNameCopy + "_[edited]" + ".sfc'\n\nAnd don't forget to fix checksum!");
                }

                else
                {
                    // Save edited file without header
                    File.WriteAllBytes(@romSavePathCopy + @"\" + romNameCopy + "_[edited]" + ".sfc", editedROM);
                    MessageBox.Show("ROM successfully edited!\n\nFile saved to: '" + @romSavePathCopy + @"\" + romNameCopy + "_[edited]" + ".sfc'\n\nAnd don't forget to fix checksum!");
                }

                this.Close();
            }

            else if (!versionIsValid)
            {
                MessageBox.Show("Enter version number between 1.0 and 1.255");
            }

            else
            {
                MessageBox.Show("No changes have been detected!\n\nPlease press 'Cancel' or [X] to return to main window.");
            }
        }

        private void buttonCancelChages_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        class comboBoxChangeCountryRegionList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        class comboBoxChangeCompanyList
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}