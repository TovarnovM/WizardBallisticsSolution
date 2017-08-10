using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.Interfaces {
    public interface ILayerCollection<T> where T: struct {
        /// <summary>
        /// Здесь хранятся текущий временной слой узллов и несколько предыдущих
        /// </summary>
        LinkedList<IWBNodeLayer<T>> LayerList { get; }
        /// <summary>
        /// Здесь можно получить самый актуальный по времени слой узлов
        /// </summary>
        IWBNodeLayer<T> CurrLayer { get; }
    }
}
