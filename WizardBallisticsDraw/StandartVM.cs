using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsDraw {
    public class StandartVM {
        public PlotModel PM { get; set; }
        public StandartVM() {
            PM = GetNewModel();
        }
        public static PlotModel GetNewModel(string title = "", string xname = "", string yname = "") {

            var m = new PlotModel { Title = title };
            var linearAxis1 = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                MinimumPadding = 0,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Title = xname
            };
            m.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                MinimumPadding = 0,
                MinorGridlineStyle = LineStyle.Dot,
                Title = yname
            };
            m.Axes.Add(linearAxis2);

            return m;
        }
    }
}
