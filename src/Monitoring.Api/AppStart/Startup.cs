using Monitoring.Api.Configuration;

namespace Monitoring.Api.AppStart
{
    public class Startup
    {
        private WebApplicationBuilder _builder;

        public Startup(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void Initialize()
        {
            InitConfigs();
            ConfigureHealthChecks();
        }

        private void InitConfigs()
        {
            if (!_builder.Environment.IsDevelopment())
            {
                _builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
            }

            _builder.Services.Configure<TelegramSettings>(_builder.Configuration.GetSection(TelegramSettings.SectionName));
        }

        private void ConfigureHealthChecks()
        {            
            _builder.Services
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    var telegramSettings = _builder.Configuration.GetSection(TelegramSettings.SectionName).Get<TelegramSettings>();

                    using var serviceProvider = _builder.Services.BuildServiceProvider();
                    var logger = serviceProvider.GetService<ILogger<Startup>>();
                    logger.LogInformation(telegramSettings.BotToken);
                    logger.LogInformation(telegramSettings.ChatId);

                    setup.AddWebhookNotification(
                        "Telegram",
                        $"https://api.telegram.org/bot{telegramSettings.BotToken}/sendMessage",
                        payload: $"{{\"chat_id\":\"{telegramSettings.ChatId}\",\"text\":\"⚠️ Health Check FAILED\\nService: [[LIVENESS]]\\nError: [[FAILURE]]\\nDetails: [[DESCRIPTIONS]]\"}}",
                        restorePayload: $"{{\"chat_id\":\"{telegramSettings.ChatId}\",\"text\":\"✅ Health Check RESTORED\\nService: [[LIVENESS]]\"}}"
                    );
                })
                .AddInMemoryStorage();
        }
    }
}
