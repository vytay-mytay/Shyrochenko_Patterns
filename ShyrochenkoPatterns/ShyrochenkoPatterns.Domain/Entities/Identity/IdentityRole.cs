using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using ShyrochenkoPatterns.Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShyrochenkoPatterns.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<int>, IEntity
    {
        public override int Id { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationRole()
        {
            UserRoles = UserRoles.Empty();
        }
    }
}
