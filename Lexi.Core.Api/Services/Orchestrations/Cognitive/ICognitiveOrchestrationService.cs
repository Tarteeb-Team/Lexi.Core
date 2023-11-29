//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.ObjcetModels;
using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public interface ICognitiveOrchestrationService
    {
        void StartListening();
        ValueTask<ResponseCognitive> GetResponseCognitive();
        ValueTask<User> AddNewUserAsync();
        ValueTask MapFeedbackToStringAndSendMessage(long telegramId, Feedback feedback, string sentence);
    }
}
