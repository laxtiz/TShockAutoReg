using System;
using System.IO;
using Newtonsoft.Json;

namespace PiQiu.AutoReg
{
    public class Config
    {
        public bool Enabled { get; set; } = true;
        public bool NameOnlyLetterOrDigit { get; set; } = true;
        public RandomPasswordSettings Settings { get; set; } = new RandomPasswordSettings();

        public static Config Load(string path)
        {
            if (!File.Exists(path))
                return new Config();

            using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Load(fs);
            }
        }

        public static Config Load(Stream stream)
        {
            using(var sr = new StreamReader(stream))
            {
                var json = sr.ReadToEnd();
                var cfg = JsonConvert.DeserializeObject<Config>(json);
                return cfg;
            }
        }

        public void Save(string path)
        {
            var dir = Directory.GetParent(path);
            if (!dir.Exists)
                dir.Create();

            using(var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                Save(fs);
            }
        }

        public void Save(Stream stream)
        {
            using(var sw = new StreamWriter(stream))
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(json);
            }
        }
    }
}
