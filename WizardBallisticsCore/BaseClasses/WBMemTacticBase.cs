using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    /// <summary>
    /// Базовый класс для описания тактики сохранения данных / контрля памяти у IWBGrid
    /// </summary>
    public class WBMemTacticBase {
        /// <summary>
        /// Собственно какую IWBGrid обслуживаем
        /// </summary>
        public IWBGrid OwnerGrid { get; set; }
        /// <summary>
        /// Что делать на каждом шаге?
        /// </summary>
        public virtual void StepWhatToDo() { 
            // пока ничего
        }

        public virtual void LoadWhatToDo() {
            //var currAndPastlayers = OwnerGrid.LayerList
            //    .Where(lr => lr.Time <= OwnerGrid.TimeCurr)
            //    .OrderBy(lr => lr.Time)
            //    .ToList();
            //OwnerGrid.LayerList.Clear();
            //foreach (var lr in currAndPastlayers) {
            //    OwnerGrid.LayerList.AddFirst(lr);
            //}
        }

        public WBMemTacticBase(IWBGrid OwnerGrid) {
            this.OwnerGrid = OwnerGrid;
        }
        public WBMemTacticBase() {
        }
    }

    public class WBMemTacticTimeStep: WBMemTacticBase {
        public double timeStemSave = 0.00001;
        public override void StepWhatToDo() {
            double t0 = OwnerGrid.CurrLayer.Time;
            var node = OwnerGrid.LayerList.First.Next;
            while(node != null && node.Next != null) {                
                if(t0 - node.Value.Time < timeStemSave) {
                    node = node.Next;
                    var tdel = node.Previous.Value.Time;
                    node.List.Remove(node.Previous);
                } else {
                    break;
                }
            }
            
        }
    }
}
