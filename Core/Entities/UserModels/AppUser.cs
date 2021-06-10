using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.UserModels
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public AddressUser AddressUser { get; set; }
    }
}
