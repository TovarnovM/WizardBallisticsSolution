using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore;
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
        static List<DataDummy> GetDataDummiesList<T>(this IEnumerable<WBOneDemNode<T>> nodes, 
            string yAxisFieldName, 
            string xAxisFieldName = nameof(WBOneDemNode<T>.X), 
            double yScaler = 1d,
            double xScaler = 1d) where T : struct 
        {
            
            var xPoints = nodes.Values<T>(xAxisFieldName);
            var yPoints = nodes.Values<T>(yAxisFieldName);
            return xPoints.Zip(yPoints, (x, y) => new DataDummy(x*xScaler, y*yScaler)).ToList();
        }
        public static ScatterSeries GetScatterSerries<T>(this IEnumerable<WBOneDemNode<T>> nodes,
            string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode<T>.X),
            double yScaler = 1d,
            double xScaler = 1d) where T : struct {

            try {
                return new ScatterSeries() {
                    DataFieldX = nameof(DataDummy.X),
                    DataFieldY = nameof(DataDummy.Y),
                    ItemsSource = nodes.GetDataDummiesList(yAxisFieldName, xAxisFieldName, yScaler, xScaler)
                };
            } catch (Exception e) {
                return new ScatterSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
        public static LineSeries GetLineSerries<T>(this IEnumerable<WBOneDemNode<T>> nodes,
            string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode<T>.X),
            double yScaler = 1d,
            double xScaler = 1d) where T : struct {

            try {
                return new LineSeries() {
                    DataFieldX = nameof(DataDummy.X),
                    DataFieldY = nameof(DataDummy.Y),
                    ItemsSource = nodes.GetDataDummiesList(yAxisFieldName, xAxisFieldName, yScaler, xScaler)
                };
            } catch (Exception e) {
                return new LineSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
    }
}
