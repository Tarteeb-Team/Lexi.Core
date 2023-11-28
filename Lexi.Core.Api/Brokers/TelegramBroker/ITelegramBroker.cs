using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public interface ITelegramBroker
    {
        void StartListening();
        void ReturningConvertOggToWav(Stream stream);
        string ReturnFilePath();
        Task SendTextMessageAsync(long chatId, string text);
    }
}
