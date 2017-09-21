using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Draw {
    public class StandartVM {
        public LinearAxis Xaxis;
        public LinearAxis Yaxis;

        public PlotModel PM { get; set; }
        public StandartVM() {
            PM = GetNewModel();
        }
        public PlotModel GetNewModel(string title = "", string xname = "", string yname = "") {

            var m = new PlotModel { Title = title };
            Xaxis = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                MinimumPadding = 0,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Title = xname
            };
            m.Axes.Add(Xaxis);
            Yaxis = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                MinimumPadding = 0,
                MinorGridlineStyle = LineStyle.Dot,
                Title = yname
            };
            m.Axes.Add(Yaxis);

            return m;
        }
    }
}
