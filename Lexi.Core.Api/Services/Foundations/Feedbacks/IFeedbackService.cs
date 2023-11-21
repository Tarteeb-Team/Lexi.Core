//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Feedbacks
{
    public interface IFeedbackService
    {
        ValueTask<Feedback> AddFeedbackAsync(Feedback feedback);
        IQueryable<Feedback> RetrieveAllFeedbacks();
    }
}
