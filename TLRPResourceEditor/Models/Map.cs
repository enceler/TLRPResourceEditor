using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TLRPResourceEditor.Data;

namespace TLRPResourceEditor.Models
{
    [ImplementPropertyChanged]
    public class Map
    {
        private static List<SpawnRule> temporarySpawnRules;
        private static string temporaryFileName;

        public static List<Map> Maps { get; set; }

        public ObservableCollection<SpawnEntry> Spawns { get; set; }
        public string Name { get; set; }

        public static void LoadData()
        {
            Maps = new List<Map>();

            foreach (var entry in Files.MapFiles)
            {
                var data          = File.ReadAllBytes(entry.MapFile);
                temporaryFileName = entry.MapFile;
                var offsets       = FindEnemeyOffsets(data);
                var dataOffset    = offsets.Item1;
                var dataSize      = offsets.Item2;
                var innerOffset   = dataOffset + 0x148;
                var nameoffset    = BitConverter.ToInt32(data, dataOffset + 0x34) + 0x20;
                var ruleoffset    = BitConverter.ToInt32(data, dataOffset + 0x3C) + 0x20;
                var enemiesoffset = BitConverter.ToInt32(data, dataOffset + 0x2C) + 0x20;

                var map = new Map
                {
                    Spawns = new ObservableCollection<SpawnEntry>(),
                    Name = entry.MapName,
                };

                for (var i = 0; i < 255; i++)
                {
                    if (innerOffset + 0x50 > dataSize + dataOffset || data[innerOffset + 0x51] != 255 || data[innerOffset + 0x52] != 255)
                        break;

                    var nameId    = BitConverter.ToInt32(data, innerOffset + 24);
                    var spawnrule = BitConverter.ToInt32(data, innerOffset + 12);
                    var location  = BitConverter.ToInt32(data, innerOffset + 16);
                    var model     = BitConverter.ToInt32(data, innerOffset + 20);

                    if (nameId >= 0)
                    {
                        var spawnEntry = new SpawnEntry
                        {
                            Name = "Unknown",
                            NameId = nameId,
                            Rules = new List<SpawnRule>(),
                            Offset = innerOffset,
                            FileName = entry.MapFile,
                        };
                        foreach (var formation in MonsterFormation.MonsterFormations)
                        {
                            if (formation.Id == nameId)
                            {
                                spawnEntry.Name = $"[{nameId}] {formation.Name}";
                            }
                        }
                        temporarySpawnRules = new List<SpawnRule>();
                        spawnEntry.SpawnRuleText = ParseSpawnRule(data, dataOffset + ruleoffset + spawnrule, 0);
                        spawnEntry.Rules = temporarySpawnRules;
                        map.Spawns.Add(spawnEntry);
                    }

                    innerOffset += 0x50;
                }

                Maps.Add(map);
            }
        }

        private static string ParseSpawnRule(byte[] data, int offset, int line)
        {
            if (data[offset + (line * 8)] == 7)
            {
                return ParseSpawnRule(data, offset, data[(offset + line * 8) + 1]) + " AND " + ParseSpawnRule(data, offset, data[(offset + line * 8) + 2]);
            }
            else if (data[offset + (line * 8)] == 8)
            {
                return "\n(" + ParseSpawnRule(data, offset, data[(offset + line * 8) + 1]) + ")\n OR \n(" + ParseSpawnRule(data, offset, data[(offset + line * 8) + 2]) + ")\n";
            }
            else
            {
                var value1 = BitConverter.ToInt32(data, (offset + line * 8) + 4 + 8);
                var value2 = BitConverter.ToInt32(data, (offset + line * 8) + 4 + 16);
                var rule = new SpawnRule();
                if (value1 == 0)
                    rule.Type = "Progress";
                else if (value1 == 136)
                    rule.Type = "Seed 1";
                else if (value1 == 137)
                    rule.Type = "Seed 2";
                else if (value1 == 138)
                    rule.Type = "Seed 3";
                else
                    rule.Type = $"Flag [{value1}]";

                rule.Value = value2;

                var op = data[offset + line * 8];
                if (op == 1)
                    rule.ComparisonOperator = "=";
                else if (op == 4)
                    rule.ComparisonOperator = ">=";
                else if (op == 5)
                    rule.ComparisonOperator = "<";
                else if (op == 3)
                    rule.ComparisonOperator = ">";
                rule.Offset = offset + (line * 8);
                rule.FileName = temporaryFileName;

                if (!rule.Type.StartsWith("Flag"))
                    temporarySpawnRules.Add(rule);

                return $" {rule.Type} {rule.ComparisonOperator} {rule.Value} ";
            }
        }

