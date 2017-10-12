using Interpolator;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bikas_comp1D2D {
    public class BicasStats {
        public SerializableDictionary<int, AutodynInfo> dict;
        public List<int> Keys;
        public int Count {
            get {
                return Keys.Count;
            }
        }
        public BicasStats(SerializableDictionary<int, AutodynInfo> dict) {
            this.dict = dict;
            Keys = dict.Keys.ToList();
        }
        public (List<DataP> el, List<DataP> podd, double vel) GetDataAtTime(int key, double ymulty = 1d) {
            var ai_vels = dict[key].Vels;

            var vel = dict[key].vel;
            var el = ConvertToDataP(ai_vels["el"], ymulty);
            var podd = ConvertToDataP(ai_vels["podd"], ymulty);
            return (el, podd, vel);
        }
        public List<DataP> ConvertToDataP(InterpXY interp, double ymulty) {
            var res = new List<DataP>(interp.Data.Count);
            foreach (var item in interp.Data) {
                var dp = new DataP() {
                    X = item.Key,
                    Y = item.Value.Value * ymulty
                };
                res.Add(dp);
            }
            return res;
        }
        public List<DataP> GetMaxPress(int key, int nPoints = 3000,double ymulty = 1d) {
            var ps = dict[key].gPress;
            var res = new List<DataP>(nPoints);
            var resInterp = new InterpXY(nPoints);
            var t0 = dict[key].gPress.Values.First().Data.Keys.First();
            var t1 = dict[key].gPress.Values.First().Data.Keys.Last();
            var dt = (t1 - t0) / (nPoints-1);
            var ts = Enumerable.Range(0, nPoints).Select(i => t0 + i * dt);
            foreach (var t in ts) {
                var maxP = 0d;
                foreach (var interp in ps.Values) {
                    var p = interp[t];
                    if (p > maxP)
                        maxP = p;
                }
                resInterp.Add(t, maxP * ymulty);
                
            }
            resInterp = resInterp.GetSmootherByN_uniform(nPoints/100);
            foreach (var kw in resInterp.Data) {
                res.Add(new DataP() {
                    X = kw.Key,
                    Y = kw.Value.Value
                });
            }
            return res;
        }
        public List<DataP> GetMaxPress_x(int key, int nPoints = 3000, double ymulty = 1d) {
            var x0 = 0.1;
            var dx = 0.01;
            var gaugesN = 21;
            var tn = dict[key].gPress.Values.First().Count;
            var smoothN = tn / 300;
            var regx = new Regex(@"\d{1,2}");
            var gList = dict[key].gPress
                .OrderBy(kw => int.Parse(regx.Match(kw.Key).Value))
                .Select(kw => kw.Value)
                .ToList();
            gList = gList.Select(interp => interp.GetSmootherByN_uniform(smoothN)).ToList();
            var maxes = gList.Select(interp => interp.Get_MaxElem_T()).ToList();
            var batches = maxes.Batch(gaugesN).ToList();
            maxes = batches.Aggregate(
                Enumerable.Range(0, gaugesN).Select(i => 0d).ToList(),
                (oldSum, batch) => oldSum.Zip(batch, (os, b) => Math.Max(os,b)).ToList()
                );

            return Enumerable.Range(0, gaugesN)
                .Select(i => x0 + i * dx)
                .Zip(maxes, (x, pMax) => new DataP() { X = x, Y = pMax * ymulty })
                .ToList();
        }
        public List<DataP> GetMaxVels_x(int key, int nPoints = 3000, double ymulty = 1d) {
            var x0 = 0.1;
            var dx = 0.01;
            var gaugesN = 21;
            var tn = dict[key].gVels.Values.First().Count;
            var smoothN = tn / 300;
            var regx = new Regex(@"\d{1,2}");
            var gList = dict[key].gVels
                .OrderBy(kw => int.Parse(regx.Match(kw.Key).Value))
                .Select(kw => kw.Value)
                .ToList();
            gList = gList.Select(interp => interp.GetSmootherByN_uniform(smoothN)).ToList();
            var maxes = gList.Select(interp => interp.Get_MaxElem_T()).ToList();
            var batches = maxes.Batch(gaugesN).ToList();
            maxes = batches.Aggregate(
                Enumerable.Range(0, gaugesN-1).Select(i => 0d).ToList(),
                (oldSum, batch) => oldSum.Zip(batch, (os, b) => Math.Max(os, b)).ToList()
                );

            return Enumerable.Range(0, gaugesN)
                .Select(i => x0 + i * dx)
                .Zip(maxes, (x, pMax) => new DataP() { X = x, Y = pMax * ymulty })
                .ToList();
        }
        public List<DataP> GetMaxI_x(int key, int nPoints = 3000, double ymulty = 1d) {
            var x0 = 0.1;
            var dx = 0.01;
            var gaugesN = 21;
            var tn = dict[key].gPress.Values.First().Count;
            var smoothN = tn / 300;
            var regx = new Regex(@"\d{1,2}");
            var gList = dict[key].gPress
                .OrderBy(kw => int.Parse(regx.Match(kw.Key).Value))
                .Select(kw => kw.Value)
                .ToList();
            gList = gList.Select(interp => interp.GetSmootherByN_uniform(smoothN)).ToList();
            var maxes = gList.Select(interp => interp.Get_Integral()).ToList();
            var batches = maxes.Batch(gaugesN).ToList();
            maxes = batches.Aggregate(
                Enumerable.Range(0, gaugesN).Select(i => 0d).ToList(),
                (oldSum, batch) => oldSum.Zip(batch, (os, b) => os+ b).ToList(),
                sumz => sumz.Select(el => el/batches.Count).ToList()
                );

            return Enumerable.Range(0, gaugesN)
                .Select(i => x0 + i * dx)
                .Zip(maxes, (x, pMax) => new DataP() { X = x, Y = pMax * ymulty })
                .ToList();
        }
        public static (double maxAbsdiff, double maxPercDiff) GetMaxDiffs(InterpXY i1, InterpXY i2) {
            var v0 = i1.Data.Values.First().Value;
            var maxDiff = 0d;
            foreach (var t1 in i1.Data.Keys) {
                var diff = Math.Abs(i1[t1] - i2[t1]);
                if(diff> maxDiff) {
                    maxDiff = diff;
                }
                
            }
            return (maxDiff, maxDiff / v0*100);
        }

    }

    public class DataP {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
