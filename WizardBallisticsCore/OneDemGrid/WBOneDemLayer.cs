using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    //===========================================================================
    //В процессе

    /// <summary>
    /// Шаблон для одномерной подвижной эйлеровой сетки
    /// </summary>
    /// <typeparam name="T">Структура данных для ячейки/задачи</typeparam>
    public class WBOneDemLayer<T> : WBNodeLayerBase<T> where T : WBOneDemNode {
        public WBOneDemLayerOptions Opt { get; set; }
        [JsonIgnore]
        public List<T> LeftNodes, RightNodes, RealNodes, RealNodesRev;

        public IEnumerable<T> GetRealNodes() {
            for (int i = Opt.LeftNodesCount; i < Nodes.Count - Opt.RightNodesCount; i++) {
                yield return Nodes[i];
            }
        }

        /// <summary>
        /// Получить левые псевдо узлы (справа налево)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetLeftNodes() {

            for (int i = Opt.LeftNodesCount - 1; i >= 0; i--) {
                yield return Nodes[i];
            }
        }

        /// <summary>
        /// Получить правые псевдо узлы (слева направо)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetRightNodes() {

            for (int i = Nodes.Count - Opt.RightNodesCount; i < Nodes.Count; i++) {
                yield return Nodes[i];
            }

        }
        public void NodeIndexing() {
            int ind = 0;
            foreach (var ln in LeftNodes) {
                ln.Index = (--ind) * 10;
            }
            ind = 0;
            foreach (var realNd in RealNodes) {
                realNd.Index = (ind++) * 10;
            }
            foreach (var realNd in RightNodes) {
                realNd.Index = (ind++) * 10;
            }
        }

        public void InitLists() {
            RealNodes = new List<T>(Opt.RealNodesCount);
            RealNodes.AddRange(GetRealNodes());

            RealNodesRev = new List<T>(Opt.RealNodesCount);
            for (int i = RealNodes.Count - 1; i >= 0; i--) {
                RealNodesRev.Add(RealNodes[i]);
            }

            LeftNodes = new List<T>(Opt.LeftNodesCount);
            LeftNodes.AddRange(GetLeftNodes());

            RightNodes = new List<T>(Opt.RightNodesCount);
            RightNodes.AddRange(GetRightNodes());


        }

        public override void CloneLogic(IWBNodeLayer clone) {
            (clone as WBOneDemLayer<T>).InitLists();
        }

        public void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, T> initCondFunc) {
            Opt = opts;
            Time = time;
            Nodes?.Clear();
            var nds = Enumerable.Range(-Opt.LeftNodesCount, Opt.AllNodesCount)
                .Select(ind => {
                    double x = Opt.X_left + ind * Opt.H;
                    var nd = initCondFunc(Time, x);
                    nd.X = x;
                    nd.Index = ind * 10;
                    return nd;
                });
            Nodes.AddRange(nds);
            for (int i = 0; i < Nodes.Count; i++) {
                Nodes[i].IndexInList = i;
            }
            InitLists();
        }

        public override void ActionWhenLoad() {
            InitLists();
        }

        public void SynchNodes_X_V() {
            double xLeft = RealNodes[0].X;
            double dx = (RealNodesRev[0].X - xLeft) / (RealNodes.Count - 1);
            double vLeft = RealNodes[0].V;
            double dv = (RealNodesRev[0].V - vLeft) / (RealNodes.Count - 1);
            for (int i = 1; i < RealNodes.Count - 1; i++) {
                RealNodes[i].X = xLeft + dx * i;
                RealNodes[i].V = vLeft + dv * i;
            }
            for (int i = 0; i < LeftNodes.Count; i++) {
                LeftNodes[0].X = xLeft - dx * i;
                LeftNodes[0].V = vLeft - dv * i;
            }
            for (int i = 0; i < RightNodes.Count; i++) {
                LeftNodes[0].X = RealNodesRev[0].X + dx * i;
                LeftNodes[0].V = RealNodesRev[0].V + dv * i;
            }
        }
        public T GetNeib(T fromWho, int indDiff) {
            return fromWho.IndexInList + indDiff > 0 && fromWho.IndexInList < Nodes.Count
                ? Nodes[fromWho.IndexInList + indDiff]
                : null;
        }
        public T NeibL(T node) {
            return GetNeib(node, -1);
        }
        public T NeibR(T node) {
            return GetNeib(node, 1);
        }
    }
    public class WBOneDemLayerOptions {
        public int RealNodesCount { get; set; }
        public int LeftNodesCount { get; set; }
        public int RightNodesCount { get; set; }
        public int AllNodesCount => RealNodesCount + LeftNodesCount + RightNodesCount;
        public double X_left { get; set; }
        public double X_right { get; set; }
        public double H { get; set; }
        public void SynchH() {
            H = (X_right - X_left) / RealNodesCount;
        }
        public void SynchX_right() {
            X_right = X_left + RealNodesCount * H;
        }
        public void SynchX_left() {
            X_left = X_right - RealNodesCount * H;
        }
    }

}
