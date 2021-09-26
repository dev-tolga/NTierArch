using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string UserFName { get; set; }
        [Required]
        public string UserLName { get; set; }
    }
}
