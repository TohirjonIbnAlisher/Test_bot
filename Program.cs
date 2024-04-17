using System.ComponentModel.Design;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using test_bot.Brokers.QuestionBrokers;
using test_bot.Brokers.SubjectBrokers;
using test_bot.Brokers.UserBroker;
using test_bot.Brokers.UserBrokers;
using test_bot.Models;

namespace test_bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string token = "6984020267:AAHtHT9F1aeEgxdXokI8O6-Jg3pQzI5WEXc";
            TelegramBotClient botClient = new TelegramBotClient(token);

            botClient.StartReceiving(
                updateHandler: UpdateHandlerAsync,
                pollingErrorHandler: ErrorHandlerAsync);
            Console.ReadKey();
        }

        private static Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        static ISubjectBroker subjectBroker = new SubjectBroker();
        static IQuestionBroker questionBroker = new QuestionBroker();
        static Message currentMessage = null;
        private static async Task UpdateHandlerAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            //return;
            if (update.CallbackQuery is not null)
            {
                string message = update.CallbackQuery.Data;

                if(message.StartsWith("sub"))
                {
                    await SubjectOperationsAsync(client, update.CallbackQuery);
                }
                
                else if(message.Substring(3, 3) is ("Que"))
                {
                    await QuestionOperationAsync(client, update);
                }
                else if(message.Substring(3, 3) is "Sub")
                {      
                    await OnCallBackQuerySubjectAsync(client, update.CallbackQuery);

                }
            }
            else if (update.Message.Type is MessageType.Text)
            {
                string text = update.Message.Text;

                if(text == "/start")
                {
                    await OnSendStartAsync(client,update.Message);  
                }

                else if(text.StartsWith("que"))
                {
                    bool isInserted = await questionBroker.InsertQuestionAsync(
                        new Question()
                        {
                            Text = text.Split('%')[1],
                            Level = (Level)int.Parse(text.Split('%')[2]),
                            SubjectId = int.Parse(text.Split('%')[3])
                        });

                    if(isInserted)
                    {
                        await client.SendTextMessageAsync(chatId: update.Message.Chat.Id, "Question inserted");
                    }
                }
                else if(text.Substring(0, 3) == "sub")
                {
                    await client.DeleteMessageAsync(chatId: update.Message.Chat.Id, messageId: update.Message.MessageId - 1);
                    string subName = text.Substring(4);

                    bool isInserted = await subjectBroker.InsertSubjectAsync(subName);
 
                    if (isInserted)
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Subject muvaffaqqiyatli qo'shildi");
                        await AdminOptionAsync(client, update.Message);
                    }
                }
            }
        }

        static async Task QuestionOperationAsync(ITelegramBotClient client, Update update)
        {
            string data = update.CallbackQuery.Data;
            if (data.StartsWith("Add"))
            {
                await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Savol teksti va darajasini kiriting\n" +
                   "Savol darajasi \n1.HIGH \n2.MEDIUM\n3.EASY\n" +
                   $"Subject id {update.CallbackQuery.Data.Substring(6)}\n" +
                   "Qolip {que%question%level%subjectId}");
            }
            else if (data.StartsWith("get"))
            {
                List<Question> questions = await questionBroker.SelectQuestionsBySubjectIdAsync(int.Parse(data.Substring(6)));


            }
        }

        private static async Task OnCallBackQuerySubjectAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            switch(callbackQuery.Data.Substring(0, 3))
            {
                case "ins":
                    await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, currentMessage.MessageId);
                    currentMessage = await client.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, "Subject name : {sample sub%SubjectName}");
                    break;
                case "get":
                    await RetrieveAllSubjectsAsync(client, callbackQuery.Message);
                    break;
                case "del":
                    bool isDeleted = await subjectBroker.DeleteSubjectAsync(int.Parse(callbackQuery.Data.Substring(6)));
                    if(isDeleted)
                    {
                        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Subject deleted");
                    }
                    break;
                
            }
        }

        static async Task SubjectOperationsAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Quyidagilardan birini tanlang",
                replyMarkup: new InlineKeyboardMarkup(
                    new InlineKeyboardButton[][]
                    {
                         new InlineKeyboardButton[]
                         {
                             InlineKeyboardButton.WithCallbackData("Delete Subject", $"delSub{callbackQuery.Data.Substring(3)}"),
                             InlineKeyboardButton.WithCallbackData("Add Question", $"AddQue{callbackQuery.Data.Substring(3)}")
                         },
                         new InlineKeyboardButton[]
                         {
                             InlineKeyboardButton.WithCallbackData("Get Questions", $"getQue{callbackQuery.Data.Substring(3)}")
                         },
                    }
                    ));
        }

        static async Task RetrieveAllSubjectsAsync(ITelegramBotClient client, Message message)
        {
            ISubjectBroker subjectBroker = new SubjectBroker();
            List<Subject> subjects = await subjectBroker.SelectSubjectAsync();


            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
            string add = "sub";
            for (int i = 0, j = 0; i < subjects.Count / 2; i++)
            {
                buttons.Add(
                   new InlineKeyboardButton[]
                   {
                        InlineKeyboardButton.WithCallbackData(subjects[j].Name, add + subjects[j].Id.ToString()),
                        InlineKeyboardButton.WithCallbackData(subjects[j + 1].Name, add + subjects[j + 1].Id.ToString()),
                   }
                    );

                j += 2;
            }

            if (subjects.Count % 2 == 1)
            {
                buttons.Add(
                   new InlineKeyboardButton[]
                   {
                        InlineKeyboardButton.WithCallbackData(subjects[^1].Name, add + subjects[^1].Id.ToString()),
                   }
                    );
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(
                buttons
               );

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Quyidafilardan birini tanlang",
                replyMarkup: inlineKeyboardMarkup);
        }

        static IUserBroker userBroker = new UserBroker();
        private static async Task OnSendStartAsync(ITelegramBotClient client, Message message)
        {
            User tgUser = message.From;

            Role? storedUserRole = await userBroker.LoginAsync(tgUser.Id);
            if (storedUserRole is null)
            {
                bool isRegistered = await userBroker.RegisterUserAsync(
                    new TgUser()
                    {
                        TgId = tgUser.Id,
                        FullName = tgUser.FirstName + " " + tgUser.LastName,
                        UserName = tgUser.Username,
                        UserRole = Role.USER,
                    }
                    );

            }

            else if (storedUserRole is Role.ADMIN)
            {
                await AdminOptionAsync(client, message);
            }
        }

        private static async Task AdminOptionAsync(ITelegramBotClient client, Message message)
        {
            await client.DeleteMessageAsync(chatId : message.Chat.Id, message.MessageId);
            currentMessage = await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Quyidagilardan birini tanlang",
                replyMarkup: new InlineKeyboardMarkup(
                    new InlineKeyboardButton[][]
                    {
                         new InlineKeyboardButton[]
                         {
                             InlineKeyboardButton.WithCallbackData("Add subject", "insSub"),
                             InlineKeyboardButton.WithCallbackData("Get subjects", "getSub")
                         }
                    }
                    ));
        }
    }
}