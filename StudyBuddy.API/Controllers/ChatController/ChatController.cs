using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Services.ChatService;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Controllers.ChatController;

[ApiController]
[Route("api/v1/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessagesByConversation(Guid conversationId)
    {
        var messages = await _chatService.GetMessagesByConversationAsync(conversationId);
        return Ok(messages);
    }

    [HttpGet("messages/{messageId:guid}")]
    public async Task<IActionResult> GetMessageById(Guid messageId)
    {
        var message = await _chatService.GetMessageByIdAsync(messageId);

        return Ok(message);
    }

    [HttpPost("messages/add")]
    public async Task<IActionResult> AddMessage([FromBody] ChatMessage message)
    {
        await _chatService.AddMessageAsync(message);
        return CreatedAtAction(nameof(GetMessageById), new { messageId = message.Id }, null);
    }

    [HttpDelete("messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        await _chatService.DeleteMessageAsync(messageId);
        return NoContent();
    }
}
