using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Share
    {
        [Key]
        public int ShareID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public int PostID { get; set; }
        public string Name { get; set; }
    }
}
