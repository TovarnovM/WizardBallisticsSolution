using Interpolator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bikas_comp1D2D {
    public class AutodynConverter {
        public string dir = @"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d";
        public string filePattern = @"_2318_";
        public SerializableDictionary<int, AutodynInfo> GetMegaDict(string dir = @"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d", string filePattern = @"_2318_") {
            this.dir = dir;
            this.filePattern = filePattern;
            AutodynDatas();
            return dict_ai;
        }

        public Task<SerializableDictionary<int, AutodynInfo>> GetMegaDictAsync(string dir = @"D:\расчетики\бикалиберный ствол\23 мм\comparison_1d", string filePattern = @"_2318_") {
            return Task.Factory.StartNew<SerializableDictionary<int, AutodynInfo>>(() => GetMegaDict(dir, filePattern));
        }

        #region InitAutodynDatas from orig    dict_2318_ai
        void AutodynDatas() {
            InitDicts();
            foreach (var f_v in dict2318_Vels) {
                var vel = int.Parse(f_v.Key);
                var ai = new AutodynInfo() {
                    vel = vel
                };
                dict_ai.Add(vel, ai);
            }
            var t1 = InitAut_VelsAsync();
            var t2 = InitAut_gVelsAsync();
            var t3 = InitAut_gPresAsync();
            Task.WaitAll(t1, t2, t3);

            //var fileName = "autodyn_infos_2318.bf";
            //var sw = new FileStream(fileName, FileMode.OpenOrCreate);
            //var bf = new BinaryFormatter();
            //bf.Serialize(sw, dict_2318_ai);
            //sw.Close();


            //XmlSerializer serial = new XmlSerializer(dict_2318_ai.GetType());
            //var fileName = "autodyn_infos_2318.xml";
            //var sw = new StreamWriter(fileName);
            //serial.Serialize(sw, dict_2318_ai);
            //sw.Close(); 
        }


        Task AutodynDatasAsync() {
            return Task.Factory.StartNew(AutodynDatas);
        }
        Dictionary<string, string> dict2318_gVels = new Dictionary<string, string>();
        Dictionary<string, string> dict2318_gPress = new Dictionary<string, string>();
        Dictionary<string, string> dict2318_Vels = new Dictionary<string, string>();
        void InitDicts() {
            var flist = FileSearcher.Search(dir, ".uhs");
            var lst_ = new List<string>();
            var r_vel = new Regex(@"v\d{3,4}");
            var r_int = new Regex(@"\d+");
            var r2318 = new Regex(filePattern);
            var r_gPres = new Regex("PRESSURE");
            var r_gVels = new Regex("X-VELOCITY");
            var r_Vels = new Regex("Ave.X.vel");

            foreach (var f in flist) {

                if (r2318.IsMatch(System.IO.Path.GetFileName(f))){//r2318.IsMatch(System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f)))) {
                    lst_.Add(f);
                } else {
                    continue;
                }
                using (TextReader fs = new StreamReader(f)) {

                    var vel = r_int.Match(r_vel.Match(System.IO.Path.GetFileName(f)).Value).Value;
                    for (int i = 0; i < 4; i++) {
                        var line = fs.ReadLine();

                        if (r_gPres.IsMatch(line)) {
                            dict2318_gPress.Add(vel, f);
                            break;
                        } else if (r_gVels.IsMatch(line)) {
                            dict2318_gVels.Add(vel, f);
                            break;
                        } else if (r_Vels.IsMatch(line)) {
                            dict2318_Vels.Add(vel, f);
                            break;
                        }
                    }
                }
            }
        }
        void InitAut_Vels() {
            Parallel.ForEach(dict2318_Vels, f_v => {
                using (TextReader fs = new StreamReader(f_v.Value)) {
                    var vel = int.Parse(f_v.Key);
                    var ai = dict_ai[vel];
                    fs.ReadLine();
                    var heads = fs.ReadLine().Split(',').Select(s => s.Trim()).ToArray();
                    int t_ind = 1;
                    heads[t_ind] = "TIME (ms)";
                    fs.ReadLine();

                    for (int i = t_ind + 1; i < heads.Length; i++) {
                        ai.Vels.Add(heads[i], new InterpXY());
                    }

                    do {
                        var l = fs.ReadLine();
                        if (l == null)
                            break;
                        var datas = l.Split(',').Select(s => s.Trim()).ToArray();
                        for (int i = t_ind + 1; i < datas.Length; i++) {
                            ai.Vels[heads[i]].Add(GetDouble(datas[t_ind]), GetDouble(datas[i]));
                        }
                    } while (true);
                    foreach (var intxy in ai.Vels.Values) {
                        intxy.SynchArrays();
                    }
                }
            });
        }
        Task InitAut_VelsAsync() {
            return Task.Factory.StartNew(InitAut_Vels);
        }
        void InitAut_gVels() {
            Parallel.ForEach(dict2318_gVels, f_v => {
                using (TextReader fs = new StreamReader(f_v.Value)) {
                    var vel = int.Parse(f_v.Key);
                    var ai = dict_ai[vel];
                    fs.ReadLine();
                    var heads = fs.ReadLine().Split(',').Select(s => s.Trim()).ToArray();
                    int t_ind = 1;
                    heads[t_ind] = "TIME (ms)";
                    fs.ReadLine();

                    for (int i = t_ind + 1; i < heads.Length; i++) {
                        ai.gVels.Add(heads[i], new InterpXY());
                    }

                    do {
                        var l = fs.ReadLine();
                        if (l == null)
                            break;
                        var datas = l.Split(',').Select(s => s.Trim()).ToArray();
                        for (int i = t_ind + 1; i < datas.Length; i++) {
                            ai.gVels[heads[i]].Add(GetDouble(datas[t_ind]), GetDouble(datas[i]));
                        }
                    } while (true);
                    foreach (var intxy in ai.gVels.Values) {
                        intxy.SynchArrays();
                    }
                }
            });
        }
        Task InitAut_gVelsAsync() {
            return Task.Factory.StartNew(InitAut_gVels);
        }
        void InitAut_gPres() {
            Parallel.ForEach(dict2318_gPress, f_v => {
                using (TextReader fs = new StreamReader(f_v.Value)) {
                    var vel = int.Parse(f_v.Key);
                    var ai = dict_ai[vel];
                    fs.ReadLine();
                    var heads = fs.ReadLine().Split(',').Select(s => s.Trim()).ToArray();
                    int t_ind = 1;
                    heads[t_ind] = "TIME (ms)";
                    fs.ReadLine();

                    for (int i = t_ind + 1; i < heads.Length; i++) {
                        ai.gPress.Add(heads[i], new InterpXY());
                    }

                    do {
                        var l = fs.ReadLine();
                        if (l == null)
                            break;
                        var datas = l.Split(',').Select(s => s.Trim()).ToArray();
                        for (int i = t_ind + 1; i < datas.Length; i++) {
                            ai.gPress[heads[i]].Add(GetDouble(datas[t_ind]), GetDouble(datas[i]));
                        }
                    } while (true);
                    foreach (var intxy in ai.gPress.Values) {
                        intxy.SynchArrays();
                    }
                }
            });
        }
        Task InitAut_gPresAsync() {
            return Task.Factory.StartNew(InitAut_gPres);
        }
        SerializableDictionary<int, AutodynInfo> dict_ai = new SerializableDictionary<int, AutodynInfo>();
        

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
        #endregion
    }
}
