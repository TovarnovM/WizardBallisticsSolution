using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.OneDemCellGrid {
    public struct WBOneDemCell<T> where T:struct {
        public int Index;        
        public double X;
        public double V;
        public T Data;
    }
}
