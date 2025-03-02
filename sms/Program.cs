using Microsoft.Extensions.Options;
using static SMSGateway.Models.Result;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind RateLimiterSettings from appsettings.json
builder.Services.Configure<RateLimiterSettings>(builder.Configuration.GetSection("RateLimiterSettings"));

builder.Services.AddHostedService<RateLimiterCleanupService>();

// Register SmsRateLimiter as a singleton
builder.Services.AddSingleton<SmsRateLimiter>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RateLimiterSettings>>().Value;
    return new SmsRateLimiter(settings);
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();