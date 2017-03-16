using Facebook;
using FacebookGroupArchiver.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    /// <summary>
    /// Retrieves a list of user's Facebook groups
    /// </summary>
    public class GroupsRetriever
    {
        /// <summary>
        /// Retrieves a list of user's Facebook groups
        /// </summary>
        /// <param name="authenticationToken">The user's authentication token</param>
        /// <returns>A list of Facebook groups</returns>
        public async Task<List<FacebookGroup>> ListGroups(string authenticationToken)
        {
            return await Task.Run(() =>
            {
                List<FacebookGroup> groupList = new List<FacebookGroup>();
                FacebookClient client = new FacebookClient(authenticationToken);
                dynamic groups = client.Get("/me/groups");
                dynamic data = groups.data;
                foreach (var group in data)
                {
                    FacebookGroup facebookGroup = new FacebookGroup()
                    {
                        Name = group.name,
                        Id = group.id
                    };
                    groupList.Add(facebookGroup);
                }
                return groupList;
            });
        }
    }
}
