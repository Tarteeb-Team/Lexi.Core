using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Services.Orchestrations;
using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public interface ITelegramHandleService
    {
        ValueTask<ExternalUser> CreateExternalUserAsync();
        void ListenTelegramUserMessage();
        void ReturningConvertOggToWav(Stream stream, long userId);
        string ReturningConvertOggToWavSecond(Stream stream, long userId);
        string ReturnFilePath();
        Task SendTextMessageAsync(long chatId, string text);

        void SetOrchestrationService(
           IOrchestrationService orchestrationService, long telegramId = default);
    }
}
