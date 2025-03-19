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
    public partial class FullSearchLimitationsViewModel : ObservableValidator, ILimitations
    {
        [ObservableProperty]
        private bool iterationMode = false;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Epsilon должен быть больше 0")]
        private double step = 0.1;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue, ErrorMessage = "Количество итераций должно быть больше 0")]
        private int itterationCount = 1000;

        private int _precision = 1;

        public FullSearchLimitationsViewModel()
        {
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

            return ((IterationMode, ItterationCount), Step, _precision);
        }

        partial void OnStepChanged(double value)
        {
            if (Step <= 0)
            {
                _precision = 1;
                return;
            }
            double logStep = Math.Floor(Math.Log10(Math.Abs(Step)));
            _precision = (int)Math.Max(0, -logStep);
        }
    }
}
