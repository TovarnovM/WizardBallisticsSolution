using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;

namespace WizardBallisticsCore.OneDemGrid {
    /// <summary>
    /// Структура одномерной подвижной эйлеровой сетки
    /// </summary>
    /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
    public class WBOneDemNode:WBNodeBase {
        /// <summary>
        /// Индекс данного УЗЛА в массиве Nodes в IWBNodeLayer
        /// </summary>
        public int IndexInArray;

        /// <summary>
        /// Индекс узла, показывающий тип узла, елси индекс кратен только 5, то это граница между ячеек, 
        /// если кратен только 10, то этот узел показывает значения в эйлеровой ячейке
        /// Пример индексов: -5, 0, 5, 10, 15, 20, 25. -5,25 - левая и правая границы сетки, 0,10,20 - ячейки, 5,15 - границы, между ячейками
        /// </summary>
        public int Index;

        /// <summary>
        /// координата узла в одномерном пространстве
        /// </summary>
        public double X;

        /// <summary>
        /// скорость узла в одномерном пространстве
        /// </summary>
        public double V;

        ///// <summary>
        ///// Данные для конкретныой задачи
        ///// 
        ///// Пример для идеального газа
        /////     public struct IdealGas {
        /////         public double Ro;
        /////         public double P;
        /////         public static double k;
        /////         public double E {
        /////             get {
        /////                 return P / ((k - 1) * Ro);
        /////             }
        /////         }
        /////     }
        ///// </summary>
        //public T Data;

        
    }

}
