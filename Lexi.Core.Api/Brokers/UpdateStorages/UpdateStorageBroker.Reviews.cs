using Lexi.Core.Api.Models.Foundations.Reviews;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker
    {
        public DbSet<Review> Reviews { get; set; }

        public async ValueTask<Review> InsertReviewAsync(Review review) =>
            await InsertAsync(review);

        public IQueryable<Review> SelectAllReviews() =>
            SelectAll<Review>();

        public ValueTask<Review> DeleteReviewAsync(Review review) =>
            DeleteAsync(review);

        public ValueTask<Review> SelectReviewByIdAsync(Guid id) =>
            SelectAsync<Review>();
    }
}
