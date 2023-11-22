//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Linq;
using System;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;

namespace Lexi.Core.Api.Services.Orchestrations.Speech
{
    public interface ISpeechOrchestrationService
    {
        Task MapToFeedback(ResponseCognitive responseCognitive);
        Task MapToSpeech(ResponseCognitive responseCognitive);

        ValueTask<Feedback> AddFeedbackAsync(Feedback feedback);
        IQueryable<Feedback> RetrieveAllFeedbacks();
        ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id);
        ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback);
    }
}
