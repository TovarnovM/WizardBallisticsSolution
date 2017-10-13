using Interpolator;
using MiracleGun.IdealGas;
using MoreLinq;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Serialization;
using WizardBallistics.Draw;

namespace Bikas_comp1D2D {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public StandartVM vm { get; set; }

        SerializableDictionary<int, AutodynInfo> dict_2318_ai;
        private SerializableDictionary<int, AutodynInfo> dict1d;
        SettingsWindows windowSet;

        public MainWindow() {
            DataContext = this;
            vm = new StandartVM();
            windowSet = new SettingsWindows();
            synchDirs();
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            var oldContent = btn_autodynInit.Content;
            try {
                btn_autodynInit.IsEnabled = false;
                btn_autodynInit.Content = "In process...";
                var aconv = new AutodynConverter();
                dict_2318_ai = await aconv.GetMegaDictAsync(ad_all_dir, @"_2318_");//@"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d",@"_2318_");
                var all_ts = dict_2318_ai.Values.SelectMany(vs => vs.gPress.Values.Select(vv => vv.Data.Keys.ToList()));
                var all_dts = new List<double>();
                foreach (var ts in all_ts) {
                    var dts = ts.Zip(ts.Skip(1), (t0, t1) => t1 - t0).ToList();
                    var dt = dts.Average();
                    all_dts.Add(dt);
                }
                var sss = all_dts.Average();
                var keys = dict_2318_ai.Keys.ToList();
            } finally {
                btn_autodynInit.IsEnabled = true;
                btn_autodynInit.Content = oldContent;
            }
            
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

        BicasStats ad, oneD;
        private string one_mega_dir;
        private string ad_mega_dir;
        private string ad_all_dir;

        private async void btn_autodynInit_fromXml_Click(object sender, RoutedEventArgs e) {
            try {
                btn_autodynInit_fromXml.IsEnabled = false;
                Cursor = Cursors.Wait;
                btn_autodynInit_fromXml.Content = "Loading...";
                await LoadJSONz(sender, e);
                lb.ItemsSource = ad.Keys.OrderBy(k=>k).ToList();
                ad.GetMaxPress(1000);
            } finally {
                btn_autodynInit_fromXml.Content = "Done!";
                Cursor = Cursors.Arrow;
            }

            //int countz = 1;
            //var lst = new List<SerializableDictionary<int, AutodynInfo>>(countz);
            //for (int i = 0; i < countz; i++) {
            //    var cl = new SerializableDictionary<int, AutodynInfo>();
            //    foreach (var d in dict_2318_ai) {
            //        cl.Add(d.Key, d.Value.Copy());
            //    }
            //    lst.Add(cl);
            //}
        }
        public Task LoadJSONz(object sender, RoutedEventArgs e) {
            return Task.Factory.StartNew(() => {
                Parallel.ForEach(new Action[] {
                () => btn_autodynInit_fromXml_Copy1_Click(sender, e),
                () => Button_Click_2(sender, e)
            }, a => a());
                ad = new BicasStats(dict_2318_ai);
                oneD = new BicasStats(dict1d);
            });
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e) {
            dict1d = await Get1DDictAsync(dict_2318_ai);
            var dir = one_mega_dir;//@"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega_1D_.json";
            using (var jsw = new JsonTextWriter(new StreamWriter(dir))) {
                var ser = JsonSerializer.Create();
                ser.Serialize(jsw, dict1d);
            }
            //var opts = new Piston_el_params() {
            //    V0 = 1000,
            //    max_x_elem = 0.24
            //};
            //var p = new Piston_1D();
            //var sol = p.GetSolverElastic(opts);
            //sol.RunCalc();
            //var gr = (BikasGrid)sol.Grids.Values.First();
            //var megaInterp = Piston_1D.GetMegaInterp(gr.LayerList.Cast<GasLayer>(), c => c.u);

            //var gr = (BikasGrid)sol.Grids.Values.First();
            //var oldTimes = gr.LayerList.Select(lr => lr.Time).ToList();
            //var oldDeltas = oldTimes.Zip(oldTimes.Skip(1), (t1, t0) => t1 - t0).ToList();


        }

        public SerializableDictionary<int, AutodynInfo> Get1DDict(SerializableDictionary<int, AutodynInfo> dict) {
            var locker = new object();
            var dict1d = new SerializableDictionary<int, AutodynInfo>();
            
            Parallel.ForEach(
                dict, 
                new ParallelOptions() {
                    MaxDegreeOfParallelism = 7
                }, 
                kw => {
                    var info1d = Get_1D_sol(kw.Value);
                    lock (locker) {
                        dict1d.Add(kw.Key, info1d);
                    }
                });
            return dict1d;
        }

        public Task<SerializableDictionary<int, AutodynInfo>> Get1DDictAsync(SerializableDictionary<int, AutodynInfo> dict) {
            return Task.Factory.StartNew(() => Get1DDict(dict));
        }

        private void btn_autodynInit_fromXml_Copy_Click(object sender, RoutedEventArgs e) {
            var dir = ad_mega_dir;// @"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega.json";
            using (var jsw = new JsonTextWriter(new StreamWriter(dir))) {
                var ser = JsonSerializer.Create();
                ser.Serialize(jsw, dict_2318_ai);
            }
            using (var jstr = new JsonTextReader(new StreamReader(dir))) {

                var ser = JsonSerializer.Create();
                var slc = ser.Deserialize<SerializableDictionary<int, AutodynInfo>>(jstr);
            }
                //foreach (var item in dict_2318_ai) {
                //    item.Value.SaveToFile(dir+$"{item.Key}.json");
                //}
                //var ai = new AutodynInfo();
                //ai.LoadFromFile(dir);
            }

        private void btn_autodynInit_fromXml_Copy1_Click(object sender, RoutedEventArgs e) {
            var dir = ad_mega_dir;//@"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega.json";
            using (var jstr = new JsonTextReader(new StreamReader(dir))) {
                var ser = JsonSerializer.Create();
                dict_2318_ai = ser.Deserialize<SerializableDictionary<int, AutodynInfo>>(jstr);
            }
            foreach (var ai in dict_2318_ai.Values) {
                ai.Reduce();
            }
        }

        public AutodynInfo Get_1D_sol(AutodynInfo info) {

            var opts = new Piston_el_params() {
                V0 = info.vel,
                max_x_elem = 0.295
            };
            var p = new Piston_1D();
            var sol = p.GetSolverElastic(opts);
            sol.RunCalc();
            var gr = (BikasGrid)sol.Grids.Values.First();
            var res = new AutodynInfo() {
                vel = info.vel
            };
            var megaInterp_v = Piston_1D.GetMegaInterp(gr.LayerList.Cast<GasLayer>(), c => c.u);
            var megaInterp_p = Piston_1D.GetMegaInterp(gr.LayerList.Cast<GasLayer>(), c => c.p / 1000);
            var tupV = Piston_1D.GetPoddElV(gr.LayerList.Cast<GasLayer>());

            var gauges = Enumerable.Range(1, 21)
                .Select(i => (name: $"Gauge#  {i}", x_coord: 0.1 + (i - 1) * 0.01))
                .ToList();
            foreach (var g in gauges) {
                res.gVels.Add(g.name, Piston_1D.GetSrez(megaInterp_v, g.x_coord));
            }
            foreach (var g in gauges) {
                res.gPress.Add(g.name, Piston_1D.GetSrez(megaInterp_p, g.x_coord));
            }
            res.Vels.Add("el", tupV.elV);
            res.Vels.Add("podd", tupV.poddV);

            return res;
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (rbV.IsChecked == true)
                DrawVels(e);
            else if (rbP.IsChecked == true)
                DrawPress(e);
            else if (rbP_x.IsChecked == true)
                DrawPress_x(e);
            else if (rbV_x.IsChecked == true)
                DrawVels_x(e);
            else if (rbI_x.IsChecked == true)
                DrawI_x(e);
        }

        private void DrawVels(SelectionChangedEventArgs e) {
            vm.PM.Series.Clear();

            var ad_tup = ad.GetDataAtTime((int)e.AddedItems[0]);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup.el,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - скорость элемента, м/с",
                Color = OxyColors.Red
            });
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup.podd,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - скорость поддона, м/с",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Dash
            });

            var oneD_tup = oneD.GetDataAtTime((int)e.AddedItems[0]);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup.el,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - скорость элемента, м/с",
                Color = OxyColors.Green
            });
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup.podd,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - скорость поддона, м/с",
                Color = OxyColors.Green,
                LineStyle = LineStyle.Dash
            });
            vm.PM.InvalidatePlot(true);
        }
        private void DrawPress(SelectionChangedEventArgs e) {
            vm.PM.Series.Clear();

            var ad_tup = ad.GetMaxPress((int)e.AddedItems[0],10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - Давление, кПа",
                Color = OxyColors.Red
            });


            var oneD_tup = oneD.GetMaxPress((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - Давление, кПа",
                Color = OxyColors.Green
            });

            vm.PM.InvalidatePlot(true);
        }
        private void DrawVels_x(SelectionChangedEventArgs e) {
            vm.PM.Series.Clear();

            var ad_tup = ad.GetMaxVels_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - Давление, м/с",
                Color = OxyColors.Red,
                Smooth = true
            });


            var oneD_tup = oneD.GetMaxVels_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - Скорость, м/с",
                Color = OxyColors.Green,
                Smooth = true
            });

            vm.PM.InvalidatePlot(true);
        }
        private void DrawPress_x(SelectionChangedEventArgs e) {
            vm.PM.Series.Clear();

            var ad_tup = ad.GetMaxPress_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - Давление, кПа",
                Color = OxyColors.Red,
                Smooth = true
            });


            var oneD_tup = oneD.GetMaxPress_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - Давление, кПа",
                Color = OxyColors.Green,
                Smooth = true
            });

            vm.PM.InvalidatePlot(true);
        }
        private void DrawI_x(SelectionChangedEventArgs e) {
            vm.PM.Series.Clear();

            var ad_tup = ad.GetMaxI_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = ad_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "2D - Импульс, кПа*мс",
                Color = OxyColors.Red,
                Smooth = true
            });


            var oneD_tup = oneD.GetMaxI_x((int)e.AddedItems[0], 10000);
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = oneD_tup,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "1D - Импульс, кПа*мс",
                Color = OxyColors.Green,
                Smooth = true
            });

            vm.PM.InvalidatePlot(true);
        }
        private void btn_autodynInit_fromXml_Copy2_Click(object sender, RoutedEventArgs e) {
            var keys = ad.Keys.Except(new int[] { 586, 890, 927 }).OrderBy(k => k).ToList();
            var el_tup = keys
                .Select(k => {
                    var i1 = ad.dict[k].Vels["el"];
                    var i2 = oneD.dict[k].Vels["el"];
                    return (vel:(double)k, tup:BicasStats.GetMaxDiffs(i1, i2));
                })
                .ToList();
            var podd_tup = keys
               .Select(k => {
                   var i1 = ad.dict[k].Vels["podd"];
                   var i2 = oneD.dict[k].Vels["podd"];
                   return (vel: (double)k, tup: BicasStats.GetMaxDiffs(i1, i2));
               })
               .ToList();
            var el_abs = el_tup.Select(t => new DataP() { X = t.vel, Y = t.tup.maxAbsdiff }).ToList();
            var el_perc = el_tup.Select(t => new DataP() { X = t.vel, Y = t.tup.maxPercDiff }).ToList();
            var podd_abs = podd_tup.Select(t => new DataP() { X = t.vel, Y = t.tup.maxAbsdiff }).ToList();
            var podd_perc = podd_tup.Select(t => new DataP() { X = t.vel, Y = t.tup.maxPercDiff }).ToList();

            vm.PM.Series.Clear();

            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = el_abs,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "Максимальное отличие скорости элемента, м/с",
                Color = OxyColors.Red,
                Smooth = true
            });
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = podd_abs,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "Максимальное отличие скорости поддона, м/с",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Dash,
                Smooth = true
            });
            //vm.PM.Series.Add(new LineSeries() {
            //    ItemsSource = el_perc,
            //    DataFieldX = "X",
            //    DataFieldY = "Y",
            //    Title = "Максимальное отличие скорости элемента, %",
            //    Color = OxyColors.Red
            //});
            //vm.PM.Series.Add(new LineSeries() {
            //    ItemsSource = podd_perc,
            //    DataFieldX = "X",
            //    DataFieldY = "Y",
            //    Title = "Максимальное отличие скорости поддона, %",
            //    Color = OxyColors.Red,
            //    LineStyle = LineStyle.Dash
            //});
            vm.PM.InvalidatePlot(true);
        }

        private void btn_autodynInit_fromXml_Copy3_Click(object sender, RoutedEventArgs e) {
            var keys = ad.Keys.Except(new int[] { 586,890, 927 }).OrderBy(k => k).ToList();
            var el_tup = keys
                .Select(k => {
                    var i1 = ad.dict[k].Vels["el"].Data.Values.Last().Value;
                    var i2 = oneD.dict[k].Vels["el"].Data.Values.Last().Value;
                    return (vel: (double)k, ad: i1, one: i2);
                })
                .ToList();
            var podd_tup = keys
               .Select(k => {
                   var i1 = ad.dict[k].Vels["podd"].Data.Values.Last().Value;
                   var i2 = oneD.dict[k].Vels["podd"].Data.Values.Last().Value;
                   return (vel: (double)k, ad: i1, one: i2);
               })
               .ToList();
            var el_abs = el_tup.Select(t => new DataP() { X = t.vel, Y = (t.one -t.ad) }).ToList();
            var el_perc = el_tup.Select(t => new DataP() { X = t.vel, Y = (t.one - t.ad)/t.ad*100 }).ToList();
            var podd_abs = podd_tup.Select(t => new DataP() { X = t.vel, Y = (t.one - t.ad) }).ToList();
            var podd_perc = podd_tup.Select(t => new DataP() { X = t.vel, Y = (t.one - t.ad) / t.ad*100 }).ToList();

            vm.PM.Series.Clear();

            //vm.PM.Series.Add(new LineSeries() {
            //    ItemsSource = el_abs,
            //    DataFieldX = "X",
            //    DataFieldY = "Y",
            //    Title = "отличие дульной скорости элемента, м/с",
            //    Color = OxyColors.Red
            //});
            //vm.PM.Series.Add(new LineSeries() {
            //    ItemsSource = podd_abs,
            //    DataFieldX = "X",
            //    DataFieldY = "Y",
            //    Title = "отличие конечной скорости поддона, м/с",
            //    Color = OxyColors.Red,
            //    LineStyle = LineStyle.Dash
            //});
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = el_perc,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "отличие дульной скорости элемента, %",
                Color = OxyColors.Red
            });
            vm.PM.Series.Add(new LineSeries() {
                ItemsSource = podd_perc,
                DataFieldX = "X",
                DataFieldY = "Y",
                Title = "отличие конечной скорости поддона, %",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Dash
            });
            vm.PM.InvalidatePlot(true);
        }

        private void btn_autodynInit_fromXml_Copy4_Click(object sender, RoutedEventArgs e) {
            var keys = ad.Keys.Except(new int[] { 586, 890, 927 }).OrderBy(k => k).Batch(3).SelectMany(b => b.Take(1)).ToList();
            var palette = OxyPalettes.Jet(keys.Count);
            vm.PM.Series.Clear();
            foreach (var k in keys.Zip(MoreEnumerable.Generate(0, i => i + 1), (key, index) => (key: key, index: index))) {
                var tup = GetP_x_tup(k.key);
                var ser = new AreaSeries() {
                    Title = $"[V_0 = {k.key} м/с] макс давление от координаты, кПа",
                    Color = palette.Colors[k.index],
                    Color2 = OxyColor.FromAColor(1, palette.Colors[k.index]),
                    Fill = OxyColor.FromAColor(100, palette.Colors[k.index]),
                    Smooth = true,
                    MarkerType = MarkerType.Square
                };
                var ser2 = new LineSeries() {
                    Color = palette.Colors[k.index],
                    LineStyle = LineStyle.Dash,
                    Smooth = true,
                    MarkerType = MarkerType.Circle
                };
                ser.Points.Capacity = tup.p_x_ad.Count;
                foreach (var dp in tup.p_x_ad) {
                    ser.Points.Add(new DataPoint(dp.X, dp.Y));
                }
                ser.Points2.Capacity = tup.p_x_one.Count;
                ser2.Points.Capacity = tup.p_x_one.Count;
                foreach (var dp in tup.p_x_one) {
                    ser.Points2.Add(new DataPoint(dp.X, dp.Y));
                    ser2.Points.Add(new DataPoint(dp.X, dp.Y));
                }
                vm.PM.Series.Add(ser);
                vm.PM.Series.Add(ser2);
            }
            vm.PM.InvalidatePlot(true);
        }
        (List<DataP> p_x_ad, List<DataP> p_x_one) GetP_x_tup(int key) {
            return (ad.GetMaxPress_x(key, 10000), oneD.GetMaxPress_x(key, 10000));
        }

        private void btn_autodynInit_fromXml_Copy5_Click(object sender, RoutedEventArgs e) {
            var keys = ad.Keys.Except(new int[] { 586, 890, 927 }).OrderBy(k => k).Batch(2).SelectMany(b=>b.Take(1)).ToList();
            var palette = OxyPalettes.Jet(keys.Count);
            vm.PM.Series.Clear();
            foreach (var k in keys.Zip(MoreEnumerable.Generate(0,i=>i+1),(key, index) => (key:key, index:index))) {
                var tup = GetV_t_tup(k.key);
                var ser = new AreaSeries() {
                    Title = $"[V_0 = {k.key} м/с] Прирост, относительно V_0, м/с",
                    Color = palette.Colors[k.index],
                    Color2 = OxyColor.FromAColor(1, palette.Colors[k.index]),
                    Fill = OxyColor.FromAColor(100, palette.Colors[k.index]),
                    Smooth = true,
                    MarkerType = MarkerType.Square
                };
                var ser2 = new LineSeries() {
                    Color = palette.Colors[k.index],
                    LineStyle = LineStyle.Dash,
                    Smooth = true,
                    MarkerType = MarkerType.Circle
                };
                ser.Points.Capacity = tup.p_x_ad.Count;
                foreach (var dp in tup.p_x_ad) {
                    ser.Points.Add(new DataPoint(dp.X, dp.Y));
                }
                ser.Points2.Capacity = tup.p_x_one.Count;
                ser2.Points.Capacity = tup.p_x_one.Count;
                foreach (var dp in tup.p_x_one) {
                    ser.Points2.Add(new DataPoint(dp.X, dp.Y));
                    ser2.Points.Add(new DataPoint(dp.X, dp.Y));
                }
                vm.PM.Series.Add(ser);
                vm.PM.Series.Add(ser2);
            }
            vm.PM.InvalidatePlot(true);
        }
        (List<DataP> p_x_ad, List<DataP> p_x_one) GetV_t_tup(int key) {
            var p_x_ad = new List<DataP>(ad.dict[key].Vels["el"].Count);
            var p_x_oneD = new List<DataP>(oneD.dict[key].Vels["el"].Count);
            foreach (var it in ad.dict[key].Vels["el"].Data) {
                p_x_ad.Add(new DataP() { X = it.Key, Y = it.Value.Value - key });
            }
            foreach (var it in oneD.dict[key].Vels["el"].Data) {
                p_x_oneD.Add(new DataP() { X = it.Key, Y = it.Value.Value - key });
            }
            return (p_x_ad, p_x_oneD);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            windowSet.ShowDialog();
            synchDirs();
        }

        void synchDirs() {
            one_mega_dir = windowSet.tb_one_mega.Text;
            ad_mega_dir = windowSet.tb_ad_mega.Text;
            ad_all_dir = windowSet.tb_ad_all.Text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            windowSet.Close();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            var dir = one_mega_dir;// @"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega_1D_.json";
            using (var jstr = new JsonTextReader(new StreamReader(dir))) {

                var ser = JsonSerializer.Create();
                dict1d = ser.Deserialize<SerializableDictionary<int, AutodynInfo>>(jstr);
            }

        }
    }
}
