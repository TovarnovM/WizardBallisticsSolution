﻿using OxyPlot.Series;
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
using MiracleGun.IdealPiston;
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
            var pg = new IdealPistonBound();
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
            vm.PM.Series.Add(nodes.GetLineSerries(yAxisFieldName: "p",yScaler:1E-5));
           // vm.PM.Series.Add(nodes.GetLineSerries("e"));
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

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            try {
                var list_fact = solver.Grids[gridname].LayerList.Cast<GasLayer>().Select(gl => {
                    return new {
                        time = gl.Time,
                        vel = gl.RealBoundsRev[0].V
                    };
                }).ToList();
                var list_ideal = list_fact.Select(tl => {
                    return new {
                        time = tl.time,
                        vel = (solver.Grids[gridname] as PnGrid).Get_Vanal(tl.time)
                    };
                }).ToList();
                var list_diff = list_fact.Zip(list_ideal, (f, i) => new {
                    time = f.time,
                    vel = (f.vel - i.vel)/Math.Abs(i.vel)*100
                })
                    .ToList();
                var t_max_analit = 2* (solver.Grids[gridname] as PnGrid).lrInit.L_real/ (solver.Grids[gridname] as PnGrid).lrInit.RealCells[3].CSound;

                vm.PM.Title = $"Максимальное время, пока годна аналитика t = {t_max_analit} c";
                vm.PM.Series.Clear();
                vm.PM.Series.Add(new OxyPlot.Series.LineSeries() {
                    DataFieldX = "time",
                    DataFieldY = "vel",
                    ItemsSource = list_fact,
                    Title = "Насчитанный"
                });
                vm.PM.Series.Add(new OxyPlot.Series.LineSeries() {
                    DataFieldX = "time",
                    DataFieldY = "vel",
                    ItemsSource = list_ideal,
                    Title = "Теоретический"
                });
                vm.PM.Series.Add(new OxyPlot.Series.ScatterSeries() {
                    DataFieldX = "time",
                    DataFieldY = "vel",
                    ItemsSource = list_diff,
                    Title = "Насчитанный - Теоретический, %"
                });
                vm.PM.InvalidatePlot(true);
                vm.PM.ResetAllAxes();
                vm.PM.InvalidatePlot(true);
            }
            catch {
                MessageBox.Show("Errrrrrrorrrrr");
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

        void saveGif(string fp) {
            GifBitmapEncoder gEnc = new GifBitmapEncoder();
            int fi = (int)slider.Value;
            int ti = (int)slider.Maximum;
            int step = Math.Abs(fi - ti) / 100;
            foreach (var bmpImage in GetFrames(fi, ti,step)) {
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

        private void btnGif_click(object sender, RoutedEventArgs e) {
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

        IEnumerable<BitmapSource> GetFrames(int fromInd, int toInd, int step) {
            for (int i = fromInd; i <= toInd; i+= step) {
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
