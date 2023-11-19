using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Areas.Identity
{
    public class CustomUserValidator<TUser> : UserValidator<TUser> where TUser : User
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var errors = new List<IdentityError>();
            //await ValidateUserName(manager, user, errors).ConfigureAwait(false);
            if (manager.Options.User.RequireUniqueEmail)
            {
                await ValidateEmail(manager, user, errors).ConfigureAwait(false);
            }
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task ValidateEmail(UserManager<TUser> manager, TUser user, List<IdentityError> errors)
        {
            var email = await manager.GetEmailAsync(user).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }
            if (!new EmailAddressAttribute().IsValid(email))
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }
            var owner = await manager.FindByEmailAsync(email).ConfigureAwait(false);
            if (owner != null &&
                !string.Equals(await manager.GetUserIdAsync(owner).ConfigureAwait(false), await manager.GetUserIdAsync(user).ConfigureAwait(false)))
            {
                errors.Add(Describer.DuplicateEmail(email));
            }
        }
    }
}
