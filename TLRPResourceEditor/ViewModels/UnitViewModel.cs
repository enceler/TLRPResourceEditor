using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using PropertyChanged;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class UnitViewModel
    {
        public CollectionView UnitView { get; set; }
        public Unit SelectedUnit { get; set; }
        private string _unitFilterString;
        public string UnitFilterString { get { return _unitFilterString; } set { _unitFilterString = value; UnitView.Refresh(); } }

        // All possible learnable mystic arts
        public List<MysticArts> MysticArtsList { get; set; } = Enum.GetValues(typeof(MysticArts)).Cast<MysticArts>().ToList();
        private MysticArts? selectedMysticArts;
        public MysticArts? SelectedMysticArts
        {
            get { return (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) ? SelectedUnit.PartyTalkSelect.MysticArtLearned : selectedMysticArts; }
            set { if (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) SelectedUnit.PartyTalkSelect.MysticArtLearned = (MysticArts)value; }
        }

        // All possible learnable item arts
        public List<ItemArts> ItemArtsList { get; set; } = Enum.GetValues(typeof(ItemArts)).Cast<ItemArts>().ToList();
        private ItemArts? selectedItemArts;
        public ItemArts? SelectedItemArts
        {
            get { return (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) ? SelectedUnit.PartyTalkSelect.ItemArtLearned : selectedItemArts; }
            set { if (SelectedUnit != null && SelectedUnit.PartyTalkSelect != null) SelectedUnit.PartyTalkSelect.ItemArtLearned = (ItemArts)value; }
        }

        public UnitViewModel()
        {
            UnitView = (CollectionView)new CollectionViewSource { Source = Unit.Units}.View;
            UnitView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            UnitView.Filter = UnitFilter;
        }

        private bool UnitFilter(object obj)
        {
            var unit = obj as Unit;
            if (_unitFilterString == null || unit == null)
                return true;

            return unit.Name.ToLower().Contains(_unitFilterString.ToLower());
        }
    }
}
