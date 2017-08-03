using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.BaseClasses {
    public abstract class WBGridBase : IWBGrid {
        #region Constructors
        public WBGridBase(string name, IWBNodeLayer initLayer) {
            SaveTactic = new WBGridSaveTacticBase();
            SaveTactic.OwnerGrid = this;
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
        /// тактика сохранения данных / контрля памяти
        /// </summary>
        public WBGridSaveTacticBase SaveTactic { get; set; }
        /// <summary>
        /// Текущее время
        /// </summary>
        public double TimeCurr => CurrLayer.Time;
        /// <summary>
        /// Имя сетки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список функция для остановки расчета
        /// </summary>
        public List<Func<bool>> StopFuncList = new List<Func<bool>>();
        #endregion

        #region Methods
        /// <summary>
        /// добавляет еще один слой в начало LayerList, клонирует с CurrLayer, добавляет deltaTau
        /// </summary>
        /// <param name="deltaTau"></param>
        void CloneUp(double deltaTau = 0d) {
            LayerList.AddFirst(CurrLayer.Clone());
            CurrLayer.Time += deltaTau;
        }

        /// <summary>
        /// функция эволюции секти за шаг по времени с применением логики сохранения/контроля памяти
        /// </summary>
        /// <param name="deltaTau">шаг по времени</param>
        public void StepUp(double deltaTau) {
            StepUpLogic(deltaTau);
            SaveTactic.WhatToDo();
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
}
