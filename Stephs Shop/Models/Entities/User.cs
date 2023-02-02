using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Models.Entities
{
    public class User
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
