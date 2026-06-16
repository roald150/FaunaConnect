using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly FaunaDbContext _context;

    public ChatController(FaunaDbContext context)
    {
        _context = context;
    }

    [HttpGet("group")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetGroupChat()
    {
        return await _context.ChatMessages
            .Where(m => m.IsGroupChat)
            .Include(m => m.Sender)
            .OrderBy(m => m.Timestamp)
            .Take(50)
            .ToListAsync();
    }

    [HttpGet("private/{otherUserId}")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetPrivateChat(int currentUserId, int otherUserId)
    {
        return await _context.ChatMessages
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
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return Ok(message);
    }
}