using FacebookGroupArchiver.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver.Data
{
    public class DiskSummaryRepository : ISummaryRepository
    {
        IConfigurationManager _configManager;
        public DiskSummaryRepository(IConfigurationManager configManager)
        {
            _configManager = configManager;
        }
        public void SaveSummary(List<string> postIds)
        {
            postIds.Reverse();
            string path = _configManager.ReadSetting<string>("path");
            string summaryPath = Path.Combine(path, "summary");
            Directory.CreateDirectory(summaryPath);
            int lastsummaryIndex = GetLastsummaryNumber(path);//fix last summary off by one
            string lastsummary = "summary_page_" + lastsummaryIndex + ".json";
            int firstPatchCount = GetsummaryEntriesCount(Path.Combine(summaryPath, lastsummary));
            int increment = 10;
            if (firstPatchCount < 10)
            {
                increment = 10 - firstPatchCount;
            }
            else
            {
                lastsummaryIndex++;
            }
            for (int i = 0; i < postIds.Count;)
            {
                List<string> patch = new List<string>();
                for (int x = i; x < postIds.Count && x - i < increment; x++)
                {
                    patch.Add(postIds[x]);
                }
                SummerizePatch(patch, Path.Combine(summaryPath, lastsummary), increment < 10 && lastsummaryIndex != 1);
                lastsummaryIndex++;
                lastsummary = "summary_page_" + lastsummaryIndex + ".json";

                i += increment;
                increment = 10;
            }
            File.WriteAllText(Path.Combine(path, "summaryindex.txt"), (lastsummaryIndex - 1).ToString());

        }
        private void SummerizePatch(List<string> postIds, string path, bool append = false)
        {
            List<dynamic> output = new List<dynamic>();
            foreach (var file in postIds)
            {
                dynamic data = JObject.Parse(File.ReadAllText(Path.Combine(_configManager.ReadSetting<string>("path"), "post", file + ".json")));
                dynamic summary = new ExpandoObject();
                summary.id = data.id;
                summary.from = data.from;
                summary.message = data.message;
                summary.caption = data.caption;
                summary.description = data.description;
                summary.created_time = data.created_time;
                summary.total_likes = data.likes == null ? 0 : data.likes.data.Count;
                summary.total_comments = data.comments == null ? 0 : data.comments.data.Count;
                output.Add(summary);
            }
            string outputString;
            if (append)
            {
                dynamic existing = JObject.Parse(File.ReadAllText(path));
                foreach (var item in output)
                {
                    existing.posts.Add(JsonConvert.SerializeObject(item));
                }
                outputString = existing.ToString();
            }
            else
            {
                dynamic container = new ExpandoObject();
                container.posts = output;
                outputString = JsonConvert.SerializeObject(container);
            }
            File.WriteAllText(path, outputString);
        }
        public int GetLastsummaryNumber(string path)
        {
            if (!File.Exists(path + "\\summaryindex.txt"))
            {
                File.WriteAllText(path + "\\summaryindex.txt", "1");
                return 1;
            }
            int index = 1;
            int.TryParse(File.ReadAllText(path + "\\summaryindex.txt"), out index);
            return index;
        }
        public static int GetsummaryEntriesCount(string path)
        {

            try
            {
                var data = File.ReadAllText(path).ToJson();
                return data.posts.Count;
            }
            catch (Exception)
            {

                return 1;
            }

        }
        public void ResetSummaryCount(string path)
        {
            File.WriteAllText(path + "\\summaryindex.txt", "1");
        }
    }
}
