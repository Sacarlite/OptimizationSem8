namespace OptimizationSem8.ViewModels.PagesVievModels.Interface
{
    public interface ILimitations
    {
        public ((bool terationMode, int itterationCount), double epsilon, int precision) GetLimitation();
    }
}
