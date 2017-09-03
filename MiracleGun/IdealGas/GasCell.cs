using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    /// <summary>
    /// 
    /// </summary>
    public class GasCell:WBOneDemNode {
        public double u;
        /// <summary>
        /// d(i)  Согласно constants.f90  
        /// </summary>
        public double ro;
        public double p;
        public double e;
        public GasConstants g;
        public GunShape Geom;
        public GasBound LeftBound, RightBound;
        public WBVec q = WBVec.Zeros(3), h = WBVec.Zeros(3);
        public GasCell(GasConstants g) {
            this.g = g;
        }
        public virtual double GetE() {
            return (p / g[9]) * (1 / ro - g.covolume);
        }
        public virtual double GetPressure() {
            return g[9]*e /(1 / ro - g.covolume);
        }
        public WBVec Get_dQS() {
            return h * dx - (RightBound.S * RightBound.flux - LeftBound.S * LeftBound.flux);
        }
        public void InitQ() {
            q[1] = ro;
            q[2] = ro * u;
            q[3] = ro *(e + 0.5 * u * u);
        }
        public virtual void Init_h() {
            h[1] = 0d;
            h[2] = p * Geom.Get_dS(LeftBound?.X ?? X, RightBound?.X ?? X);
            h[3] = 0d;
        }
        public void SetQ(WBVec q) {
            this.q = q;
            ro = q[1];
            u = q[2] / ro;
            e = q[3] / ro - 0.5 * u * u;
            p = GetPressure();
            Init_h();
        }
        public void Sync() {
            e = GetE();
            InitQ();
            Init_h();
        }
        public virtual double GetCSound() {
            return Math.Sqrt(p / (g[8] * ro * (1d - g.covolume * ro)));
        }

        public double CSound => GetCSound();
        /// <summary>
        /// Энтальпия
        /// </summary>
        public double H => e + 0.5 * u * u + p / ro;
        /// <summary>
        /// объем ячейки
        /// </summary>
        public double W => Geom.GetW(LeftBound.X, RightBound.X);
        public double dx => RightBound.X - LeftBound.X;
    }

    /// <summary>
    /// Согласно constants.f90
    /// </summary>
    public class GasConstants {
        public double[] g = new double[10];
        public double gamma;
        /// <summary>
        /// Согласно constants.f90   c(1,8)
        /// </summary>
        public double covolume = 0.001027d; 
        public GasConstants(double gamma) {
            SynchArr(gamma);
        }

        public GasConstants(double gamma, double covolume) {
            this.covolume = covolume;
            SynchArr(gamma);
        }

        public void SynchArr(double gamma) {
            this.gamma = gamma;
            g[0] = gamma;
            g[1] = 0.5 * (gamma - 1d) / gamma;
            g[2] = 0.5 * (gamma + 1d) / gamma;
            g[3] = 2d* gamma / (gamma - 1d);
            g[4] = 2d/ (gamma - 1d);
            g[5] = 2d/ (gamma + 1d);
            g[6]= (gamma - 1d) / (gamma + 1d);
            g[7] = 0.5 * (gamma - 1d);
            g[8] = 1d/ gamma;
            g[9] = gamma - 1d;
        }
        public double this[int ind] {
            get {
                return g[ind];
            }
        }
    }
}
