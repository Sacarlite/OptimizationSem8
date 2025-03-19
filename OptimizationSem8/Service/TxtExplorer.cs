using System.IO;
using System.Text;
using System.Windows;
using Models;
using Models.Interface;

namespace OptimizationSem8.Service
{
    public static class TxtExplorer
    {
        /// <summary>
        /// Экспортирует параметры задачи и результат в файл в текстовый файл.
        /// </summary>
        public static async Task ExportResultsToTxtAsync(FuncPoint funcPoint, ITask task, string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Создаем или перезаписываем текстовый файл
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        // Записываем формулу задачи
                        string formula = task.GetFormula();
                        writer.WriteLine("Формула задачи:");
                        writer.WriteLine(formula);
                        writer.WriteLine();

                        // Записываем параметры задачи
                        string parametersString = task.GetParametersString();
                        string[] parameters = parametersString.Split(new[] { ", " }, StringSplitOptions.None);

                        writer.WriteLine("Параметры задачи:");
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(": ");
                            if (parts.Length == 2)
                            {
                                writer.WriteLine($"{parts[0]}: {parts[1]}");
                            }
                        }
                        writer.WriteLine();

                        // Записываем точку оптимизации
                        writer.WriteLine("Оптимальная точка:");
                        writer.WriteLine($"First={funcPoint.First}");
                        writer.WriteLine($"Second={funcPoint.Second}");
                        writer.WriteLine($"FuncNum={funcPoint.FuncNum}");
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Результаты успешно экспортированы в файл: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка при экспорте результатов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }
        /// <summary>
        /// Экспортирует формулу и параметры задачи в текстовый файл.
        /// </summary>
        public static async Task ExportParametersToTxtAsync(ITask task, string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Получаем формулу и параметры
                    string formula = task.GetFormula();
                    string parametersString = task.GetParametersString();

                    // Формируем содержимое файла
                    string content = $"Формула задачи: {formula}\n\nПараметры:\n";
                    content += parametersString.Replace(", ", "\n");

                    // Записываем в файл
                    File.WriteAllText(filePath, content);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Формула и параметры успешно экспортированы в файл: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }
    }
}
