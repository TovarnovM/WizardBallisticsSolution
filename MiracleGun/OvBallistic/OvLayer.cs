using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {

    public class OvLayer : WBOneDemCellLayer<OvCell,OvBound> {
        public GunShape geom;
        public GunPowder powder;


        public override void InitDataRefs() {
            foreach (var c in AllCells) {
                c.geom = geom;
                c.powder = powder;
            }
            foreach (var b in AllBounds) {
                b.geom = geom;
            }
        }
        public override void InitBoundCellRefs() {
            foreach (var tpl in GetCellBoundNeibs()) {
                tpl.cell.LeftBound = tpl.leftB;
                if(tpl.leftB != null)
                    tpl.leftB.RightCell = tpl.cell;

                tpl.cell.RightBound = tpl.rightB;
                if (tpl.rightB != null)
                    tpl.rightB.LeftCell = tpl.cell;

            }  
        }

    }
}
