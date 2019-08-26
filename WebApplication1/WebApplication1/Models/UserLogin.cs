using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserLogin: UserLogout
    {
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string password { get; set; }
    }
}
