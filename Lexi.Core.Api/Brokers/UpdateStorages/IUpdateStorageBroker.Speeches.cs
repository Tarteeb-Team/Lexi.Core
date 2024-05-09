using Lexi.Core.Api.Models.Foundations.Speeches;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<Speech> InsertSpeechAsync(Speech speech);
        IQueryable<Speech> SelectAllSpeeches();
        ValueTask<Speech> SelectSpeechByIdAsync(Guid id);
        ValueTask<Speech> DeleteSpeechAsync(Speech speech);
    }
}
