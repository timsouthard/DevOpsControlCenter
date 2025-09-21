namespace DevOpsControlCenter.Web.Dtos;

/// <summary>
/// Data Transfer Object (DTO) representing a login request
/// sent from the client UI to the authentication endpoint.
/// 
/// This DTO is intentionally lightweight and contains only
/// the fields required for validating credentials and
/// optionally persisting the login session.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The email address used to identify the user.
    /// This should match the <see cref="ApplicationUser.Email"/> value.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The plain-text password submitted by the user.
    /// This value is validated against the hashed password stored
    /// by ASP.NET Core Identity during login.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the authentication cookie should persist
    /// beyond the current session.
    /// 
    /// If <c>true</c>, the login cookie will remain after the browser closes.
    /// If <c>false</c> or <c>null</c>, the session will end when the browser closes.
    /// 
    /// Declared as nullable (<c>bool?</c>) to allow the field to be omitted
    /// in incoming requests without causing model binding errors.
    /// </summary>
    public bool? RememberMe { get; set; }
}
