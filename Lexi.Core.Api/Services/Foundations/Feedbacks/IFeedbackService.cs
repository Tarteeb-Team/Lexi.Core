//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public interface IFeedbackService
    {
        ValueTask<Feedback> AddFeedbackAsync(Feedback feedback);
        IQueryable<Feedback> RetrieveAllFeedbacks();
        ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id);
        ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback);
    }
}