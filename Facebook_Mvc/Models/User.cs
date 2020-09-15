using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Mvc.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Adın nedir?")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Adın nedir?")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "İçinde en az 6 rakam ,harf ve noktalama işareti bulunan bir şifre gir.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Giriş yaparken ve şifreni yenilemen gerekirse bunu kullanacaksın.")]
        public string Eposta { get; set; }

        [Required(ErrorMessage = "Lütfen bir cinsiyet seç.Bunları kimin görebileceğini daha sonra değiştirebilirsin.")]
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string BackgroundImg { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }
        [NotMapped]
        public virtual ICollection<Friend> SentFriendRequests { get; set; }
        [NotMapped]
        public virtual ICollection<Friend> ReceievedFriendRequests { get; set; }
        [NotMapped]
        public virtual ICollection<Friend> Friends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.Approved).ToList();
                friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));
                return friends;
            }
        }
        public User()
        {
            SentFriendRequests = new List<Friend>();
            ReceievedFriendRequests = new List<Friend>();
        }
    }   
    }

