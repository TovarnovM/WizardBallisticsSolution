using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class GasBound:WBOneDemNode {
        public GasCell LeftCell, RightCell;
        public GunShape Geom;
    }
}
