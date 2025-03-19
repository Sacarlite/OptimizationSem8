using System.Text;

namespace OptimizationSem8.Service
{
    public static class ErorrsCombinator
    {
        public static string CombinateErorrs(List<string> erorrs)
        {
            StringBuilder stringBuilder = new();
            foreach (string str in erorrs)
            {
                stringBuilder.Append(str);
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }
    }
}
