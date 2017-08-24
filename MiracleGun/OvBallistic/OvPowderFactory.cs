using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.OvBallistic {
    public static class OvPowderFactory {
        class PowderDummy {
            public string name { get; set; }
            public double f { get; set; }
            public double etta { get; set; }
            public double alpha_k { get; set; }
            public double T_1 { get; set; }
            public double ro { get; set; }
            public double I_k { get; set; }
            public double Z_k { get; set; }
            public double k_1 { get; set; }
            public double lambda_1 { get; set; }
            public double k_2 { get; set; }
            public double lambda_2 { get; set; }
            public double k_f { get; set; }
            public double k_l { get; set; }
            public OvPowder ConvertToOvPowder() {
                var res = new OvPowder();
                res.f = f;
                res.Ik = I_k;
                res.alpha_k = alpha_k;
                res.k = etta + 1;
                res.dest = ro;
                res.T1 = T_1;
                res.zk = Z_k;
                res.kappa_f = k_f;
                res.kappa_l = k_l;
                res.kappa_1 = k_1;
                res.kappa_2 = k_2;
                res.lambda_1 = lambda_1;
                res.lambda_2 = lambda_2;
                return res;
            }

        }
        static Lazy<Dictionary<string, PowderDummy>> dict = new Lazy<Dictionary<string, PowderDummy>>(LoadDict,System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        static Dictionary<string, PowderDummy> LoadDict() {
            try {
                var jstring = Properties.Resources.gpowders;
                var sett = new JsonSerializerSettings() {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var res = JsonConvert.DeserializeObject<Dictionary<string, PowderDummy>>(jstring,sett);
                return res;
            } catch (Exception e) {

                throw e;
            }


        }
        public static OvPowder Get(string powderName) {
            if (dict.Value.ContainsKey(powderName)) {
                return dict.Value[powderName].ConvertToOvPowder();
            } else {
                throw new Exception($"{powderName} нет такого пороха");
            }
        }
        public static string[] GetAllPowderNames() {
            return dict.Value.Keys.ToArray();
        }
    }
}
