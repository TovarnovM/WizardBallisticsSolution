using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.Interfaces {
    /// <summary>
    /// Интерфейс для расчетной сетки
    /// </summary>
    public interface IWBGrid<T> where T:struct {
        /// <summary>
        /// Здесь хранятся текущий временной слой узллов и несколько предыдущих
        /// </summary>
        LinkedList<IWBNodeLayer<T>> LayerList { get; }
        /// <summary>
        /// Здесь можно получить самый актуальный по времени слой узлов
        /// </summary>
        IWBNodeLayer<T> CurrLayer { get; }
        /// <summary>
        /// Функция для получения текущего максимального шага по времени
        /// </summary>
        /// <returns>максимальный шаг по времени</returns>
        double GetMaxTimeStep();
        /// <summary>
        /// Текущее время
        /// </summary>
        double TimeCurr { get; }
        /// <summary>
        /// Имя сетки
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Функция остановки расчета
        /// </summary>
        /// <returns>надо ли остановить расчет?</returns>
        bool StopCalculating();
        /// <summary>
        /// функция эволюции секти за шаг по времени
        /// </summary>
        /// <param name="deltaTau">шаг по времени</param>
        void StepUp(double deltaTau);
        /// <summary>
        /// функция обмена/получения внешней информации
        /// </summary>
        void InfoСommunication();
    }
}
