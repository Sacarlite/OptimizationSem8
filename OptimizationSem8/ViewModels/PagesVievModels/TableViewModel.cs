using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Models.Interface;

namespace OptimizationSem8.ViewModels.PagesVievModels
{
    public partial class TableViewModel : ObservableObject
    {
        private readonly ITask _task;
        private readonly int _precision;
        private readonly double _step;

        [ObservableProperty]
        private ObservableCollection<ObservableCollection<string>> _matrix;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private double _progress;

        public TableViewModel(ITask task, double step, int precision)
        {
            _task = task;
            _precision = precision;
            _step = step;
            Matrix = new ObservableCollection<ObservableCollection<string>>();
            IsLoading = false;
            Progress = 0;
            LoadMatrixAsync();
        }

        private async void LoadMatrixAsync()
        {
            IsLoading = true;
            var tempMatrix = await Task.Run(async () => CreateMatrix());

            await AddDataToUiAsync(tempMatrix);

            IsLoading = false;
        }

        private List<ObservableCollection<string>> CreateMatrix()
        {
            var (firstLower, secondLower) = _task.GetLowerBounds();
            var (firstUpper, secondUpper) = _task.GetUpperBounds();
            var tempMatrix = new List<ObservableCollection<string>>();
            var firstRow = new ObservableCollection<string>() { "-" };
            for (double i = secondLower; i <= secondUpper; i += _step)
            {

                firstRow.Add(
                    $"T2={Math.Round(i, _precision)}"
                    );
            }
            tempMatrix.Add(firstRow);
            for (double i = firstLower; i < firstUpper; i += _step)
            {
                var row = new ObservableCollection<string>();
                row.Add(
                         $"T1={Math.Round(i, _precision)}"
                        );

                for (double j = secondLower; j < secondUpper; j += _step)
                {
                    var point = new FuncPoint(i, j);
                    double funcValue = _task.CalculateObjectiveFunction(point);
                    funcValue = Math.Round(funcValue, _precision);
                    if (_task.CheckSecondOrderConstraints(point))
                    {
                        row.Add(
                            Convert.ToString(funcValue)
                        );
                    }
                    else
                    {
                        row.Add("-");
                    }

                }
                tempMatrix.Add(row);
            }

            return tempMatrix;
        }

        private async Task AddDataToUiAsync(List<ObservableCollection<string>> tempMatrix)
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

                await Task.Delay(500);
            }
        }
    }


}