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
}