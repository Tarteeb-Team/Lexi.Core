using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<QuestionType> InsertQuestionTypeAsync(QuestionType questionType);
        IQueryable<QuestionType> SelectAllQuestionTypes();
        ValueTask<QuestionType> DeleteQuestionTypeAsync(QuestionType questionType);
        ValueTask<QuestionType> SelectQuestionTypeByIdAsync(Guid id);
    }
}
