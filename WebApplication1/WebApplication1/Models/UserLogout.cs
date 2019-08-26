using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserLogout
    {
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string username { get; set; }
    }
}
