using EFxceptions;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Questions;
using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker : EFxceptionsContext, IUpdateStorageBroker
    {
        public UpdateStorageBroker() =>
            this.Database.EnsureCreated();

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }

        public async ValueTask<T> InsertAsync<T>(T @object)
        {
            try
            {
                var broker = new UpdateStorageBroker();
                broker.Entry(@object).State = EntityState.Added;
                await broker.SaveChangesAsync();

                return @object;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<T> SelectAll<T>() where T : class
        {
            var broker = new UpdateStorageBroker();

            return broker.Set<T>();
        }

        public async ValueTask<T> SelectAsync<T>(params object[] objectsId) where T : class
        {
            var broker = new UpdateStorageBroker();

            return await broker.FindAsync<T>(objectsId);
        }

        public async ValueTask<T> UpdateAsync<T>(T @object)
        {

            var broker = new UpdateStorageBroker();
            broker.Entry(@object).State = EntityState.Modified;
            await broker.SaveChangesAsync();

            return @object;
        }

        public async ValueTask<T> DeleteAsync<T>(T @object)
        {
            try
            {
                var broker = new UpdateStorageBroker();
                broker.Entry(@object).State = EntityState.Deleted;
                await broker.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data source = UpdatedLexi.db";
            optionsBuilder.UseSqlite(connectionString);
        }

        public override void Dispose() { }
    }
}
