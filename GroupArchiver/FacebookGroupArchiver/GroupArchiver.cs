using FacebookGroupArchiver.Entities;
using FacebookGroupArchiver.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    public class GroupArchiver
    {
        public async Task Archive(IConfigurationManager configManager,IPostsRepository postsRepo, ISummaryRepository summaryRepo)
        {
            FaceBookAuthentication fb = new FaceBookAuthentication();
            string token = await fb.RetrieveUserToken(configManager);
            FacebookGroup group = new FacebookGroup() { Id = configManager.ReadSetting<string>("groupId") };
            PostsRetriever ret = new PostsRetriever();

            List<string> savedPostsIds = new List<string>();

            DateTime lastUpdate = configManager.ReadSetting<DateTime>("lastUpdate");
            string lastUpdateTime = await ret.GetAllPosts(token, group, (posts) =>
            {

                foreach (var post in posts)
                {
                    postsRepo.SavePost(post);
                    savedPostsIds.Add(post.id);
                }

            }, lastUpdate.ToUnixTimeStamp());
            if (!String.IsNullOrEmpty(lastUpdateTime))
            {
                configManager.WriteSetting<string>("lastUpdate", lastUpdateTime);
            }
            summaryRepo.SaveSummary(savedPostsIds);
        }
    }
}
