using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.BaseClasses {
    [Serializable]
    public class WBNodelayerBase : IWBNodeLayer {
        /// <summary>
        /// Узлы
        /// </summary>
        public List<IWBNode> Nodes { get; set; } = new List<IWBNode>();
        /// <summary>
        /// Время слоя
        /// </summary>
        public double Time { get; set; } = 0d;
        /// <summary>
        /// Клонировать слой
        /// </summary>
        /// <returns></returns>
        public IWBNodeLayer Clone() {
            var layerclone = (IWBNodeLayer)this.MemberwiseClone();
            layerclone.Nodes = new List<IWBNode>(this.Nodes.Capacity);
            foreach (var nd in Nodes) {
                layerclone.Nodes.Add(nd.Clone());
            }
            return layerclone;
        }
    }
}
