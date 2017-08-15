using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.OneDemGrid {
    /// <summary>
    /// Структура одномерной подвижной эйлеровой сетки
    /// </summary>
    /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
    public struct WBOneDemNode<T> where T : struct {
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
                var firstTry = typeof(WBOneDemNode<T>).GetField(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                if (firstTry == null) {
                    var secondTry = typeof(T).GetField(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                    return secondTry?.GetValue(Data);
                }

                return firstTry?.GetValue(this);
            }
        }

        /// <summary>
        /// Получает все имена double полей в ячейке
        /// </summary>
        /// <returns>имена всех полей</returns>
        public static List<string> GetDataFieldsNames() {
            return GetDataFieldsNames<double>();
        }

        /// <summary>
        /// Получает все имена полей типа T1 в ячейке
        /// </summary>
        /// <typeparam name="T1">тип полей</typeparam>
        /// <returns>имена всех полей</returns>
        public static List<string> GetDataFieldsNames<T1>() {
            return typeof(WBOneDemNode<T>)
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Concat(typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(ti => ti.FieldType == typeof(T1))
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
        public static IEnumerable<T1> GetValues<T1>(IEnumerable<WBOneDemNode<T>> cells, string fieldName) {
            var firstTry = typeof(WBOneDemNode<T>).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
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

    public static class CellExtentions {
        /// <summary>
        /// Лениво выводит значения поля fieldName у коллекции cells
        /// </summary>
        /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
        /// <typeparam name="T1">тип данных поля fieldName</typeparam>
        /// <param name="cells">коллекция узлов WBOneDemNode</param>
        /// <param name="fieldName">имя поля у WBOneDemNode, которое надо выводить</param>
        /// <returns>Ленивое перечисление значений полей </returns>
        public static IEnumerable<T1> Values<T, T1>(this IEnumerable<WBOneDemNode<T>> cells, string fieldName) where T : struct {
            return WBOneDemNode<T>.GetValues<T1>(cells, fieldName);
        }

        /// <summary>
        /// Лениво выводит значения double поля fieldName у коллекции cells
        /// </summary>
        /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
        /// <param name="cells">коллекция узлов WBOneDemNode</param>
        /// <param name="fieldName">имя поля у WBOneDemNode, которое надо выводить</param>
        /// <returns>Ленивое перечисление значений полей </returns>
        public static IEnumerable<double> Values<T>(this IEnumerable<WBOneDemNode<T>> cells, string fieldName) where T : struct {
            try {
                return WBOneDemNode<T>.GetValues<double>(cells, fieldName);
            } catch (Exception) {
                return Enumerable.Repeat<double>(0d, 1);
            }

        }
    }

}
