//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.ObjcetModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpeechModel = Lexi.Core.Api.Models.Foundations.Speeches.Speech;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public interface IOrchestrationService
    {
        ValueTask GenerateSpeechFeedbackForUser(long? telegramId);
        IQueryable<Feedback> RetrieveAllFeedbacks();
        ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id);
        ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback);
        IQueryable<SpeechModel> RetrieveAllSpeeches();
        ValueTask<SpeechModel> RetrieveSpeechByIdAsync(Guid id);
        ValueTask<SpeechModel> RemoveSpeechAsync(SpeechModel speechModel);
    }
}
