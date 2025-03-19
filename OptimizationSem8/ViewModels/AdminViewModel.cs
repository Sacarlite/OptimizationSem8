using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Methods;
using Models;
using Models.DbModels;
using OptimizationSem8.DbConnector;
using OptimizationSem8.Service;
using OptimizationSem8.Views;

namespace OptimizationSem8.ViewModels
{
    /// <summary>
    /// ViewModel для окна администратора.
    /// Предоставляет функциональность для управления пользователями: просмотр, сброс пароля, удаление и добавление.
    /// </summary>
    public partial class AdminViewModel : ObservableObject
    {
        private readonly AppDbContext _context;
        /// <summary>
        // Свойство для хранения списка формул
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FormulaModel> _formulas;
        /// <summary>
        // Свойство для хранения списка методов
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MethodModel> _methods;
        /// <summary>
        /// Список пользователей.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<User> _users;

        /// <summary>
        /// Выбранный пользователь.
        /// </summary>
        [ObservableProperty]
        private User _selectedUser;

        /// <summary>
        /// Конструктор ViewModel.
        /// Инициализирует контекст базы данных и загружает пользователей.
        /// </summary>
        public AdminViewModel()
        {
            // Инициализация списка формул
            Formulas = new ObservableCollection<FormulaModel>
            {
                new() { Formula = "S=\\alpha\\cdot G\\cdot ((T_2-\\beta\\cdot A)^2+(\\mu\\cdot e^{(T_1+T_2)})^N+\\Delta\\cdot(T_2-T_1))" }
            };
            // Инициализация списка формул
            Methods = new ObservableCollection<MethodModel>
            {
                new BoxComplexMethod(),
                new FullSearchMethod()

            };
            _context = new AppDbContext();
            LoadUsers();
        }

        /// <summary>
        /// Загружает пользователей из базы данных.
        /// </summary>
        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_context.Users.ToList());
        }

        /// <summary>
        /// Команда для сброса пароля выбранного пользователя.
        /// </summary>
        [RelayCommand]
        private void ResetPassword()
        {
            if (SelectedUser != null)
            {
                // Генерируем случайный пароль
                var newPassword = GenerationService.GenerateRandomPassword();
                SelectedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.SaveChanges();
                LoadUsers();
                MessageBox.Show($"Пароль успешно сброшен. Новый пароль для входа: {newPassword}", "Сброс пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для сброса пароля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Команда для удаления выбранного пользователя.
        /// </summary>
        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                _context.Users.Remove(SelectedUser);
                _context.SaveChanges();
                LoadUsers();
                MessageBox.Show("Пользователь успешно удален.", "Удаление пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Команда для добавления нового пользователя.
        /// Открывает диалоговое окно для ввода данных нового пользователя.
        /// </summary>
        [RelayCommand]
        private void AddUser()
        {
            var addUserDialog = new AddUserWindow();
            if (addUserDialog.ShowDialog() == true)
            {
                var newUser = addUserDialog.NewUser;
                if (newUser != null)
                {
                    // Хэшируем пароль перед сохранением
                    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    LoadUsers();
                    MessageBox.Show("Пользователь успешно добавлен.", "Добавление пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        // Команда для добавления метода
        [RelayCommand]
        private void AddMethod()
        {
            MessageBox.Show("Функционал добавления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Команда для удаления метода
        [RelayCommand]
        private void DeleteMethod()
        {
            MessageBox.Show("Функционал удаления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Команда для добавления функции
        [RelayCommand]
        private void AddFunction()
        {
            MessageBox.Show("Функционал добавления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Команда для удаления функции
        [RelayCommand]
        private void DeleteFunction()
        {
            MessageBox.Show("Функционал удаления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
