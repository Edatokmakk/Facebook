using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Friend
    {
        //public int UserID { get; set; }
        //public int RequestID { get; set; }
        //public User User { get; set; }
        
        public int RequestedById { get; set; }
        
        public int RequestedToId { get; set; }
        [NotMapped]
        public User RequestedBy { get; set; }
        [NotMapped]

        public User RequestedTo { get; set; }

        public FriendRequestFlag FriendRequestFlag { get; set; }

        [NotMapped]
        public bool Approved => FriendRequestFlag == FriendRequestFlag.Approved;
        
    }
    public enum FriendRequestFlag
    {
        None,
        Approved,
        Rejected
    };
}
