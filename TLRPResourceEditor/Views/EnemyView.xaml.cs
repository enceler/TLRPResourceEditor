using System.Windows;
using System.Windows.Controls;

namespace TLRPResourceEditor.Views
{
    /// <summary>
    /// Interaktionslogik für EnemyView.xaml
    /// </summary>
    public partial class EnemyView
    {
        public EnemyView()
        {
            InitializeComponent();
        }

        private void EquipmentChangeMainhandFlyoutOpen(object sender, RoutedEventArgs e)
        {
            EquipChangeMain.IsOpen = true;
            EquipChangeOff.IsOpen = false;
        }

        private void EquipmentChangeOffhandFlyoutOpen(object sender, RoutedEventArgs e)
        {
            EquipChangeOff.IsOpen = true;
            EquipChangeMain.IsOpen = false;
        }

        private void MonsterTypeFlyoutOpen(object sender, RoutedEventArgs e)
        {
            MonsterChange.IsOpen = true;
        }

        private void FlyoutDone(object sender, RoutedEventArgs e)
        {
            MonsterChange.IsOpen = false;
            EquipChangeMain.IsOpen = false;
            EquipChangeOff.IsOpen = false;
        }

        private void CloseFlyout(object sender, SelectionChangedEventArgs e)
        {
            if (MonsterChange != null) MonsterChange.IsOpen = false;
            if (EquipChangeMain != null) EquipChangeMain.IsOpen = false;
            if (EquipChangeOff != null) EquipChangeOff.IsOpen = false;

            OpenMonsterChangeFlyout.IsEnabled = Monsterlist?.SelectedItem != null;
            EquipMain.IsEnabled = Monsterlist?.SelectedItem != null;
            EquipOff.IsEnabled = Monsterlist?.SelectedItem != null;
        }
    }
}
