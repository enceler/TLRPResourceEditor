﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using PropertyChanged;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Properties;

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
        public int GoldDrop { get; set; }
        public int SurelyItem { get; set; }
        public int Normal1Item { get; set; }
        public int Normal2Item { get; set; }
        public int RareItem { get; set; }
        public int VeryRareItem { get; set; }
        public int WeeklyItem { get; set; }
        public int DailyItem { get; set; }
        public int SurelyItemIndex { get; set; }
        public int Normal1ItemIndex { get; set; }
        public int Normal2ItemIndex { get; set; }
        public int RareItemIndex { get; set; }
        public int VeryRareItemIndex { get; set; }
        public int WeeklyItemIndex { get; set; }
        public int DailyItemIndex { get; set; }
        public int SurelyAmount { get; set; }
        public int Normal1Amount { get; set; }
        public int Normal2Amount { get; set; }
        public int RareAmount { get; set; }
        public int VeryRareAmount { get; set; }
        public int WeeklyAmount { get; set; }
        public int DailyAmount { get; set; }
        public int SurelyChance { get; set; }
        public int Normal1Chance { get; set; }
        public int Normal2Chance { get; set; }
        public int RareChance { get; set; }
        public int VeryRareChance { get; set; }
        public int WeeklyChance { get; set; }
        public int DailyChance { get; set; }

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


        public string SurelyItemName { get; set; }
        public string Normal1ItemName { get; set; }
        public string Normal2ItemName { get; set; }
        public string RareItemName { get; set; }
        public string VeryRareItemName { get; set; }
        public string WeeklyItemName { get; set; }
        public string DailyItemName { get; set; }

        public static void LoadData()
        {
            Monsters.Clear();
            //Monsters = new ObservableCollection<Monster>();
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

                var equip1Id = BitConverter.ToInt16(data, 108 + insideOffset);
                var equip1IdOriginal = equip1Id;
                if (equip1Id >= 0)
                {
                    var id = BitConverter.ToInt16(data, 108 + insideOffset);
                    if (id < Names.EquipmentToName.Count && Names.EquipmentToName[id] < Names.ItemNames.Count)
                        equip1 = $"[{equip1IdOriginal}] {Names.ItemNames[Names.EquipmentToName[BitConverter.ToInt16(data, 108 + insideOffset)]]}";
                    else
                        equip1 = "NAME NOT FOUND:" + id;
                }

                var equip2Id = BitConverter.ToInt16(data, 114 + insideOffset);
                var equip2IdOriginal = equip2Id;
                if (equip2Id >= 0)
                {
                    var id = BitConverter.ToInt16(data, 114 + insideOffset);
                    if (id < Names.EquipmentToName.Count && Names.EquipmentToName[id] < Names.ItemNames.Count)
                        equip2 = $"[{equip1IdOriginal}] {Names.ItemNames[Names.EquipmentToName[BitConverter.ToInt16(data, 114 + insideOffset)]]}";
                    else
                        equip1 = "NAME NOT FOUND:" + id;

                }

                Monsters.Add(new Monster
                {
                    Id                  = i,
                    Name                = name,
                    EquipmentMain       = equip1,
                    EquipmentOff        = equip2,
                    Equipment1Id        = equip1IdOriginal,
                    Equipment2Id        = equip2IdOriginal,
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

                    GoldDrop = BitConverter.ToInt32(data, 140 + insideOffset),
                    //SurelyItem = BitConverter.ToInt16(data, 146 + insideOffset),
                    //Normal1Item = BitConverter.ToInt16(data, 154 + insideOffset),
                    //Normal2Item = BitConverter.ToInt16(data, 162 + insideOffset),
                    //RareItem = BitConverter.ToInt16(data, 170 + insideOffset),
                    //VeryRareItem = BitConverter.ToInt16(data, 178 + insideOffset),
                    //WeeklyItem = BitConverter.ToInt16(data, 186 + insideOffset),
                    //DailyItem = BitConverter.ToInt16(data, 198 + insideOffset),

                    SurelyChance = data[148 + insideOffset],
                    Normal1Chance = data[156 + insideOffset],
                    Normal2Chance = data[164 + insideOffset],
                    RareChance = data[172 + insideOffset],
                    VeryRareChance = data[180 + insideOffset],
                    WeeklyChance = data[190 + insideOffset],
                    DailyChance = data[202 + insideOffset],

                    SurelyItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 144 + insideOffset), BitConverter.ToInt16(data, 146 + insideOffset))),
                    Normal1ItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 152 + insideOffset), BitConverter.ToInt16(data, 154+ insideOffset))),
                    Normal2ItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 160 + insideOffset), BitConverter.ToInt16(data, 162 + insideOffset))),
                    RareItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 168 + insideOffset), BitConverter.ToInt16(data, 170 + insideOffset))),
                    VeryRareItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 176 + insideOffset), BitConverter.ToInt16(data, 178 + insideOffset))),
                    WeeklyItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 184 + insideOffset), BitConverter.ToInt16(data, 186 + insideOffset))),
                    DailyItemName = Names.DropItem(new Tuple<int, int>(BitConverter.ToInt16(data, 196 + insideOffset), BitConverter.ToInt16(data, 198 + insideOffset))),

                    Normal2Amount = data[165 + insideOffset],
                    SurelyAmount = data[149 + insideOffset],
                    Normal1Amount = data[157 + insideOffset],
                    RareAmount = data[173 + insideOffset],
                    VeryRareAmount = data[181 + insideOffset],
                    WeeklyAmount = data[189 + insideOffset],
                    DailyAmount = data[201 + insideOffset],


                    Offset = insideOffset,
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
                        stream.Write(new[] { (byte)value, (byte)(value >> 8) }, 0, 2);
                    else
                        stream.Write(buffer: new[] { (byte)value }, offset: 0, count: 1);
                }
            }
            catch (IOException)
            {
                MessageBox.Show(Resources.FileCannotBeOverwritten);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }
    }
}
