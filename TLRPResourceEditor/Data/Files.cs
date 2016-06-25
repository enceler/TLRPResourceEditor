using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using PropertyChanged;
using TLRPResourceEditor.Models;
using TLRPResourceEditor.ViewModels;

namespace TLRPResourceEditor.Data
{
    /// <summary>
    /// Stores a list of all general upk files and offsets therein that are used by this tool.
    /// For language-dependent files (such as PLAN_BLT_DE_xx.upk) it provides localized names.
    /// TODO: Move all ChangeState methods form the models here; should use DI to write changes
    /// </summary>
    [ImplementPropertyChanged]
    internal static class Files
    {
        public static String         BattleFile   { get { return Path.Combine(CookedPCPath, @"PLAN/Battle", LanguageFiles[(int)language]); } }
        public static string         CookedPCPath { get; set; }
        public static Language       Language     { get { return language; } set { ChangeLanguage(value); } }
        public static int[]          TableOffsets { get; set; }
        public static List<MapFiles> MapFiles     { get; set; }

        public static EnemyViewModel enemyViewModel { get; set; }

        private static List<string> LanguageFiles = new List<string> { "PLAN_BTL_DT_US.upk", "PLAN_BTL_DT_DE.upk", "PLAN_BTL_DT_ES.upk", "PLAN_BTL_DT_FR.upk", "PLAN_BTL_DT_IT.upk", "PLAN_BTL_DT_JA.upk" };
        private static Language language = Language.English;

        /// <summary>
        /// Initializes the static variables:
        /// Looks for tlr's path using the registry entry for steram.
        /// Reads all table offsets for the selected (or default) language
        /// </summary>
        static Files()
        {
            ReadBasicInfo(null);
        }

