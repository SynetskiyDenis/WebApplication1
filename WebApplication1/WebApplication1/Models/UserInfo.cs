using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserInfo: UserLogin
    {
        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 6)]
        public string email { get; set; }
        public bool isLogined { get; set; }

    }

}
