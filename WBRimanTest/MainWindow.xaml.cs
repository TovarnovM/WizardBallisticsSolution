using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using WizardBallisticsCore;
using WizardBallisticsDraw;

namespace WBRimanTest {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public StandartVM vm { get; set; }
        WBSolver solver;
        public MainWindow() {
            DataContext = this;
            vm = new StandartVM();
            solver = WBSolver.Factory("RimanTest", WBProjectOptions.Default);
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            solver = WBSolver.Factory("RimanTest", WBProjectOptions.Default);
            double timeMax = GetDouble(tb1.Text, 0.5);
            solver.MyStopFunc = slv => slv.TimeCurr >= timeMax;
            solver.RunCalc();
            SynchSlider();


        }

        void SynchSlider() {
            slider.Minimum = 0;
            slider.Maximum = solver.Grids["RimanGrid"].LayerList.Count-1;

            slider.Value = slider.Maximum;
        }

        void DrawSituation(RmLayer lr) {
            var nodes = lr.Nodes;
            vm.PM.Series.Clear();
            vm.PM.Series.Add(nodes.GetLineSerries("ro"));
            vm.PM.Series.Add(nodes.GetLineSerries("p"));
            vm.PM.Series.Add(nodes.GetLineSerries("u"));
            vm.PM.Series.Add(nodes.GetLineSerries("e"));
            vm.PM.Title = $"{lr.Time} sec";
            vm.PM.InvalidatePlot(true);
        }

        public static double GetDouble(string value, double defaultValue = 0d) {

            //Try parsing in the current culture
            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out double result) &&
                //Then try in US english
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result)) {
                result = defaultValue;
            }

            return result;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var lr = ((solver.Grids["RimanGrid"] as RmGrid)[(int)e.NewValue] as RmLayer);
            DrawSituation(lr);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            try {
                var sd = new Microsoft.Win32.SaveFileDialog() {
                    Filter = "json Files|*.json",
                    FileName = "testy"
                };
                if (sd.ShowDialog() == true) {
                    solver.SaveToFile(sd.FileName);
                }


            } finally {

            }
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            try {
                var sd = new Microsoft.Win32.OpenFileDialog() {
                    Filter = "json Files|*.json",
                    FileName = "testy"
                };
                if (sd.ShowDialog() == true) {
                    solver.LoadFromFile(sd.FileName);
                    SynchSlider();
                }


            } finally {

            }
        }
    }
}
