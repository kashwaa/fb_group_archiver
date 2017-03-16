using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver.Interfaces
{
    public interface IPostsRepository
    {
        void SavePost(dynamic post);
    }
}
