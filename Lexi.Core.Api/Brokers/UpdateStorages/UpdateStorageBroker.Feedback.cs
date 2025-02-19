﻿using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial class UpdateStorageBroker
    {
        DbSet<Feedback> Feedbacks { get; set; }

        public async ValueTask<Feedback> InsertFeedbackAsync(Feedback feedback) =>
            await InsertAsync(feedback);

        public IQueryable<Feedback> SelectAllFeedbacks() =>
            SelectAll<Feedback>();

        public ValueTask<Feedback> DeleteFeedbackAsync(Feedback feedback) =>
            DeleteAsync(feedback);

        public ValueTask<Feedback> SelectFeedbackByIdAsync(Guid id) =>
            SelectAsync<Feedback>();
    }
}
