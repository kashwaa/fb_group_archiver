using FacebookGroupArchiver.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    public class HardCodedConfigurationManager : IConfigurationManager
    {
        public T ReadSetting<T>(string key)
        {
            switch (key)
            {
                case "clientId":
                    return (T)(object)"287827628048234";
                case "redirectUri":
                    return (T)(object)"http://localhost:12345/";
                case "clientSecret":
                    return (T)(object)"0fe438bcb61bd4f594ad35df0d17bef6";
                case "path":
                    return (T)(object)Path.Combine(Environment.CurrentDirectory, "data");
                default:
                    return default(T);
            }
        }

        public void WriteSetting<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
