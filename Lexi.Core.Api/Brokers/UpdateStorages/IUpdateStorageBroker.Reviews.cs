using Lexi.Core.Api.Models.Foundations.Reviews;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<Review> InsertReviewAsync(Review review);
        IQueryable<Review> SelectAllReviews();
        ValueTask<Review> DeleteReviewAsync(Review review);
        ValueTask<Review> SelectReviewByIdAsync(Guid id);
    }
}
