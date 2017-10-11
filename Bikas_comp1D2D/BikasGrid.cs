using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using WizardBallistics.Core;

namespace Bikas_comp1D2D {
    public class BikasGrid: WBGridBase {
        public double m_l, m_r;
        public double p_l, p_r;
        public double max_x_elem;
        public int smooth = 2;
        public BikasGrid(string v, GasLayer initLayer, double m_l, double m_r, double p_l, double p_r, double max_x_elem, int smooth = 2):base(v,initLayer) {
            this.m_l = m_l;
            this.m_r = m_r;
            this.p_l = p_l;
            this.p_r = p_r;
            this.max_x_elem = max_x_elem;
            this.smooth = smooth;
            initLayer.LeftBorder = new GasBorderWall() {
                OwnerLayer = initLayer,
                A_0_func = A_0_function_L,
                BorderPos = WBBorderPos.leftBorder
            };
            initLayer.RightBorder = new GasBorderWall() {
                OwnerLayer = initLayer,
                A_0_func = A_0_function_R,
                BorderPos = WBBorderPos.rightBorder
            };
            AddStopFunc(MyStopF);
        }

        public double A_0_function_L(double t, IList<GasCell> realCells) {
            return Get_a_lft(realCells);
        }

        public double A_0_function_R(double t, IList<GasCell> realCells) {
            return Get_a_rght(realCells);
        }

        public double Get_a_lft(IList<GasCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = (revList[i].p - p_l) * revList[i].LeftBound.S / m_l;
                a += a;
            }
            return a / smooth;

        }

        public double Get_a_rght(IList<GasCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = (revList[i].p - p_r) * revList[i].RightBound.S / m_r;
                a += a;
            }
            return a / smooth;

        }


        public override double GetMaxTimeStep() {
            double tau = 0.4* (CurrLayer as GasLayer).GetMaxTimeStep();
            //tau = tau < 1E-6 ? 1E-6 : tau;
            //tau =  Math.Min(0.01, tau);
            return tau;
        }

        public override void InfoСommunication() {
            
        }

        public override void StepUpLogic(double deltaTau) {
            var lr0 = (GasLayer)CurrLayer;
            var lr05 = lr0.EulerStep(0.5 * deltaTau, lr0);

            List<ComplexStepContainer<GasCell, GasBound>> rightPart = new List<ComplexStepContainer<GasCell, GasBound>> {
                new ComplexStepContainer<GasCell, GasBound>() {
                    calcFluxes = true,
                    synch = true,
                    multipl = 1d,
                    dyLayer = lr05
                }
            };

            var lrr = lr0.ComplexStep(deltaTau, false, rightPart);
            AddLayer(lrr);
        }
        public bool MyStopF() {
            var gl = (GasLayer)CurrLayer;
            
            return gl.RealBoundsRev[0].X >= max_x_elem;
        }

    }
}