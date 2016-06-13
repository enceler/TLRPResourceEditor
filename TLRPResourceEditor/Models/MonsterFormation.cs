using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using TLRPResourceEditor.Data;

namespace TLRPResourceEditor.Models
{
    [ImplementPropertyChanged]
    public class MonsterFormation
    {
        public static ObservableCollection<MonsterFormation> MonsterFormations { get; set; } = new ObservableCollection<MonsterFormation>();

        private int bRMin1        { get; set; }
        private int bRMax1        { get; set; }
        private int bRMin2        { get; set; }
        private int bRMax2        { get; set; }
        private int bR1PlayerBR   { get; set; }
        private int bRMin3        { get; set; }
        private int bRMax3        { get; set; }
        private int bR2PlayerBR   { get; set; }
        private int bRMaxLevel    { get; set; }
        private int bRAddLevelMin { get; set; }
        private int bRAddLevelMax { get; set; }

        public string FileName    { get; set; }
        public int Offset         { get; set; }
        public int Id             { get; set; }
        public string Name        { get; set; }
        public string Formation   { get; set; }
        public ObservableCollection<Monster> MonsterList { get; set; }

        public int BR1PlayerBR   { get; set; }
        public int BRMin1        { get { return bRMin1; }        set { ChangeStat(28, value, 1); bRMin1 = value; } }
        public int BRMax1        { get { return bRMax1; }        set { ChangeStat(29, value, 1); bRMax1 = value; } }
        public int BR2PlayerBR   { get { return bR1PlayerBR; }   set { ChangeStat(31, value, 1); bR1PlayerBR = value; } }
        public int BRMin2        { get { return bRMin2; }        set { ChangeStat(34, value, 1); bRMin2 = value; } }
        public int BRMax2        { get { return bRMax2; }        set { ChangeStat(35, value, 1); bRMax2 = value; } }
        public int BR3PlayerBR   { get { return bR2PlayerBR; }   set { ChangeStat(37, value, 1); bR2PlayerBR = value; } }
        public int BRMin3        { get { return bRMin3; }        set { ChangeStat(40, value, 1); bRMin3 = value; } }
        public int BRMax3        { get { return bRMax3; }        set { ChangeStat(41, value, 1); bRMax3 = value; } }
        public int BRMaxLevel    { get { return bRMaxLevel; }    set { ChangeStat(43, value, 1); bRMaxLevel = value; } }
        public int BRAddLevelMin { get { return bRAddLevelMin; } set { ChangeStat(44, value, 1); bRAddLevelMin = value; } }
        public int BRAddLevelMax { get { return bRAddLevelMax; } set { ChangeStat(45, value, 1); bRAddLevelMax = value; } }

        public static void LoadData()
        {
            MonsterFormations.Clear();
            var data = File.ReadAllBytes(Files.BattleFile);
            
            for (var i = 0; i < 2292; i++)
            {
                /// Formation data
                var insideOffset = Files.TableOffsets[123] + (i * 240);
                var monster = new MonsterFormation
                {
                    Id = i * 5,
                    Name = Names.UnitNames[Names.Id262To36[Names.Id259to262[BitConverter.ToInt16(data, 26 + insideOffset)]]],
                    MonsterList = new ObservableCollection<Monster>(),
                    BRMin1 = data[28 + insideOffset],
                    BRMax1 = data[29 + insideOffset],
                    BR1PlayerBR = 0,

                    BRMin2 = data[34 + insideOffset],
                    BRMax2 = data[35 + insideOffset],
                    BR2PlayerBR = data[31 + insideOffset],

                    BRMin3 = data[40 + insideOffset],
                    BRMax3 = data[41 + insideOffset],
                    BR3PlayerBR = data[37 + insideOffset],

                    BRMaxLevel = data[43 + insideOffset],
                    BRAddLevelMin = data[44 + insideOffset],
                    BRAddLevelMax = data[45 + insideOffset],

                    FileName = Files.BattleFile,
                    Offset = insideOffset
                };

                /// Formation monster entries
                for (int j = 0; j < 5; j++)
                {
                    var monsterId = BitConverter.ToInt16(data, 26 + insideOffset + (j * 48));
                    if (monsterId != -1)
                    {
                        monster.MonsterList.Add(Monster.Monsters[monsterId]);
                    }
                }

                MonsterFormations.Add(monster);
            }
        }


        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }

        public void WriteNewMonsterList()
        {
            try
            {
                using (Stream stream = new FileStream(FileName, FileMode.Open))
                {
                    var unused = 0xFFFF;
                    for (var i = 0; i < 5; i++)
                    {
                        stream.Seek(Offset + 26 + (48 * i), SeekOrigin.Begin);
                        if (i > MonsterList.Count - 1)
                            stream.Write(new byte[] { (byte)unused, (byte)(unused >> 8) }, 0, 2);
                        else
                            stream.Write(new byte[] { (byte)MonsterList[i].Id, (byte)(MonsterList[i].Id >> 8) }, 0, 2);
                    }
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
