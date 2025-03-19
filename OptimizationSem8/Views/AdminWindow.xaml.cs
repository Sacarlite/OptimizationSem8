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
using System.Windows.Shapes;
using OptimizationSem8.ViewModels;

namespace OptimizationSem8.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public event EventHandler? IsWindowClosing;
        public AdminWindow(AdminViewModel adminViewModel)
        {
            DataContext = adminViewModel;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsWindowClosing != null)
            {
                IsWindowClosing.Invoke(this, e);
            }
        }
    }
}
