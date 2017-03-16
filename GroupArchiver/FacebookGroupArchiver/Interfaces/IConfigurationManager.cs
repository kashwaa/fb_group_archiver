using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver.Interfaces
{
    public interface IConfigurationManager
    {
        T ReadSetting<T>(string key);
        void WriteSetting<T>(string key, T value);
    }
}
