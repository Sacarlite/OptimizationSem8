using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OptimizationSem8.ViewModels.PagesVievModels.Interface;

namespace OptimizationSem8.ViewModels.PagesVievModels
{
    public partial class BoxLimitationsViewModel : ObservableValidator, ILimitations
    {
        [ObservableProperty]
        private bool iterationMode = false;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Epsilon должен быть больше 0")]
        private double epsilon = 0.1;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue, ErrorMessage = "Количество итераций должно быть больше 0")]
        private int itterationCount = 1000;

        private int _precision = 1;

        public BoxLimitationsViewModel()
        {
            // Валидация начальных значений
            ValidateAllProperties();
        }

        public ((bool terationMode, int itterationCount), double epsilon, int precision) GetLimitation()
        {
            ClearErrors();


            ValidateAllProperties();


            if (HasErrors)
            {
                throw new ValidationException("Невозможно получить значения из-за ошибок валидации");
            }

            return ((IterationMode, ItterationCount), Epsilon,_precision);
        }
        partial void OnEpsilonChanged(double value)
        {
            if (Epsilon <= 0)
            {
                _precision = 1;
                return;
            }
            double logStep = Math.Floor(Math.Log10(Math.Abs(Epsilon)));
            _precision = (int)Math.Max(0, -logStep);
        }


    }
}
