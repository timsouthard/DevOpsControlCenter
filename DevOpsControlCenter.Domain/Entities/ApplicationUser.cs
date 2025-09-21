using Microsoft.AspNetCore.Identity;

namespace DevOpsControlCenter.Domain.Entities
{
    /// <summary>
    /// Represents an authenticated user within the DevOps Control Center (DOCC).
    /// 
    /// Inherits from <see cref="IdentityUser"/>, which provides the standard
    /// ASP.NET Core Identity fields such as Id, UserName, Email, PasswordHash,
    /// SecurityStamp, and lockout information.
    /// 
    /// This class can be extended with additional properties to capture
    /// application-specific user details.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// A human-friendly display name for the user.
        /// This is used in UI elements (e.g., navigation bars, greetings)
        /// instead of the raw username or email.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
    }
}
