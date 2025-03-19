using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
