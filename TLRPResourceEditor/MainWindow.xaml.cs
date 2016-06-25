using TLRPResourceEditor.Data;

namespace TLRPResourceEditor
{
    /// <summary>
    /// 
    /// TODO: move flyouts into their own view and viewmodels (controls)
    /// TODO: move tab contents into their own view and viewmodels (controls)
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            Files.Language = Data.Language.English;
            InitializeComponent();
        }


    }
}
