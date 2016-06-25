using System.ComponentModel;
using System.Windows.Data;
using PropertyChanged;
using TLRPResourceEditor.Models;

namespace TLRPResourceEditor.ViewModels
{
    [ImplementPropertyChanged]
    class EquipmentViewModel
    {
        public Equipment SelectedEquipment { get; set; }
        public CollectionView EquipmentListView { get; set; }
        private string _equipFilterString;
        public string EquipmentFilterStringString { get { return _equipFilterString; } set { _equipFilterString = value; EquipmentListView.Refresh(); } }

        public EquipmentViewModel()
        {
            EquipmentListView = (CollectionView)new CollectionViewSource { Source = Equipment.Equipments }.View;
            EquipmentListView.SortDescriptions.Add(new SortDescription("NameString", ListSortDirection.Ascending));
            EquipmentListView.Filter = EquipmentFilter;
        }

        private bool EquipmentFilter(object obj)
        {
            var equipment = obj as Equipment;
            if (_equipFilterString == null || equipment == null)
                return true;

            return equipment.NameString.ToLower().Contains(_equipFilterString.ToLower());
        }
    }
}
