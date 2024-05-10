using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.Foundations.Users;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> VoiceMessage(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (update.Message.Voice is not null && user.State is State.TestSpeechPronun)
            {
                var loadingMessage = await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"🎙️ Checking Pronunciation 🎙️\n\n" +
                          $"Loading...");

                messageId.Value = loadingMessage.MessageId;

                var file = await client.GetFileAsync(update.Message.Voice.FileId);

                using (var stream = new MemoryStream())
                {
                    await client.DownloadFileAsync(file.FilePath, stream);
                    stream.Position = 0;

                    ReturningConvertOggToWav(stream, update.Message.Chat.Id);
                }

                await CreateExternalUserAsync();

                SetOrchestrationService(orchestrationService, update.Message.Chat.Id);

                return true;
            }
            else if (user.State is State.PartOneTest && update.Message.Voice is not null)
            {
                var loadingMessage = await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: $"📝 Submitting Answer for IELTS Part 1\n\n" +
                      $"Loading...");

                messageId2.Value = loadingMessage.MessageId;
                var file = await client.GetFileAsync(update.Message.Voice.FileId);
                string filePath;

                using (var stream = new MemoryStream())
                {
                    await client.DownloadFileAsync(file.FilePath, stream);
                    stream.Position = 0;

                    filePath = ReturningConvertOggToWavSecond(stream, update.Message.Chat.Id);
                }

                var speechText = await this.speechBroker.RecognizeSpeechAsync(filePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                string feedbackTemplate = $@"
                📝 **IELTS Part 1 Feedback** 📝

                🔍 **Student's Answer:**
                '{speechText}'

                👍 **Strengths:**
                - Clear expression of ideas
                - Good fluency

                👎 **Areas for Improvement:**
                - Provide more detailed responses
                - Include specific examples

                💡 **Suggestions:**
                - Use a wider range of vocabulary
                - Organize ideas more effectively

                🔍 **Grammar:**
                - The sentence 'I Zafar' was not correct,...

                🚩 **Note:**
                - Ensure to provide sufficient detail in your answers.

                🔥 **Overall Feedback:**
                Based on your response, focus on expanding your answers with specific details and 
                utilizing a wider vocabulary range to enhance your performance.

                📊 **Approximate IELTS Score:**
                Considering the content and language proficiency demonstrated in your response, 
                your approximate IELTS score for this task would likely be [insert score here].";

                string prompt = $"As you've attempted IELTS Part 1, please provide feedback " +
                    $"based on the given answer:\n\n'{speechText}'\n\n{feedbackTemplate}. Remember, only feedback based on template, without extra words.";

                string secondPromt = $"Just improve this answer of part one question based on IELTS 7 score, and return only improved one.";

                var feedback = await this.openAIService.AnalizeRequestAsync(speechText, prompt);
                var improvedSpeech = await this.openAIService.AnalizeRequestAsync(speechText, secondPromt);

                var improvedSpeechPath = await this.speechBroker
                    .CreateAndSaveSpeechAudioPartOneAsync(improvedSpeech, $"{user.TelegramId}");

                var improvedSpeechText = await this.speechBroker
                    .RecognizeSpeechAsync(improvedSpeechPath);

                if (System.IO.File.Exists(improvedSpeechPath))
                {
                    using (var fileStream = System.IO.File.OpenRead(improvedSpeechPath))
                    {
                        await botClient.DeleteMessageAsync(chatId: user.TelegramId, messageId: messageId2.Value);

                        await botClient.SendTextMessageAsync(
                            chatId: user.TelegramId,
                            replyMarkup: PartOneMarkup(),
                            text: feedback);

                        await botClient.SendVoiceAsync(
                            chatId: user.TelegramId,
                            caption: $"🎙️ Improved Version - Part 1 🎙️\n\n{improvedSpeechText}\n\nTry like this 🤯",
                            voice: InputFile.FromStream(fileStream));
                    }
                }

                if (System.IO.File.Exists(improvedSpeechPath))
                {
                    System.IO.File.Delete(improvedSpeechPath);
                }

                return true;
            }
            else if (user.State is State.PartOneTest && update.Message.Text is "Types of questions 🎁")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    replyMarkup: PartOneQuestionsMarkup(),
                    text: $"📚 Practice IELTS Part 1 📚\n\n" +
                          $"Choose a type of question to practice:");

                user.State = State.ChooseTypeOfQuestion;
                await this.updateStorageBroker.UpdateUserAsync(user);











                return true;
            }
            else if (user.State is State.TestSpeechPronun && update.Message.Voice is null)
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message.Chat.Id,
                      text: $"🎓LexiEnglishBot🎓\n\n" +
                      $"Send only voice message, please 🙂");

                return true;
            }

            if (update.Message.Text is "/start")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: MenuMarkup(),
                   text: $"Choose 👇🏼");

                user.State = State.Active;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message.Chat.Id,
                      text: $"🎓LexiEnglishBot🎓\n\n" +
                      $"Wrong choice 🙂");

                return true;
            }

        }
    }
}
