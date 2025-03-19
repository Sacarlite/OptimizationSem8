using System.Windows;
using System.Windows.Controls;
using ChartDirector;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для _2DChart.xaml
    /// </summary>
    public partial class _2DChart : Page
    {

        //Data
        private List<double> dataX = new();
        private List<double> dataY = new();
        private List<double> dataZ = new();
        private readonly VisualizationViewModel visualizationViewModel;
        public _2DChart(VisualizationViewModel visualizationViewModel)
        {
            InitializeComponent();
            this.visualizationViewModel = visualizationViewModel;
        }
        private void WPFChartViewer1_ViewPortChanged(object sender, WPFViewPortEventArgs e)
        {
            if (e.NeedUpdateChart)
                drawChart((WPFChartViewer)sender);
        }

        public void drawChart(WPFChartViewer viewer)
        {
            XYChart _chart = new XYChart(600, 600);
            _chart.addTitle("2D тепловая карта", "Arial Italic", 15);
            _chart.setPlotArea(65, 40, 360, 360, -1, -1, -1, unchecked((int)0xc0000000), -1);

            _chart.xAxis().setTitle("T1", "Arial Bold Italic", 10);
            _chart.yAxis().setTitle("T2", "Arial Bold Italic", 10);
            _chart.xAxis().setLabelStyle("Arial Bold");
            _chart.yAxis().setLabelStyle("Arial Bold");
            _chart.xAxis().setColors(Chart.Transparent);
            _chart.yAxis().setColors(Chart.Transparent);

            ContourLayer layer = _chart.addContourLayer(
                dataX.ToArray(),
                dataY.ToArray(),
                dataZ.ToArray()
            );
            _chart.getPlotArea().moveGridBefore(layer);
            ColorAxis cAxis = layer.setColorAxis(245, 455, Chart.TopCenter, 330, Chart.Top);
            cAxis.setBoundingBox(Chart.Transparent, Chart.LineColor);
            cAxis.setTitle("Color Legend Title Place Holder", "Arial Bold Italic", 10);
            cAxis.setLabelStyle("Arial Bold");
            cAxis.setLinearScale(0, 20, 2);
            viewer.Chart = _chart;
            viewer.ImageMap = _chart.getHTMLImageMap(
                "",
                "",
                "title='<*cdml*>X: {x|2}<*br*>Y: {y|2}<*br*>Z: {z|2}'"
            );
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var data = visualizationViewModel.ParseData();
            dataX = data.DataX;
            dataY = data.DataY;
            dataZ = data.DataZ;
            WPFChartViewer1.updateViewPort(true, false);
        }
    }
}
