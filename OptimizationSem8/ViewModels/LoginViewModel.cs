using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.DbModels;
using OptimizationSem8.Service;
using OptimizationSem8.Views;

namespace OptimizationSem8.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }
        [RelayCommand]
        private void Login()
        {
            var user = _authService.Login(Username, Password);
            if (user != null)
            {
                if (user.Role == Role.User)
                {
                    var mainWindow = new MainWindow(new MainViewModel(user));
                    mainWindow.IsWindowClosing += IsWindowClosing;
                    mainWindow.Show();
                }
                else
                {
                    var adminWindow = new AdminWindow(new AdminViewModel(user));
                    adminWindow.IsWindowClosing += IsWindowClosing;
                    adminWindow.Show();
                }
                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Hide();
            }
            else
            {
                MessageBox.Show("Некорректное имя пользователя или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IsWindowClosing(object? sender, EventArgs e)
        {
            var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();

            if (loginWindow != null)
            {
                loginWindow.Show();

                loginWindow.Activate();
            }
        }
    }
}