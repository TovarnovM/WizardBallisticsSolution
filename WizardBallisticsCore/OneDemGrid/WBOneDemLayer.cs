using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {

    /// <summary>
    /// Шаблон для одномерной подвижной сетки
    /// </summary>
    /// <typeparam name="T">Структура данных для ячейки/задачи</typeparam>
    public class WBOneDemLayer<T> : WBNodeLayerBase<T> where T : WBOneDemNode {

        #region Поля/Свойства
        /// <summary>
        /// Опции слоя
        /// </summary>
        public WBOneDemLayerOptions Opt { get; set; }

        public double L_real => Math.Abs(RealNodes[0].X - RealNodesRev[0].X);

        /// <summary>
        /// Список узлов (которые все еще находятся и в Nodes), играющих роль фиктивных узлов, расплолженных слева. Нумерация идет справа налево
        /// </summary>
        [JsonIgnore]
        public List<T> LeftNodes;

        /// <summary>
        /// Список узлов (которые все еще находятся и в Nodes), играющих роль фиктивных узлов, расплолженных справа. Нумерация идет слева направо
        /// </summary>
        [JsonIgnore]
        public List<T> RightNodes;

        /// <summary>
        /// Список узлов (которые все еще находятся и в Nodes), играющих роль основной сетки. Нумерация идет слева направо
        /// </summary>
        [JsonIgnore]
        public List<T> RealNodes;

        /// <summary>
        /// Список узлов (которые все еще находятся и в Nodes), играющих роль основной сетки. Нумерация идет справа налево
        /// </summary>
        [JsonIgnore]
        public List<T> RealNodesRev;


        #endregion

        #region Методы
        /// <summary>
        /// функция Синхронизации значений Х и V у узлов. При этом значения крайних реальных узлов не меняются
        /// </summary>
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

        /// <summary>
        /// Получить соседний узел
        /// </summary>
        /// <param name="fromWho">от кого</param>
        /// <param name="indDiff">на сколько далеко стоит сосед</param>
        /// <returns>соседа</returns>
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

        /// <summary>
        /// Получить перечисление настоящих узлов
        /// </summary>
        /// <returns></returns>
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
        #endregion

        #region Виртуальные методы
        /// <summary>
        /// Индексирования узлов, первый реальный узел имеет индекс 0, второй = 10, третий = 20 и т.д.
        /// </summary>
        public virtual void NodeIndexing() {
            for (int i = 0; i < Nodes.Count; i++) {
                Nodes[i].IndexInList = i;
            }
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

        /// <summary>
        /// функция инициализации списков
        /// </summary>
        public virtual void InitLists() {
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

        /// <summary>
        /// ЧТо сделать с клоном при клонировании
        /// </summary>
        /// <param name="clone">клон</param>
        public override void CloneLogic(IWBNodeLayer clone) {
            (clone as WBOneDemLayer<T>).InitLists();
        }

        /// <summary>
        /// Инициализация слоя
        /// </summary>
        /// <param name="time">время слоя</param>
        /// <param name="opts">опции слоя</param>
        /// <param name="initCondFunc">функция генерации узлов (на вход идет время и координата Х, возвращаемое значение = новый узел</param>
        public virtual void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, T> initCondFunc) {
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
            NodeIndexing();
        }

        /// <summary>
        /// действие при загрузке слоя
        /// </summary>
        public override void ActionWhenLoad() {
            InitLists();
        }

        public override IEnumerable<IWBNode> GetNodesForDraw(string variantName) {
            switch (variantName.ToUpper()) {
                case "REAL":
                    return RealNodes;
                default:
                    return Nodes;
            }
        }
        #endregion
    }
}
