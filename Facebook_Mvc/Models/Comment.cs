using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostID { get; set; }
        public string Message { get; set; }
        public User CommentedBy { get; set; }
        public User PostedBy { get; set; }
        public string CommentedByName { get; set; }
        public string CommentedByPicture { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentedDate { get; set; }
    }
}
