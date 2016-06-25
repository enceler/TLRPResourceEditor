using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Commander;
using PropertyChanged;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Models;
using TLRPResourceEditor.Properties;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class StartViewModel
    {
        // The selected language
        public List<Language> LanguageList { get; set; } = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
        private Language _selectedLanguage = Language.English;
        public Language SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { _selectedLanguage = value; Files.Language = value; }
        }

        public string CookedPcPath { get; set; } = Files.CookedPCPath;

        [OnCommand("RestoreBattleData")]
        private void RestoreBattleDataExecute()
        {
            try
            {
                if (File.Exists(Files.BattleFile + ".backup"))
                    File.Copy(Files.BattleFile + ".backup", Files.BattleFile, true);
                Files.Language = Files.Language;
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

        [OnCommandCanExecute("RestoreBattleData")]
        private bool RestoreBattleDataCanExecute()
        {
            return File.Exists(Files.BattleFile);
        }

        [OnCommand("RestoreMapData")]
        private void RestoreMapDataExecute()
        {
            try
            {
                foreach (var entry in Files.MapFiles)
                {
                    if (File.Exists(entry.MapFile + ".backup"))
                        File.Copy(entry.MapFile + ".backup", entry.MapFile, true);
                }
                Files.Language = Files.Language;
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

        [OnCommandCanExecute("RestoreMapData")]
        private bool RestoreMapDataExecuteCanExecute()
        {
            return File.Exists(Files.BattleFile);
        }

        [OnCommand("RandomizeEnemyStats")]
        private void RandomizeAllEnemyStats()
        {
            // TODO: possibly move to Monster.cs and MonsterFormation.cs

            var r = new Random();
            var maxHP = 40000;
            var mapAP = 2000;
            byte maxStat = 255;
            byte maxDefense = 100;
            byte maxBR = 255;

            try
            {
                // Change the raw bytes themselves all at once, otherwise there would be
                // hundreds of thousands of costly disk accesses
                var data = File.ReadAllBytes(Files.BattleFile);
                for (var i = 0; i < 2344; i++)
                {
                    var offset = Files.TableOffsets[259] + i * 208;
                    var hp = BitConverter.ToInt32(data, 12 + offset);
                    var ap = BitConverter.ToInt32(data, 16 + offset);
                    var str = data[21 + offset];
                    var itl = data[22 + offset];
                    var spd = data[23 + offset];
                    var unq = data[26 + offset];
                    var ten = data[27 + offset];
                    var defSlash = data[92 + offset];
                    var defBludgeon = data[93 + offset];
                    var defMaul = data[94 + offset];
                    var defPierce = data[95 + offset];
                    var defFlame = data[96 + offset];
                    var defThunder = data[97 + offset];
                    var defFrost = data[98 + offset];
                    var defAcid = data[99 + offset];
                    var defVenom = data[100 + offset];

                    data[21 + offset] = Math.Min(maxStat, (byte)r.Next(str / 2, str * 2));
                    data[22 + offset] = Math.Min(maxStat, (byte)r.Next(itl / 2, itl * 2));
                    data[23 + offset] = Math.Min(maxStat, (byte)r.Next(spd / 2, spd * 2));
                    data[26 + offset] = Math.Min(maxStat, (byte)r.Next(unq / 2, unq * 2));
                    data[27 + offset] = Math.Min(maxStat, (byte)r.Next(ten / 2, ten * 2));
                    data[92 + offset] = Math.Min(maxDefense, (byte)r.Next(defSlash / 2, defSlash * 2));
                    data[93 + offset] = Math.Min(maxDefense, (byte)r.Next(defBludgeon / 2, defBludgeon * 2));
                    data[94 + offset] = Math.Min(maxDefense, (byte)r.Next(defMaul / 2, defMaul * 2));
                    data[95 + offset] = Math.Min(maxDefense, (byte)r.Next(defPierce / 2, defPierce * 2));
                    data[96 + offset] = Math.Min(maxDefense, (byte)r.Next(defFlame / 2, defFlame * 2));
                    data[97 + offset] = Math.Min(maxDefense, (byte)r.Next(defThunder / 2, defThunder * 2));
                    data[98 + offset] = Math.Min(maxDefense, (byte)r.Next(defFrost / 2, defFrost * 2));
                    data[99 + offset] = Math.Min(maxDefense, (byte)r.Next(defAcid / 2, defAcid * 2));
                    data[100 + offset] = Math.Min(maxDefense, (byte)r.Next(defVenom / 2, defVenom * 2));
                    var newHP = BitConverter.GetBytes(Math.Min(maxHP, r.Next(hp / 2, hp * 2)));
                    var newAP = BitConverter.GetBytes(Math.Min(mapAP, r.Next(ap / 2, ap * 2)));
                    Array.Copy(newHP, 0, data, 12 + offset, 4);
                    Array.Copy(newAP, 0, data, 16 + offset, 4);
                }

                for (var i = 0; i < 2292; i++)
                {
                    var offset = Files.TableOffsets[123] + i * 240;
                    var brMax = data[43 + offset];
                    var brAddMin = data[44 + offset];
                    var brAddMax = data[45 + offset];

                    data[43 + offset] = Math.Min(maxBR, (byte)r.Next(brMax / 2, brMax * 2));
                    data[44 + offset] = (byte)Math.Min(maxBR - 1, (byte)r.Next(brAddMin / 2, brAddMin * 2));
                    data[45 + offset] = (byte)Math.Min(data[44 + offset] + 1, (byte)r.Next(brAddMax / 2, brAddMax * 2));
                }

                File.WriteAllBytes(Files.BattleFile, data);
                Files.Language = Files.Language;
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

        [OnCommandCanExecute("RandomizeEnemyStats")]
        private bool RandomizeAllEnemyStatsCanExecute()
        {
            return File.Exists(Files.BattleFile) && Monster.Monsters.Count > 0;
        }

        [OnCommand("RandomizeAllArts")]
        private void RandomizeAllUnitArts()
        {
            // TODO: changes should be written directly insides the byte array.
            //       Until then, first check whether we have access to the files:
            // TOTO: Possibly move to Unit.cs

            FileStream stream = null;
            try
            {
                stream = File.Open(Files.BattleFile, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (IOException)
            {
                MessageBox.Show(Resources.FileCannotBeOverwritten);
                return;
            }
            finally
            {
                stream?.Close();
            }

            try
            {
                var brMin = 0;
                var brMax = 80;

                var r = new Random();
                var itemvalues = Enum.GetValues(typeof(ItemArts));
                var mysticvalues = Enum.GetValues(typeof(MysticArts));

                foreach (var unit in Unit.Units)
                {
                    if (unit.PartyTalkSelect == null)
                        continue;

                    unit.PartyTalkSelect.ItemArtLearned = (ItemArts)itemvalues.GetValue(r.Next(itemvalues.Length));
                    unit.PartyTalkSelect.MysticArtLearned = (MysticArts)mysticvalues.GetValue(r.Next(mysticvalues.Length));
                    unit.PartyTalkSelect.ItemBR = r.Next(brMin, brMax + 1);
                    unit.PartyTalkSelect.MysticBR = r.Next(brMin, brMax + 1);
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

        [OnCommandCanExecute("RandomizeAllArts")]
        private bool RandomizeAllUnitArtsCanExecute()
        {
            return File.Exists(Files.BattleFile) && Unit.Units.Count > 0;
        }

        [OnCommand("EnterNewTLRPath")]
        private void EnterNewPath()
        {
            using (var dialog = new FolderBrowserDialog { Description = Resources.MainWindowViewModel_EnterNewPath_ })
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Files.ReadBasicInfo(Path.Combine(dialog.SelectedPath, "RushGame/CookedPC/"));
                    Files.Language = Files.Language;
                    CookedPcPath = Files.CookedPCPath;
                }
            }
        }
    }
}
