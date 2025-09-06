using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Contracts.Services.Identity.Security;

public interface ICustomClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
{
}
