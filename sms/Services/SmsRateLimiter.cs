using SMSGateway.Models;
using static SMSGateway.Models.Result;

public class SmsRateLimiter
{
    private readonly RateLimiterSettings _settings;
    private readonly Dictionary<string, Dictionary<string, Queue<DateTime>>> _accountMsgDict = new();

    public SmsRateLimiter(RateLimiterSettings settings)
    {
        _settings = settings;
    }

    public Result CanSendMessage(SmsRequest request)
    {
        // Initialize the account dict if it doesn't exist
        if (!_accountMsgDict.ContainsKey(request.AccountName))
        {
            _accountMsgDict[request.AccountName] = new Dictionary<string, Queue<DateTime>>();
        }

        var businessNumMsgDict = _accountMsgDict[request.AccountName];

        // Initialize the business number dict if it doesn't exist
        if (!businessNumMsgDict.ContainsKey(request.BusinessNumber))
        {
            businessNumMsgDict[request.BusinessNumber] = new Queue<DateTime>();
        }

        var msgTimesDict = businessNumMsgDict[request.BusinessNumber];

        DateTime currentTime = DateTime.UtcNow;

        //Remove old messages from the queue(older than 1 second)
        while (msgTimesDict.Count > 0 && (currentTime - msgTimesDict.Peek()).TotalSeconds >= _settings.InActiveActTimeoutHours)
        {
            msgTimesDict.Dequeue();
        }

        // Check messages per second for the business number
        if (msgTimesDict.Count >= _settings.MaxMessagesPerNumber)
        {
            return Result.Fail("Maximum messages per second reached for this business number.");
        }

        // Check total messages for the account
        int totalAccountMsgsSent = businessNumMsgDict.Values.Sum(q => q.Count);
        if (totalAccountMsgsSent >= _settings.MaxTotalMessagesForAccount)
        {
            return Result.Fail("Maximum total messages for the account reached.");
        }

        // If all checks pass, allow the message
        msgTimesDict.Enqueue(currentTime);
        return Result.Ok();
    }

    public void CleanupOldEntries()
    {
        DateTime currentTime = DateTime.UtcNow;

        // Iterate through accounts
        foreach (var account in _accountMsgDict.Keys.ToList())
        {
            var businessNumMsgDict = _accountMsgDict[account];

            // Iterate through business numbers
            foreach (var businessNumber in businessNumMsgDict.Keys.ToList())
            {
                var msgTimesDict = businessNumMsgDict[businessNumber];

                // Remove business number if it has no messages in the past hour
                if (msgTimesDict.Count == 0 || (currentTime - msgTimesDict.Last()).TotalHours >= 1)
                {
                    businessNumMsgDict.Remove(businessNumber);
                }
            }

            // Remove account if it has no active business numbers
            if (businessNumMsgDict.Count == 0)
            {
                _accountMsgDict.Remove(account);
            }
        }
    }

}