using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IChatService
{
    Task<List<ChatMessageResource>> GetGroupChatAsync();
    Task<List<ChatMessageResource>> GetPrivateChatAsync(int currentUserId, int otherUserId);
    Task<ChatMessageResource> SendMessageAsync(Models.ChatMessage message);
}
