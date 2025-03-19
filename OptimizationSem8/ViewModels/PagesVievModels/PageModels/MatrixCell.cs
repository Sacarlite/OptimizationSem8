using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OptimizationSem8.ViewModels.PagesVievModels.PageModels
{
    public partial class MatrixCell : ObservableObject
    {
        [ObservableProperty]
        private string _first;

        [ObservableProperty]
        private string _second;

        [ObservableProperty]
        private string _funcValue;
    }
}
