using System.Windows;
using System.Windows.Controls;
using Models.DbModels;

namespace OptimizationSem8.Views
{
    /// <summary>
    /// Диалоговое окно для добавления нового пользователя.
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Новый пользователь, созданный в этом окне.
        /// </summary>
        public User NewUser { get; private set; }

        /// <summary>
        /// Конструктор окна.
        /// </summary>


        /// <summary>
        /// Обработчик нажатия кнопки "Добавить".
        /// Создает нового пользователя и закрывает окно с результатом true.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем нового пользователя
            NewUser = new User
            {
                Username = UsernameTextBox.Text,
                PasswordHash = PasswordBox.Password,
                Role = RoleComboBox.SelectedIndex == 0 ? Role.User : Role.Admin
            };

            // Закрываем окно с результатом true
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена".
        /// Закрывает окно с результатом false.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем окно с результатом false
            DialogResult = false;
            Close();
        }
    }

}
