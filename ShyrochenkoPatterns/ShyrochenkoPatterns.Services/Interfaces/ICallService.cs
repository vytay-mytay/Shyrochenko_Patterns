using ShyrochenkoPatterns.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces
{
    public interface ICallService
    {
        Task VerificationCall(ApplicationUser user);
    }
}
