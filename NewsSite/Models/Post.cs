using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_config.Models
{
    public class Post
    {
        [Key]
        public int Id_Post { get; set; }
        public string Photo_post { get; set; }
        public string Name_Post { get; set; }
        public string Text_Post { get; set; }
        public int Owner_Post { get; set; }
        public bool Is_Approved { get; set; }
        public DateTime WasCreated { get; set; }
        
    }
}
