using Microsoft.AspNetCore.Mvc;
using SMSGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api")]
public class SmsController : ControllerBase
{
    private readonly SmsRateLimiter _rateLimiter;

    public SmsController(SmsRateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    // POST: /api/sms/send
    [HttpPost("sms/send")]
    public IActionResult SendSms([FromBody] SmsRequest request)
    {
        Result result = _rateLimiter.CanSendMessage(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok("SMS sent successfully!");
    }

    // GET: /api/messages/total
    [HttpGet("messages/total")]
    public IActionResult GetTotalMessages()
    {
        int totalMessagesSent = _rateLimiter.GetTotalMessagesSent();
        return Ok(new { TotalMessagesSent = totalMessagesSent });
    }

    // GET: /api/messages/per-number?number=12345
    [HttpGet("messages/per-number")]
    public IActionResult GetMessagesByNumber([FromQuery] string number)
    {
        var messages = _rateLimiter.GetMessagesByNumber(number);
        return Ok(new
        {
            Number = number,
            MessagesSent = messages.Count,
            MessageTimes = messages
        });
    }

    // GET: /api/messages/rate
    [HttpGet("messages/rate")]
    public IActionResult GetMessagesPerSecond()
    {
        int messagesPerSecond = _rateLimiter.GetMessagesPerSecond();
        return Ok(new { MessagesPerSecond = messagesPerSecond });
    }

    // GET: /api/messages/filter?start=2023-10-01T00:00:00Z&end=2023-10-01T23:59:59Z
    [HttpGet("messages/filter")]
    public IActionResult FilterMessages([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var filteredMessages = _rateLimiter.FilterMessages(start, end);
        return Ok(filteredMessages);
    }
}