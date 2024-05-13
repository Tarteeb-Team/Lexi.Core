using System.Collections.Generic;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public interface IWordsToLearn
    {
        List<(string, string, string)> NewWordsToLearn();
    }
}
