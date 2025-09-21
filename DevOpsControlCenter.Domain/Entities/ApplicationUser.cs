using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DevOpsControlCenter.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Add any domain-specific fields
        public string DisplayName { get; set; } = string.Empty;
    }
}
