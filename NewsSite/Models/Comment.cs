using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_config.Models
{
    public class Comment
    {
        [Key]
        public int Id_Comment { get; set; }
        public int WhoCreated { get; set; }
        public string Text { get; set; }
        public DateTime WasCreated { get; set; }

    }
}
