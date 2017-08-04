using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.OneDemCellGrid {
    //===========================================================================
    //В процессе

    /// <summary>
    /// Шаблон для одномерной подвижной эйлеровой сетки
    /// </summary>
    /// <typeparam name="T">Структура данных для ячейки/задачи</typeparam>
    public class WBOneDemCellLayer<T> : WBNodeLayerBase<WBOneDemCell<T>> where T : struct {
        public void SynchXV() {
            var x0 = LeftBorder.X;
            var dx = (RightBorder.X - x0) / Nodes.Length;
            var v0 = LeftBorder.V;
            var dv = (RightBorder.V- v0) / Nodes.Length;
            for (int i = 1; i < Nodes.Length-1; i++) {
                Nodes[i].X = x0 + i * dx;
                Nodes[i].V = v0 + i * dv;
            }
        }
        public ref WBOneDemCell<T> LeftBorder { get { return ref Nodes[0]; } }
        public ref WBOneDemCell<T> RightBorder => ref Nodes[Nodes.Length-1];
        //public ref WBOneDemCell<T> Cells(int index) {

        //}
        private int myVar;

        public int MyProperty {
            get { return myVar; }
            set { myVar = value; }
        }

        public override void CloneLogic() {
            throw new NotImplementedException();
        }
    }
}
