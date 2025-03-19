using CommunityToolkit.Mvvm.ComponentModel;
using OptimizationSem8.Service;

namespace OptimizationSem8.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string sourceString = PathService.GetCurentFolderPath("Resources\\About.html");
        public AboutViewModel() { }
    }
}
