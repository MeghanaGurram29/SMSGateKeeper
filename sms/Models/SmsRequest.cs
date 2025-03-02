namespace SMSGateway.Models
{
    public class SmsRequest
    {
        public required string AccountName { get; set; }
        public required string BusinessNumber { get; set; }
        public required string Message { get; set; }
    }
    
    public class Result
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        private Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Ok()
        {
            return new Result(true, errorMessage: null);
        }

        public static Result Fail(string errorMessage) => new Result(false, errorMessage);
        
        public class RateLimiterSettings
        {
            public int MaxMessagesPerNumber { get; set; }
            public int MaxTotalMessagesForAccount { get; set; }
        }
    }
}