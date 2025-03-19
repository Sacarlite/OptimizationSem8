using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using OptimizationSem8.ViewModels.PagesVievModels.PageModels;
using System.Windows;
using Models.Interface;
using DocumentFormat.OpenXml.Drawing;
using System.IO;
using Models;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace OptimizationSem8.Service
{
    public static class ExcelExplorer
    {
        /// <summary>
        /// Экспортирует формулу, параметры задачи, точку оптимизации и изображения графиков в файл Excel.
        /// </summary>
        /// <param name="funcPoint">Точка, найденная в результате оптимизации.</param>
        /// <param name="task">Задание, реализующее интерфейс ITask.</param>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="sheetName">Имя листа в Excel.</param>
        public static async Task ExportResultsToExcelAsync(FuncPoint funcPoint, ITask task, string filePath, string sheetName = "FormulaAndParameters")
        {
            await Task.Run(() =>
            {
                try
                {
                    // Создаем Excel-документ
                    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        // Записываем формулу задачи
                        string formula = task.GetFormula();
                        Row formulaRow = new Row() { RowIndex = 1U };
                        Cell formulaLabelCell = new Cell()
                        {
                            CellReference = "A1",
                            DataType = CellValues.String
                        };
                        formulaLabelCell.Append(new CellValue("Формула задачи:"));
                        Cell formulaCell = new Cell()
                        {
                            CellReference = "B1",
                            DataType = CellValues.String
                        };
                        formulaCell.Append(new CellValue(formula));
                        formulaRow.Append(formulaLabelCell);
                        formulaRow.Append(formulaCell);
                        sheetData.Append(formulaRow);

                        // Записываем параметры задачи
                        string parametersString = task.GetParametersString();
                        string[] parameters = parametersString.Split(new[] { ", " }, StringSplitOptions.None);

                        Row parametersHeaderRow = new Row() { RowIndex = 3U };
                        Cell parametersHeaderCell = new Cell()
                        {
                            CellReference = "A3",
                            DataType = CellValues.String
                        };
                        parametersHeaderCell.Append(new CellValue("Параметры задачи:"));
                        parametersHeaderRow.Append(parametersHeaderCell);
                        sheetData.Append(parametersHeaderRow);

                        int rowIndex = 4;
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(": ");
                            if (parts.Length == 2)
                            {
                                Row dataRow = new Row() { RowIndex = (uint)rowIndex };
                                Cell nameCell = new Cell()
                                {
                                    CellReference = $"A{rowIndex}",
                                    DataType = CellValues.String
                                };
                                nameCell.Append(new CellValue(parts[0]));
                                dataRow.Append(nameCell);

                                Cell valueCell = new Cell()
                                {
                                    CellReference = $"B{rowIndex}",
                                    DataType = CellValues.String
                                };
                                valueCell.Append(new CellValue(parts[1]));
                                dataRow.Append(valueCell);

                                sheetData.Append(dataRow);
                                rowIndex++;
                            }
                        }

                        // Записываем точку оптимизации
                        Row optimalPointRow = new Row() { RowIndex = (uint)(rowIndex + 1) };
                        Cell optimalPointHeaderCell = new Cell()
                        {
                            CellReference = $"A{rowIndex + 1}",
                            DataType = CellValues.String
                        };
                        optimalPointHeaderCell.Append(new CellValue("Оптимальная точка:"));
                        optimalPointRow.Append(optimalPointHeaderCell);

                        // Записываем First
                        Cell firstLabelCell = new Cell()
                        {
                            CellReference = $"A{rowIndex + 2}",
                            DataType = CellValues.String
                        };
                        firstLabelCell.Append(new CellValue("First="));
                        Cell firstValueCell = new Cell()
                        {
                            CellReference = $"B{rowIndex + 2}",
                            DataType = CellValues.Number
                        };
                        firstValueCell.Append(new CellValue(funcPoint.First.ToString()));

                        // Записываем Second
                        Cell secondLabelCell = new Cell()
                        {
                            CellReference = $"C{rowIndex + 2}",
                            DataType = CellValues.String
                        };
                        secondLabelCell.Append(new CellValue("Second="));
                        Cell secondValueCell = new Cell()
                        {
                            CellReference = $"D{rowIndex + 2}",
                            DataType = CellValues.Number
                        };
                        secondValueCell.Append(new CellValue(funcPoint.Second.ToString()));

                        // Записываем FuncNum
                        Cell funcNumLabelCell = new Cell()
                        {
                            CellReference = $"E{rowIndex + 2}",
                            DataType = CellValues.String
                        };
                        funcNumLabelCell.Append(new CellValue("FuncNum="));
                        Cell funcNumValueCell = new Cell()
                        {
                            CellReference = $"F{rowIndex + 2}",
                            DataType = CellValues.Number
                        };
                        funcNumValueCell.Append(new CellValue(funcPoint.FuncNum.ToString()));

                        // Добавляем ячейки в строку
                        Row optimalPointDataRow = new Row() { RowIndex = (uint)(rowIndex + 2) };
                        optimalPointDataRow.Append(firstLabelCell);
                        optimalPointDataRow.Append(firstValueCell);
                        optimalPointDataRow.Append(secondLabelCell);
                        optimalPointDataRow.Append(secondValueCell);
                        optimalPointDataRow.Append(funcNumLabelCell);
                        optimalPointDataRow.Append(funcNumValueCell);

                        sheetData.Append(optimalPointRow);
                        sheetData.Append(optimalPointDataRow);

                        // Сохраняем документ
                        workbookPart.Workbook.Save();
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
        /// Экспортирует формулу и параметры задачи в файл Excel.
        /// </summary>
        public static async Task ExportParametersToExcelAsync(ITask task, string filePath, string sheetName = "FormulaAndParameters")
        {
            await Task.Run(() =>
            {
                try
                {
                    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        // Выводим формулу
                        string formula = task.GetFormula();
                        Row formulaRow = new Row() { RowIndex = 1U };
                        Cell formulaCell = new Cell()
                        {
                            CellReference = "A1",
                            DataType = CellValues.String
                        };
                        formulaCell.Append(new CellValue("Формула задачи: " + formula));
                        formulaRow.Append(formulaCell);
                        sheetData.Append(formulaRow);

                        // Выводим параметры
                        string parametersString = task.GetParametersString();
                        string[] parameters = parametersString.Split(new[] { ", " }, StringSplitOptions.None);

                        // Заголовки таблицы параметров
                        Row headerRow = new Row() { RowIndex = 3U }; // Начинаем с 3-й строки после формулы
                        Cell headerNameCell = new Cell()
                        {
                            CellReference = "A3",
                            DataType = CellValues.String
                        };
                        headerNameCell.Append(new CellValue("Параметры"));
                        headerRow.Append(headerNameCell);

                        Cell headerValueCell = new Cell()
                        {
                            CellReference = "B3",
                            DataType = CellValues.String
                        };
                        headerValueCell.Append(new CellValue("Value"));
                        headerRow.Append(headerValueCell);
                        sheetData.Append(headerRow);

                        // Заполняем данные параметров
                        int rowIndex = 4; // Начинаем с 4-й строки
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(": ");
                            if (parts.Length == 2)
                            {
                                Row dataRow = new Row() { RowIndex = (uint)rowIndex };
                                Cell nameCell = new Cell()
                                {
                                    CellReference = $"A{rowIndex}",
                                    DataType = CellValues.String
                                };
                                nameCell.Append(new CellValue(parts[0]));
                                dataRow.Append(nameCell);

                                Cell valueCell = new Cell()
                                {
                                    CellReference = $"B{rowIndex}",
                                    DataType = CellValues.String
                                };
                                valueCell.Append(new CellValue(parts[1]));
                                dataRow.Append(valueCell);

                                sheetData.Append(dataRow);
                                rowIndex++;
                            }
                        }

                        // Сохраняем документ
                        workbookPart.Workbook.Save();
                    }

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
