using System.Windows.Controls;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage(TableViewModel tableViewModel)
        {
            InitializeComponent();
            DataContext = tableViewModel;
        }


    }
}
