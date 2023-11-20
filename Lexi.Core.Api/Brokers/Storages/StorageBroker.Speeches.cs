//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Threading.Tasks;
using System.Linq;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Microsoft.EntityFrameworkCore;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Speech> Speeches { get; set; }
        
        public async ValueTask<Speech> InsertSpeechAsync(Speech speech) =>
            await InsertAsync(speech);

        public IQueryable<Speech> SelectAllSpeeches() =>
            SelectAll<Speech>();

        public ValueTask<Speech> DeleteSpeechAsync(Speech speech) =>
            DeleteAsync(speech);
    }
}
