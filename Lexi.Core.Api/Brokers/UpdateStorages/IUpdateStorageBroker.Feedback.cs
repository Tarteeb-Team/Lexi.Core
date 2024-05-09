using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<Feedback> InsertFeedbackAsync(Feedback feedback);
        IQueryable<Feedback> SelectAllFeedbacks();
        ValueTask<Feedback> DeleteFeedbackAsync(Feedback feedback);
        ValueTask<Feedback> SelectFeedbackByIdAsync(Guid id);
    }
}
