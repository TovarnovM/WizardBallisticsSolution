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
    public class WBOneDemLayer<T> : WBNodeLayerBase<T> where T: WBOneDemNode {
        public WBOneDemLayerOptions Opt { get; set; }
        [JsonIgnore]
        public List<T> LeftNodes, RightNodes, RealNodes;

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
                ln.Index = (--ind)*10;
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
            LeftNodes = new List<T>(Opt.LeftNodesCount);
            LeftNodes.AddRange(GetLeftNodes());
            RightNodes = new List<T>(Opt.RightNodesCount);
            RightNodes.AddRange(GetRightNodes());
        }

        public override void CloneLogic(IWBNodeLayer clone) {
            (clone as WBOneDemLayer<T>).InitLists();
        }

        public void InitLayer(double time, WBOneDemLayerOptions opts, Func<double,double,T> initCondFunc) {
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
            InitLists();
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
