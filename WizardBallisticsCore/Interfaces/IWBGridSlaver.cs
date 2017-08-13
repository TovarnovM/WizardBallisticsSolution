using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.Interfaces {
    public interface IWBGridSlaver<T> where T : struct {
        /// <summary>
        /// Собственно какую IWBGrid обслуживаем
        /// </summary>
        IWBGrid<T> OwnerGrid { get; set; }
        /// <summary>
        /// Что делать на каждом шаге?
        /// </summary>
        void StepWhatToDo();

    }
}
