using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.BaseClasses {
    public class WBNodeBase : IWBNode {
        public IWBNode Clone() {
            return (IWBNode)this.MemberwiseClone();
        }

        public T Clone<T>() {
            return (T)this.MemberwiseClone();
        }
        /// <summary>
        /// Получает значение поля/свойства fieldKey, используя Reflection
        /// </summary>
        /// <param name="fieldKey">имя поля</param>
        /// <returns>значение поля</returns>
        public object this[string fieldKey] {
            get {
                var firstTry = this.GetType().GetField(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                if (firstTry == null) {
                    var secondTry = this.GetType().GetProperty(fieldKey, BindingFlags.Public | BindingFlags.Instance);
                    return secondTry?.GetValue(this);
                }

                return firstTry?.GetValue(this);
            }
        }

        /// <summary>
        /// Получает все имена double полей в ячейке
        /// </summary>
        /// <returns>имена всех полей</returns>
        public List<string> GetDataFieldsNames() {
            return GetDataFieldsNames<double>();
        }

        /// <summary>
        /// Получает все имена полей типа T1 в ячейке
        /// </summary>
        /// <typeparam name="T1">тип полей</typeparam>
        /// <returns>имена всех полей</returns>
        public List<string> GetDataFieldsNames<T1>() {
            var props = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                // .Concat(typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(ti => ti.PropertyType == typeof(T1))
                .Select(ti => ti.Name);
            return this.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                // .Concat(typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                .Where(ti => ti.FieldType == typeof(T1))
                .Select(ti => ti.Name)
                .Concat(props)
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
        public static IEnumerable<T1> GetValues<T1>(IEnumerable<WBNodeBase> cells, string fieldName) {
            var tp = cells.First().GetType();
            var firstTry = tp.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (firstTry == null) {
                var secondTry = tp.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (secondTry == null) {
                    return null;
                }
                return cells.Select(c => (T1)secondTry.GetValue(c));
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
        public static IEnumerable<T1> Values<T1>(this IEnumerable<WBNodeBase> cells, string fieldName) {
            return WBNodeBase.GetValues<T1>(cells, fieldName);
        }

        /// <summary>
        /// Лениво выводит значения double поля fieldName у коллекции cells
        /// </summary>
        /// <typeparam name="T">структура данных для конкретной задачи</typeparam>
        /// <param name="cells">коллекция узлов WBOneDemNode</param>
        /// <param name="fieldName">имя поля у WBOneDemNode, которое надо выводить</param>
        /// <returns>Ленивое перечисление значений полей </returns>
        public static IEnumerable<double> Values(this IEnumerable<WBNodeBase> cells, string fieldName) {
            try {
                return WBNodeBase.GetValues<double>(cells, fieldName);
            } catch (Exception) {
                return Enumerable.Repeat<double>(0d, 1);
            }

        }
    }
}
