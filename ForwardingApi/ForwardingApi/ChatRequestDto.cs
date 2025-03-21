namespace ForwardingApi;

public class ChatRequestDto
{
    public string OrganizationId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid ThreadId { get; set; }
    public string messageText { get; set; } = string.Empty;
    public string? MessageId { get; set; }
}