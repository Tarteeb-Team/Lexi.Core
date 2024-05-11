using Lexi.Core.Api.Models.Foundations.Questions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Lexi.Core.Api.Models.Foundations.QuestionTypes;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker
    {
        DbSet<QuestionType> QuestionTypes { get; set; }

        public async ValueTask<QuestionType> InsertQuestionTypeAsync(QuestionType questionType) =>
            await InsertAsync(questionType);

        public IQueryable<QuestionType> SelectAllQuestionTypes() =>
            SelectAll<QuestionType>();

        public ValueTask<QuestionType> DeleteQuestionTypeAsync(QuestionType questionType) =>
            DeleteAsync(questionType);

        public async ValueTask<QuestionType> UpdateQuestionTypeAsync(QuestionType questionType) =>
            await UpdateAsync(questionType);

        public ValueTask<QuestionType> SelectQuestionTypeByIdAsync(Guid id) =>
            SelectAsync<QuestionType>();
    }
}
