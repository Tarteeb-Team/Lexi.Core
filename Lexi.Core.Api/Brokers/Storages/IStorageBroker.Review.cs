using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Linq;
using System.Threading.Tasks;
using System;
using Lexi.Core.Api.Models.Foundations.Reviews;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Review> InsertReviewAsync(Review review);
        IQueryable<Review> SelectAllReviews();
        ValueTask<Review> DeleteReviewAsync(Review review);
        ValueTask<Review> SelectReviewByIdAsync(Guid id);
    }
}
