using FacebookGroupArchiver.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    public class DiskConfigurationManager : IConfigurationManager
    {
        string path = Path.Combine(Environment.CurrentDirectory, "config.json");
        IDictionary<string,object> settings;
        public DiskConfigurationManager()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            try
            {
                settings = File.ReadAllText(path).ToJson();
            }
            catch (Exception)
            {
                settings = new ExpandoObject();
            }
        }
        public T ReadSetting<T>(string key)
        {
           
            if (settings.ContainsKey(key))
            {
                try
                {
                    return (T)settings[key];
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
            return default(T);
        }

        public void WriteSetting<T>(string key, T value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(settings));
        }
    }
}
