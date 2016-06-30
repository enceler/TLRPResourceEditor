using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Commander;
using PropertyChanged;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class MapSpawnViewModel
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

        public List<int> GroupedSpawnRulesRegular => SelectedMap == null ? null : new List<int>(SelectedMap.OrderedSpawns.Where(p => p.Value.Count > 1).ToDictionary(p => p.Key, p => p.Value).Keys);
        public List<int> GroupedSpawnRulesRare => SelectedMap == null ? null : new List<int>(SelectedMap.OrderedSpawns.Where(p => p.Value.Count == 1).ToDictionary(p => p.Key, p => p.Value).Keys);

        public int SelectedOrderdEntry { get; set; }

        public List<SpawnEntry> GroupedSpawnEntries => SelectedMap != null && SelectedMap.OrderedSpawns.ContainsKey((SelectedOrderdEntry))
            ? SelectedMap.OrderedSpawns[SelectedOrderdEntry]
            : null;
        public string GroupedSpawnRule => SelectedMap != null && SelectedMap.OrderedSpawns.ContainsKey((SelectedOrderdEntry))
            ? SelectedMap.OrderedSpawns[SelectedOrderdEntry][0].SpawnRuleText
            : "";

        public MapSpawnViewModel()
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




        
    }
}
