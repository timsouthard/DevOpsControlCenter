namespace DevOpsControlCenter.Domain.Entities;

public class DevOpsConnection
{
    public int DevOpsConnectionId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public string PersonalAccessToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
}
