using System.Windows;
using TLRPResourceEditor.Data;
using TLRPResourceEditor.Models;
using TLRPResourceEditor.ViewModels;

namespace TLRPResourceEditor
{
    /// <summary>
    /// 
    /// TODO: move flyouts into their own view and viewmodels (controls)
    /// TODO: move tab contents into their own view and viewmodels (controls)
    /// </summary>
    public partial class MainWindow
    {
        public MainWindowViewModel _viewmodel;

        public MainWindow()
        {
            _viewmodel = new MainWindowViewModel();
            DataContext = _viewmodel;
            Files.mainwindow = this;
            Files.Language = Data.Language.English;
            InitializeComponent();
        }

        private void EquipmentChangeMainhandFlyoutOpen(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewmodel.EquipmentChangeSelection = 2;
            EquipFlyout.IsOpen = true;
        }

        private void EquipmentChangeOffhandFlyoutOpen(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewmodel.EquipmentChangeSelection = 3;
            EquipFlyout.IsOpen = true;
        }
        private void MonsterFormationFlyoutOpen(object sender, System.Windows.RoutedEventArgs e)
        {
            MonsterFormationChangeFlyout.IsOpen = true;
        }

        private void MonsterTypeFlyoutOpen(object sender, System.Windows.RoutedEventArgs e)
        {
            MonsterChangeFlyout.IsOpen = true;
        }

        private void FlyoutDone(object sender, RoutedEventArgs e)
        {
            CloseFlyout();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CloseFlyout();
        }

        private void CloseFlyout()
        {
            // Damn flyouts don't close on their own even though they should.
            MonsterChangeFlyout.IsOpen = false;
            MonsterFormationChangeFlyout.IsOpen = false;
            EquipFlyout.IsOpen = false;

        }

    }
}
