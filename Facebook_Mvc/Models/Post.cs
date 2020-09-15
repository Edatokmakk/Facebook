using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }
        public string PostText { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateDate
        { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string Eposta { get; set; }
        public User User { get; set; }
        public string ProfilePicture { get; set; }
        public string PostPicture { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Share> Shares { get; set; }
        public int LikeCount { get; set; }
    }
}
