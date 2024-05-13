using Lexi.Core.Api.Models.Foundations.Questions;
using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker
    {
        DbSet<Question> Questions { get; set; }

        public async ValueTask<Question> InsertQuestionAsync(Question question) =>
            await InsertAsync(question);

        public IQueryable<Question> SelectAllQuestions() =>
            SelectAll<Question>();

        public async ValueTask<Question> UpdateQuestionAsync(Question question) =>
            await UpdateAsync(question);

        public ValueTask<Question> DeleteQuestionAsync(Question question) =>
            DeleteAsync(question);

        public ValueTask<Question> SelectQuestionByIdAsync(Guid id) =>
            SelectAsync<Question>();
    }
}
