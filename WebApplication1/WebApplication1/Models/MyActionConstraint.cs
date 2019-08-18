using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

//вспомогательный класс для обработки команды Delete

namespace WebApplication1.Models
{
    public class MyActionConstraint : Attribute, IActionConstraint
    {
        private readonly bool hasbody;
        public MyActionConstraint(bool hasbody)
        {
            this.hasbody = hasbody;
        }
        public int Order => 0;
        public bool Accept(ActionConstraintContext context)
        {
            return hasbody == (context.RouteContext.HttpContext.Request.ContentLength != 0);            
        }
    }
}

