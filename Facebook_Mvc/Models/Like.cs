using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Like
    {
        [Key]
        public int LikeID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public int PostID { get; set; }
        public string Name { get; set; }

    }
}
