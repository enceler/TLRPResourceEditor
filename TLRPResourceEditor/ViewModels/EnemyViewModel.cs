using System.ComponentModel;
using System.Windows.Data;
using Commander;
using PropertyChanged;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class EnemyViewModel
    {
        public Monster SelectedMonster           { get; set; }
        public Monster SelectedTypeChangeMonster { get; set; }
        public MonsterFormation SelectedMonsterFormation { get; set; }

        public CollectionView MonsterListView { get; set; }
        public CollectionView MonsterChangeListView { get; set; }
        private string _monsterFormationFilterString;
        private string _monsterChangeFormationFilterString;
        public int MonsterListSelection          { get; set; }
        public string MonsterFormationFilterString { get { return _monsterFormationFilterString; } set { _monsterFormationFilterString = value; MonsterListView.Refresh(); } }
        public string MonsterChangeFormationFilterString { get { return _monsterChangeFormationFilterString; } set { _monsterChangeFormationFilterString = value; MonsterChangeListView.Refresh(); } }

        public Equipment SelectedEquipment { get; set; }
        public CollectionView EquipmentListView { get; set; }
        private string _equipFilterString;
        public string EquipmentFilterStringString { get { return _equipFilterString; } set { _equipFilterString = value;EquipmentListView.Refresh();} }

        public EnemyViewModel()
        {
            Files.enemyViewModel = this;

            MonsterListView = (CollectionView)new CollectionViewSource { Source = MonsterFormation.MonsterFormations }.View;
            MonsterListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MonsterListView.Filter = MonsterFormationFilter;

            MonsterChangeListView = (CollectionView)new CollectionViewSource { Source = Monster.Monsters }.View;
            MonsterChangeListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MonsterChangeListView.Filter = MonsterChangeFormationFilter;

            EquipmentListView = (CollectionView)new CollectionViewSource{Source = Equipment.Equipments}.View;
            EquipmentListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            EquipmentListView.Filter = EquipmentFilter;
        }

        private bool EquipmentFilter(object obj)
        {
            var equipment = obj as Equipment;
            if (_equipFilterString == null || equipment == null)
                return true;

            return equipment.NameString.ToLower().Contains(_equipFilterString.ToLower());
        }

        private bool MonsterFormationFilter(object obj)
        {
            var monsterFormation = obj as MonsterFormation;
            if (_monsterFormationFilterString == null || monsterFormation == null)
                return true;

            int id;
            var isId = int.TryParse(_monsterFormationFilterString, out id);

            if (isId)
                return monsterFormation.Id == id;

            return monsterFormation.Name.ToLower().Contains(_monsterFormationFilterString.ToLower());
        }

        private bool MonsterChangeFormationFilter(object obj)
        {
            var monsterFormation = obj as Monster;
            if (_monsterChangeFormationFilterString == null || monsterFormation == null)
                return true;

            int id;
            var isId = int.TryParse(_monsterChangeFormationFilterString, out id);

            if (isId)
                return monsterFormation.Id == id;

            return monsterFormation.Name.ToLower().Contains(_monsterChangeFormationFilterString.ToLower());
        }

        [OnCommand("ChangeMonster")]
        private void ChangeMonsterType()
        {
            if (SelectedTypeChangeMonster == null || SelectedMonsterFormation == null)
                return;

            if (SelectedMonsterFormation.MonsterList.Count - 1 < MonsterListSelection || MonsterListSelection < 0)
                return;

            SelectedMonsterFormation.MonsterList[MonsterListSelection] = SelectedTypeChangeMonster;
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        [OnCommandCanExecute("ChangeMonster")]
        private bool CanChangeMonsterType()
        {
            if (SelectedTypeChangeMonster == null || SelectedMonsterFormation == null)
                return false;

            if (SelectedMonsterFormation.MonsterList.Count - 1 < MonsterListSelection || MonsterListSelection < 0)
                return false;

            if (SelectedMonster == null)
                return false;

            return true;
        }

        [OnCommand("ChangeEquipmentMain")]
        private void ChangeSelectedEquipment()
        {
            if (SelectedEquipment == null || SelectedMonster == null)
                return;

            SelectedMonster.Equipment1Id = SelectedEquipment.Id;
            SelectedMonster.EquipmentMain = $"[{SelectedEquipment.Id}] {SelectedEquipment.NameString}";
        }

        [OnCommandCanExecute("ChangeEquipmentMain")]
        private bool ChangeSelectedEquipmentCanExecute()
        {
            return SelectedEquipment != null && SelectedMonster != null;
        }

        [OnCommand("ChangeEquipmentOff")]
        private void ChangeSelectedEquipmentOff()
        {
            if (SelectedEquipment == null || SelectedMonster == null)
                return;

            SelectedMonster.Equipment2Id = SelectedEquipment.Id;
            SelectedMonster.EquipmentOff = $"[{SelectedEquipment.Id}] {SelectedEquipment.NameString}";
        }

        [OnCommandCanExecute("ChangeEquipmentOff")]
        private bool ChangeSelectedEquipmentOffCanExecute()
        {
            return SelectedEquipment != null && SelectedMonster != null;
        }

        [OnCommand("AddEnemyToFormation")]
        private void AddMonsterToFormation()
        {
            SelectedMonsterFormation.MonsterList.Add(Monster.Monsters[0]);
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        [OnCommandCanExecute("AddEnemyToFormation")]
        private bool CanAddEnemyToFormation()
        {
            return (SelectedMonsterFormation != null && SelectedMonsterFormation.MonsterList.Count < 5);
        }

        [OnCommand("RemoveEnemyFromFormation")]
        private void RemoveMonsterFromFormation()
        {
            if (SelectedMonsterFormation == null || SelectedMonsterFormation.MonsterList.Count == 1 || MonsterListSelection < 1)
                return;

            SelectedMonsterFormation.MonsterList.RemoveAt(MonsterListSelection);
            SelectedMonsterFormation.WriteNewMonsterList();
        }

        [OnCommandCanExecute("RemoveEnemyFromFormation")]
        private bool CanRemoveMonsterFromFormation()
        {
            return (SelectedMonsterFormation != null && SelectedMonsterFormation.MonsterList.Count > 1 && MonsterListSelection > 0);
        }
    }
}
