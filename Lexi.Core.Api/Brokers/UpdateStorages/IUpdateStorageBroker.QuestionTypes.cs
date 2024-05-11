using Lexi.Core.Api.Models.Foundations.QuestionTypes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<QuestionType> InsertQuestionTypeAsync(QuestionType questionType);
        IQueryable<QuestionType> SelectAllQuestionTypes();
        ValueTask<QuestionType> DeleteQuestionTypeAsync(QuestionType questionType);
        ValueTask<QuestionType> UpdateQuestionTypeAsync(QuestionType questionType);
        ValueTask<QuestionType> SelectQuestionTypeByIdAsync(Guid id);
    }
}
