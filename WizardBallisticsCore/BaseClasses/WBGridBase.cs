using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.BaseClasses {
    public abstract class WBGridBase: IWBGrid  {
        #region Constructors
        public WBGridBase(string name, IWBNodeLayer initLayer) {
            Slaver = new WBMemTacticBase(this);
            LayerList.AddFirst(initLayer);
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Здесь хранятся текущий временной слой узллов и несколько предыдущих
        /// </summary>
        public LinkedList<IWBNodeLayer> LayerList { get; } = new LinkedList<IWBNodeLayer>();
        /// <summary>
        /// Здесь можно получить самый актуальный по времени слой узлов
        /// </summary>
        public IWBNodeLayer CurrLayer => LayerList.First();
        /// <summary>
        /// тактика для контрля памяти
        /// </summary>
        public WBMemTacticBase Slaver { get; set; }
        /// <summary>
        /// Текущее время
        /// </summary>
        public double TimeCurr { get; set; }
        /// <summary>
        /// Имя сетки
        /// </summary>
        public string Name { get; set; }

        public IWBNodeLayer this[int index] {
            get {
                var ll = LayerList.Last;
                for (int i = 0; i < index; i++) {
                    ll = ll.Previous;
                }
                return ll.Value;
            }
        }

        /// <summary>
        /// Список функция для остановки расчета
        /// </summary>
        public List<Func<bool>> StopFuncList = new List<Func<bool>>();
        #endregion

        #region Methods
        /// <summary>
        /// добавляет еще один слой в начало LayerList
        /// </summary>
        /// <param name="deltaTau"></param>
        public void AddLayer(IWBNodeLayer layer) {
            LayerList.AddFirst(layer);
        }

        

        /// <summary>
        /// функция эволюции секти за шаг по времени с применением логики сохранения/контроля памяти
        /// </summary>
        /// <param name="deltaTau">шаг по времени</param>
        public void StepUp(double deltaTau) {
            StepUpLogic(deltaTau);
            Slaver.StepWhatToDo();
            TimeCurr += deltaTau;
        }

        /// <summary>
        /// добавить стоп-функцию
        /// </summary>
        /// <param name="stopF"></param>
        public void AddStopFunc(Func<bool> stopF) {
            StopFuncList.Add(stopF);
        }
        /// <summary>
        /// Функция остановки расчета
        /// </summary>
        /// <returns>надо ли остановить расчет?</returns>
        public bool StopCalculating() {
            foreach (var sf in StopFuncList) {
                if (sf())
                    return true;
            }
            return false;
        }
        #endregion

        #region IO


        public GridSaveLoadObj GetSaveObj() {
            return new GridSaveLoadObj() {
                Time = this.TimeCurr,
                Layers = LayerList,
                MemTactic = Slaver
            };
        }

        public bool Load(GridSaveLoadObj loadObj, double objTime) {
            try {
                var obj = (GridSaveLoadObj)loadObj;
                TimeCurr = obj.Time;
                LayerList.Clear();
                foreach (var lr in obj.Layers.OrderBy(ll => ll.Time)) {
                    LayerList.AddFirst(lr);
                }
                Slaver = obj.MemTactic;

                Slaver.LoadWhatToDo();

            } catch (Exception) {
                return false;
            }
            return true;
        }
        #endregion

        #region AbstractMethods
        /// <summary>
        /// Функция для получения текущего максимального шага по времени
        /// </summary>
        /// <returns>максимальный шаг по времени</returns>
        public abstract double GetMaxTimeStep();
        /// <summary>
        /// функция обмена/получения внешней информации
        /// </summary>
        public abstract void InfoСommunication();
        /// <summary>
        /// функция эволюции секти за шаг по времени
        /// </summary>
        /// <param name="deltaTau">шаг по времени</param>
        public abstract void StepUpLogic(double deltaTau);
        #endregion
    }

    public class GridSaveLoadObj {
        public double Time { get; set; }
        public LinkedList<IWBNodeLayer> Layers { get; set; }
        public WBMemTacticBase MemTactic { get; set; }
    }
}
