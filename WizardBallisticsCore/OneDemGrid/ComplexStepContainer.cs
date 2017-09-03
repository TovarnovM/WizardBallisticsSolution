using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public class ComplexStepContainer<TCell, TBound> where TCell : WBOneDemNode where TBound : WBOneDemNode {
        public double multipl;
        public WBOneDemCellLayer<TCell, TBound> dyLayer;
        public bool synch;
        public bool calcFluxes;
    }
}
