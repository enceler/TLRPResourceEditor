using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using PropertyChanged;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Properties;

namespace TLRPResourceEditor.Models
{
    [ImplementPropertyChanged]
    public class Unit
    {
        public static ObservableCollection<Unit> Units { get; set; } = new ObservableCollection<Unit>();
        private static readonly List<Talk> Talks = new List<Talk>();

        public static void LoadData()
        {
            Units.Clear();
            Talks.Clear();

            var data = File.ReadAllBytes(Files.BattleFile);

            // Collects the names of all units that share the same party talks
            var mapping = new Dictionary<int, List<string>>();

            // Read the possible party talks (including which arts to learn at what BR)
            var offset = Files.TableOffsets[270];
            for (var i = 0; i < 57; i++)
            {
                Talks.Add(new Talk()
                {
                    MysticBR         = data[42 + offset],
                    ItemBR           = data[66 + offset],
                    MysticArtLearned = (MysticArts)BitConverter.ToInt16(data, 46 + offset),
                    ItemArtLearned   = (ItemArts)BitConverter.ToInt16(data, 70 + offset),
                    FileName         = Files.BattleFile,
                    Offset           = offset,
                });
                offset += 136;
            }

            // Read all 404 usable characters
            offset = Files.TableOffsets[260];
            for (var i = 0; i < 404; i++)
            {
                var unit = new Unit()
                {
                    Name              = Names.UnitNames[Names.CharacterToName[BitConverter.ToInt16(data, 0 + offset)]],
                    Id                = i,
                    SortId            = BitConverter.ToInt16(data, 8 + offset),
                    Class             = Names.ClassNames[Names.CharacterToClass[BitConverter.ToInt16(data, 10 + offset)]],
                    BaseRank          = BitConverter.ToInt32(data, 12 + offset),
                    BaseHP            = BitConverter.ToInt32(data, 16 + offset),
                    BaseAP            = BitConverter.ToInt32(data, 20 + offset),
                    BaseAPCharge      = data[24 + offset],
                    BaseStr           = data[25 + offset],
                    BaseInt           = data[26 + offset],
                    BaseSpd           = data[27 + offset],
                    BaseUnqName       = BitConverter.ToInt16(data, 28 + offset).ToString(),
                    BaseUnq           = data[30 + offset],
                    BaseTension       = data[31 + offset],
                    TensionStyle      = data[32 + offset],
                    CombatArtsList    = BitConverter.ToInt16(data, 106 + offset),
                    MysticArtsList    = BitConverter.ToInt16(data, 108 + offset),
                    ItemArtsList      = BitConverter.ToInt16(data, 110 + offset),
                    EquipmentMain     = BitConverter.ToInt16(data, 112 + offset),
                    EquipmentOff      = BitConverter.ToInt16(data, 118 + offset),
                    Accessory1        = BitConverter.ToInt16(data, 122 + offset),
                    Accessory2        = BitConverter.ToInt16(data, 124 + offset),
                    BaseMoney         = BitConverter.ToInt16(data, 150 + offset),
                    AIMTypeEquipment  = BitConverter.ToInt16(data, 214 + offset),
                    AIMTypeAccessory1 = BitConverter.ToInt16(data, 216 + offset),
                    AIMTypeAccessory2 = BitConverter.ToInt16(data, 218 + offset),
                    AIMOpenBalance    = data[220 + offset],
                    AIMOpenPhysical   = data[221 + offset],
                    AIMOpenMystic     = data[222 + offset],
                    AIMEnd            = data[223 + offset],
                    Offset            = offset,
                    FileName          = Files.BattleFile,

                };

                var partytalk = BitConverter.ToInt16(data, 228 + offset);
                if (partytalk >= 0 && partytalk < 47) // only those with actual, valid choices
                {
                    unit.PartyTalkSelect = Talks[partytalk];
                    if (mapping.ContainsKey(partytalk))
                        mapping[partytalk].Add(unit.Name);
                    else
                        mapping[partytalk] = new List<string> { unit.Name };
                }
                Units.Add(unit);
                offset += 232;
            }
        }

        private int baseRank;
        private int baseHP;
        private int baseAP;
        private int baseAPCharge;
        private int baseStr;
        private int baseInt;
        private int baseSpd;
        private int baseUnq;
        private int baseTension;

        public int Offset            { get; set; }
        public string FileName       { get; set; }
        public string Name           { get; set; }
        public int SortId            { get; set; }
        public string Class          { get; set; }
        public int Id                { get; set; }
        public int BaseRank          { get { return baseRank; }     set { ChangeStat(12, value, 4); baseRank = value; } }
        public int BaseHP            { get { return baseHP; }       set { ChangeStat(16, value, 4); baseHP = value; } }
        public int BaseAP            { get { return baseAP; }       set { ChangeStat(20, value, 4); baseAP = value; } }
        public int BaseAPCharge      { get { return baseAPCharge; } set { ChangeStat(24, value, 1); baseAPCharge = value; } }
        public int BaseStr           { get { return baseStr; }      set { ChangeStat(25, value, 1); baseStr = value; } }
        public int BaseInt           { get { return baseInt; }      set { ChangeStat(26, value, 1); baseInt = value; } }
        public int BaseSpd           { get { return baseSpd; }      set { ChangeStat(27, value, 1); baseSpd = value; } }
        public int BaseUnq           { get { return baseUnq; }      set { ChangeStat(30, value, 1); baseUnq = value; } }
        public int BaseTension       { get { return baseTension; }  set { ChangeStat(31, value, 1); baseTension = value; } }
        public string BaseUnqName    { get; set; }
        public int TensionStyle      { get; set; }
        public int CombatArtsList    { get; set; }
        public int MysticArtsList    { get; set; }
        public int ItemArtsList      { get; set; }
        public int EquipmentMain     { get; set; }
        public int EquipmentOff      { get; set; }
        public int Accessory1        { get; set; }
        public int Accessory2        { get; set; }
        public int BaseMoney         { get; set; }
        public int AIMTypeEquipment  { get; set; }
        public int AIMTypeAccessory1 { get; set; }
        public int AIMTypeAccessory2 { get; set; }
        public int AIMOpenBalance    { get; set; }
        public int AIMOpenPhysical   { get; set; }
        public int AIMOpenMystic     { get; set; }
        public int AIMEnd            { get; set; }
        public Talk PartyTalkSelect  { get; set; }

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

    }

    public class Talk
    {
        private int mysticBR;
        private int itemBR;
        private MysticArts mysticArtLearned;
        private ItemArts itemArtLearned;

        public static List<string> MysticChoices = new List<string>();
        public int Offset                   { get; set; }
        public string FileName              { get; set; }
        public int MysticBR                 { get { return mysticBR; }         set { ChangeStat(42, value, 1); mysticBR = value;  } }
        public int ItemBR                   { get { return itemBR; }           set { ChangeStat(66, value, 1); itemBR = value;  } }
        public MysticArts MysticArtLearned  { get { return mysticArtLearned; } set { ChangeStat(46, (int)value, 2); mysticArtLearned = value; } }
        public ItemArts ItemArtLearned      { get { return itemArtLearned; }   set { ChangeStat(70, (int)value, 2); itemArtLearned = value; } }

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
    }
    public enum ItemArts
    {
        //None = 0,
        Herbs = 21,
        Potions = 22,
        Lotions = 23,
        Explosives = 24,
        Shards = 25,
        Traps = 26
    }
    public enum MysticArts
    {
        //None = 0,
        Invocations = 14,
        Evocations = 15,
        Hexes = 16,
        Remedies = 17,
        Psi = 18,
        Wards = 19
    }
}
