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
    public class Monster
    {
        public static ObservableCollection<Monster> Monsters { get; set; } = new ObservableCollection<Monster>();

        public string FileName { get; set; } 
        public int Offset { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        private bool rare;
        private bool boss;
        private int hPBase;
        private int aPBase;
        private int aPCharge;
        private int strBase;
        private int intBase;
        private int spdBase;
        private int uniqueBase;
        private int tensionBase;
        private int slashDefense;
        private int bludgeonDefense;
        private int maulDefense;
        private int pierceDefense;
        private int flameDefense;
        private int thunderDefense;
        private int frostDefense;
        private int acidDefense;
        private int venomDefense;
        private int equipment1Id;
        private int equipment2Id;

        public bool Rare             { get { return rare; }            set { rare = value;            ChangeStat(4, value ? 1 : 0, 1); } }
        public bool Boss             { get { return boss; }            set { boss = value;            ChangeStat(5, value ? 1 : 0, 1); } }
        public int HPBase            { get { return hPBase; }          set { hPBase = value;          ChangeStat(12, value, 4); } }
        public int APBase            { get { return aPBase; }          set { aPBase = value;          ChangeStat(16, value, 4); } }
        public int APCharge          { get { return aPCharge; }        set { aPCharge = value;        ChangeStat(20, value, 1); } }
        public int StrBase           { get { return strBase; }         set { strBase = value;         ChangeStat(21, value, 1); } }
        public int IntBase           { get { return intBase; }         set { intBase = value;         ChangeStat(22, value, 1); } }
        public int SpdBase           { get { return spdBase; }         set { spdBase = value;         ChangeStat(23, value, 1); } }
        public int UniqueBase        { get { return uniqueBase; }      set { uniqueBase = value;      ChangeStat(26, value, 1); } }
        public int TensionBase       { get { return tensionBase; }     set { tensionBase = value;     ChangeStat(27, value, 1); } }
        public int SlashDefense      { get { return slashDefense; }    set { slashDefense = value;    ChangeStat(92, value, 1); } }
        public int BludgeonDefense   { get { return bludgeonDefense; } set { bludgeonDefense = value; ChangeStat(93, value, 1); } }
        public int MaulDefense       { get { return maulDefense; }     set { maulDefense = value;     ChangeStat(94, value, 1); } }
        public int PierceDefense     { get { return pierceDefense; }   set { pierceDefense = value;   ChangeStat(95, value, 1); } }
        public int FlameDefense      { get { return flameDefense; }    set { flameDefense = value;    ChangeStat(96, value, 1); } }
        public int ThunderDefense    { get { return thunderDefense; }  set { thunderDefense = value;  ChangeStat(97, value, 1); } }
        public int FrostDefense      { get { return frostDefense; }    set { frostDefense = value;    ChangeStat(98, value, 1); } }
        public int AcidDefense       { get { return acidDefense; }     set { acidDefense = value;     ChangeStat(99, value, 1); } }
        public int VenomDefense      { get { return venomDefense; }    set { venomDefense = value;    ChangeStat(100, value, 1); } }
        public int Equipment1Id      { get { return equipment1Id; }    set { equipment1Id = value;    ChangeStat(108, value, 2); } }
        public int Equipment2Id      { get { return equipment2Id; }    set { equipment2Id = value;    ChangeStat(114, value, 2); } }
        public string EquipmentMain  { get; set; }
        public string EquipmentOff   { get; set; }
        public int EquipMainLevel    { get; set; }
        public int EquipOffhandLevel { get; set; }
        public int Accessory1        { get; set; }
        public int Accessory2        { get; set; }


        public static void LoadData()
        {
            Monsters.Clear();
            var data = File.ReadAllBytes(Files.BattleFile);
            for (var i = 0; i < 2344; i++)
            {
                var insideOffset = Files.TableOffsets[259] + (i * 208);
                var nameid = BitConverter.ToInt16(data, insideOffset);
                var name = "NAME NOT FOUND: " + nameid;
                if (nameid < Names.CharacterToName.Count && Names.CharacterToName[nameid] < Names.UnitNames.Count)
                    name = Names.UnitNames[Names.CharacterToName[nameid]];

                var equip1 = "[None]";
                var equip2 = "[None]";

                var equip1id = BitConverter.ToInt16(data, 108 + insideOffset);
                var equip1idOriginal = equip1id;
                if (equip1id >= 0)
                {
                    var id = BitConverter.ToInt16(data, 108 + insideOffset);
                    if (id < Names.EquipmentToName.Count && Names.EquipmentToName[id] < Names.ItemNames.Count)
                        equip1 = $"[{equip1idOriginal}] {Names.ItemNames[Names.EquipmentToName[BitConverter.ToInt16(data, 108 + insideOffset)]]}";
                    else
                        equip1 = "NAME NOT FOUND:" + id;
                }

                var equip2id = BitConverter.ToInt16(data, 114 + insideOffset);
                var equip2idOriginal = equip2id;
                if (equip2id >= 0)
                {
                    var id = BitConverter.ToInt16(data, 114 + insideOffset);
                    if (id < Names.EquipmentToName.Count && Names.EquipmentToName[id] < Names.ItemNames.Count)
                        equip2 = Names.ItemNames[Names.EquipmentToName[BitConverter.ToInt16(data, 114 + insideOffset)]];
                    else
                        equip1 = "NAME NOT FOUND:" + id;

                }

                Monsters.Add(new Monster
                {
                    Id                  = i,
                    Name                = name,
                    EquipmentMain       = equip1,
                    EquipmentOff        = equip2,
                    Equipment1Id        = equip1idOriginal,
                    Equipment2Id        = equip2idOriginal,
                    Rare                = BitConverter.ToBoolean(data, 4 + insideOffset),
                    Boss                = BitConverter.ToBoolean(data, 5 + insideOffset),
                    HPBase              = BitConverter.ToInt32(data, 12 + insideOffset),
                    APBase              = BitConverter.ToInt32(data, 16 + insideOffset),
                    APCharge            = data[20 + insideOffset],
                    StrBase             = data[21 + insideOffset],
                    IntBase             = data[22 + insideOffset],
                    SpdBase             = data[23 + insideOffset],
                    UniqueBase          = data[26 + insideOffset],
                    TensionBase         = data[27 + insideOffset],
                    SlashDefense        = data[92 + insideOffset],
                    BludgeonDefense     = data[93 + insideOffset],
                    MaulDefense         = data[94 + insideOffset],
                    PierceDefense       = data[95 + insideOffset],
                    FlameDefense        = data[96 + insideOffset],
                    ThunderDefense      = data[97 + insideOffset],
                    FrostDefense        = data[98 + insideOffset],
                    AcidDefense         = data[99 + insideOffset],
                    VenomDefense        = data[100 + insideOffset],
                    EquipMainLevel      = data[110 + insideOffset],
                    EquipOffhandLevel   = data[116 + insideOffset],
                    Accessory1          = BitConverter.ToInt16(data, 118 + insideOffset),
                    Accessory2          = BitConverter.ToInt16(data, 118 + insideOffset),
                    Offset              = insideOffset,
                    FileName            = Files.BattleFile
                });

            }
        }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }

        private void ChangeStat(int offset, int value, int length)
        {
            if (Offset == 0 || FileName == null)
                return;

            try
            {
                using (Stream stream = new FileStream(FileName, FileMode.Open))
                {
                    stream.Seek(Offset + offset, SeekOrigin.Begin);

                    if (length == 4)
                        stream.Write(BitConverter.GetBytes(value), 0, 4);
                    else if (length == 2)
                        stream.Write(new byte[] { (byte)value, (byte)(value >> 8) }, 0, 2);
                    else
                        stream.Write(new byte[] { (byte)value }, 0, 1);
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
    }
}
