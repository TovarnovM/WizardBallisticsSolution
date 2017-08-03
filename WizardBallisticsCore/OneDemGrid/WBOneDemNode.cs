using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;

namespace WizardBallisticsCore.OneDemGrid {
    [Serializable]
    public class WBOneDemCell<T>: WBNodeBase where T: struct{
        public T Data;
        public WBOneDemCell<T> LeftCell { get; set; }
        public WBOneDemCell<T> RightCell { get; set; }
        public double Xc { get; set; }
        public double Vc { get; set; }

    }
}
