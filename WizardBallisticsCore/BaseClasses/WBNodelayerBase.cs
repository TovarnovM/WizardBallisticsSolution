using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WizardBallistics.Core {
    [Serializable]
    public abstract class WBNodeLayerBase<T> : IWBNodeLayer<T> where T : IWBNode {
        /// <summary>
        /// Все все все Узлы
        /// </summary>
        public List<T> Nodes { get; set; } = new List<T>();

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
            CloneLogic(layerclone);
            return layerclone;
        }

        /// <summary>
        /// логика/действия при копировании
        /// </summary>
        public abstract void CloneLogic(IWBNodeLayer clone);
        /// <summary>
        /// логика/действия при загрузке из файла
        /// </summary>
        public abstract void ActionWhenLoad();
        public abstract IEnumerable<IWBNode> GetNodesForDraw(string variantName);
    }
}
