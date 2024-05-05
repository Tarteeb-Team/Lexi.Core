using Lexi.Core.Api.Models.Foundations.Reviews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
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
