namespace FaunaConnect2.Api.Resources;

public class ChatMessageResource
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int SenderId { get; set; }
    public string? SenderName { get; set; }
    public int? ReceiverId { get; set; }
    public bool IsGroupChat { get; set; }
}
