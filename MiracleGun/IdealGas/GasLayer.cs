using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class GasLayer : WBOneDemCellLayer<GasCell, GasBound> {
        public GunShape Geom { get; set; }
        public override void InitBoundCellRefs() {
            foreach (var tpl in GetCellBoundNeibs()) {
                tpl.cell.LeftBound = tpl.leftB;
                if (tpl.leftB != null)
                    tpl.leftB.RightCell = tpl.cell;

                tpl.cell.RightBound = tpl.rightB;
                if (tpl.rightB != null)
                    tpl.rightB.LeftCell = tpl.cell;

            }
        }

        public override void InitDataRefs() {
            foreach (var c in AllCells) {
                c.Geom = Geom;
            }
            foreach (var b in AllBounds) {
                b.Geom = Geom;
            }
        }
    }
}
