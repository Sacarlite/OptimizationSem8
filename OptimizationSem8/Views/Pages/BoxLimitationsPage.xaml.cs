using System.Windows.Controls;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для BoxLimitationsPage.xaml
    /// </summary>
    public partial class BoxLimitationsPage : Page
    {
        public BoxLimitationsPage(BoxLimitationsViewModel boxLimitationsViewModel)
        {
            DataContext = boxLimitationsViewModel;
            InitializeComponent();
        }
    }
}
