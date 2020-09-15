using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Facebook_Mvc.Models;

namespace Facebook_Mvc.ViewModel
{
    public class AllModels : UserViewModel
    {
        public Comment Comment { get; set; }
        public Post Post { get; set; }
        public User Users { get; set; }
        public Like Like { get; set; }
        public Messages Messages { get; set; }
        public Share Share { get; set; }
        public UserViewModel UserViewModel { get; set; }
        public IEnumerable<Friend> Friends { get; set; }
    }
}
