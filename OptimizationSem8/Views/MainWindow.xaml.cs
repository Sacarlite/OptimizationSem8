using System.Windows;
using OptimizationSem8.ViewModels;

namespace OptimizationSem8.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler? IsWindowClosing;
        public MainWindow(MainViewModel mainViewModel)
        {
            DataContext = mainViewModel;
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (IsWindowClosing != null)
            {
                IsWindowClosing.Invoke(this, e);
            }
        }
    }
}
