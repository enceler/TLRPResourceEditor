using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Commander;
using PropertyChanged;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class MapViewModel
    {
        public List<Map> Maps { get; set; } = Map.Maps;
        public Map SelectedMap { get; set; }
        public List<SpawnEntry> MapEntries { get; set; }
        public SpawnEntry SelectedMapEntry { get; set; }
        public SpawnRule SelectedMapSpawnRule { get; set; }

        public CollectionView MonsterFormationView { get; set; }
        public MonsterFormation SelectedMonsterFormationChange { get; set; }
        private string _monsterFormationChangeFilter;
        public string MonsterFormationChangeFilterString { get { return _monsterFormationChangeFilter; } set { _monsterFormationChangeFilter = value; MonsterFormationView.Refresh(); } }

        public MapViewModel()
        {
            MonsterFormationView = (CollectionView)new CollectionViewSource { Source = MonsterFormation.MonsterFormations }.View;
            MonsterFormationView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MonsterFormationView.Filter = MonsterFormationChangeFilter;
        }

        private bool MonsterFormationChangeFilter(object obj)
        {
            var monsterFormation = obj as MonsterFormation;
            if (_monsterFormationChangeFilter == null || monsterFormation == null)
                return true;

            int id;
            var isId = int.TryParse(_monsterFormationChangeFilter, out id);

            if (isId)
                return monsterFormation.Id == id;

            return monsterFormation.Name.ToLower().Contains(_monsterFormationChangeFilter.ToLower());
        }

        [OnCommand("ChangeMonsterFormation")]
        private void ChangeSelectedMonsterFormation()
        {
            if (SelectedMonsterFormationChange == null || SelectedMapEntry == null)
                return;

            SelectedMapEntry.NameId = SelectedMonsterFormationChange.Id;
            SelectedMapEntry.Name = SelectedMonsterFormationChange.ToString();
        }

        [OnCommandCanExecute("ChangeMonsterFormation")]
        private bool ChangeSelectedMonsterFormationCanExecute()
        {
            return SelectedMonsterFormationChange != null && SelectedMapEntry != null;
        }

    }
}
