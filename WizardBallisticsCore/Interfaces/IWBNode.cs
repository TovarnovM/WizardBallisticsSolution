using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.Interfaces {
    /// <summary>
    /// Интерфейс для узла/ячейки/частицы и т.д.
    /// </summary>
    public interface IWBNode {
        /// <summary>
        /// Индекс узла в свойстве Nodes в слое 
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Создать клона
        /// </summary>
        /// <returns>клон</returns>
        IWBNode Clone();
    }
}
