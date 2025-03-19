using System.Windows;
using OptimizationSem8.ViewModels;

namespace OptimizationSem8.Views
{
    /// <summary>
    /// Логика взаимодействия для AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(AboutViewModel aboutViewModel)
        {
            DataContext = aboutViewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutViewModel aboutViewModel)
            {
                MainView.Navigate(aboutViewModel.SourceString);
            };
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
