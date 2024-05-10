using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Lexi.Core.Api.Models.Foundations.Questions;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        DbSet<Question> Questions { get; set; }

        public async ValueTask<Question> InsertQuestionAsync(Question question) =>
            await InsertAsync(question);

        public IQueryable<Question> SelectAllQuestions() =>
            SelectAll<Question>();

        public ValueTask<Question> DeleteQuestionAsync(Question question) =>
            DeleteAsync(question);

        public ValueTask<Question> SelectQuestionByIdAsync(Guid id) =>
            SelectAsync<Question>();
    }
}
