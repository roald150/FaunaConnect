using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class ChatService(FaunaDbContext context) : IChatService
{
    public async Task<List<ChatMessageResource>> GetGroupChatAsync()
    {
        return await context.ChatMessages
            .Where(m => m.IsGroupChat)
            .Include(m => m.Sender)
            .OrderBy(m => m.Timestamp)
            .Take(50)
            .Select(m => new ChatMessageResource
            {
                Id = m.Id,
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                SenderName = m.Sender != null ? m.Sender.Name : null,
                ReceiverId = m.ReceiverId,
                IsGroupChat = m.IsGroupChat
            })
            .ToListAsync();
    }

    public async Task<List<ChatMessageResource>> GetPrivateChatAsync(int currentUserId, int otherUserId)
    {
        return await context.ChatMessages
            .Where(m => !m.IsGroupChat &&
                       ((m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId)))
            .Include(m => m.Sender)
            .OrderBy(m => m.Timestamp)
            .Select(m => new ChatMessageResource
            {
                Id = m.Id,
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                SenderName = m.Sender != null ? m.Sender.Name : null,
                ReceiverId = m.ReceiverId,
                IsGroupChat = m.IsGroupChat
            })
            .ToListAsync();
    }

    public async Task<ChatMessageResource> SendMessageAsync(ChatMessage message)
    {
        message.Timestamp = DateTime.Now;
        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();

        return new ChatMessageResource
        {
            Id = message.Id,
            Content = message.Content,
            Timestamp = message.Timestamp,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            IsGroupChat = message.IsGroupChat
        };
    }
}
