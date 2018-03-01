using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginIteration.Models;
using System.ComponentModel.DataAnnotations;

namespace LoginIteration.Models.Extended
{
    [MetadataType(typeof(CRUDMetadata))]
    public partial class CRUD
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<int> Shares { get; set; }
    }

    public class CRUDMetadata
    {
        [Display(Name = "First name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:mm-dd-yyyy}")]
        public DateTime DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum of 6 Characters")]
        public string Password { get; set; }

        public Nullable<int> Shares { get; set; }

    }
}