using Domain.Entities.Indentity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> CheckSmsCodeAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user, string code)
        {
            bool result = false;
            if (user.AccessFailedCount > 3 || DateTime.UtcNow > user.DateOfConfirmationCodeExpiration)
            {
                user.ConfirmationCode = null;
                user.DateOfConfirmationCodeExpiration = null;
            }
            else
            {
                result = user.ConfirmationCode == code;
                if (result)
                {
                    user.ConfirmationCode = null;
                    user.DateOfConfirmationCodeExpiration = null;
                    user.AccessFailedCount = 0;
                } else
                {
                    user.AccessFailedCount++;
                }
            }

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            return result;
        }
    }
}
