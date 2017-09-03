using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public interface IWBNodeLayer {
        /// <summary>
        /// Время слоя
        /// </summary>
        double Time { get; set; }
        /// <summary>
        /// Клонировать слой
        /// </summary>
        /// <returns></returns>
        IWBNodeLayer Clone();

        IEnumerable<IWBNode> GetNodesForDraw(string variantName);

        void ActionWhenLoad();
    }


    /// <summary>
    /// Интерфейс для временнОго слоя узлов
    /// </summary>
    public interface IWBNodeLayer<T>: IWBNodeLayer where T : IWBNode {
        /// <summary>
        /// Узлы
        /// </summary>
        List<T> Nodes {get;set;}
    }


    public interface IWB_ODE_Layer<TCell, TBound>:IWBNodeLayer<TCell> where TCell : WBOneDemNode where TBound : WBOneDemNode {
        WBOneDemCellLayer<TCell, TBound> ComplexStep(double tau, bool synch, IList<ComplexStepContainer<TCell, TBound>> rightPart);
    }
}
