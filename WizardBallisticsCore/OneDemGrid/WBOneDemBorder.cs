using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public abstract class WBEulerBorder<TCell,TBound>: ICloneable where TCell : WBOneDemNode where TBound: WBOneDemNode {
        public virtual double GetX(double dt) {
            double sign = BorderPos == WBBorderPos.rightBorder
                ? 1d
                : -1d;
            return X_0 + V_0 * dt + sign * 0.5 * A_0_abs * dt * dt;
        }
        public virtual double GetV(double dt) {
            double sign = BorderPos == WBBorderPos.rightBorder
                ? 1d
                : -1d;
            return V_0 + sign * A_0_abs * dt;
        }

        public abstract void InitFakeCells();
        public void SetCond(double dt=0d) {
            A_0_abs = A_0_func?.Invoke(Time_0,RealCells) ?? A_0_abs;
            V_0 = GetV(dt);
            X_0 = GetX(dt);
        }
        public object Clone() {
            return MemberwiseClone();
        }
        public Func<double, IList<TCell>, double> A_0_func { get; set; }
        public double A_0_abs { get; set; }
        public double A_0 {
            get {
                double sign = BorderPos == WBBorderPos.rightBorder
                ? 1d
                : -1d;
                return sign * A_0_abs;
            }
        }
        public double X_0 {
            get {
                return BorderNode.X;
            }
            set {
                BorderNode.X = value;
            }
        }
        public double V_0 {
            get {
                return BorderNode.V;
            }
            set {
                BorderNode.V = value;
            }
        }
        public double Time_0 {
            get {
                return OwnerLayer.Time;
            }
        }
        public WBOneDemCellLayer<TCell, TBound> OwnerLayer { get; set; }
        public TBound BorderNode {
            get {
                switch (BorderPos) {
                    case WBBorderPos.leftBorder:
                        return OwnerLayer.RealBounds[0];
                    case WBBorderPos.rightBorder:
                        return OwnerLayer.RealBoundsRev[0];
                    default:
                        throw new Exception("Baaaaad border");
                }
            }
        }
        public IList<TCell> RealCells {
            get {
                switch (BorderPos) {
                    case WBBorderPos.leftBorder:
                        return OwnerLayer.RealCells;
                    case WBBorderPos.rightBorder:
                        return OwnerLayer.RealCellsRev;
                    default:
                        throw new Exception("Baaaaad border");
                }
            }
        }
        public IList<TCell> FakeCells {
            get {
                switch (BorderPos) {
                    case WBBorderPos.leftBorder:
                        return OwnerLayer.LeftCells;
                    case WBBorderPos.rightBorder:
                        return OwnerLayer.RightCells;
                    default:
                        throw new Exception("Baaaaad border");
                }
            }
        }
        public WBBorderPos BorderPos {get;set; }       
    }

    public enum WBBorderPos { leftBorder, rightBorder };
}
