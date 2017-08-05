using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.BaseClasses {
    [Serializable]
    public abstract class WBNodeLayerBase<T> : IWBNodeLayer<T> where T: struct {
        /// <summary>
        /// Узлы
        /// </summary>
        public T[] Nodes { get; set; }
        /// <summary>
        /// Время слоя
        /// </summary>
        public double Time { get; set; } = 0d;
        /// <summary>
        /// Клонировать слой
        /// </summary>
        /// <returns></returns>
        public IWBNodeLayer<T> Clone() {
            var layerclone = (WBNodeLayerBase<T>)this.MemberwiseClone();
            layerclone.Nodes = new T[Nodes.Length];
            Array.Copy(Nodes, layerclone.Nodes, Nodes.Length);
            CloneLogic();
            return layerclone;
        }

        /// <summary>
        /// логика/действия при копировании
        /// </summary>
        public abstract void CloneLogic();
    }
}
