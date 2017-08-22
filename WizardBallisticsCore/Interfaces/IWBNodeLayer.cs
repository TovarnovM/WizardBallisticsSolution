using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics {
    public interface IWBNodeLayer {
        /// <summary>
        /// Время слоя
        /// </summary>
        double Time { get; set; }
        /// <summary>
        /// Клонировать слой
        /// </summary>
        /// <returns></returns>
        IWBNodeLayer Clone();

        void ActionWhenLoad();
    }
    /// <summary>
    /// Интерфейс для временнОго слоя узлов
    /// </summary>
    public interface IWBNodeLayer<T>: IWBNodeLayer where T : IWBNode {
        /// <summary>
        /// Узлы
        /// </summary>
        List<T> Nodes {get;set;}
    }
}
