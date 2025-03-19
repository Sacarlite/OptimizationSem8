using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Models.Interface;

namespace OptimizationSem8.Service
{
    public static class TaskFactory
    {
        /// <summary>
        /// Создает задачу на основе данных, загруженных из Excel-файла.
        /// </summary>
        /// <param name="filePath">Путь к Excel-файлу.</param>
        /// <param name="taskType">Тип задачи, который должен быть создан (должен наследоваться от ITask).</param>
        /// <returns>Экземпляр задачи, реализующей ITask.</returns>
        public static ITask CreateTaskFromExcel(string filePath, Type taskType)
        {
            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Тип задачи должен наследоваться от ITask.");
            }

            try
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    var rows = sheetData.Elements<Row>().ToList();

                    // Получаем формулу задачи из первой строки
                    string formula = rows[0].Elements<Cell>().First().CellValue.Text.Replace("Формула задачи: ", "").Trim();

                    // Создаем словарь для параметров
                    var parameters = new Dictionary<string, string>();

                    // Начинаем с третьей строки (первая строка - формула, вторая строка - заголовок "Параметры | Value")
                    for (int i = 2; i < rows.Count; i++)
                    {
                        var cells = rows[i].Elements<Cell>().ToList();
                        if (cells.Count >= 2)
                        {
                            // Получаем имя параметра из первой колонки
                            string paramName = GetCellValue(cells[0], workbookPart);
                            // Получаем значение параметра из второй колонки
                            string paramValue = GetCellValue(cells[1], workbookPart);

                            if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramValue))
                            {
                                parameters[paramName] = paramValue;
                            }
                        }
                    }

                    // Создаем задачу на основе параметров
                    return CreateTaskFromParameters(taskType, formula, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке задачи из Excel-файла: " + ex.Message);
            }
        }

        /// <summary>
        /// Создает задачу на основе данных, загруженных из текстового файла.
        /// </summary>
        /// <param name="filePath">Путь к текстовому файлу.</param>
        /// <param name="taskType">Тип задачи, который должен быть создан (должен наследоваться от ITask).</param>
        /// <returns>Экземпляр задачи, реализующей ITask.</returns>
        public static ITask CreateTaskFromTxt(string filePath, Type taskType)
        {
            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Тип задачи должен наследоваться от ITask.");
            }

            try
            {
                string content = File.ReadAllText(filePath);
                var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string formula = lines[0].Replace("Формула задачи: ", "").Trim();
                var parameters = new Dictionary<string, string>();

                for (int i = 2; i < lines.Length; i++) // Пропускаем первую строку (формула) и вторую (заголовок "Параметры:")
                {
                    var parts = lines[i].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        parameters[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                return CreateTaskFromParameters(taskType, formula, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке задачи из текстового файла: " + ex.Message);
            }
        }

        /// <summary>
        /// Создает задачу на основе типа, формулы и параметров.
        /// </summary>
        /// <param name="taskType">Тип задачи, который должен быть создан.</param>
        /// <param name="formula">Формула задачи.</param>
        /// <param name="parameters">Параметры задачи.</param>
        /// <returns>Экземпляр задачи, реализующей ITask.</returns>
        private static ITask CreateTaskFromParameters(Type taskType, string formula, Dictionary<string, string> parameters)
        {
            // Получаем конструктор задачи
            var constructor = taskType.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                throw new Exception($"Тип {taskType.Name} не имеет публичного конструктора.");
            }

            // Получаем параметры конструктора
            var constructorParameters = constructor.GetParameters();

            // Создаем массив значений для передачи в конструктор
            var parameterValues = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                var paramInfo = constructorParameters[i];
                string paramName = paramInfo.Name; // Имя параметра в конструкторе

                // Ищем параметр в словаре, игнорируя регистр
                var matchingParameter = parameters.FirstOrDefault(p =>
                    p.Key.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                if (matchingParameter.Key != null)
                {
                    // Преобразуем значение из строки в нужный тип
                    parameterValues[i] = Convert.ChangeType(matchingParameter.Value, paramInfo.ParameterType);
                }
                else
                {
                    // Если параметр не найден, используем значение по умолчанию
                    parameterValues[i] = paramInfo.HasDefaultValue ? paramInfo.DefaultValue : throw new Exception($"Параметр {paramName} не найден.");
                }
            }

            // Создаем экземпляр задачи
            var task = (ITask)Activator.CreateInstance(taskType, parameterValues);

            // Устанавливаем формулу, если это возможно
            var formulaProperty = taskType.GetProperty("Formula");
            if (formulaProperty != null && formulaProperty.CanWrite)
            {
                formulaProperty.SetValue(task, formula);
            }

            return task;
        }
        /// <summary>
        /// Получает значение ячейки из Excel-файла.
        /// </summary>
        private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            if (cell.CellValue != null)
            {
                string value = cell.CellValue.Text;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    // Если значение является shared string, извлекаем его из таблицы строк
                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    if (stringTable != null)
                    {
                        value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                    }
                }
                return value;
            }
            return string.Empty;
        }
    }
}