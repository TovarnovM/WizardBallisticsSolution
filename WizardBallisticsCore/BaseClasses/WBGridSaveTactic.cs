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
    public class WBGridSaveTacticBase<T> where T : struct {
        /// <summary>
        /// Собственно какую IWBGrid обслуживаем
        /// </summary>
        public ILayerCollection<T> OwnerGrid { get; set; }
        /// <summary>
        /// Что делать на каждом шаге?
        /// </summary>
        public virtual void WhatToDo() { 
            // пока ничего
        }
    }
}
