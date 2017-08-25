using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;


namespace WizardBallistics.Draw {
    public static class SerriesGetter {
        public class DataDummy {
            public double X { get; set; }
            public double Y { get; set; }
            public DataDummy(double x, double y) {
                X = x;
                Y = y;
            }
        }
        public static List<DataDummy> GetDataDummiesList(this IEnumerable<WBNodeBase> nodes, 
            string yAxisFieldName, 
            string xAxisFieldName = nameof(WBOneDemNode.X), 
            double yScaler = 1d,
            double xScaler = 1d) 
        {
            
            var xPoints = nodes.Values(xAxisFieldName);
            var yPoints = nodes.Values(yAxisFieldName);
            return xPoints.Zip(yPoints, (x, y) => new DataDummy(x*xScaler, y*yScaler)).ToList();
        }
        public static ScatterSeries GetScatterSerries(this IEnumerable<WBNodeBase> nodes,
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
        public static LineSeries GetLineSerries(this IEnumerable<WBNodeBase> nodes,
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
        public static LineSeries GetLineSerries(this IWBNodeLayer layer, string variantName, string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode.X),
            double yScaler = 1d,
            double xScaler = 1d) {

            try {
                return layer.GetNodesForDraw(variantName).GetLineSerries(yAxisFieldName, xAxisFieldName, yScaler, xScaler);

            } catch (Exception e) {
                return new LineSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };
            }
        }
        public static ScatterSeries GetScatterSerries(this IWBNodeLayer layer, string variantName, string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode.X),
            double yScaler = 1d,
            double xScaler = 1d) {

            try {
                return layer.GetNodesForDraw(variantName).GetScatterSerries(yAxisFieldName, xAxisFieldName, yScaler, xScaler);

            } catch (Exception e) {
                return new ScatterSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };
            }
        }

        public static ScatterSeries GetScatterSerries(this IEnumerable<IWBNode> nodes,
                string yAxisFieldName,
                string xAxisFieldName = nameof(WBOneDemNode.X),
                double yScaler = 1d,
                double xScaler = 1d) {

            try {
                return nodes.Cast<WBNodeBase>().GetScatterSerries(yAxisFieldName, xAxisFieldName, yScaler, xScaler);
  
            } catch (Exception e) {
                return new ScatterSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
        public static LineSeries GetLineSerries(this IEnumerable<IWBNode> nodes,
            string yAxisFieldName,
            string xAxisFieldName = nameof(WBOneDemNode.X),
            double yScaler = 1d,
            double xScaler = 1d) {

            try {
                return nodes.Cast<WBNodeBase>().GetLineSerries(yAxisFieldName, xAxisFieldName, yScaler, xScaler);

            } catch (Exception e) {
                return new LineSeries() { Title = $"{yAxisFieldName} Ошибка: {e.Message}" };

            }
        }
    }
}
