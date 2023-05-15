using Microsoft.AspNetCore.Identity;
using System;

namespace Stephs_Shop.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }

    }




    public enum Gender
    {
        Male, 
        Female,
        Other
    }
}
