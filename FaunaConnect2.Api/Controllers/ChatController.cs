using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController(FaunaDbContext context) : ControllerBase
{
    [HttpGet("group")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetGroupChat()
    {
        return await context.ChatMessages
            .Where(m => m.IsGroupChat)
            .Include(m => m.Sender)
            .OrderBy(m => m.Timestamp)
            .Take(50)
            .ToListAsync();
    }

    [HttpGet("private/{otherUserId}")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetPrivateChat(int currentUserId, int otherUserId)
    {
        return await context.ChatMessages
            .Where(m => !m.IsGroupChat && 
                       ((m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId)))
            .Include(m => m.Sender)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ChatMessage>> SendMessage(ChatMessage message)
    {
        message.Timestamp = DateTime.Now;
        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();
        return Ok(message);
    }
}
