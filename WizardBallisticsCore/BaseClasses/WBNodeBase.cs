using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.BaseClasses {
    [Serializable]
    public class WBNodeBase : IWBNode {
        /// <summary>
        /// Индекс узла в свойстве Nodes в слое 
        /// </summary>
        public int Index { get; set; } = -1;
        /// <summary>
        /// Создать клона
        /// </summary>
        /// <returns>клон</returns>
        public IWBNode Clone() {
            var ndClone = (IWBNode)this.MemberwiseClone();
            return ndClone;
        }
    }
}
