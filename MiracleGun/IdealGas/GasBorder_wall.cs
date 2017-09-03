using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class GasBorder_wall : WBEulerBorder<GasCell, GasBound> {

        public override void InitFakeCells() {
            throw new NotImplementedException();
        }
    }
}
