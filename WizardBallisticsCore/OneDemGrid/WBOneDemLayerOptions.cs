using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    /// <summary>
    /// Опции одномерного слоя
    /// </summary>
    public class WBOneDemLayerOptions {
        /// <summary>
        /// Количество реальных узлов
        /// </summary>
        public int RealNodesCount { get; set; }
        
        /// <summary>
        /// Количество фиктивных узлов слева
        /// </summary>
        public int LeftNodesCount { get; set; }
        
        /// <summary>
        /// Количество фиктивных узлов справа
        /// </summary>
        public int RightNodesCount { get; set; }
        
        /// <summary>
        /// Суммарное количество узлов
        /// </summary>
        public int AllNodesCount => RealNodesCount + LeftNodesCount + RightNodesCount;
        
        /// <summary>
        /// Координата левого реального узла
        /// </summary>
        public double X_left { get; set; }
        
        /// <summary>
        /// Координата правого реального узла
        /// </summary>
        public double X_right { get; set; }
        
        /// <summary>
        /// Шаг сетки
        /// </summary>
        public double H { get; set; }
        
        /// <summary>
        /// Синхронизировать шаг
        /// </summary>
        public void SynchH() {
            H = (X_right - X_left) / RealNodesCount;
        }
        
        /// <summary>
        /// Синхронизировать правую границу
        /// </summary>
        public void SynchX_right() {
            X_right = X_left + RealNodesCount * H;
        }
        
        /// <summary>
        /// синхронизировать левую границу
        /// </summary>
        public void SynchX_left() {
            X_left = X_right - RealNodesCount * H;
        }
    }
}
