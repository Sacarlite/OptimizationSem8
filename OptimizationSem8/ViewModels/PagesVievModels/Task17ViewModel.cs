using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Models.Interface;
using OptimizationSem8.Service;

namespace OptimizationSem8.ViewModels.PagesVievModels
{
    public partial class Task17ViewModel : ObservableValidator
    {

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Alpha должен быть больше 0")]
        private double _alpha;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Beta должен быть больше 0")]
        private double _betta;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Mu должен быть больше 0")]
        private double _mu;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Delta должен быть больше 0")]
        private double _delta;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Расход реакционной массы (G) должен быть больше 0")]
        private double _g;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Давление (A) должно быть больше 0")]
        private double _a;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue, ErrorMessage = "Количество теплообменных устройств (N) должно быть больше 0")]
        private int _n;

        [ObservableProperty]
        private bool _extrType = true;

        [ObservableProperty]
        private double _firstLowerBound;

        [ObservableProperty]
        private double _firstUpperBound;

        [ObservableProperty]
        private double _secondLowerBound;

        [ObservableProperty]
        private double _secondUpperBound;

        [ObservableProperty]
        private double _minDiff;

        [ObservableProperty]
        private double _step = 0.1;

        [ObservableProperty]
        private int _precision = 1;

        [ObservableProperty]
        private double tau = 1;
        public Task17ViewModel(TaskVariant17 taskVariant17)
        {
            // Инициализация свойств ViewModel из объекта TaskVariant17
            Alpha = taskVariant17.Alpha;
            Betta = taskVariant17.Beta;
            Mu = taskVariant17.Mu;
            Delta = taskVariant17.Delta;
            G = taskVariant17.G;
            A = taskVariant17.A;
            N = taskVariant17.N;
            MinDiff = taskVariant17.MinDiff;
            FirstLowerBound = taskVariant17.FirstLowerBound;
            FirstUpperBound = taskVariant17.FirstUpperBound;
            SecondLowerBound = taskVariant17.SecondLowerBound;
            SecondUpperBound = taskVariant17.SecondUpperBound;

            ValidateAllProperties();
        }

        public (ITask task, bool extrType, double tau) GetTask()
        {
            ValidateAllProperties();
            // Список для накопления ошибок
            List<string> errors = new List<string>();
            ClearErrors();


            ValidateAllProperties();


            if (HasErrors)
            {
                throw new ValidationException("Невозможно получить значения из-за ошибок валидации");
            }

            if (FirstLowerBound > FirstUpperBound)
            {
                errors.Add("Нижняя граница первой переменной должна быть меньше или равна верхней границе.");
            }

            if (SecondLowerBound > SecondUpperBound)
            {
                errors.Add("Нижняя граница второй переменной должна быть меньше или равна верхней границе.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(ErorrsCombinator.CombinateErorrs(errors));
            }


            var task = new TaskVariant17(
                alpha: Alpha,
                beta: Betta,
                mu: Mu,
                delta: Delta,
                g: G,
                a: A,
                n: N,
                mindiff: MinDiff,
                firstLowerBound: FirstLowerBound,
                firstUpperBound: FirstUpperBound,
                secondLowerBound: SecondLowerBound,
                secondUpperBound: SecondUpperBound
            );

            return (task, ExtrType, Tau);

        }


    }
}