using System;

namespace WebApplication1.Models
{
    public class UserInfo: UserLogin
    {
        public string email { get; set; }
        public bool isLogined { get; set; }

    }

}
