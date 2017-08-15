using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.BaseClasses {
    /// <summary>
    /// Базовый класс для описания тактики сохранения данных / контрля памяти у IWBGrid
    /// </summary>
    public class WBMemTacticBase {
        /// <summary>
        /// Собственно какую IWBGrid обслуживаем
        /// </summary>
        [JsonIgnore]
        public IWBGrid OwnerGrid { get; set; }
        /// <summary>
        /// Что делать на каждом шаге?
        /// </summary>
        public virtual void StepWhatToDo() { 
            // пока ничего
        }

        public virtual void LoadWhatToDo() {
            var currAndPastlayers = OwnerGrid.LayerList
                .Where(lr => lr.Time <= OwnerGrid.TimeCurr)
                .OrderBy(lr => lr.Time)
                .ToList();
            OwnerGrid.LayerList.Clear();
            foreach (var lr in currAndPastlayers) {
                OwnerGrid.LayerList.AddFirst(lr);
            }
        }

        public WBMemTacticBase(IWBGrid OwnerGrid) {
            this.OwnerGrid = OwnerGrid;
        }
        public WBMemTacticBase() {
        }
    }
}
