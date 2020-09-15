using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class Messages
    {
        [Key]
        public int MessageID { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public string RecipientEposta{ get; set; }
        public string SenderEposta { get; set; }

        public string RecipientName { get; set; }
        public long timestamp { get; set; }
        public string Message { get; set; }
        [DataType(DataType.Date)]
        public DateTime SendDate { get; set; }
    }
}
