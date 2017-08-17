using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.OneDemGrid;

namespace WBRimanTest {
    public class RmNode : WBOneDemNode {
        public double ro, p, e;
        public static double k = 1.4;
        public static double vyaz = 0.016;

        public double GetE() {
            return p / (ro * (k - 1));
        }
        public double GetP() {
            return ro * e * (k - 1);
        }
        public void InitS() {
            s[1] = ro;
            s[2] = ro * u;
            s[3] = ro * (e + u * u / 2);
        }
        public void InitF() {
            f[1] = ro * u;
            f[2] = ro * u * u + p;
            f[3] = u * (ro * e + ro * u * u / 2 + p);
        }
        public void Synch() {
            e = GetE();
            InitS();
            InitF();
        }
    
        public WBVec f = new WBVec(0,0,0), s = new WBVec(0, 0, 0);

        public WBVec F(WBVec S) {
            return F(S[1], S[2], S[3]);
        }

        public WBVec F(double s1, double s2, double s3) {
            s = new WBVec(s1, s2, s3);
            ro = s1;
            u = s2 / s1;
            p = (k - 1) * (s3 - s2 * s2 * 0.5 / s1);
            e = GetE();           
            InitF();
            return f;
        }
        public static WBVec F_func(WBVec s) {
            var p = (k - 1) * (s[3] - s[2] * s[2] * 0.5 / s[1]);
            return new WBVec(
                s[2],
                s[2] * s[2] / s[1] + p,
                s[2] * (s[3] + p) / s[1]);
        }


    }
  


}
