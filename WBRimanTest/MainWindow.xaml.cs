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
using WizardBallistics.Core;
using WizardBallistics.Draw;
using MiracleGun.IdealGas;
using MiracleGun;
using System.IO;
using OxyPlot.Wpf;
using System.Windows.Threading;

namespace SolverDrawTsts {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public StandartVM vm { get; set; }
        WBSolver solver;
        string solName, gridname;
        public MainWindow() {
            DataContext = this;
            vm = new StandartVM();
            //solver = WBSolver.Factory("RimanTest", WBProjectOptions.Default);
            InitializeComponent();
            var gn = new GasBound();
            FillLB();
        }

        void FillLB() {
            var s = WBSolver.FactoryVariants;
            lb.ItemsSource = s;
            fillStrs(s[0]);
        }
        void fillStrs(string solName) {
            this.solName = solName;
            solver = WBSolver.Factory(solName, WBProjectOptions.Default);
            gridname = solver.Grids.Keys.First();
            SynchSlider();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            
            double timeMax = GetDouble(tb1.Text, 0.5);
            solver.MyStopFunc = slv => slv.TimeCurr >= timeMax;
            solver.RunCalc();
            SynchSlider();


        }

        void SynchSlider() {
            slider.Minimum = 0;
            slider.Maximum = solver.Grids[gridname].LayerList.Count-1;

            slider.Value = slider.Maximum;
        }

        void DrawSituation(IWBNodeLayer lr) {
            var nodes = lr.GetNodesForDraw("").ToList();
            vm.PM.Series.Clear();
            //foreach (var s in nodes[0].GetDataFieldsNames<double>().Where(ss => ss.ToUpper() != "X")) {
            //    vm.PM.Series.Add(nodes.GetLineSerries(s));
            //}
            vm.PM.Series.Add(nodes.GetLineSerries("ro"));
            vm.PM.Series.Add(nodes.GetLineSerries("u"));
            vm.PM.Series.Add(nodes.GetLineSerries("p"));
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
            var lr = solver.Grids[gridname][(int)e.NewValue];
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

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var s = (string)e.AddedItems[0];
            fillStrs(s);
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

        void saveGif(string fp) {
            GifBitmapEncoder gEnc = new GifBitmapEncoder();
            int fi = (int)slider.Value;
            int ti = (int)slider.Maximum;
            foreach (var bmpImage in GetFrames(fi, ti)) {
                //var bmp = bmpImage.GetHbitmap();
                //var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                //    bmp,
                //    IntPtr.Zero,
                //    Int32Rect.Empty,
                //    BitmapSizeOptions.FromEmptyOptions());
                gEnc.Frames.Add(BitmapFrame.Create(bmpImage));
                //DeleteObject(bmp); // recommended, handle memory leak
            }
            using (FileStream fs = new FileStream(fp, FileMode.Create)) {
                gEnc.Save(fs);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            try {
                btnGif.IsEnabled = false;

                var sd = new Microsoft.Win32.SaveFileDialog() {
                    Filter = "GIF Files|*.gif",
                    FileName = "XY"
                };
                if (sd.ShowDialog() == true) {
                    saveGif(sd.FileName);
                }
            } finally {
                btnGif.IsEnabled = true;
            }
        }

        IEnumerable<BitmapSource> GetFrames(int fromInd, int toInd) {
            for (int i = fromInd; i <= toInd; i++) {
                slider.Value = i;
                DoEvents();
                var pngExporter = new PngExporter();
                yield return pngExporter.ExportToBitmap(vm.PM);

            }
        }
        public static void DoEvents() {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
    }
}
