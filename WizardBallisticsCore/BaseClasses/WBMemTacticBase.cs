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
        public double timeStepSave = 0.001;
        public override void StepWhatToDo() {
            double t0 = OwnerGrid.CurrLayer.Time;
            if(t0 - lastGoodTime >= timeStepSave) {
                lastGoodTime = t0;
                var nnode = OwnerGrid.LayerList.First.Next;
                if(nnode != null) {
                    lastGoodTime = nnode.Value.Time;
                }
                return;
            }
            var node = OwnerGrid.LayerList.First.Next;
            while(node != null) {             
                if(node.Value.Time - lastGoodTime < timeStepSave && node.Value.Time > lastGoodTime) {
                    var nextNode = node.Next;
                    node.List.Remove(node);
                    node = nextNode;
                    //var tdel = node.Previous.Value.Time;

                } else {
                    break;
                }
            }
            
        }
        public double lastGoodTime = 0d;

    }

    public class WBMemTacticDellAll : WBMemTacticBase {
        public override void StepWhatToDo() {
            double t0 = OwnerGrid.CurrLayer.Time;
            var node = OwnerGrid.LayerList.First.Next;
            while (node != null) {
                var nodeNext = node.Next;
                OwnerGrid.LayerList.Remove(node);
                node = nodeNext;
            }

        }
    }
}
