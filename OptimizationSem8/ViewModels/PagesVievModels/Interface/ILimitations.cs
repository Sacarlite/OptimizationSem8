using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationSem8.ViewModels.PagesVievModels.Interface
{
    public interface ILimitations
    {
        public ((bool terationMode, int itterationCount), double epsilon, int precision) GetLimitation();
    }
}
