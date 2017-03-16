using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FacebookGroupArchiver
{
    /// <summary>
    /// A utility class for extention methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a DateTime object to Unix time stamp
        /// </summary>
        /// <param name="datetime">The DateTime object</param>
        /// <returns>Unix time stamp</returns>
        public static string ToUnixTimeStamp(this DateTime datetime)
        {
            try
            {
                DateTimeOffset offset = new DateTimeOffset(datetime);
                string unixtimestamp = offset.ToUnixTimeSeconds().ToString();
                return unixtimestamp;
            }
            catch (Exception)
            {
                return "0";
            }
           
        }

        public static dynamic ToJson(this string data)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
        }
    }
}
