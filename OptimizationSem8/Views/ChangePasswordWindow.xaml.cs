using System.Windows;

namespace OptimizationSem8.Views
{
    /// <summary>
    /// Диалоговое окно для изменения пароля пользователя.
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Новый пароль, введенный пользователем.
        /// </summary>
        public string NewPassword { get; private set; }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить".
        /// Проверяет, совпадают ли новый пароль и его подтверждение, и закрывает окно с результатом true.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новый пароль и подтверждение пароля не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewPassword = NewPasswordBox.Password;

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