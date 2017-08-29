using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    /// <summary>
    /// Класс, описывающий абстрактную одномерную подвижную Эйлерову сетку
    /// В Nodes расположены поочередно TCell и TBound. 
    /// </summary>
    /// <typeparam name="TCell">Тип ячейки</typeparam>
    /// <typeparam name="TBound">Тип границы между ячейками</typeparam>
    public abstract class WBOneDemCellLayer<TCell,TBound>: WBOneDemLayer<WBOneDemNode> 
        where TCell : WBOneDemNode 
        where TBound : WBOneDemNode {

        #region Поля/свойства
        /// <summary>
        /// Список всех реальных узлов-ячеек, нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TCell> RealCells;

        /// <summary>
        /// Список всех реальных узлов-ячеек, нумерация cправа налево
        /// </summary>
        [JsonIgnore]
        public List<TCell> RealCellsRev;

        /// <summary>
        /// Списко фиктивных узлов-ячеек, расположенных слева. Нумерация справа налево
        /// </summary>
        [JsonIgnore]
        public List<TCell> LeftCells;

        /// <summary>
        /// Списко фиктивных узлов-ячеек, расположенных справа. Нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TCell> RightCells;

        /// <summary>
        /// Список всех узлов-ячеек, нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TCell> AllCells;

        /// <summary>
        /// Список всех узлов-ячеек, нумерация cправа налево
        /// </summary>
        [JsonIgnore]
        public List<TCell> AllCellsRev;

        /// <summary>
        /// Список реальных узлов-границ. Нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TBound> RealBounds;

        /// <summary>
        /// Список реальных узлов-границ. Нумерация справа налево
        /// </summary>
        [JsonIgnore]
        public List<TBound> RealBoundsRev;

        /// <summary>
        /// Список фиктивных узлов-границ, расположенных слева. Нумерация справа налево
        /// </summary>
        [JsonIgnore]
        public List<TBound> LeftBounds;

        /// <summary>
        /// Список фиктивных узлов-границ, расположенных справа. Нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TBound> RightBounds;

        /// <summary>
        /// Список всех узлов-границ. Нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TBound> AllBounds;

        /// <summary>
        /// Список всех узлов-границ. Нумерация слева направо
        /// </summary>
        [JsonIgnore]
        public List<TBound> AllBoundsRev;

        #endregion

        #region Методы генерации списков
        public IEnumerable<TCell> GetAllCells() {
            int indexOfFirstCell = Opt.LeftNodesCount % 2 == 0
                ? 1
                : 0;
            for (int i = indexOfFirstCell; i < Nodes.Count; i += 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetAllBounds() {
            int indexOfFirstBound = Opt.LeftNodesCount % 2 == 0
                ? 0
                : 1;
            for (int i = indexOfFirstBound; i < Nodes.Count; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }
        public IEnumerable<TCell> GetLeftCells() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            for (int i = realBoundsIndexLeft - 1; i >= 0; i -= 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TCell> GetRightCells() {
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexRight + 1; i < Nodes.Count; i += 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TCell> GetRealCells() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexLeft + 1; i < realBoundsIndexRight; i += 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetLeftBounds() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            for (int i = realBoundsIndexLeft - 2; i >= 0; i -= 2) {
                yield return (TBound)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetRightBounds() {
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexRight + 2; i < Nodes.Count; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetRealBounds() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexLeft; i <= realBoundsIndexRight; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }
        #endregion

        #region Методы
        /// <summary>
        /// Возвращает общее количество узлов (узлов-ячейки и узлов-границы), при условии, что каждая ячейка имеет 2 границы.
        /// </summary>
        /// <param name="CellCount">Количество ячеек</param>
        /// <returns>общее количество узлов</returns>
        public static int GetNumOfRealNodes(int CellCount) {
            return CellCount * 2 + 1;
        }

        /// <summary>
        /// Метод генерации пар-троек "левая граница - ячейка - правая граница"
        /// </summary>
        /// <returns>все пары-тройки "левая граница - ячейка - правая граница"</returns>
        public IEnumerable<CellBoundNeibs> GetCellBoundNeibs() {
            int indexOfFirstCell = Opt.LeftNodesCount % 2 == 0
                ? 1
                : 0;
            int indShifter = 0;
            switch (indexOfFirstCell) {
                case 0:
                    yield return new CellBoundNeibs() {
                        cell = AllCells[0],
                        leftB = null,
                        rightB = AllBounds[0]
                    };
                    indShifter = 1;
                    break;
                default:
                    yield return new CellBoundNeibs() {
                        cell = AllCells[0],
                        leftB = AllBounds[0],
                        rightB = AllBounds[1]
                    };
                    break;
            }

            for (int i = 1; i < AllCells.Count - 1; i++) {
                yield return new CellBoundNeibs() {
                    cell = AllCells[i],
                    leftB = AllBounds[i - indShifter],
                    rightB = AllBounds[i + 1 - indShifter]
                };
            }

            bool cellLast = AllCells.Count == (AllBounds.Count + indShifter);
            if (cellLast) {
                yield return new CellBoundNeibs() {
                    cell = AllCellsRev[0],
                    leftB = AllBoundsRev[0],
                    rightB = null
                };
            } else {
                yield return new CellBoundNeibs() {
                    cell = AllCellsRev[0],
                    leftB = AllBoundsRev[1],
                    rightB = AllBoundsRev[0]
                };
            }


        }

        /// <summary>
        /// Класс для метода GetCellBoundNeibs()
        /// </summary>
        public class CellBoundNeibs {
            public TCell cell;
            public TBound leftB, rightB;
        }
        #endregion

        #region Overriting methods
        /// <summary>
        /// Индексирует все Nodes
        /// </summary>
        public override void NodeIndexing() {
            int ind = -5 * Opt.LeftNodesCount;
            for (int i = 0; i < Nodes.Count; i++) {
                Nodes[i].IndexInList = i;
                Nodes[i].Index = ind;
                ind += 5;
            }
        }

        /// <summary>
        /// Инициализация ВСЕХ списков
        /// </summary>
        public override void InitLists() {
            base.InitLists();
            AllBounds = new List<TBound>(GetAllBounds());
            AllBoundsRev = new List<TBound>(AllBounds.Count);
            for (int i = AllBounds.Count - 1; i >= 0; i--) {
                AllBoundsRev.Add(AllBounds[i]);
            }

            AllCells = new List<TCell>(GetAllCells());
            AllCellsRev = new List<TCell>(AllCells.Count);
            for (int i = AllCells.Count - 1; i >= 0; i--) {
                AllCellsRev.Add(AllCells[i]);
            }

            RealCells = new List<TCell>(GetRealCells());
            RealCellsRev = new List<TCell>(RealCells.Capacity);
            for (int i = RealCells.Count - 1; i >= 0; i--) {
                RealCellsRev.Add(RealCells[i]);
            }

            RealBounds = new List<TBound>(GetRealBounds());

            RealBoundsRev = new List<TBound>(RealBounds.Capacity);
            for (int i = RealBounds.Count - 1; i >= 0; i--) {
                RealBoundsRev.Add(RealBounds[i]);
            }

            LeftCells = new List<TCell>(GetLeftCells());
            LeftBounds = new List<TBound>(GetLeftBounds());

            RightBounds = new List<TBound>(GetRightBounds());
            RightCells = new List<TCell>(GetRightCells());
        }
       
        /// <summary>
        /// Инициализация слоя
        /// </summary>
        /// <param name="time">время слоя</param>
        /// <param name="opts">опции слоя</param>
        /// <param name="initCellFunc">функция генерации ячеек</param>
        /// <param name="initBoundFunc">функция генерации границ</param>
        public virtual void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, TCell> initCellFunc, Func<double, double, TBound> initBoundFunc) {
            Opt = opts;
            Time = time;
            Nodes?.Clear();
            int indexOfFirstBound = Opt.LeftNodesCount % 2 == 0
                    ? 1
                    : 0;
            var nds = Enumerable.Range(0, Opt.AllNodesCount)
                .Select(ind => {
                    double x = Opt.X_left + ind * Opt.H;
                    WBOneDemNode nd = ind % 2 == indexOfFirstBound
                      ? (WBOneDemNode)initCellFunc(Time, x)
                      : (WBOneDemNode)initBoundFunc(Time, x);
                    nd.X = x;
                    return nd;
                });
            Nodes.AddRange(nds);

            InitLists();
            NodeIndexing();

            InitBoundCellRefs();
            InitDataRefs();
        }

        /// <summary>
        /// Немного дополненная версия
        /// </summary>
        /// <param name="time"></param>
        /// <param name="opts"></param>
        /// <param name="initCondFunc"></param>
        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, WBOneDemNode> initCondFunc) {
            base.InitLayer(time, opts, initCondFunc);
            InitBoundCellRefs();
            InitDataRefs();
        }

        public override void ActionWhenLoad() {
            base.ActionWhenLoad();
            InitBoundCellRefs();
        }

        public override void CloneLogic(IWBNodeLayer clone) {
            base.CloneLogic(clone);
            (clone as WBOneDemCellLayer<TCell, TBound>).InitBoundCellRefs();
        }

        public override IEnumerable<IWBNode> GetNodesForDraw(string variantName) {
            switch (variantName.ToUpper()) {
                case "ALLBOUNDS":
                    return AllBounds;
                case "BOUNDS":
                    return RealBounds;
                case "ALLCELLS":
                    return AllCells;
                default:
                    return RealCells;                    
            }
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// В этом методе нужно определить, что делать с парами-тройками "левая граница - ячейка - правая граница"
        /// Их генерацией занимается функция GetCellBoundNeibs()
        /// </summary>
        public abstract void InitBoundCellRefs();

        /// <summary>
        /// В этом методе нужно передавать в ячейки/границы все внешние связи (типа геометрий/опций и т.д.)
        /// </summary>
        public abstract void InitDataRefs();
        #endregion
    }
}
