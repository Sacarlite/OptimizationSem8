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
    /// Предоставляет функциональность для управления пользователями, методами, формулами и изменения пароля администратора.
    /// </summary>
    public partial class AdminViewModel : ObservableObject
    {
        private readonly AppDbContext _context;

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
        /// Текущий администратор (пользователь, который вошел в систему).
        /// </summary>
        [ObservableProperty]
        private User _currentAdmin;

        /// <summary>
        /// Список формул.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FormulaModel> _formulas;

        /// <summary>
        /// Список методов.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MethodModel> _methods;

        /// <summary>
        /// Конструктор ViewModel.
        /// Инициализирует контекст базы данных, загружает пользователей, методы и формулы.
        /// </summary>
        public AdminViewModel(User currentAdmin)
        {
            _context = new AppDbContext();
            CurrentAdmin = currentAdmin; // Устанавливаем текущего администратора
            LoadUsers();
            InitializeFormulas();
            InitializeMethods();
        }

        /// <summary>
        /// Загружает пользователей из базы данных.
        /// </summary>
        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_context.Users.ToList());
        }

        /// <summary>
        /// Инициализирует список формул.
        /// </summary>
        private void InitializeFormulas()
        {
            Formulas = new ObservableCollection<FormulaModel>
            {
                new() { Formula = "S=\\alpha\\cdot G\\cdot ((T_2-\\beta\\cdot A)^2+(\\mu\\cdot e^{(T_1+T_2)})^N+\\Delta\\cdot(T_2-T_1))" }
            };
        }

        /// <summary>
        /// Инициализирует список методов.
        /// </summary>
        private void InitializeMethods()
        {
            Methods = new ObservableCollection<MethodModel>
            {
                new BoxComplexMethod(),
                new FullSearchMethod()
            };
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
        /// Администратор не может удалить самого себя.
        /// </summary>
        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                // Проверяем, не пытается ли администратор удалить самого себя
                if (SelectedUser.Id == CurrentAdmin.Id)
                {
                    MessageBox.Show("Вы не можете удалить самого себя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Удаляем пользователя
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

        /// <summary>
        /// Команда для изменения пароля текущего администратора.
        /// </summary>
        [RelayCommand]
        private void ChangePassword()
        {
            var changePasswordWindow = new ChangePasswordWindow();
            if (changePasswordWindow.ShowDialog() == true)
            {


                // Обновляем пароль администратора
                CurrentAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordWindow.NewPassword);
                _context.SaveChanges();
                MessageBox.Show("Пароль успешно изменен.", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Команда для добавления метода.
        /// </summary>
        [RelayCommand]
        private void AddMethod()
        {
            MessageBox.Show("Функционал добавления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Команда для удаления метода.
        /// </summary>
        [RelayCommand]
        private void DeleteMethod()
        {
            MessageBox.Show("Функционал удаления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Команда для добавления функции.
        /// </summary>
        [RelayCommand]
        private void AddFunction()
        {
            MessageBox.Show("Функционал добавления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Команда для удаления функции.
        /// </summary>
        [RelayCommand]
        private void DeleteFunction()
        {
            MessageBox.Show("Функционал удаления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}