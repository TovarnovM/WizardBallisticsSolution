using Interpolator;
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
                dict_2318_ai = await aconv.GetMegaDictAsync(@"C:\Users\mi\Google Диск\autodyn_uhss", @"_2318_");//@"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d",@"_2318_");
                var all_ts = dict_2318_ai.Values.SelectMany(vs => vs.gPress.Values.Select(vv => vv.Data.Keys.ToList()));
                var all_dts = new List<double>();
                foreach (var ts in all_ts) {
                    var dts = ts.Zip(ts.Skip(1), (t0, t1) => t1 - t0).ToList();
                    var dt = dts.Average();
                    all_dts.Add(dt);
                }
                var sss = all_dts.Average();
                
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

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            var opts = new Piston_el_params() {
                V0 = 1000,
                max_x_elem = 0.24
            };
            var p = new Piston_1D();
            var sol = p.GetSolverElastic(opts);
            sol.RunCalc();

            var gr = (BikasGrid)sol.Grids.Values.First();
            var oldTimes = gr.LayerList.Select(lr => lr.Time).ToList();
            var oldDeltas = oldTimes.Zip(oldTimes.Skip(1), (t1, t0) => t1 - t0).ToList();
            
        }
    }
}
