using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using PropertyChanged;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Properties;

namespace TLRPResourceEditor.Models
{
    [ImplementPropertyChanged]
    public class Equipment
    {
        public static ObservableCollection<Equipment> Equipments { get; set; } = new ObservableCollection<Equipment>();

        public static void LoadData()
        {
            Equipments.Clear();
            var data = File.ReadAllBytes(Files.BattleFile);

            var offset = Files.TableOffsets[127];
            for (var i = 0; i < 1515; i++)
            {
                var item = new Equipment
                {
                    Id                      = i,
                    //Name                    = 
                    NameString              = Names.ItemNames[BitConverter.ToInt16(data, 0 + offset)],
                    MaterialId              = data[6 + offset],
                    SortId                  = BitConverter.ToInt16(data, 18),
                    BuyPrice                = BitConverter.ToInt32(data, 20),
                    SellPrice               = BitConverter.ToInt32(data, 24),
                    BreakPrice              = BitConverter.ToInt32(data, 28),
                    Break                   = BitConverter.ToBoolean(data, 32 + offset),
                    Sell                    = BitConverter.ToBoolean(data, 33 + offset),
                    EquipType               = BitConverter.ToInt16(data, 36 + offset),
                    AttackAttribute         = BitConverter.ToInt16(data, 38 + offset),
                    Burden                  = data[56 + offset],
                    Attack                  = data[57 + offset],
                    MysticAttack            = data[58 + offset],
                    Defense                 = data[59 + offset],
                    MysticDefense           = data[60 + offset],
                    Evasion                 = data[61 + offset],
                    MysticEvasion           = data[62 + offset],
                    CritOffenseOnRate       = data[63 + offset],
                    CritOffenseSucccessRate = data[64 + offset],
                    CritDefenseOnRate       = data[65 + offset],
                    CritDefenseSuccessRate  = data[66 + offset],
                    CritButton              = BitConverter.ToInt16(data, 68 + offset),
                    ExpEquipStyle           = data[70 + offset],
                    ExpEquipCat             = data[71 + offset],
                    ExpMysticCat            = data[72 + offset],
                    ExpItemCat              = data[73 + offset],
                    ExpCombatArts           = data[74 + offset],
                    ExpCombatArtsDirection1 = data[75 + offset],
                    ExpCombatArtsDirection2 = data[76 + offset],
                    ExpMysticArts           = data[77 + offset],
                    ExpItemArts             = data[78 + offset],
                    ExpEquipMaster          = data[79 + offset],
                    FileName                = Files.BattleFile,
                    Offset                  = offset
                };
                Equipments.Add(item);
                offset += 128;
            }
        }

        private int buyPrice;
        private int sellPrice;
        private int breakPrice;
        private int burden;
        private int attack;
        private int mysticAttack;
        private int defense;
        private int mysticDefense;
        private int evasion;
        private int mysticEvasion;
        private int critOffenseOnRate;
        private int critOffenseSucccessRate;
        private int critDefenseOnRate;
        private int critDefenseSuccessRate;

        public int Offset                   { get; set; }
        public string Name                  { get; set; }
        public string FileName              { get; set; }
        public int Id                       { get; set; }
        public string NameString            { get; set; }
        public int Model                    { get; set; }
        public int MaterialId               { get; set; }
        public int SortId                   { get; set; }
        public bool Break                   { get; set; }
        public bool Sell                    { get; set; }
        public int EquipType                { get; set; }
        public int AttackAttribute          { get; set; }
        public int BuyPrice                 { get { return buyPrice; }                set { ChangeStat(20, value, 4); buyPrice = value; } }
        public int SellPrice                { get { return sellPrice; }               set { ChangeStat(24, value, 4); sellPrice = value; } }
        public int BreakPrice               { get { return breakPrice; }              set { ChangeStat(28, value, 4); breakPrice = value; } }
        public int Burden                   { get { return burden; }                  set { ChangeStat(56, value, 1); burden = value; } }
        public int Attack                   { get { return attack; }                  set { ChangeStat(57, value, 1); attack = value; } }
        public int MysticAttack             { get { return mysticAttack; }            set { ChangeStat(58, value, 1); mysticAttack = value; } }
        public int Defense                  { get { return defense; }                 set { ChangeStat(59, value, 1); defense = value; } }
        public int MysticDefense            { get { return mysticDefense; }           set { ChangeStat(60, value, 1); mysticDefense = value; } }
        public int Evasion                  { get { return evasion; }                 set { ChangeStat(61, value, 1); evasion = value; } }
        public int MysticEvasion            { get { return mysticEvasion; }           set { ChangeStat(62, value, 1); mysticEvasion = value; } }
        public int CritOffenseOnRate        { get { return critOffenseOnRate; }       set { ChangeStat(63, value, 1); critOffenseOnRate = value; } }
        public int CritOffenseSucccessRate  { get { return critOffenseSucccessRate; } set { ChangeStat(64, value, 1); critOffenseSucccessRate = value; } }
        public int CritDefenseOnRate        { get { return critDefenseOnRate; }       set { ChangeStat(65, value, 1); critDefenseOnRate = value; } }
        public int CritDefenseSuccessRate   { get { return critDefenseSuccessRate; }  set { ChangeStat(66, value, 1); critDefenseSuccessRate = value; } }
        public int CritButton               { get; set; }
        public int ExpEquipStyle            { get; set; }
        public int ExpEquipCat              { get; set; }
        public int ExpItemCat               { get; set; }
        public int ExpMysticCat             { get; set; }
        public int ExpCombatArts            { get; set; }
        public int ExpCombatArtsDirection1  { get; set; }
        public int ExpCombatArtsDirection2  { get; set; }
        public int ExpMysticArts            { get; set; }
        public int ExpItemArts              { get; set; }
        public int ExpEquipMaster           { get; set; }

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
                        stream.Write(new[] { (byte)value }, 0, 1);
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

        public override string ToString()
        {
            return $"[{Id}] {NameString.Substring(NameString.IndexOf("]", StringComparison.Ordinal) + 1)}";
        }

    }
}