        /// <summary>
        /// Call to initialize directory structure and basic file information.
        /// Searches for TLR path in registry if path is null
        /// Set to CookedPC path otherwise
        /// </summary>
        /// <param name="path">todo: describe path parameter on ReadBasicInfo</param>
        public static void ReadBasicInfo(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    CookedPCPath = FindTLRPath();
                else
                    CookedPCPath = path;

                TableOffsets = ReadOffsets();
                MapFiles = InitializeMapNames();

                if (!File.Exists(BattleFile))
                {
                    if (string.IsNullOrEmpty(path))
                        MessageBox.Show("Could not find path to The Last Remnant in the registy. Please enter the path manually.");
                    else
                        MessageBox.Show("Could not find The Last Remnant in the selected path.");
                    return;
                }

                // Create backups
                if (!File.Exists(BattleFile + ".backup"))
                    File.Copy(BattleFile, BattleFile + ".backup");

                foreach (var entry in MapFiles)
                {
                    if (!File.Exists(entry.MapFile + ".backup"))
                        File.Copy(entry.MapFile, entry.MapFile + ".backup");
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Could not find path to The Last Remnant in the registy. Please enter the path manually.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// Changes the language used by the data files.
        /// Reads all data (names, tables) again and updates
        /// the viewmodel
        /// </summary>
        /// <param name="newLanguage"></param>
        private static void ChangeLanguage(Language newLanguage)
        {
            if (CookedPCPath == null)
                return;

            if (!File.Exists(BattleFile))
            {
                //MessageBox.Show("Could not find path to The Last Remnant in the registy. Please enter the path manually.");
                return;
            }
            try
            {
                language = newLanguage;
                TableOffsets = ReadOffsets();
                Names.LoadData();
                Monster.LoadData();
                MonsterFormation.LoadData();
                Equipment.LoadData();
                Unit.LoadData();
                Map.LoadData();
        }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
}

        private static int[] ReadOffsets()
        {
            /*
             * To reduce calculation time, the offsets for all used tables were calculated once, and then added to the list.
             * So long as the length of the string entries don't change, they will stay correct.
             */
            var offsets = new int[320];

            // Use this method to recalculate the offsets:
            // CalculateOffsets() // Currently being reworked

            if (Language == Language.English)
            {
                offsets[35] = 0x308B0C; // Monster Union Names
                offsets[36] = 0x30D826; // Character Names
                offsets[37] = 0x349530; // Item Names
                offsets[60] = 0x440B56; // Class Names

                offsets[123] = 0x6E1FFC; // Monster Formations
                offsets[127] = 0x78AB19; // Equipment
                offsets[257] = 0xBF5A02; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB; // Monsters
                offsets[260] = 0xC78AEB; // Friendly Characters
                offsets[262] = 0xC94177; // Character Main Table
                offsets[263] = 0xCCF340; // Classes
                offsets[270] = 0xD148F5; // Party Talk Answers
            }
            else if (Language == Language.Deutsch)
            {
                offsets[35] = 0x3277A2; // Monster Union Names
                offsets[36] = 0x32C47C; // Character Names
                offsets[37] = 0x368C5A; // Item Names
                offsets[60] = 0x463BEB; // Class Names

                offsets[123] = 0x6E1FFC + 0x23A93; // Monster Formations
                offsets[127] = 0x78AB19 + 0x23A93; // Equipment
                offsets[257] = 0xBF5A02 + 0x23A93; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB + 0x23A93; // Monsters
                offsets[260] = 0xC78AEB + 0x23A93; // Friendly Characters
                offsets[262] = 0xC94177 + 0x23A93; // Character Main Table
                offsets[263] = 0xCCF340 + 0x23A93; // Classes
                offsets[270] = 0xD148F5 + 0x23A93; // Party Talk Answers
            }
            else if (Language == Language.Español)
            {
                offsets[35] = 0x331301; // Monster Union Names
                offsets[36] = 0x335DF1; // Character Names
                offsets[37] = 0x37202F; // Item Names
                offsets[60] = 0x46FAC0; // Class Names

                offsets[123] = 0x6E1FFC + 0x2F848; // Monster Formations
                offsets[127] = 0x78AB19 + 0x2F848; // Equipment
                offsets[257] = 0xBF5A02 + 0x2F848; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB + 0x2F848; // Monsters
                offsets[260] = 0xC78AEB + 0x2F848; // Friendly Characters
                offsets[262] = 0xC94177 + 0x2F848; // Character Main Table
                offsets[263] = 0xCCF340 + 0x2F848; // Classes
                offsets[270] = 0xD148F5 + 0x2F848; // Party Talk Answers
            }
            else if (Language == Language.Français)
            {
                offsets[35] = 0x3188E6; // Monster Union Names
                offsets[36] = 0x31D68C; // Character Names
                offsets[37] = 0x359CF6; // Item Names
                offsets[60] = 0x453C65; // Class Names

                offsets[123] = 0x6E1FFC + 0x137E5; // Monster Formations
                offsets[127] = 0x78AB19 + 0x137E5; // Equipment
                offsets[257] = 0xBF5A02 + 0x137E5; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB + 0x137E5; // Monsters
                offsets[260] = 0xC78AEB + 0x137E5; // Friendly Characters
                offsets[262] = 0xC94177 + 0x137E5; // Character Main Table
                offsets[263] = 0xCCF340 + 0x137E5; // Classes
                offsets[270] = 0xD148F5 + 0x137E5; // Party Talk Answers
            }
            else if (Language == Language.Italiano)
            {
                offsets[35] = 0x3171B3; // Monster Union Names
                offsets[36] = 0x31BD8D; // Character Names
                offsets[37] = 0x357B4D; // Item Names
                offsets[60] = 0x4527F0; // Class Names

                offsets[123] = 0x6E1FFC + 0x122B4; // Monster Formations
                offsets[127] = 0x78AB19 + 0x122B4; // Equipment
                offsets[257] = 0xBF5A02 + 0x122B4; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB + 0x122B4; // Monsters
                offsets[260] = 0xC78AEB + 0x122B4; // Friendly Characters
                offsets[262] = 0xC94177 + 0x122B4; // Character Main Table
                offsets[263] = 0xCCF340 + 0x122B4; // Classes
                offsets[270] = 0xD148F5 + 0x122B4; // Party Talk Answers
            }
            else if (Language == Language.日本語)
            {
                offsets[35] = 0x2A279A; // Monster Union Names
                offsets[36] = 0x2A6AA4; // Character Names
                offsets[37] = 0x2DFA8E; // Item Names
                offsets[60] = 0x3C9B66; // Class Names

                offsets[123] = 0x6E1FFC - 0x78542; // Monster Formations
                offsets[127] = 0x78AB19 - 0x78542; // Equipment
                offsets[257] = 0xBF5A02 - 0x78542; // Preset Mystic Skills
                offsets[259] = 0xBFF6FB - 0x78542; // Monsters
                offsets[260] = 0xC78AEB - 0x78542; // Friendly Characters
                offsets[262] = 0xC94177 - 0x78542; // Character Main Table
                offsets[263] = 0xCCF340 - 0x78542; // Classes
                offsets[270] = 0xD148F5 - 0x78542; // Party Talk Answers
            }

            return offsets;
        }

        private static int[] CalculateOffsets()
        {
            var result = new int[320];
            //var content = File.ReadAllBytes(BattleFile);
            //var offsetStart = BitConverter.ToInt32(content, 8);
            //var tableCount = BitConverter.ToInt32(content, offsetStart + 28); // + BitConverter.ToInt32(content, offsetStart + 12);

            //var tableentries = new List<int>();
            //for (var i = 0;  i < tableCount; i++)
            //{
            //    tableentries.Add(BitConverter.ToInt32(content, offsetStart + 32 + 84 + (i * 292)));
            //}
            //var tt = offsetStart + 32 + (tableCount * 292);

            // -> 11AE06-131A46 (752 * 124)
            return result;
        }

        /// <summary>
        /// Reads the TLR path from the registry.
        /// Returns null if not found or no read access to registry.
        /// </summary>
        private static string FindTLRPath()
        {
            var regKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");

            if (regKey != null)
            {
                return Path.Combine(regKey.GetValue("SteamPath").ToString(), "SteamApps/common/The Last Remnant/RushGame/CookedPC/");
            }

            throw new FileNotFoundException("TLR path not found in registry");
        }

        private static List<MapFiles> InitializeMapNames()
        {
            var result = new List<MapFiles>
                {
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/CKD/CKD_DATA_000.upk"), MapName = "Ancient Ruins" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/NWD/NWD_DATA_000.upk"), MapName = "Aqueducts" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/DLD/DLD_DATA_000.upk"), MapName = "Aveclyff" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/ETV/ETV_DATA_000.upk"), MapName = "Berechevaltelle" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/BDL/BDL_DATA_000.upk"), MapName = "Blackdale" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/UMD/UMD_DATA_000.upk"), MapName = "Catacombs" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/CRK/CRK_DATA_000.upk"), MapName = "Crookfen" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/DKF/DKF_DATA_000.upk"), MapName = "Darken Forest" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/DLP/DLP_DATA_000.upk"), MapName = "Dillmoor" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/GST/GST_DATA_000.upk"), MapName = "Flaumello Tower" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/FRN/FRN_DATA_000.upk"), MapName = "Fornstrand" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/GAS/GAS_DATA_000.upk"), MapName = "Gaslin Caves" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/GSS/GSS_DATA_000.upk"), MapName = "Great Sand Sea" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/GSD/GSD_DATA_000.upk"), MapName = "Great Subterrane" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/GTN/GTN_DATA_000.upk"), MapName = "Heroic Ramparts" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/BTE/BTE_DATA_000.upk"), MapName = "Ivory Peaks" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/STR/STR_DATA_000.upk"), MapName = "Lavafender" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/BHK/BHK_DATA_000.upk"), MapName = "Mojcado Castle" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/VKL/VKL_DATA_000.upk"), MapName = "Mt. Vackel" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/NUD/NUD_DATA_000.upk"), MapName = "Numor Mines" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/RBC/RBC_DATA_000.upk"), MapName = "Ruins of Robelia Castle" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/ESB/ESB_DATA_000.upk"), MapName = "Sacred Lands" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/SBC/SBC_DATA_000.upk"), MapName = "Siebenbur (1)" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/SBF/SBF_DATA_000.upk"), MapName = "Siebenbur (2)" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/SBM/SBM_DATA_000.upk"), MapName = "Siebenbur (4/5)" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/SBN/SBN_DATA_000.upk"), MapName = "Siebenbur (6/7)" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/ETB/ETB_DATA_000.upk"), MapName = "Southwestern Road" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/STB/STB_DATA_000.upk"), MapName = "Vale of the Gods" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/BND/BND_DATA_000.upk"), MapName = "Yvalock's Nest" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/DRC/DRC_DATA_000.upk"), MapName = "Wyrmskeep" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/YFD/YFD_DATA_000.upk"), MapName = "Yamarn Cave" },
                    new MapFiles {MapFile = Path.Combine(CookedPCPath, @"PLAN/MAP/FIELD/BND/BND_DATA_000.upk"), MapName = "Yvalock's Nest" },
                };
            return result;
        }
    }

    public class MapFiles
    {
        public string MapName { get; set; }
        public string MapFile { get; set; }
    }

    public enum Language
    {
        English,
        Deutsch,
        Español,
        Français,
        Italiano,
        日本語
    }
}