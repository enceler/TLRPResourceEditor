using System.Windows;
using System.Windows.Controls;

namespace TLRPResourceEditor.Views
{
    /// <summary>
    /// Interaktionslogik für MapView.xaml
    /// </summary>
    public partial class MapView
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void MonsterFormationFlyoutOpen(object sender, RoutedEventArgs e)
        {
            MonsterFormationChangeFlyout.IsOpen = true;
        }

        private void CloseFlyout(object sender, SelectionChangedEventArgs e)
        {
            MonsterFormationChangeFlyout.IsOpen = false;
            ChangeFormation.IsEnabled = MapMonsterList.SelectedItem != null;
        }

        private void FlyoutDone(object sender, RoutedEventArgs e)
        {
            MonsterFormationChangeFlyout.IsOpen = false;
            ChangeFormation.IsEnabled = MapMonsterList.SelectedItem != null;
        }
    }
}
