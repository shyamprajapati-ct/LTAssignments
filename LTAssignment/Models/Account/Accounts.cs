using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace LTAssignment.Models.Account
{
    public class Accounts
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool KeepLoggedIn { get; set; } = true;
    }
    public class Register
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, ErrorMessage = "User Name max 50 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }
        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid Mobile number")]
        [StringLength(15, ErrorMessage = "Mobile number max 15 characters")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        [StringLength(50, ErrorMessage = "Email cannot max 50 characters")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage = "Password max 20 characters")]
        public string? Password { get; set; }
        public IFormFile? Image { get; set; }  
        public string? FinalDTFile { get; set; }  
        public string? ProfileImage { get; set; }  

    }
}
