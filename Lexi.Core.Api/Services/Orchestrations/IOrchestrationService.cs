//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.ObjcetModels;
using Microsoft.AspNetCore.Http;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public interface IOrchestrationService
    {
        Task<ResponseCognitive> GetOggFile(Stream stream);

        IQueryable<Feedback> RetrieveAllFeedbacks();
        ValueTask<Feedback> RetrieveFeedbackByIdAsync(Guid id);
        ValueTask<Feedback> RemoveFeedbackAsync(Feedback feedback);
    }
}
