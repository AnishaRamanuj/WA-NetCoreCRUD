using System.ComponentModel.DataAnnotations;

namespace CreateWebApplication.Models
{
        public class UserModel
        {
            [Display(Name = "ID")]
        public int ID { get; set; } 
        
        public int RoleId { get; set; }

            [Required(ErrorMessage = "Please enter Name")]
            [Display(Name = "Name")]
            public string FullName { get; set; }

            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
            public string MobileNumber { get; set; }

            [Required]
            [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please enter password")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
            [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase      Alphabet, 1 Number and 1 Special Character")]
            public string Password { get; set; }

            [Display(Name = "Confirm password")]
            [Required(ErrorMessage = "Please enter confirm password")]
            [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
            [DataType(DataType.Password)]
            public string Confirmpwd { get; set; }

            [Display(Name = "Date of Birth")]
            [Required(ErrorMessage = "Please Select the DOB")]
            [DataType(DataType.Date)]
            public DateTime? DOB { get; set; }

            [Display(Name = "Gender")]
            [Required(ErrorMessage = "Please Select the gender")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Please selct your hobbies")]
            [Display(Name = "Hobbies")]
            public string Hobbies { get; set; }
            public bool LastLoggedIn { get; set; }
        [Required(ErrorMessage = "Please select your country")]
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
