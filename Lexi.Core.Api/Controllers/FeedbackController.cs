//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;

namespace Lexi.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : Controller
    {
        private readonly IOrchestrationService orchestrationService;

        public FeedbackController(IOrchestrationService orchestrationService)
        {
            this.orchestrationService = orchestrationService;
        }

        [HttpGet]
        public IQueryable<Feedback> GetAllFeedbacks()
        {
            IQueryable<Feedback> feedbacks = this.orchestrationService.RetrieveAllFeedbacks();

            return feedbacks;
        }

        [HttpGet]
        public async ValueTask<Feedback> GetFeedbackByIdAsync(Guid id)
        {
            Feedback feedback = await this.orchestrationService.RetrieveFeedbackByIdAsync(id);

            return feedback;
        }

        [HttpDelete]
        public async void DeleteFeedbackAsync(Feedback feedback) =>
            await this.orchestrationService.RemoveFeedbackAsync(feedback);
    }
}
