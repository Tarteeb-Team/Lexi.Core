using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private static ReplyKeyboardMarkup ShpionMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Admin tools")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("All users"),
                    new KeyboardButton("/start"),
                    new KeyboardButton("Count of users")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup OptionMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Menu 🎙")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Change my English level 🤯"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }
        private static ReplyKeyboardMarkup FeedbackMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Leave a review 📝")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("View other reviews 🤯"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup MenuMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Test speech 🎙"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Me 👤"),
                    new KeyboardButton("Feedback 📝")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup TestSpeechMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Practice IELTS part 1 🎯"),
                    new KeyboardButton("Test pronunciation 🎧")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Menu 🎙")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }



        private static ReplyKeyboardMarkup PronunciationMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Generate a question 🎁"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Menu 🎙")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup LevelMarkup()
        {
            var keyboardButtons = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("A1 😊"),
                    new KeyboardButton("A2 😉"),
                    new KeyboardButton("B1 😄"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("B2 😎"),
                    new KeyboardButton("C1 😇"),
                    new KeyboardButton("C2 🤗"),
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }
    }
}
