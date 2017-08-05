using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore {

    /// <summary>
    /// Базовый класс для решения/вычисления
    /// </summary>
    /// <typeparam name="T">тип ячейки/узла</typeparam>
    public class WBSolver<T> where T : struct {

        string _stopReason = "";

        /// <summary>
        /// Здесь хранятся все сетки для вычисления
        /// </summary>
        public List<IWBGrid<T>> Grids { get; set; }

        /// <summary>
        /// Текущий статус решателя
        /// </summary>
        public string State { get; protected set; }

        /// <summary>
        /// Максимальный шаг по времени
        /// </summary>
        public double MaxTimeStep { get; set; } = 100;

        public WBSolver(IEnumerable<IWBGrid<T>> grids) {
            State = "initial";
            Grids = new List<IWBGrid<T>>(grids);
        }

        
        /// <summary>
        /// Собственная глобальная стоп функция
        /// </summary>
        public Func<bool> MyStopFunc;

        bool StopCalculating() {
            bool flag = false;
            foreach (var gr in Grids) {
                if (gr.StopCalculating()) {
                    _stopReason += " " + gr.Name;
                    flag = true;
                }       
            }
            if (MyStopFunc?.Invoke() == true) {
                _stopReason += " MyStopFunc";
                flag = true;
            }
            return flag;
        }
        double MinArrVal(double[] arr) {
            var minV = arr[0];
            for (int i = 1; i < arr.Length; i++) {
                if (arr[i] < minV)
                    minV = arr[i];
            }
            return minV;
        }

        /// <summary>
        /// Основная функция, в ней производятся все вычисления сеток
        /// После расчета в свойстве State сохраняется информация о причинах остановки расчета
        /// </summary>
        public void RunCalc() {
            try {

                State = "calculating";
                var timeSteps = new double[Grids.Count + 1]; //хранятся максимальные шаги по времени для каждой сетки
                timeSteps[timeSteps.Length - 1] = MaxTimeStep;
                while (!StopCalculating()) {
                    foreach (var gr in Grids) {
                        //Обмен информацией между сетками
                        gr.InfoСommunication(); 
                    }
                    for (int i = 0; i < Grids.Count; i++) {
                        //сбор информации о шагах по времени
                        timeSteps[i] = Grids[i].GetMaxTimeStep();
                    }
                    double timeStep = MinArrVal(timeSteps);
                    foreach (var gr in Grids) {
                        //Эволюция сеток на минимальный шаг по времени
                        gr.StepUp(timeStep);
                    }
                }
                State = "solved. Stop reason = "+_stopReason;
            } catch (Exception e) {
                State = "ошибка при вычислении :" + e.Message;                
            }
        }

        public WBSolver() {

        }
    }
}