        private static Tuple<int, int> FindEnemeyOffsets(byte[] data)
        {
            var NameCount = BitConverter.ToInt32(data, 25);
            var NameOffset = BitConverter.ToInt32(data, 29);
            var Names = new List<string>();
            for (var i = 0; i < NameCount; i++)
            {
                var length = BitConverter.ToInt32(data, NameOffset);
                if (length < 0 || length > data.Length + NameOffset)
                    break;

                Names.Add(System.Text.Encoding.UTF8.GetString(data, NameOffset + 4, length - 1));
                NameOffset += length + 12;
            }


            var ExportCount = BitConverter.ToInt32(data, 33);
            var ExportOffset = BitConverter.ToInt32(data, 37);
            for (var i = 0; i < ExportCount; i++)
            {
                var name = "UNKNOWN";
                var nameId = BitConverter.ToInt32(data, ExportOffset + 12);
                if (nameId < Names.Count)
                    name = Names[nameId];

                var additionalFields = BitConverter.ToInt32(data, ExportOffset + 44);
                var dataOffset = BitConverter.ToInt32(data, ExportOffset + 36);
                var dataSize = BitConverter.ToInt32(data, ExportOffset + 32);

                if (name.ToLower().Contains("ene_set"))
                {
                    return Tuple.Create(dataOffset, dataSize);
                }

                if (dataSize < 1)
                {
                    ExportOffset += (additionalFields * 4) + 72;
                    continue;
                }

                var type = BitConverter.ToInt32(data, ExportOffset);
                var parent = BitConverter.ToInt32(data, ExportOffset + 4);

                if (dataSize > data.Length)
                {
                    ExportOffset += (additionalFields * 4) + 72;
                    continue;
                }

                ExportOffset += (additionalFields * 4) + 72;
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [ImplementPropertyChanged]
    public class SpawnEntry
    {
        private int nameId;

        public int NameId            { get { return nameId; } set { ChangeStat(24, value); nameId = value; } }
        public string FileName       { get; set; }
        public int Offset            { get; set; }
        public string Name           { get; set; }
        public List<SpawnRule> Rules { get; set; }
        public string SpawnRuleText  { get; set; }

        private void ChangeStat(int offset, int value)
        {
            if (Offset == 0 || FileName == null)
                return;

            try
            {
                using (Stream stream = new FileStream(FileName, FileMode.Open))
                {
                    stream.Seek(Offset + offset, SeekOrigin.Begin);
                    stream.Write(BitConverter.GetBytes(value), 0, 4);
                }
            }
            catch (IOException x)
            {
                MessageBox.Show("Could not write to the game files. Make sure the game isn't running and that the files can be overwritten.");
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [ImplementPropertyChanged]
    public class SpawnRule
    {
        private int valueValue;

        public string FileName           { get; set; }
        public int Offset                { get; set; }
        public int Value                 { get { return valueValue; } set { ChangeStat(20, value); valueValue = value; } }
        public string Type               { get; set; }
        public string ComparisonOperator { get; set; }
        public string Description        { get { return $"{Type} {ComparisonOperator}"; } }

        private void ChangeStat(int offset, int value)
        {
            if (Offset == 0 || FileName == null)
                return;

            try
            {
                using (Stream stream = new FileStream(FileName, FileMode.Open))
                {
                    stream.Seek(Offset + offset, SeekOrigin.Begin);
                    stream.Write(BitConverter.GetBytes(value), 0, 4);
                }
            }
            catch (IOException x)
            {
                MessageBox.Show("Could not write to the game files. Make sure the game isn't running and that the files can be overwritten.");
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }

        public override string ToString()
        {
            return Type;
        }
    }
}
