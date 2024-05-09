//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.ObjcetModels;
using Lexi.Core.Api.Services.Cognitives;
using Lexi.Core.Api.Services.Foundations.Telegrams;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Orchestrations.Cognitive
{
    public class CognitiveOrchestrationService : ICognitiveOrchestrationService
    {
        private readonly ICognitiveServices cognitiveServices;
        private readonly ITelegramService telegramService;
        private readonly IUpdateStorageBroker updateStorageBroker;
        public CognitiveOrchestrationService(
            ICognitiveServices cognitiveServices,
            ITelegramService telegramService,
            IUpdateStorageBroker updateStorageBroker)
        {
            this.cognitiveServices = cognitiveServices;
            this.telegramService = telegramService;
            this.updateStorageBroker = updateStorageBroker;
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

            User user = await MapToUser(externalUser);

            return user;
        }

        public async ValueTask MapFeedbackToStringAndSendMessage(long telegramId, Feedback feedback, string sentence) =>
            await this.telegramService.MapFeedbackToStringAndSendMessage(telegramId, feedback, sentence);

        private async ValueTask<User> MapToUser(ExternalUser externalUser)
        {
            var user = this.updateStorageBroker
                .SelectAllUsers().FirstOrDefault(u => u.TelegramId == externalUser.TelegramId);

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
                    TelegramName = externalUser.TelegramName,
                    State = State.Level
                };

                return await this.updateStorageBroker.InsertUserAsync(newUser);

            }
        }
    }
}
