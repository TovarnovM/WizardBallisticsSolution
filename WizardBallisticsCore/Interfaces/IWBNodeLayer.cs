using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.Interfaces {
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
    }
    /// <summary>
    /// Интерфейс для временнОго слоя узлов
    /// </summary>
    public interface IWBNodeLayer<T>: IWBNodeLayer where T : struct {
        /// <summary>
        /// Узлы
        /// </summary>
        T[] Nodes {get;set;}
    }
}
