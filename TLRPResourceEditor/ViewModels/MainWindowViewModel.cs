using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    /// <summary>
    /// Implementing a simplyfied ViewModel for TLRPRE
    /// </summary>
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        private ICommand enterNewTLRPath;
        private ICommand addEnemyToFormation;
        private ICommand removeEnemyFromFormation;
        private ICommand changeMonster;
        private ICommand changeEquipment;
        private ICommand changeMonsterFormation;
        public ICommand RandomizeEnemyStats { get; } = new RelayCommand(param => RandomizeAllEnemyStats(), param => true);
        public ICommand RandomizeAllArts { get; } = new RelayCommand(param => RandomizeAllUnitArts(), param => true);
        public ICommand RestoreBattleData { get; } = new RelayCommand(param => RestoreBattleDataExecute(), param => true);
        public ICommand RestoreMapData { get; } = new RelayCommand(param => RestoreMapDataExecute(), param => true);
        public ICommand EnterNewTLRPath { get { if (enterNewTLRPath == null) enterNewTLRPath = new RelayCommand(EnterNewPath, param => true); return enterNewTLRPath; } }
        public ICommand AddEnemyToFormation { get { if (addEnemyToFormation == null) addEnemyToFormation = new RelayCommand(AddMonsterToFormation, param => true); return addEnemyToFormation; } }
        public ICommand RemoveEnemyFromFormation { get { if (removeEnemyFromFormation == null) removeEnemyFromFormation = new RelayCommand(RemoveMonsterFromFormation, param => true); return removeEnemyFromFormation; } }
        public ICommand ChangeMonster { get { if (changeMonster == null) changeMonster = new RelayCommand(ChangeMonsterType, param => true); return changeMonster; } }
        public ICommand ChangeEquipment { get { if (changeEquipment == null) changeEquipment = new RelayCommand(ChangeSelectedEquipment, param => true); return changeEquipment; } }
        public ICommand ChangeMonsterFormation { get { if (changeMonsterFormation == null) changeMonsterFormation = new RelayCommand(ChangeSelectedMonsterFormation, param => true); return changeMonsterFormation; } }

        // List of all monsters, including currently selected one, as well as a search filter
        public ObservableCollection<Monster> MonsterList { get; set; } = Monster.Monsters;
        public Monster SelectedMonster { get; set; }
        public Monster SelectedTypeChangeMonster { get; set; }
        private ICollectionView myMonsterList;
        private string monsterFilterString;
        public int MonsterListSelection { get; set; }
        public string MonsterFilterString { get { return monsterFilterString; } set { monsterFilterString = value; myMonsterList.Refresh(); } }


        // List of all monster formations, including currently selected one, as well as a search filter
        public ObservableCollection<MonsterFormation> MonsterFormations { get; set; } = MonsterFormation.MonsterFormations;
        public MonsterFormation SelectedMonsterFormation { get; set; }
        private ICollectionView myMonsterFormations;
        private string monsterFormationFilterString;
        public string MonsterFormationFilterString { get { return monsterFormationFilterString; } set { monsterFormationFilterString = value; myMonsterFormations.Refresh(); } }

        // List of all monster formations, including currently selected one, as well as a search filter to change selected monster formation
        public ObservableCollection<MonsterFormation> MonsterFormationsChange { get; set; } = MonsterFormation.MonsterFormations;
        public MonsterFormation SelectedMonsterFormationChange { get; set; }
        private ICollectionView myMonsterFormationsChange;
        private string monsterFormationFilterStringChange;
        public string MonsterFormationFilterStringChange { get { return monsterFormationFilterStringChange; } set { monsterFormationFilterStringChange = value; myMonsterFormationsChange.Refresh(); } }

        // List of all equipment, including currently selected one, as well as a search filter
        public ObservableCollection<Equipment> Equipments { get; set; } = Equipment.Equipments;
        public Equipment SelectedEquipment { get; set; }
        private ICollectionView myEquipments;
        private string equipmentFilterString;
        public string EquipmentFilterString { get { return equipmentFilterString; } set { equipmentFilterString = value; myEquipments.Refresh(); } }

        // List of all equipment, including currently selected one, as well as a search filter to change selected equipment
        public ObservableCollection<Equipment> ChangeEquipments { get; set; } = Equipment.Equipments;
        public Equipment SelectedChangeEquipment { get; set; }
        private ICollectionView myChangeEquipments;
        private string equipmentChangeFilterString;
        public string EquipmentChangeFilterString { get { return equipmentChangeFilterString; } set { equipmentChangeFilterString = value; myChangeEquipments.Refresh(); } }
        public int EquipmentChangeSelection { get; set; }

        // List of all units, including currently selected one, as well as a search filter
        public ObservableCollection<Unit> Units { get; set; } = Unit.Units;
        public Unit SelectedUnit { get; set; }
        private ICollectionView myUnits { get; set; }
        private string unitFilterString;
        public string UnitFilterString { get { return unitFilterString; } set { unitFilterString = value; myUnits.Refresh(); } }

        // List of all maps and map details
        public List<Map> Maps { get; set; } = Map.Maps;
        public Map SelectedMap { get; set; }
        public List<SpawnEntry> MapEntries { get; set; }
        public SpawnEntry SelectedMapEntry { get; set; }
        public SpawnRule SelectedMapSpawnRule { get; set; }

        // All possible learnable mystic arts
        public List<MysticArts> MysticArtsList { get; set; } = Enum.GetValues(typeof(MysticArts)).Cast<MysticArts>().ToList();
        private MysticArts? selectedMysticArts;
        public MysticArts? SelectedMysticArts
        {
            get { return (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) ? SelectedUnit.PartyTalkSelect.MysticArtLearned : selectedMysticArts;}
            set { if (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) SelectedUnit.PartyTalkSelect.MysticArtLearned = (MysticArts)value;  }
        }

        // All possible learnable item arts
        public List<ItemArts> ItemArtsList { get; set; } = Enum.GetValues(typeof(ItemArts)).Cast<ItemArts>().ToList();
        private ItemArts? selectedItemArts;
        public ItemArts? SelectedItemArts
        {
            get { return (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) ? SelectedUnit.PartyTalkSelect.ItemArtLearned : selectedItemArts; }
            set { if (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) SelectedUnit.PartyTalkSelect.ItemArtLearned = (ItemArts)value; }
        }

        // The selected language
        public List<Language> LanguageList { get; set; } = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
        private Language selectedLanguage = Language.English;
        public Language SelectedLanguage
        {
            get { return selectedLanguage; }
            set { selectedLanguage = value; Files.Language = value; }
        }

        public string CookedPCPath { get; set; } = Files.CookedPCPath;

        /// <summary>
        /// Initializes default views for lists
        /// </summary>
        public MainWindowViewModel()
        {
            myMonsterFormations = CollectionViewSource.GetDefaultView(MonsterFormation.MonsterFormations);
            myMonsterFormations.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            myMonsterFormations.Filter = MonsterFormationFilter;

            myMonsterFormationsChange = CollectionViewSource.GetDefaultView(MonsterFormation.MonsterFormations);
            myMonsterFormationsChange.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            myMonsterFormationsChange.Filter = MonsterFormationChangeFilter;

            myMonsterList = CollectionViewSource.GetDefaultView(Monster.Monsters);
            myMonsterList.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            myMonsterList.Filter = MonsterFilter;

            myEquipments = CollectionViewSource.GetDefaultView(Equipment.Equipments);
            myEquipments.SortDescriptions.Add(new SortDescription("NameString", ListSortDirection.Ascending));
            myEquipments.Filter = EquipmentFilter;

            myChangeEquipments = CollectionViewSource.GetDefaultView(Equipment.Equipments);
            myChangeEquipments.SortDescriptions.Add(new SortDescription("NameString", ListSortDirection.Ascending));
            myChangeEquipments.Filter = EquipmentChangeFilter;

            myUnits = CollectionViewSource.GetDefaultView(Unit.Units);
            myUnits.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            myUnits.Filter = UnitFilter;
        }

        /// <summary>
        /// Change the selected monster in the formation's monter list to the chosen type
        /// </summary>
        /// <param name="obj">Not used</param>
        private void ChangeMonsterType(object obj)
        {
            if (SelectedTypeChangeMonster == null || SelectedMonsterFormation == null)
                return;

            if (SelectedMonsterFormation.MonsterList.Count - 1 < MonsterListSelection || MonsterListSelection < 0)
                return;


            SelectedMonsterFormation.MonsterList[MonsterListSelection] = SelectedTypeChangeMonster;
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        /// <summary>
        /// Changes an equipment of the selected monster
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeSelectedEquipment(object obj)
        {
            if (SelectedChangeEquipment == null || SelectedMonster == null)
                return;

            if (EquipmentChangeSelection == 2)
            {
                SelectedMonster.Equipment1Id = SelectedChangeEquipment.Id;
                SelectedMonster.EquipmentMain = $"[{SelectedChangeEquipment.Id}] {SelectedChangeEquipment.NameString}";
            }
            else if (EquipmentChangeSelection == 3)
            {
                SelectedMonster.Equipment2Id = SelectedChangeEquipment.Id;
                SelectedMonster.EquipmentOff = $"[{SelectedChangeEquipment.Id}] {SelectedChangeEquipment.NameString}";
            }
        }

        /// <summary>
        /// Changes the selected monster formation for the currently selected map spawn entry
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeSelectedMonsterFormation(object obj)
        {
            if (SelectedMonsterFormationChange == null || SelectedMapEntry == null)
                return;

            SelectedMapEntry.NameId = SelectedMonsterFormationChange.Id;
            SelectedMapEntry.Name = SelectedMonsterFormationChange.ToString();
        }

        /// <summary>
        /// Used to filter the monster formation list.
        /// </summary>
        /// <param name="obj">string containing the search term; either name part or id</param>
        /// <returns></returns>
        private bool MonsterFormationFilter(object obj)
        {
            var monsterFormation = obj as MonsterFormation;
            if (monsterFormationFilterString == null || monsterFormation == null)
                return true;

            var id = 0;
            var i = int.TryParse(monsterFormationFilterString, out id);

            if (i)
                return monsterFormation.Id == id;

            return monsterFormation.Name.ToLower().Contains(monsterFormationFilterString.ToLower());
        }

        /// <summary>
        /// Used to filter the monster formation list for the change monster formation view.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool MonsterFormationChangeFilter(object obj)
        {
            var monsterFormation = obj as MonsterFormation;
            if (monsterFormationFilterStringChange == null || monsterFormation == null)
                return true;

            var id = 0;
            var i = int.TryParse(monsterFormationFilterStringChange, out id);

            if (i)
                return monsterFormation.Id == id;

            return monsterFormation.Name.ToLower().Contains(monsterFormationFilterStringChange.ToLower());
        }

        /// <summary>
        /// Used to filter the monster list
        /// </summary>
        /// <param name="obj">string containing the search term; either name part or id</param>
        /// <returns></returns>
        private bool MonsterFilter(object obj)
        {
            var monster = obj as Monster;
            if (monsterFilterString == null || monster == null)
                return true;

            var id = 0;
            var i = int.TryParse(monsterFilterString, out id);

            if (i)
                return monster.Id == id;

            return monster.Name.ToLower().Contains(monsterFilterString.ToLower());
        }

        /// <summary>
        /// Used to filter the equipment list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool EquipmentFilter(object obj)
        {
            var equipment = obj as Equipment;
            if (equipmentFilterString == null || equipment == null)
                return true;

            return equipment.NameString.ToLower().Contains(equipmentFilterString.ToLower());
        }

        /// <summary>
        /// Used to filter the equipmentlist for the change equipment view
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool EquipmentChangeFilter(object obj)
        {
            var equipment = obj as Equipment;
            if (equipmentChangeFilterString == null || equipment == null)
                return true;

            return equipment.NameString.ToLower().Contains(equipmentChangeFilterString.ToLower());
        }

        /// <summary>
        /// Used to filter the unit list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool UnitFilter(object obj)
        {
            var unit = obj as Unit;
            if (unitFilterString == null || unit == null)
                return true;

            return unit.Name.ToLower().Contains(unitFilterString.ToLower());
        }

        /// <summary>
        /// Add a new (default) monster to the currently selected monster formation.
        /// Only works when containing less than 5 monster
        /// </summary>
        /// <param name="obj"></param>
        private void AddMonsterToFormation(object obj)
        {
            if (SelectedMonsterFormation == null || SelectedMonsterFormation.MonsterList.Count >= 5)
                return;

            SelectedMonsterFormation.MonsterList.Add(Monster.Monsters[0]);
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        /// <summary>
        /// Removes the currently selected monster from the currently selected monster formation.
        /// Only works when a monster is selected, and it is not the first one.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveMonsterFromFormation(object obj)
        {
            if (SelectedMonsterFormation == null || SelectedMonsterFormation.MonsterList.Count == 1 || MonsterListSelection < 1)
                return;

            SelectedMonsterFormation.MonsterList.RemoveAt(MonsterListSelection);
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        /// <summary>
        /// Change the path to the tlr data files
        /// </summary>
        /// <param name="obj"></param>
        private void EnterNewPath(object obj)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "Select the The Last Remnant folder.\nUsually found at \n..\\Steam\\SteamApps\\common\\The Last Remnant" })
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Files.ReadBasicInfo(Path.Combine(dialog.SelectedPath, "RushGame/CookedPC/"));
                    Files.Language = Files.Language;
                    CookedPCPath = Files.CookedPCPath;
                }
            }
        }

        /// <summary>
        /// Reverts the battle upk to its backup state
        /// </summary>
        private static void RestoreBattleDataExecute()
        {
            if (!File.Exists(Files.BattleFile))
                return;

            try
            {
                if (File.Exists(Files.BattleFile + ".backup"))
                    File.Copy(Files.BattleFile + ".backup", Files.BattleFile, true);
                Files.Language = Files.Language;
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

        /// <summary>
        /// Reverts all map files to their backup states
        /// </summary>
        private static void RestoreMapDataExecute()
        {
            if (!File.Exists(Files.BattleFile))
                return;

            try
            {
                foreach (var entry in Files.MapFiles)
                {
                    if (File.Exists(entry.MapFile + ".backup"))
                        File.Copy(entry.MapFile + ".backup", entry.MapFile, true);
                }
                Files.Language = Files.Language;
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

        /// <summary>
        /// Randomize all enemy stats
        /// </summary>
        private static void RandomizeAllEnemyStats()
        {
            /// TODO: possibly move to Monster.cs and MonsterFormation.cs

            if (Monster.Monsters.Count == 0)
                return;

            var r = new Random();
            var maxHP = 40000;
            var mapAP = 2000;
            byte maxStat = 255;
            byte maxDefense = 100;
            byte maxBR = 255;

            try
            {
                /// Change the raw bytes themselves all at once, otherwise there would be
                /// hundreds of thousands of costly disk accesses
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
            catch (IOException x)
            {
                MessageBox.Show("Could not write to the game files. Make sure the game isn't running and that the files can be overwritten.");
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }

        }

        /// <summary>
        /// Randomize all unit stats
        /// </summary>
        private static void RandomizeAllUnitArts()
        {
            /// TODO: changes should be written directly insides the byte array.
            ///       Until then, first check whether we have access to the files:
            /// TOTO: Possibly move to Unit.cs

            if (Unit.Units.Count == 0)
                return;

            FileStream stream = null;
            try
            {
                stream = File.Open(Files.BattleFile, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (IOException)
            {
                MessageBox.Show("Could not write to the game files. Make sure the game isn't running and that the files can be overwritten.");
                return;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
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
