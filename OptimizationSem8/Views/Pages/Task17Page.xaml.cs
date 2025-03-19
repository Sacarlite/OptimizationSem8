using System.Windows.Controls;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Task17Page.xaml
    /// </summary>
    public partial class Task17Page : Page
    {
        public Task17Page(Task17ViewModel task17ViewModel)
        {
            DataContext = task17ViewModel;
            InitializeComponent();
        }
    }
}
