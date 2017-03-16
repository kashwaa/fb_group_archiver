using Facebook;
using FacebookGroupArchiver.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    /// <summary>
    /// Retrieves posts from a specific facebook group
    /// </summary>
    public class PostsRetriever
    {
        /// <summary>
        /// Retrieve all posts from a specific facebook group since a specific date
        /// </summary>
        /// <param name="authenticationToken">Facebook authentication token</param>
        /// <param name="group">The group to retrieve posts from</param>
        /// <param name="WhenPostsAreRetrieved">Callback invoked when a patch of posts are retrieved</param>
        /// <param name="since">Unix timestamp after which all posts will be retrieved</param>
        /// <returns>Unix timestamp of the latests post update time</returns>
        public async Task<string> GetAllPosts(string authenticationToken, FacebookGroup group, Action<List<dynamic>> WhenPostsAreRetrieved, string since = null)
        {
            return await Task.Run(() =>
            {
                List<string> files = new List<string>();
                FacebookClient client = new FacebookClient(authenticationToken);
                string lastPostUpdateDate = null;
                string sincestr = "";
                if (!String.IsNullOrEmpty(since))
                {
                    sincestr = "&since=" + since;
                }
                dynamic data = client.Get($"/{group.Id}/feed?limit=100{sincestr}");
                while (data.data.Count > 0)
                {
                    List<dynamic> posts = new List<dynamic>();
                    foreach (var item in data.data)
                    {
                        //Set last post update time to the first post retrieved
                        if (lastPostUpdateDate == null) lastPostUpdateDate = item.updated_time;
                        posts.Add(item);
                    }
                    //Invoke the call back function
                    WhenPostsAreRetrieved(posts);
                    //Get next page 
                    string next = ExtractNextPageData(data);
                    try
                    {
                        data = client.Get($"/{group.Id}/feed?limit=100&{next}{sincestr}");
                    }
                    catch (Exception)
                    {

                    }
                }
                return lastPostUpdateDate;
            });

        }

        private string ExtractNextPageData(dynamic data)
        {
            string next = data.paging.next;
            var par = next.Split('?')[1].Split('&');
            string pageToken = par.Where(c => c.Contains("paging_token")).First();
            string until = par.Where(c => c.Contains("until")).First();
            return pageToken + "&" + until;
        }

       }
}

