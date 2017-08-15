using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore {

    /// <summary>
    /// Базовый класс для решения/вычисления
    /// </summary>
    /// <typeparam name="T">тип ячейки/узла</typeparam>
    public class WBSolver {

        string _stopReason = "";

        /// <summary>
        /// Здесь хранятся все сетки для вычисления
        /// </summary>
        public Dictionary<string,IWBGrid> Grids { get; set; }

        /// <summary>
        /// Текущий статус решателя
        /// </summary>
        public string State { get; protected set; }

        /// <summary>
        /// Максимальный шаг по времени
        /// </summary>
        public double MaxTimeStep { get; set; } = 100;

        public int Iteration { get; set; }

        public double TimeCurr { get; set; }

        public WBProjectOptions Options { get; set; }

        public WBSolver(IEnumerable<IWBGrid> grids, WBProjectOptions Options) {
            State = "initial";
            if(grids != null) {
                Grids = new Dictionary<string, IWBGrid>(grids.Count());
                foreach (var gr in grids) {
                    var key = gr.Name;
                    int index = 1;
                    while (Grids.ContainsKey(key)) {
                        key = gr.Name + $"{index++}";
                    }
                    Grids.Add(key, gr);
                }
            }
                
            this.Options = Options;
        }

        
        /// <summary>
        /// Собственная глобальная стоп функция
        /// </summary>
        public Func<WBSolver,bool> MyStopFunc;

        private object locker = new object();
        private bool stopFlag;

        public void Stop() {
            lock (locker) {
                stopFlag = true;
            }
        }

        bool StopCalculating() {
            bool flag = false;
            if (stopFlag) {
                lock (locker) {
                    flag = true;
                    _stopReason += " Stopped";
                }

            }
            foreach (var gr in Grids.Values) {
                if (gr.StopCalculating()) {
                    _stopReason += " " + gr.Name;
                    flag = true;
                }       
            }
            if (MyStopFunc?.Invoke(this) == true) {
                _stopReason += " MyStopFunc";
                flag = true;
            }
            return flag;
        }
        public void SynchTimes() {
            SynchTimes(TimeCurr);
        }

        public void SynchTimes(double time) {
            TimeCurr = time;
            foreach (var gr in Grids.Values) {
                gr.TimeCurr = time;
            }
        }

        /// <summary>
        /// Основная функция, в ней производятся все вычисления сеток
        /// После расчета в свойстве State сохраняется информация о причинах остановки расчета
        /// </summary>
        public void RunCalc() {
            try {
                _stopReason = "";
                State = "calculating";
                stopFlag = false;
                SynchTimes();
                while (!StopCalculating()) {
                    foreach (var gr in Grids.Values) {
                        //Обмен информацией между сетками
                        gr.InfoСommunication(); 
                    }
                    double timeStep = MaxTimeStep;
                    foreach (var gr in Grids.Values) {
                        var currMTS = gr.GetMaxTimeStep();
                        timeStep = currMTS < timeStep ? currMTS : timeStep;
                    }                   
                    foreach (var gr in Grids.Values) {
                        //Эволюция сеток на минимальный шаг по времени
                        gr.StepUp(timeStep);
                    }
                    TimeCurr += timeStep;
                    Iteration++;
                    SynchTimes();
                    SaveStuff();
                }
                State = "solved. Stop reason = "+_stopReason;
            } catch (Exception e) {
                State = "ошибка при вычислении :" + e.Message;                
            }
        }

        void SaveStuff() {
            throw new NotImplementedException();
        }

        //public static WBSolver Factory(string solName, WBProjectOptions options) {
        //    return SolversFactory.Get(solName, options);
        //}

        #region Save/Load
        class SaveLoadClass {
            public Dictionary<string, object> GridObj { get; set; } 
            public WBProjectOptions Opts { get; set; }
            public int Iter { get; set; }
            public double Time { get; set; }
        }

        public void SaveToFile(string filePath) {
            var slc = new SaveLoadClass() {
                GridObj = new Dictionary<string, object>(),
                Opts = Options
            };
            foreach (var gr in Grids.Values) {
                slc.GridObj.Add(gr.Name,gr.GetSaveObj());
            }
            using (var jsw = new JsonTextWriter(new StreamWriter(filePath))) {
                var settings = new JsonSerializerSettings() {
                    TypeNameHandling = TypeNameHandling.Objects
                };
                var ser = JsonSerializer.Create(settings);
                ser.Serialize(jsw, slc);
            }
        }

        public void LoadFromFile(string filePath) {
            using (var jstr = new JsonTextReader(new StreamReader(filePath))) {              
                var ser = JsonSerializer.Create();
                var slc = ser.Deserialize<SaveLoadClass>(jstr);
                foreach (var obj in slc.GridObj) {
                    Grids[obj.Key].Load(obj.Value, slc.Time);
                }
                Options = slc.Opts;
                Iteration = slc.Iter;
                TimeCurr = slc.Time;
            }
        }

        public string ValidFileName {
            get {
                return $"{Options.PrDir}\\{Options.PrName}_{Iteration}.wbproj";
            }
        }
        #endregion
    }

    public class WBProjectOptions {
        public enum SaveModeVariants { dontSave, tau, iter };

        public string PrName { get; set; }
        public string PrDir { get; set; }

        public double TauSave { get; set; }
        public int IterSave { get; set; }
        public SaveModeVariants SaveMode { get; set; }

        public static WBProjectOptions Default {
            get {
                return new WBProjectOptions();
            }
        }
    }

    //public static partial class SolversFactory {
    //    static Lazy<Dictionary<string, Func<WBProjectOptions, WBSolver>>> generDict = new Lazy<Dictionary<string, Func<WBProjectOptions, WBSolver>>>(
    //        LazyFactory, true);
    //    static Dictionary<string, Func<WBProjectOptions, WBSolver>> LazyFactory() {
    //        return typeof(SolversFactory)
    //            .GetMethods()
    //            .Where(m => m.GetCustomAttributes(typeof(SolverGeneratorAttribute), false).Length > 0)
    //            .ToDictionary(

    //                mi => {
    //                    var attr = (SolverGeneratorAttribute)mi.GetCustomAttributes(typeof(SolverGeneratorAttribute), false)[0];
    //                    return attr.Name;
    //                },
    //                mi => {
    //                    return DelegateBuilder.BuildDelegate<Func<WBProjectOptions, WBSolver>>(mi);

    //                }
    //            );
    //    }

    //    public static WBSolver Get(string name, WBProjectOptions options) {
    //        //var tt = generDict.Value;
    //        //var sss = typeof(SolversFactory)
    //        //    .GetMethods()
    //        //    .Where(m => m.GetCustomAttributes(typeof(SolverGeneratorAttribute), false).Length > 0)
    //        //    .ToList();

    //        //var m1 = sss[0];
    //        //var attr = (SolverGeneratorAttribute)m1.GetCustomAttributes(typeof(SolverGeneratorAttribute), false)[0];
    //        //var f = DelegateBuilder.BuildDelegate<Func<WBProjectOptions, WBSolver>>(m1);
    //        return generDict.Value[name](options);
    //    }

    //    static class DelegateBuilder {
    //        public static T BuildDelegate<T>(MethodInfo method, params object[] missingParamValues) {
    //            var queueMissingParams = new Queue<object>(missingParamValues);

    //            var dgtMi = typeof(T).GetMethod("Invoke");
    //            var dgtRet = dgtMi.ReturnType;
    //            var dgtParams = dgtMi.GetParameters();

    //            var paramsOfDelegate = dgtParams
    //                .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
    //                .ToArray();

    //            var methodParams = method.GetParameters();

    //            if (method.IsStatic) {
    //                var paramsToPass = methodParams
    //                    .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
    //                    .ToArray();

    //                var expr = Expression.Lambda<T>(
    //                    Expression.Call(method, paramsToPass),
    //                    paramsOfDelegate);

    //                return expr.Compile();
    //            } else {
    //                var paramThis = Expression.Convert(paramsOfDelegate[0], method.DeclaringType);

    //                var paramsToPass = methodParams
    //                    .Select((p, i) => CreateParam(paramsOfDelegate, i + 1, p, queueMissingParams))
    //                    .ToArray();

    //                var expr = Expression.Lambda<T>(
    //                    Expression.Call(paramThis, method, paramsToPass),
    //                    paramsOfDelegate);

    //                return expr.Compile();
    //            }
    //        }

    //        private static Expression CreateParam(ParameterExpression[] paramsOfDelegate, int i, ParameterInfo callParamType, Queue<object> queueMissingParams) {
    //            if (i < paramsOfDelegate.Length)
    //                return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);

    //            if (queueMissingParams.Count > 0)
    //                return Expression.Constant(queueMissingParams.Dequeue());

    //            if (callParamType.ParameterType.IsValueType)
    //                return Expression.Constant(Activator.CreateInstance(callParamType.ParameterType));

    //            return Expression.Constant(null);
    //        }
    //    }
    //}
}
