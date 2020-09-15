/*author:Eda Nur Tokmak*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Facebook_Mvc.ViewModel
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Eposta { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }

        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }
        public IFormFile BackgroundImg { get; set; }
    }
}
/*author:Eda Nur Tokmak*/
