using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Portfolio.Enums;

namespace Portfolio.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Role Role { get; set; }

    }
}