using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpGet("group")]
    public async Task<ActionResult<IEnumerable<ChatMessageResource>>> GetGroupChat()
    {
        return Ok(await chatService.GetGroupChatAsync());
    }

    [HttpGet("private/{otherUserId}")]
    public async Task<ActionResult<IEnumerable<ChatMessageResource>>> GetPrivateChat(int currentUserId, int otherUserId)
    {
        return Ok(await chatService.GetPrivateChatAsync(currentUserId, otherUserId));
    }

    [HttpPost]
    public async Task<ActionResult<ChatMessageResource>> SendMessage(ChatMessage message)
    {
        return Ok(await chatService.SendMessageAsync(message));
    }
}
