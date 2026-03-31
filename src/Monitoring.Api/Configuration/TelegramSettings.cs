namespace Monitoring.Api.Configuration
{
    public class TelegramSettings
    {
        public const string SectionName = "Telegram";

        public string BotToken { get; init; }

        public string ChatId { get; init; }
    }
}
