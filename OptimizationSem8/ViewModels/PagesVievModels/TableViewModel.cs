using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Interface;
using Models;
using OptimizationSem8.ViewModels.PagesVievModels.PageModels;

namespace OptimizationSem8.ViewModels.PagesVievModels
{
    public partial class TableViewModel : ObservableObject
    {
        private readonly ITask _task;
        private readonly int _precision;
        private readonly double _step;

        [ObservableProperty]
        private ObservableCollection<ObservableCollection<MatrixCell>> _matrix;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private double _progress;

        public TableViewModel(ITask task, double step, int precision)
        {
            _task = task;
            _precision = precision;
            _step = step;
            Matrix = new ObservableCollection<ObservableCollection<MatrixCell>>();
            IsLoading = false;
            Progress = 0;
            LoadMatrixAsync();
        }

        private async void LoadMatrixAsync()
        {
            IsLoading = true;

            // Запускаем создание данных в фоновом потоке
            var tempMatrix = await Task.Run(async () => CreateMatrix());

            // Добавляем данные в UI-поток постепенно с задержкой
            await AddDataToUiAsync(tempMatrix);

            IsLoading = false;
        }

        private List<ObservableCollection<MatrixCell>> CreateMatrix()
        {
            var (firstLower, secondLower) = _task.GetLowerBounds();
            var (firstUpper, secondUpper) = _task.GetUpperBounds();
            var tempMatrix = new List<ObservableCollection<MatrixCell>>();

            for (double i = firstLower; i < firstUpper; i += _step)
            {
                var row = new ObservableCollection<MatrixCell>();
                for (double j = secondLower; j < secondUpper; j += _step)
                {
                    var point = new FuncPoint(i, j);
                    double funcValue = _task.CalculateObjectiveFunction(point);
                    funcValue = Math.Round(funcValue, _precision);
                    if (_task.CheckSecondOrderConstraints(point))
                    {
                        row.Add(new MatrixCell
                        {
                            FuncValue = Convert.ToString( funcValue),
                        });
                    }
                    else
                    {
                        row.Add(new MatrixCell
                        {
                            FuncValue = "-",
                        });
                    }
                    
                }
                tempMatrix.Add(row);
            }

            return tempMatrix;
        }

        private async Task AddDataToUiAsync(List<ObservableCollection<MatrixCell>> tempMatrix)
        {
            double totalRows = tempMatrix.Count;
            double processedRows = 0;
            int batchSize = 2; 

            for (int i = 0; i < tempMatrix.Count; i += batchSize)
            {
                var batch = tempMatrix.Skip(i).Take(batchSize).ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var row in batch)
                    {
                        Matrix.Add(row);
                    }
                });

                processedRows += batch.Count;
                Progress = (processedRows / totalRows) * 100;

                //// Задержка для предотвращения блокировки UI
                await Task.Delay(500);
            }
        }
    }

    
}