using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Methods;
using Models;
using Models.Interface;
using OptimizationSem8.Enums;
using OptimizationSem8.Service;
using OptimizationSem8.ViewModels.PagesVievModels;
using OptimizationSem8.ViewModels.PagesVievModels.Interface;
using OptimizationSem8.Views;
using OptimizationSem8.Views.Pages;

namespace OptimizationSem8.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Dictionary<string, Type> _formulas=new Dictionary <string, Type>
            {
                { "S=\\alpha\\cdot G\\cdot ((T_2-\\beta\\cdot A)^2+(\\mu\\cdot e^{(T_1+T_2)})^N+\\Delta\\cdot(T_2-T_1))", typeof(TaskVariant17) }
            };

        [ObservableProperty]
        private Dictionary<string, Type> _methods = new Dictionary<string, Type>
            {
                { "Метод бокса", typeof(BoxComplexMethod) },
            {"Метод сканирования", typeof(FullSearchMethod) }
            };

        [ObservableProperty]
        private Type _selectedFormula;

        [ObservableProperty]
        private Visibility fullScanVisibility=Visibility.Collapsed;

        [ObservableProperty]
        private Type _selectedMethod;

        [ObservableProperty]
        private Page selectedTask;

        [ObservableProperty]
        private Page methodLimitations;

        [ObservableProperty]
        private VisualizationViewModel visualizationViewModel;

        [ObservableProperty]
        private FuncPoint extraNum;

        private object selectedTaskVievModel;

        private ILimitations selectedMethodLimitationVievModel;
        public MainViewModel()
        {
        }

        [RelayCommand]
        private async Task ExportParametersToExel()
        {
            if (selectedTaskVievModel is Task17ViewModel task)
            {
                string filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Параметры_{DateTime.Now:yyyyMMdd_HHmmss}", "Сохранить параметры в Excel");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await ExcelExplorer.ExportParametersToExcelAsync(task.GetTask().task, filePath);
                }
            }
            else
            {
                MessageBox.Show("Задание не было выбрано", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        void OpenAboutWindow()
        {
                var aboutWindow = new AboutWindow(new AboutViewModel());
                aboutWindow.Show();
                aboutWindow.Closing += AboutWindowClosing;
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Hide();
        }

        private void AboutWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.Show();

                mainWindow.Activate();
            }
        }
        
        [RelayCommand]
        private async Task ExportResultsToExel()
        {
            if (selectedTaskVievModel is Task17ViewModel task&& ExtraNum is not null)
            {
                string filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Результаты_{DateTime.Now:yyyyMMdd_HHmmss}", "Сохранить параметры в Excel");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await ExcelExplorer.ExportResultsToExcelAsync(funcPoint:ExtraNum, task:task.GetTask().task, filePath:filePath);
                }
            }
            else
            {
                MessageBox.Show("Задание не было выбрано", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task ExportResultsToTxt()
        {
            if (selectedTaskVievModel is Task17ViewModel task && ExtraNum is not null)
            {

                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Parameters_{DateTime.Now:yyyyMMdd_HHmmss}", "Сохранить параметры в TXT");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await TxtExplorer.ExportResultsToTxtAsync(funcPoint: ExtraNum, task: task.GetTask().task, filePath: filePath);
                }
            }
            else
            {
                MessageBox.Show("Задание не было выбрано", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task ExportParametersToTxt()
        {
            if (selectedTaskVievModel is Task17ViewModel task)
            {
                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Parameters_{DateTime.Now:yyyyMMdd_HHmmss}", "Сохранить параметры в TXT");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await TxtExplorer.ExportParametersToTxtAsync(task.GetTask().task, filePath);
                }
            }
            else
            {
                MessageBox.Show("Задание не было выбрано", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        [RelayCommand]
        private async Task ImportParametersFromExel()
        {
            try
            {
                if (selectedTaskVievModel is Task17ViewModel task)
                {
                    string filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";

                    string filePath = FileProvider.GetFilePath(FileMode.Open, filter, "Выберите файл с параметрами");

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        TaskVariant17 taskFromExcel = (TaskVariant17)Service.TaskFactory.CreateTaskFromExcel(filePath, typeof(TaskVariant17));

                        var viewModel = new Task17ViewModel(taskFromExcel);

                        SelectedTask = new Task17Page(viewModel);
                        selectedTaskVievModel = viewModel;

                        MessageBox.Show("Параметры успешно загружены из Excel-файла.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Задание не было выбрано", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке параметров из Excel-файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task ImportParametersFromTxt()
        {
            try
            {
                if (selectedTaskVievModel is Task17ViewModel task)
                {
                    string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                    string filePath = FileProvider.GetFilePath(FileMode.Open, filter, "Выберите текстовый файл с параметрами");

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        TaskVariant17 taskFromTxt = (TaskVariant17)Service.TaskFactory.CreateTaskFromTxt(filePath, typeof(TaskVariant17));

                        var viewModel = new Task17ViewModel(taskFromTxt);

                        SelectedTask = new Task17Page(viewModel);
                        selectedTaskVievModel = viewModel;

                        MessageBox.Show("Параметры успешно загружены из текстового файла.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Задание не было выбрано", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке параметров из текстового файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        void Calc()
        {
            try
            {
                if (selectedTaskVievModel is Task17ViewModel task && selectedMethodLimitationVievModel is not null)
                {
                    var optTask = task.GetTask();
                    var limitations = selectedMethodLimitationVievModel.GetLimitation();
                    this.VisualizationViewModel = new VisualizationViewModel(optTask.task, limitations);
                    if (SelectedMethod == typeof(BoxComplexMethod))
                    {
                        var boxComplexMethod = new BoxComplexMethod(limitations.Item1, optTask.task, optTask.extrType,epsilon: limitations.epsilon, precision: limitations.precision);
                        ExtraNum = boxComplexMethod.Optimize();
                        ExtraNum.FuncNum *= optTask.tau;
                    }
                    else if (SelectedMethod == typeof(FullSearchMethod))
                    {
                        var fullSearchMethod = new FullSearchMethod(limitations.Item1,optTask.task, maximize: optTask.extrType, step: limitations.epsilon, precision: limitations.precision);
                        ExtraNum = fullSearchMethod.Optimize();
                        ExtraNum.FuncNum *= optTask.tau;
                    }
                }
                else
                {
                    MessageBox.Show("Не выбрана формула для расчётов или метод оптимизации", "Ошибка расчётов", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при расчёте оптимального значения функции:{ex.Message}", "Ошибка расчётов", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        partial void OnSelectedFormulaChanged(Type value)
        {
            if (typeof(TaskVariant17) == value)
            {
                var vievModel = new Task17ViewModel(new TaskVariant17());
                SelectedTask = new Task17Page(vievModel);
                selectedTaskVievModel = vievModel;
            }
        }
        partial void OnSelectedMethodChanged(Type value)
        {
            FullScanVisibility = Visibility.Collapsed;
            if (typeof(BoxComplexMethod) == value)
            {                
                var limitationVievModel = new BoxLimitationsViewModel();
                MethodLimitations = new BoxLimitationsPage(limitationVievModel);
                selectedMethodLimitationVievModel = limitationVievModel;
            }
            else if (typeof(FullSearchMethod) == value)
            {
                FullScanVisibility=Visibility.Visible;
                var limitationVievModel = new FullSearchLimitationsViewModel();
                MethodLimitations = new FullSearchLimitationsPage(limitationVievModel);
                selectedMethodLimitationVievModel = limitationVievModel;
            }
        }
     

    }
}
