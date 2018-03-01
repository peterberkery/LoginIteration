using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LoginIteration.Models;

namespace LoginIteration.Models
{
    public class UserLogin
    {
        [Required(AllowEmptyStrings =false, ErrorMessage ="Email ID Required")]
        [Display(Name ="Email")]
        public string EmailId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}