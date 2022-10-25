using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_config.Models
{
    public class UserPost
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Post> Posts { get; set; }

    }
    
    public class PostList
    {
        public IEnumerable<User> Users { get; set; }
        public IPagedList<Post> Posts { get; set; }

    }
}
