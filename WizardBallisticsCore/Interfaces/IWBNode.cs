using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics {
    /// <summary>
    /// Базовый интерфейс для узла/ячейки/области
    /// </summary>
    public interface IWBNode {
        IWBNode Clone();
        T Clone<T>();
    }
}
