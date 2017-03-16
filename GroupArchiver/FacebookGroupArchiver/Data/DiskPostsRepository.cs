using FacebookGroupArchiver.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver.Data
{
    public class DiskPostsRepository : IPostsRepository
    {
        IConfigurationManager _configManager;
        public DiskPostsRepository(IConfigurationManager configManager)
        {
            _configManager = configManager;
        }
        public void SavePost(dynamic post)
        {
            string directory = _configManager.ReadSetting<string>("path");
            Directory.CreateDirectory(Path.Combine(directory, "post"));
            string path = Path.Combine(directory,"post", post.id + ".json");
            File.WriteAllText(path, post.ToString(),new UTF8Encoding(false));
        }
    }
}
