using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Orchestrations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public interface ITelegramBroker
    {
        ValueTask<ExternalUser> CreateExternalUserAsync();
        void StartListening();
        void ReturningConvertOggToWav(Stream stream);
        MemoryStream ReturnFilePath();
        Task SendTextMessageAsync(long chatId, string text);
        void SetOrchestrationService(IOrchestrationService orchestrationService);
    }
}
