using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace MiracleGun.Invariants
{
    /// <summary>
    /// Интерфейс для класса при помощи которого можно получить значения геометрии в точке X
    /// </summary>
    public interface ISx {
        double GetV(double x);
        string Name { get; set; }
    }

    /// <summary>
    /// Интерфейс для класса при помощи которого можно получить значения геометрии в точке X 
    /// И для получения значения производной геометрии в точке X 
    /// </summary>
    public interface IDSx: ISx {
        double Get_dV(double x);
    }
    
    /// <summary>
    /// класс для хранения информации о форме ствола
    /// Геометрия ствола задается точками на плоскости x-y 
    /// Класс предоставляет возможность быстрой линейной интерполяции значений между заданными точками
    /// </summary>
    public class AbstractShape: ISx, IDSx {
        #region private data
        /// <summary>
        /// отсортированный массив с координатами X
        /// </summary>
        protected double[] x_arr;

        /// <summary>
        /// массив с соответствующими координатами Y
        /// </summary>
        protected double[] y_arr;

        /// <summary>
        /// массив с коэффициентами k для лин уравнений y = kx+b
        /// </summary>
        protected double[] yk_arr;

        /// <summary>
        /// массив с коэффициентами b для лин уравнений y = kx+b
        /// </summary>
        protected double[] yb_arr;

        /// <summary>
        /// флаг, показывающий синхронизированы ли массивы и xd_lst?
        /// </summary>
        protected bool isSynch = false;
        #endregion

        #region public fields
        /// <summary>
        /// здесь хранится информация о точках до синхронизации (для удобства добавления)
        /// </summary>
        public List<(double x, double y)> xd_lst = new List<(double, double)>();

        /// <summary>
        /// Функция преобразования переданных значений y при добавлении данных при помощи AddPoint(x,y)
        /// например мы задаем функцию y => PI*0.25*y*y, добавляем диаметры, а в итоге записываются и сохраняются площади
        /// </summary>
        public Func<double,double> F_preobr { get; set; }

        /// <summary>
        /// имя этого объекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значения функции левее самой левой точки и правее самой правой
        /// </summary>
        public double left_beound = 0d, right_beound = 0;

        /// <summary>
        /// минимальное рассотяние между точками X
        /// </summary>
        public double x_delta = 1E-6;
        #endregion

        #region Конструкторы
        /// <summary>
        /// 
        /// </summary>
        /// <param name="F_preobr">Функция преобразования переданных значений y при добавлении данных при помощи AddPoint(x,y) Например мы задаем функцию y => PI*0.25*y*y, добавляем диаметры, а в итоге записываются и сохраняются площади</param>
        public AbstractShape(Func<double, double> F_preobr) {
            this.F_preobr = F_preobr;
        }

        /// <summary>
        /// По умолчанию преобразования нет
        /// </summary>
        public AbstractShape():this(y=>y) {

        }
        #endregion

        #region Методы/свойства
        /// <summary>
        /// Добавляется точка геометрии
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddPoint(double x, double y) {
            xd_lst.Add((x, y));
            isSynch = false;
        }
        public string InvalidData() {
            var ordr = xd_lst.OrderBy(t => t.x);
            var invD = ordr
                .Zip(ordr.Skip(1), (t1, t2) => (t1, t2))
                .Where(t => Abs(t.Item1.x - t.Item2.x) < x_delta)
                .ToList();
            if (invD.Count == 0) {
                return "";
            }
            return invD
                .Aggregate(
                "Too close data (x,y): \n", 
                (s0, t) => 
                    s0 + $"[({t.Item1.x}; {t.Item1.y}) and ({t.Item2.x}; {t.Item2.y})]\n"
                );
        }
        /// <summary>
        /// Создание массивов на основе данных xd_lst
        /// </summary>
        public virtual void Synch() {
            Clear(false);
            x_arr = xd_lst
                .OrderBy(xs => xs.x)
                .Select(xs => xs.x)
                .ToArray();
            y_arr = xd_lst
                .OrderBy(xs => xs.x)
                .Select(xs => F_preobr(xs.y))
                .ToArray();

            (yk_arr, yb_arr) = GetLineForm(x_arr, y_arr);
            isSynch = true;
        }

        /// <summary>
        /// Инициализация массивов для быстрой линейной интерполяции 
        /// </summary>
        /// <param name="x_arr"></param>
        /// <param name="v_arr"></param>
        /// <returns></returns>
        static (double[] k_arr, double[] b_arr) GetLineForm(double[] x_arr, double[] v_arr) {
            var k_arr = new double[x_arr.Length - 1];
            var b_arr = new double[x_arr.Length - 1];
            for (int i = 0; i < x_arr.Length - 1; i++) {
                k_arr[i] = (v_arr[i + 1] - v_arr[i]) / (x_arr[i + 1] - x_arr[i]);
                b_arr[i] = v_arr[i] - k_arr[i] * x_arr[i];
            }
            return (k_arr, b_arr);
        }

        /// <summary>
        /// очистка данных
        /// </summary>
        /// <param name="clearList">очищать ли лист?</param>
        public void Clear(bool clearList = true) {
            x_arr = null;
            y_arr = null;
            yk_arr = null;
            yb_arr = null;
            if (clearList)
                xd_lst.Clear();
            isSynch = true;
        }

        /// <summary>
        /// Собственно главная функция получения интерполированного значения Y находящегося по координате x
        /// </summary>
        /// <param name="x">абсцисса</param>
        /// <returns>интерполинованное значения</returns>
        public double GetV(double x) {
            if (!isSynch)
                Synch();
            int ind = ~Array.BinarySearch(x_arr, x);
            if (ind < 0) {
                return y_arr[~ind];
            }
            if (ind == 0) {
                return left_beound;
            } else if (ind == x_arr.Length) {
                return right_beound;
            }

            return yk_arr[ind - 1] * x + yb_arr[ind - 1];
        }

        /// <summary>
        /// Получение производной поверхности в данной точке
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Get_dV(double x) {
            if (!isSynch)
                Synch();
            int ind = ~Array.BinarySearch(x_arr, x);
            if (ind < 0) {
                return Get_dV(x - x_delta);// + Get_dV(x + x_delta));
            }
            if (ind == 0 || ind == x_arr.Length) {
                return 0;
            }
            return yk_arr[ind - 1];
        }

        public double Get_dV(double x1, double x2) {
            if (!isSynch)
                Synch();
            return (GetV(x2) - GetV(x1)) / (x2 - x1);
        }
        #endregion

        /// <summary>
        /// Получить объем участка ствола от x1 до x2
        /// Применять только при хранении диаметров
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public double GetW(double x1, double x2) {
            double w = 0d;
            double d1 = GetV(x1);
            int ind1 = ~Array.BinarySearch(x_arr, x1);
            if (ind1 < 0)
                ind1 = ~ind1;
            if (ind1 == 0) {
                w += PI * 0.25 * d1 * d1 * (x_arr[0] - x1);
                x1 = x_arr[0];
                d1 = y_arr[0];
            }
            int ind2 = ~Array.BinarySearch(x_arr, x2);
            if (ind2 < 0)
                ind2 = ~ind2;
            if (ind2 == x_arr.Length) {
                var d2 = right_beound;
                w += PI * 0.25 * d2 * d2 * (x2 - x_arr[x_arr.Length - 1]);
                x2 = x_arr[x_arr.Length-1];
            }
            if (ind2 == 0) {
                return w;
            }
            for (int i = ind1; i < ind2; i++) {
                w += ConeW(d1, y_arr[i], (x_arr[i] - x1));
                x1 = x_arr[i];
                d1 = y_arr[i];
            }
            w += ConeW(d1, GetV(x2), x2 - x1);
            return w;
        }

        /// <summary>
        /// Получить площадь внутренней поверхности участка ствола от x1 до x2
        /// Применять только при хранении диаметров
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public double GetS(double x1, double x2) {
            double s = 0d;
            double d1 = GetV(x1);
            int ind1 = ~Array.BinarySearch(x_arr, x1);
            if (ind1 < 0)
                ind1 = ~ind1;
            if (ind1 == 0) {
                s += PI * d1 * (x_arr[0] - x1);
                x1 = x_arr[0];
                d1 = y_arr[0];
            }
            int ind2 = ~Array.BinarySearch(x_arr, x2);
            if (ind2 < 0)
                ind2 = ~ind2;
            if (ind2 == x_arr.Length) {
                var d2 = right_beound;
                s += PI * d2 * (x2 - x_arr[x_arr.Length - 1]);
                x2 = x_arr[x_arr.Length - 1];
            }
            if (ind2 == 0) {
                return s;
            }
            for (int i = ind1; i < ind2; i++) {
                s += ConeS(d1, y_arr[i], (x_arr[i] - x1));
                x1 = x_arr[i];
                d1 = y_arr[i];
            }
            s += ConeS(d1, GetV(x2), x2 - x1);
            return s;
        }
        static double ConeW(double d1, double d2, double h) {
            return PI * h * (d1 * d1 + d1 * d2 + d2 * d2) / 12d;
        }
        static double ConeS(double d1, double d2, double h) {
            double l = Sqrt(h * h + (d1 - d2) * (d1 - d2));
            return PI * l * (d1 + d2) / 2d;
        }
    }
}
