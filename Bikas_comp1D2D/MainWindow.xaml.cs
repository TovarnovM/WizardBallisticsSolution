using Interpolator;
using MiracleGun.IdealGas;
using Newtonsoft.Json;
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
        public MainWindow() {
            DataContext = this;
            vm = new StandartVM();
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            var oldContent = btn_autodynInit.Content;
            try {
                btn_autodynInit.IsEnabled = false;
                btn_autodynInit.Content = "In process...";
                var aconv = new AutodynConverter();
                dict_2318_ai = await aconv.GetMegaDictAsync(@"C:\Users\User\Google Диск\autodyn_uhss", @"_2318_");//@"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d",@"_2318_");
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

        private void btn_autodynInit_fromXml_Click(object sender, RoutedEventArgs e) {
            int countz = 1;
            var lst = new List<SerializableDictionary<int, AutodynInfo>>(countz);
            for (int i = 0; i < countz; i++) {
                var cl = new SerializableDictionary<int, AutodynInfo>();
                foreach (var d in dict_2318_ai) {
                    cl.Add(d.Key, d.Value.Copy());
                }
                lst.Add(cl);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e) {
            var dict1d = await Get1DDictAsync(dict_2318_ai);
            var dir = @"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega_1D_.json";
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
            Parallel.ForEach(dict, kw => {
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
            var dir = @"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega.json";
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
            var dir = @"C:\Users\User\Google Диск\autodyn_uhss\1d_2318\2318_mega.json";
            using (var jstr = new JsonTextReader(new StreamReader(dir))) {

                var ser = JsonSerializer.Create();
                dict_2318_ai = ser.Deserialize<SerializableDictionary<int, AutodynInfo>>(jstr);
            }
        }

        public AutodynInfo Get_1D_sol(AutodynInfo info) {

            var opts = new Piston_el_params() {
                V0 = info.vel,
                max_x_elem = 0.25
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

    }
}
