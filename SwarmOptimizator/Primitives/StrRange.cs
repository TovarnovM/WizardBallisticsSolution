using MyRandomGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {
    public class StrRange {
        public bool Matters { get; set; } = true;
        public List<string> Variants { get; private set; }
        public List<double> Probabilities { get; private set; }
        int _min, _max;
        public MyRandom rnd;
        public StrRange(string name, IEnumerable<string> variants) {
            Name = name;
            Variants = new List<string>(variants);
            Probabilities = new List<double>(Variants.Count);
            for (int i = 0; i < Variants.Count; i++) {
                Probabilities.Add(1d);
            }
            _min = 0;
            _max = Variants.Count;
            rnd = new MyRandom();
        }
        public StrRange(string name, params string[] variants):this(name,variants.AsEnumerable()) {

        }

        public string Name { get; set; }
        public static int MaxSearchCycleCount = 33;
        public string GetRandValue() {
            int ind = 0, v;
            do {
                v = rnd.GetInt(_min, _max);
            } while (ind<MaxSearchCycleCount && Probabilities[v] < rnd.GetDouble());
            return Variants[v];
        }

        public bool ValidateValue(int value) {
            int v = (int)value;
            return _min <= v && v < _max;
        }

        public bool ValidateValue(double value) {
            int v = (int)value;
            return _min <= v && v < _max;
        }
        public bool ValidateValue(string value) {
            return Variants.Contains(value);
        }
        public double GetDoubleByString(string variant) {
            var answ = Variants.IndexOf(variant);
            if (answ < 0)
                throw new Exception("Такого варианта нет");
            return (double)answ;
        }

        public string GetStringByDouble(double variant) {
            if (!ValidateValue(variant))
                throw new Exception("Такого варианта нет");
            int v = (int)variant;
            return Variants[v];
        }

        public double GetNearestValidate(double value) {
            return ValidateValue(value) ? (int)value :
                value < _min ? _min :
                _max;
        }
    }
}
