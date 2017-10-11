using Interpolator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bikas_comp1D2D {
    [Serializable]
    public class AutodynInfo {
        public double vel { get; set; }
        public SerializableDictionary<string, InterpXY> gVels { get; set; } = new SerializableDictionary<string, InterpXY>();
        public SerializableDictionary<string, InterpXY> Vels { get; set; } = new SerializableDictionary<string, InterpXY>();
        public SerializableDictionary<string, InterpXY> gPress { get; set; } = new SerializableDictionary<string, InterpXY>();
        public AutodynInfo Copy() {
            var cl = new AutodynInfo() {
                vel = this.vel
            };

            foreach (var gv in gVels) {
                cl.gVels.Add(gv.Key,  gv.Value.CopyMe());
            }
            foreach (var v in Vels) {
                cl.Vels.Add(v.Key, v.Value.CopyMe());
            }
            foreach (var gp in gPress) {
                cl.gPress.Add(gp.Key, gp.Value.CopyMe());
            }
            
            return cl;
        }

        public AutodynInfo CopyMT() {
            var cl = new AutodynInfo() {
                vel = this.vel
            };
            foreach (var gv in gVels) {
                cl.gVels.Add(gv.Key, null);// gv.Value.CopyMe());
            }
            foreach (var v in Vels) {
                cl.Vels.Add(v.Key, null);//v.Value.CopyMe());
            }
            foreach (var gp in gPress) {
                cl.gPress.Add(gp.Key, null);//gp.Value.CopyMe());
            }
            void copy_gVels() {
                Parallel.ForEach(gVels, gv => {
                    cl.gVels[gv.Key] = gv.Value.CopyMe();
                });
            }
            void copy_Vels() {
                Parallel.ForEach(Vels, v => {
                    cl.gVels[v.Key] = v.Value.CopyMe();
                });
            }
            void copy_gPress() {
                Parallel.ForEach(gPress, gp => {
                    cl.gVels[gp.Key] = gp.Value.CopyMe();
                });
            }
            var functs = new Action[] { copy_gVels, copy_Vels, copy_gPress };
            Parallel.ForEach(functs, f => f());


            return cl;
        }
        public void Reduce() {
            var el_interp = new InterpXY(Vels.Values.First().Count);
            string el_str = @"el";
            var reg = new Regex(el_str);
            var els = Vels.Where(kv => reg.IsMatch(kv.Key)).Select(kv => kv.Value).ToList();
            foreach (var t in els[0].Data.Keys) {
                var v_sred = 0d;
                foreach (var el in els) {
                    v_sred += el[t];
                }
                el_interp.Add(t, v_sred / els.Count);
            }
            Vels.Add(el_str, el_interp);
        }
        public void SaveToFile(string dir) {
            using (var jsw = new JsonTextWriter(new StreamWriter(dir))) {
                var ser = JsonSerializer.Create();
                ser.Serialize(jsw, this);
            }
        }
        public void LoadFromFile(string dir) {
            using (var jstr = new JsonTextReader(new StreamReader(dir))) {

                var ser = JsonSerializer.Create();
                var slc = ser.Deserialize<AutodynInfo>(jstr);
                vel = slc.vel;
                gVels = slc.gVels;
                Vels = slc.Vels;
                gPress = slc.gPress;
            }
        }
    }
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys) {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}