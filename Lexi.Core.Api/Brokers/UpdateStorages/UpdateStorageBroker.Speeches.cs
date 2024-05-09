using Lexi.Core.Api.Models.Foundations.Speeches;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker
    {
        public DbSet<Speech> Speeches { get; set; }

        public async ValueTask<Speech> InsertSpeechAsync(Speech speech) =>
            await InsertAsync(speech);

        public IQueryable<Speech> SelectAllSpeeches() =>
            SelectAll<Speech>();

        public ValueTask<Speech> SelectSpeechByIdAsync(Guid id) =>
            SelectAsync<Speech>();
        public ValueTask<Speech> DeleteSpeechAsync(Speech speech) =>
            DeleteAsync(speech);
    }
}
