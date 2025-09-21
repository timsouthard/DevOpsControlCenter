namespace DevOpsControlCenter.Web.Dtos;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool? RememberMe { get; set; }   // 👈 allows missing field
}