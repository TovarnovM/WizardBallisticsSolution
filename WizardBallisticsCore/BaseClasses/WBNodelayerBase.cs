using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WizardBallisticsCore.BaseClasses {
    [Serializable]
    public abstract class WBNodeLayerBase<T> : IWBNodeLayer<T> where T : IWBNode {
        /// <summary>
        /// Узлы
        /// </summary>
        public List<T> Nodes { get; set; }
        /// <summary>
        /// Время слоя
        /// </summary>
        public double Time { get; set; } = 0d;
        /// <summary>
        /// Клонировать слой
        /// </summary>
        /// <returns></returns>
        public IWBNodeLayer Clone() {
            var layerclone = (WBNodeLayerBase<T>)this.MemberwiseClone();
            layerclone.Nodes = new List<T>(Nodes.Capacity);
            foreach (var node in Nodes) {
                layerclone.Nodes.Add(node.Clone<T>());
            }
            CloneLogic();
            return layerclone;
        }

        /// <summary>
        /// логика/действия при копировании
        /// </summary>
        public abstract void CloneLogic();

    }
}
