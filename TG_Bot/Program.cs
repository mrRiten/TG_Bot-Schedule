using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot
{
    internal class Program
    {

        public static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("6854187070:AAGVtITTFVSBRxEqJi6dNkUdMSJxkJvWnCg");

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Расписание" },
                ["Предыдущее", "Следующее"],
            })
            {
                ResizeKeyboard = true
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );


            var me = await botClient.GetMeAsync();
            await UpdateScheduleAsync(19);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                string botResponse = "";
                ConfigWorker configWorker = new ConfigWorker();

                if (messageText == "Расписание")
                {
                    botResponse = configWorker.GetFromArchive(DateTime.Today.DayOfWeek).TextData ?? "";
                }
                else if (messageText == "Следующее")
                {
                    botResponse = configWorker.GetFromArchive(DateTime.Today.AddDays(1).DayOfWeek).TextData ?? "";
                }
                else if (messageText == "Предыдущее")
                {
                    botResponse = configWorker.GetFromArchive(DateTime.Today.AddDays(-1).DayOfWeek).TextData ?? "";
                }

                // received message text
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: botResponse,
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        }

        static async Task UpdateScheduleAsync(int hour)
        {
            // defoult value: hour = 12, minute = 30
            while (true)
            {
                DateTime currentTime = DateTime.Now;

                if (currentTime.Hour >= hour && currentTime.Minute == 30)
                {
                    ConfigWorker configWorker = new();
                    Parser parser = new();
                    parser.ParseHTML();
                    var tableRowData = configWorker.GetTableRowData();
                    ScheduleTable scheduleTable = configWorker.GetScheduleTable(DateTime.Today.AddDays(1).DayOfWeek, tableRowData[0].DayOfSchedule);
                    string resultText = ScheduleBuilder.BuildScheduleTable(scheduleTable, tableRowData);
                    configWorker.SaveToArchive(resultText, currentTime.AddDays(1));
                    await Console.Out.WriteLineAsync("Parsing!");
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

    }
}
