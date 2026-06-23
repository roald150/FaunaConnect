using System;

namespace FaunaConnect2.Api.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public int SenderId { get; set; }
    public User? Sender { get; set; }


    public int? ReceiverId { get; set; }
    public User? Receiver { get; set; }


    public bool IsGroupChat { get; set; } = false;
}