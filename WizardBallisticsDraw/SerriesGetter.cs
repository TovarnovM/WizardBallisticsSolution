using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.OneDemGrid;

namespace WizardBallisticsDraw {
    public static class SerriesGetter {
        class DataDummy {
            public double X { get; set; }
            public double Y { get; set; }
            public DataDummy(double x, double y) {
                X = x;
                Y = y;
            }
        }
        static List<DataDummy> GetDataDummiesList(this IEnumerable<WBOneDemNode> nodes, 
            string yAxisFieldName, 
            string xAxisFieldName = nameof(WBOneDemNode.X), 
            double yScaler = 1d,
            double xScaler = 1d) 
        {
            
            var xPoints = nodes.Values(xAxisFieldName);
            var yPoints = nodes.Values(yAxisFieldName);
            return xPoints.Zip(yPoints, (x, y) => new DataDummy(x*xScaler, y*yScaler)).ToList();
        }
        public static ScatterSeries GetScatterSerries(this IEnumerable<WBOneDemNode> nodes,
            string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode.X),
            double yScaler = 1d,
            double xScaler = 1d)  {

            try {
                return new ScatterSeries() {
                    DataFieldX = nameof(DataDummy.X),
                    DataFieldY = nameof(DataDummy.Y),
                    ItemsSource = nodes.GetDataDummiesList(yAxisFieldName, xAxisFieldName, yScaler, xScaler),
                    Title = yAxisFieldName
                };
            } catch (Exception e) {
                return new ScatterSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
        public static LineSeries GetLineSerries(this IEnumerable<WBOneDemNode> nodes,
            string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode.X),
            double yScaler = 1d,
            double xScaler = 1d)  {

            try {
                return new LineSeries() {
                    DataFieldX = nameof(DataDummy.X),
                    DataFieldY = nameof(DataDummy.Y),
                    ItemsSource = nodes.GetDataDummiesList(yAxisFieldName, xAxisFieldName, yScaler, xScaler),
                    Title = yAxisFieldName
                };
            } catch (Exception e) {
                return new LineSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
    }
}
