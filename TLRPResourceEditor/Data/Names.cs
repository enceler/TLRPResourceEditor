using System;
using System.Collections.Generic;
using System.IO;

namespace TLRPResourceEditor.Data
{
    /// <summary>
    /// All name tables currently used, as well as the mapping from id's to name table to avoid having to read
    /// unneeded tables purely to lookup id's.
    /// </summary>
    class Names
    {
        public static List<string> ItemNames { get; set; }
        public static List<string> UnionNames { get; set; }
        public static List<string> UnitNames { get; set; }
        public static List<string> ClassNames { get; set; }
        public static List<int> CharacterToName { get; set; }
        public static List<int> CharacterToClass { get; set; }
        public static List<int> EquipmentToName { get; set; }
        public static List<int> Id259to262 { get; set; }
        public static List<int> Id262To36 { get; set; }

        /// <summary>
        /// Read all tables from the battle file (using currently selected language)
        /// </summary>
        public static void LoadData()
        {
            var data = File.ReadAllBytes(Files.BattleFile);

            /// ID mappings
            CharacterToClass = ReadMappingCharacterToClass(data);
            CharacterToName = ReadMappingCharacterToName(data);
            Id262To36 = ReadMappingCharacterToName(data);
            Id259to262 = ReadMappingEnemyFormationToName(data);
            EquipmentToName = ReadMappingEquipmentToName(data);

            /// Data
            ClassNames = ReadClassNames(data);
            ItemNames = ReadItemNames(data);
            UnionNames = ReadEnemyUnionNames(data);
            UnitNames = ReadUnitNames(data);
        }

        /// <summary>
        /// Create mapping from character id to class name id (table 263)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<int> ReadMappingCharacterToClass(byte[] data)
        {
            var characterToClass = new List<int>();
            for (var i = 0; i < 383; i++)
            {
                characterToClass.Add(BitConverter.ToInt16(data, Files.TableOffsets[263] + (i * 72)));
            }
            return characterToClass;
        }

        /// <summary>
        /// Create mapping from character id (they are not ordered in the table itself) to name id (table 262)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<int> ReadMappingCharacterToName(byte[] data)
        {
            var mapping = new List<int>();
            for (int i = 0; i < 1845; i++)
            {
                mapping.Add(BitConverter.ToInt16(data, Files.TableOffsets[262] + (i * 128)));
            }
            return mapping;
        }

        /// <summary>
        /// Create a mapping from a monster formation [monster 1 id] to character name id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<int> ReadMappingEnemyFormationToName(byte[] data)
        {
            var id259to262 = new List<int>();
            for (var i = 0; i < 2344; i++)
            {
                id259to262.Add(BitConverter.ToInt16(data, Files.TableOffsets[259] + (i * 208)));
            }
            return id259to262;
        }

        /// <summary>
        /// Create mapping from equipment id (they are not ordered in the table itself) to equipment name id (table 127
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<int> ReadMappingEquipmentToName(byte[] data)
        {
            var equipmentToName = new List<int>();
            for (int i = 0; i < 1515; i++)
            {
                equipmentToName.Add(BitConverter.ToInt16(data, Files.TableOffsets[127] + (i * 128)));
            }
            return equipmentToName;
        }

        /// <summary>
        /// Read all class names (table 60)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> ReadClassNames(byte[] data)
        {
            var offset = Files.TableOffsets[60];
            var classnames = new List<string>();
            for (var j = 0; j < 382; j++)
            {
                var length = BitConverter.ToInt32(data, offset);
                var name = System.Text.Encoding.Unicode.GetString(data, offset + 4, length).
                    Replace("", "").Replace("＠＠", "").Replace("", "").Replace("\u20ff＠", "")
                    .Replace("燿ｘ", "").Replace("ｦ￿", "")
                    .Replace("替￿", "").Replace("￿ｍ", "")
                    .Replace("ｦｦ￿", "")
                    ;
                classnames.Add(name);
                offset += length + 0x96;
            }
            return classnames;
        }

        /// <summary>
        /// Read all item names (table 37)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> ReadItemNames(byte[] data)
        {
            var offset = Files.TableOffsets[37];
            var itemNames = new List<string>();
            for (var i = 0; i < 1393; i++)
            {
                var length = BitConverter.ToInt32(data, offset);
                var name = System.Text.Encoding.Unicode.GetString(data, offset + 4, length).
                    Replace("", "").Replace("＠＠", "").Replace("", "").Replace("\u20ff＠", "")
                    .Replace("燿ｘ", "").Replace("ｦ￿", "")
                    .Replace("替￿", "").Replace("￿ｍ", "")
                    .Replace("ｦｦ￿", "")
                    ;
                itemNames.Add($"{name}");
                offset += length + 0x96;
            }
            return itemNames;
        }

        /// <summary>
        /// Read all monster union names (table 35)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> ReadEnemyUnionNames(byte[] data)
        {
            var offset = Files.TableOffsets[35];
            var unionNames = new List<string>();
            for (var i = 0; i < 99; i++)
            {
                var length = BitConverter.ToInt32(data, offset);
                var name = System.Text.Encoding.Unicode.GetString(data, offset + 4, length);
                unionNames.Add(name);
                offset += length + 0x96;
            }
            return unionNames;
        }

        /// <summary>
        /// Read all unit character names (table 36)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> ReadUnitNames(byte[] data)
        {
            var offset = Files.TableOffsets[36];
            var unitNames = new List<String>();
            for (int i = 0; i < 1389; i++)
            {
                var length = BitConverter.ToInt32(data, offset);
                var name = System.Text.Encoding.Unicode.GetString(data, offset + 4, length).
                    Replace("", "").Replace("＠＠", "").Replace("", "").Replace("\u20ff＠", "");
                unitNames.Add(name);
                offset += length + 0x96;
                if (name == "")
                    offset -= 0x6;
            }
            return unitNames;
        }
    }
}
