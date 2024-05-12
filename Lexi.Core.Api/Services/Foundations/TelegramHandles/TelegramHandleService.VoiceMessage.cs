using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
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

                // Split the speechText into words
                string[] words = speechText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length <= 3)
                {
                    string shortAnswerMessage = "Your answer seems to be too short. Please provide a more detailed response.";
                    await botClient.SendTextMessageAsync(chatId: user.TelegramId, text: shortAnswerMessage);
                    return true;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                string feedbackTemplate = $@"📝 *Feedback for IELTS Part 1 Answer* 📝

📝 *{user.Name}'s Answer:* 

{speechText}

🎓 *Approximate IELTS Score:* 
[Insert Score Here]

📘 *1. Grammar:* ✅
   - Check for any grammar mistakes and correct them for clarity.

📘 *2. Vocabulary:* 📚
   - Suggest using more diverse vocabulary to enhance expression.

📘 *3. Clarity:* 🌟
   - Ensure your response is clear and easy to understand.

📘 *4. Organization:* 🧩
   - Organize your ideas logically for better coherence.

📘 *5. Engagement:* 💬
   - Aim to captivate the reader with interesting language and ideas.

📚 Remember, practice makes progress! Keep up the good work! 🚀

*📔 Note:*
This feedback template is for the question '{user.ImprovedSpeech}'. If your answer is for a different question, 
please provide feedback accordingly, but if the answer is based on the question, just skip this option.
";
                string prompt = $"As you've attempted IELTS Part 1, please provide feedback " +
                    $"based on the given answer:\n\n'{feedbackTemplate}' for this question '{user.ImprovedSpeech}'. " +
                    $"Remember, provide feedback only based on this template, and keep it simple and student-friendly. 😊";

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
