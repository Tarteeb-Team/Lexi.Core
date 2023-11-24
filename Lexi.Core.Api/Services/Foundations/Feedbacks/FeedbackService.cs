//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Feedbacks;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public partial class FeedbackService : IFeedbackService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public FeedbackService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            ValidateFeedbackNotNull(feedback);

            return await this.storageBroker.InsertFeedbackAsync(feedback);
        }

        public IQueryable<Feedback> RetrieveAllFeedbacks()
        {
            return this.storageBroker.SelectAllFeedbacks();
        }

        public async ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id)
        {
            return await storageBroker.SelectFeedbackByIdAsync(id);
        }

        public async ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback)
        {
            return await this.storageBroker.DeleteFeedbackAsync(feedback);
        }
    }
}
