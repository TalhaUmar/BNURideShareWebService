using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Phone Number is Required.")]
        [StringLength(11, ErrorMessage = "Enter Phone# in this form 03**** ")]
        [RegularExpression(@"\b\d{3}[-.]?\d{3}[-.]?\d{5}\b", ErrorMessage = "Enter a Valid Phone Number")]
        public String Phone { get; set; }
    }
}