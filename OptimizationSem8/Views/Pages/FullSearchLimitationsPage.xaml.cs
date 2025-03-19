using System.Windows.Controls;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FullSearchLimitationsPage.xaml
    /// </summary>
    public partial class FullSearchLimitationsPage : Page
    {
        public FullSearchLimitationsPage(FullSearchLimitationsViewModel fullSearchLimitationsViewModel)
        {
            DataContext = fullSearchLimitationsViewModel;
            InitializeComponent();
        }
    }
}
