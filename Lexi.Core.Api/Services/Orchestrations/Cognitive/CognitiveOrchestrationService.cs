//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Lexi.Core.Api.Services.Foundations.Users;
using Newtonsoft.Json;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public class CognitiveOrchestrationService : ICognitiveOrchestrationService
    {
        private readonly ICognitiveServices cognitiveServices;
        private readonly ITelegramService telegramService;
        private readonly IUserService userService;

        public CognitiveOrchestrationService(
            ICognitiveServices cognitiveServices, 
            ITelegramService telegramService, 
            IUserService userService)
        {
            this.cognitiveServices = cognitiveServices;
            this.telegramService = telegramService;
            this.userService = userService;
        }

        public void StartListening() =>
            this.telegramService.StartListening();

        public async ValueTask<ResponseCognitive> GetResponseCognitive()
        {
            string result = await this.cognitiveServices.GetJsonString();
            ResponseCognitive responseCognitive = new ResponseCognitive();

            responseCognitive = JsonConvert.DeserializeObject<ResponseCognitive>(result);
            return responseCognitive;
        }

        public async ValueTask<User> AddNewUserAsync()
        {
            ExternalUser externalUser = await this.telegramService.GetExternalUserAsync();

            User user =  await MapToUser(externalUser);

            return user;
        }

        public async ValueTask MapFeedbackToStringAndSendMessage(long telegramId, Feedback feedback, string sentence) =>
            await this.telegramService.MapFeedbackToStringAndSendMessage(telegramId, feedback, sentence);
             
        private async ValueTask<User> MapToUser(ExternalUser externalUser)
        {
            var user = this.userService
                .RetrieveAllUsers().FirstOrDefault(u => u.TelegramId == externalUser.TelegramId);

            if (user is not null)
            {
                return user;
            }
            else
            {
                User newUser = new User
                {
                    Id = externalUser.Id,
                    Name = externalUser.Name,
                    TelegramId = externalUser.TelegramId,
                    TelegramName = externalUser.TelegramName
                };

                return await this.userService.AddUserAsync(newUser);

            }
        }
    }
}
