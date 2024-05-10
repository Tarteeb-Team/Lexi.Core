using Lexi.Core.Api.Models.Foundations.Questions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<Question> InsertQuestionAsync(Question question);
        IQueryable<Question> SelectAllQuestions();
        ValueTask<Question> DeleteQuestionAsync(Question question);
        ValueTask<Question> SelectQuestionByIdAsync(Guid id);
    }
}
