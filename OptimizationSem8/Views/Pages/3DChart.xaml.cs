using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChartDirector;
using OptimizationSem8.ViewModels.PagesVievModels;

namespace OptimizationSem8.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для _3DChart.xaml
    /// </summary>
    public partial class _3DChart : Page
    {
        private double m_elevationAngle;
        private double m_rotationAngle;
        public byte[] ChartImageBytes;

        private List<double> dataX = new();
        private List<double> dataY = new();
        private List<double> dataZ = new();

        private int m_lastMouseX;
        private int m_lastMouseY;
        private bool m_isDragging;
        private readonly VisualizationViewModel visualizationViewModel;

        public _3DChart(VisualizationViewModel visualizationViewModel)
        {
            InitializeComponent();


            m_elevationAngle = 30;
            m_rotationAngle = 45;

            m_isDragging = false;
            m_lastMouseX = -1;
            m_lastMouseY = -1;

           
            this.visualizationViewModel = visualizationViewModel;
        }
        
        private void WPFChartViewer1_ViewPortChanged(object sender, WPFViewPortEventArgs e)
        {
            if (e.NeedUpdateChart)
                drawChart((WPFChartViewer)sender);
        }

        public void drawChart(WPFChartViewer viewer)
        {
            SurfaceChart c = new SurfaceChart(600, 600);

            c.setPlotRegion(330, 290, 360, 360, 270);

            c.setData(dataX.ToArray(), dataY.ToArray(), dataZ.ToArray());

            c.setInterpolation(80, 80);

            c.setViewAngle(m_elevationAngle, m_rotationAngle);

            if (m_isDragging)
                c.setShadingMode(Chart.RectangularFrame);

            c.setColorAxis(650, 270, Chart.Left, 200, Chart.Right);
            c.xAxis().setTitle("T1", "Arial Bold", 15);
            c.yAxis().setTitle("T2", "Arial Bold", 15);
            c.xAxis().setLabelStyle("Arial", 10);
            c.yAxis().setLabelStyle("Arial", 10);
            c.zAxis().setLabelStyle("Arial", 10);
            c.colorAxis().setLabelStyle("Arial", 10);


            viewer.Chart = c;
        }


        private void WPFChartViewer1_MouseMoveChart(object sender, MouseEventArgs e)
        {
            int mouseX = WPFChartViewer1.ChartMouseX;
            int mouseY = WPFChartViewer1.ChartMouseY;


            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (m_isDragging)
                {
                    
                    m_rotationAngle += (m_lastMouseX - mouseX) * 90.0 / 360;
                    m_elevationAngle += (mouseY - m_lastMouseY) * 90.0 / 270;
                    WPFChartViewer1.updateViewPort(true, false);
                }


                m_lastMouseX = mouseX;
                m_lastMouseY = mouseY;
                m_isDragging = true;
            }
        }

        private void WPFChartViewer1_MouseUpChart(object sender, MouseEventArgs e)
        {
            m_isDragging = false;
            WPFChartViewer1.updateViewPort(true, false);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var data = visualizationViewModel.ParseData();
            dataX = data.DataX;
            dataY = data.DataY;
            dataZ = data.DataZ;
            // Draw the chart
            WPFChartViewer1.updateViewPort(true, false);
        }
    }
}
