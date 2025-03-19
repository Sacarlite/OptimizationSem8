using System.Windows;
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
