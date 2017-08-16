using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.OneDemGrid;

namespace WBRimanTest {
    public class RmNode:WBNodeBase {
        public double X,u,ro, p, e;
        public static double k = 1.4;
        public double GetE() {
            return p / (ro * (k - 1));
        }
        public double GetP() {
            return ro * e * (k - 1);
        }
        public double s1 => ro;
        public double s2 => ro*u;
        public double s3 => ro*(e+u*u/2);
        public double f1 => ro*u;
        public double f2 => ro * u * u + p;
        public double f3 => u * (ro * e * ro * u * u / 2 + p);
    }
    

}
