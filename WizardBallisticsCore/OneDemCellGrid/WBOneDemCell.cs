using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.OneDemCellGrid {
    /// <summary>
    /// Структура одномерной подвижной эйлеровой сетки
    /// </summary>
    /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
    public struct WBOneDemCell<T> where T:struct {
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

        /// <summary>
        /// Данные для конкретныой задачи
        /// 
        /// Пример для идеального газа
        ///     public struct IdealGas {
        ///         public double Ro;
        ///         public double P;
        ///         public static double k;
        ///         public double E {
        ///             get {
        ///                 return P / ((k - 1) * Ro);
        ///             }
        ///         }
        ///     }
        /// </summary>
        public T Data;

        /// <summary>
        /// Получает значение поля fieldKey, используя Reflection
        /// </summary>
        /// <param name="fieldKey">имя поля</param>
        /// <returns>значение поля</returns>
        public object this[string fieldKey] {
            get {
                var firstTry = typeof(WBOneDemCell<T>).GetField(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                if (firstTry == null) {
                    var secondTry = typeof(T).GetField(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                    return secondTry?.GetValue(Data);
                }

                return firstTry?.GetValue(this);
            }
        }

        /// <summary>
        /// Получает все имена полей в ячейке
        /// </summary>
        /// <returns>имена всех полей</returns>
        public static List<string> GetDataFieldsNames() {
            return typeof(WBOneDemCell<T>)
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Concat(typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Select(ti => ti.Name)
                .ToList();
        }


        /// <summary>
        /// Получение значений полей fieldName у набора узлов cells, приведенных к типу T1
        /// Желательно не использовать в многократных вычислениях
        /// </summary>
        /// <typeparam name="T1">тип поля fieldName</typeparam>
        /// <param name="cells">набор узлов у которых нужно получить значения полей</param>
        /// <param name="fieldName">имя поля</param>
        /// <returns>значения полей fieldName</returns>
        public static IEnumerable<T1> GetValues<T1>(IEnumerable<WBOneDemCell<T>> cells, string fieldName) {
            var firstTry = typeof(WBOneDemCell<T>).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (firstTry == null) {
                var secondTry = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (secondTry == null) {
                    return null;
                }
                return cells.Select(c => (T1)secondTry.GetValue(c.Data));
            }
            return cells.Select(c => (T1)firstTry.GetValue(c));
        } 


    }
        

}
