using MiracleGun.ElasticPiston;
using MiracleGun.IdealGas;
using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace Bikas_comp1D2D {
    public class Piston_el_params {
        public double V0 { get; set; }
        public double c0 { get; set; } = 2380d;
        public double rho0 { get; set; } = 919.03;
        public double sigmas { get; set; } = 25.2E6;
        public double k0 { get; set; } = 0.054;
        public double b1 { get; set; } = 0.027;
        public double b2 { get; set; } = 0.00675;
        public double mu { get; set; } = 0.001;
        public double taus { get; set; } = 1.0E6;
        public double gamma { get; set; } = 1.63098;
        public double rho { get; set; } = 921.0;
        public double m_elem { get; set; } = 17.56e-3;
        public double m_podd{ get; set; } = 42.52e-3;
        public double p_podd { get; set; } = 4e7;
        public double p_elem { get; set; } = 100e3;
        public double max_x_elem { get; set; }
        public double x_l { get; set; } = 0.035;
        public double x_r { get; set; } = 0.098;
        public ElasticPistonConsts GetElasticPistonConsts() {
            var gp = new ElasticPistonConsts(gamma) {
                c0 = this.c0,
                rho0 = this.rho0,
                sigmas = this.sigmas,
                k0 = this.k0,
                b1 = this.b1,
                b2 = this.b2,
                mu = this.mu,
                taus = this.taus
            };
            return gp;
        }
    }
    public class Piston_el_params_shock: Piston_el_params {
        public double Grun = 1.55,
            C_1 = 2.486000e+003,
            S_1 = 1.577000,
            p0 = 1e5;
        public ElasticPistonConsts_shockEOS GetGetElasticPistonConsts_shock() {
            var gp = new ElasticPistonConsts_shockEOS(gamma) {
                c0 = this.c0,
                rho0 = this.rho0,
                sigmas = this.sigmas,
                k0 = this.k0,
                b1 = this.b1,
                b2 = this.b2,
                mu = this.mu,
                taus = this.taus,
                C_1 = C_1,
                S_1 = S_1,
                p0 = p0
            };
            return gp;
        }
    }

    public class Piston_1D {
        public BikasGrid grid;
        public WBSolver solver;
        public int NumOdCells { get; set; } = 300;
        public double d0 { get; set; } = 0.023;
        public double d1 { get; set; } = 0.018;
        public double l0 { get; set; } = 0.1;
        public double l05 { get; set; } = 0.1;
        public double l1 { get; set; } = 0.1;
        public WBSolver GetSolverElastic(Piston_el_params prms) {
            var layerOpts = GetBikasOpts(prms.x_l, prms.x_r);
            var initLayer = new GasLayer();
            initLayer.Geom = bikasShape;
            var gp = prms.GetElasticPistonConsts() ;
            var v0 = prms.V0;
            initLayer.InitLayer(0d, layerOpts, CellFuncFactory(v0, gp, prms.rho, prms.p_podd), BoundFuncFactory(v0, gp));
            grid = new BikasGrid("ElasticP_bikas", initLayer, prms.m_podd, prms.m_elem,prms.p_podd,prms.p_elem,prms.max_x_elem);
            grid.Slaver = new WBMemTacticTimeStep() { timeStepSave = 1E-6, OwnerGrid = grid };
            solver = new WBSolver(grid, WBProjectOptions.Default);
            return solver;
        }

        public GunShape bikasShape {
            get {
                var geom = new GunShape();
                geom.AddPoint(0, d0);
                geom.AddPoint(l0, d0);
                geom.AddPoint(l0+l05, d1);
                geom.AddPoint(l0 + l05+ l1, d1);
                geom.left_beound = d0;
                geom.right_beound = d1;
                return geom;
            }
        }
        
        public WBOneDemLayerOptions GetBikasOpts(double x_l, double x_r) {

                var lo = new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = x_l,
                    X_right = x_r,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(NumOdCells),
                };
                lo.SynchH();
                return lo;
            
        }
        public Func<double,double, ElasticPistonCell> CellFuncFactory(double v0, ElasticPistonConsts gp, double ro, double p) {
            return (t, x) => {
                var answ = new ElasticPistonCell(gp);
                answ.ro = ro;
                answ.u = v0;
                answ.p = p;
                answ.X = x;
                answ.V = v0;
               // answ.p = answ.GetPressure();
                return answ;
            };
        }

        public Func<double, double, ElasticPistonBound> BoundFuncFactory(double v0, ElasticPistonConsts gp) {
            return (t, x) => {
                return new ElasticPistonBound(gp) { X = x, V = v0 };
            };
        }

        public WBSolver GetSolverElastic_shock(Piston_el_params_shock prms) {
            var layerOpts = GetBikasOpts(prms.x_l, prms.x_r);
            var initLayer = new GasLayer();
            initLayer.Geom = bikasShape;
            var gp = prms.GetGetElasticPistonConsts_shock();
            var v0 = prms.V0;
            initLayer.InitLayer(0d, layerOpts, CellFuncFactory_shock(v0, gp, prms.rho, prms.p_podd), BoundFuncFactory(v0, gp));
            grid = new BikasGrid("ElasticP_bikas", initLayer, prms.m_podd, prms.m_elem, prms.p_podd, prms.p_elem, prms.max_x_elem);
            solver = new WBSolver(grid, WBProjectOptions.Default);
            return solver;
        }
        public Func<double, double, ElasticPistonCell_shockEOS> CellFuncFactory_shock(double v0, ElasticPistonConsts_shockEOS gp, double ro, double p) {
            return (t, x) => {
                var answ = new ElasticPistonCell_shockEOS(gp);
                answ.ro = ro;
                answ.u = v0;
                answ.p = p;
                answ.X = x;
                answ.V = v0;
                answ.p = answ.GetPressure();
                return answ;
            };
        }
    }
}
