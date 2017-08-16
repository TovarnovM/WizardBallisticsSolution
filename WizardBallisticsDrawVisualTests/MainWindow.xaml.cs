using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WizardBallisticsCore;
using WizardBallisticsCore.OneDemGrid;
using WizardBallisticsDraw;

namespace WizardBallisticsDrawVisualTests {
    

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        class TestClass11: WBOneDemNode {
            public double  cos2, c_33, d_77;
            public double sin { get; set; }
        }


        public ViewModel1 vm { get; set; } 

        public MainWindow() {
            DataContext = this;
            vm = new ViewModel1();
            InitializeComponent();
            Test1();
        }

        public void Test1() {
            var tstLst = GetTestNodes(-3, 5, 19, x => new TestClass11() {
                X = x,
                V = x * x,
                sin = Math.Sin(x),
                cos2 = Math.Cos(x),
                c_33 = 33,
                d_77 = x * x * x
            });
            var flds = tstLst[0].GetDataFieldsNames();
            for (int i = 0; i < flds.Count; i++) {
                if (i % 2 == 0) {
                    vm.PM.Series.Add(tstLst.GetScatterSerries(flds[i]));
                } else {
                    vm.PM.Series.Add(tstLst.GetLineSerries(flds[i]));
                }
            }
            vm.PM.InvalidatePlot(true);
           
        }

        List<TestClass11> GetTestNodes(double x1, double x2, int n, Func<double, TestClass11> f) {
            return Enumerable.Range(0, n)
                .Select(i => f(x1 + (x2 - x1) * i / n))
                .ToList();
        }
    }
}
