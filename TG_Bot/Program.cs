using System.Reflection.PortableExecutable;
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
            var botClient = new TelegramBotClient("token");

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
            await UpdateScheduleAsync(22);

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

                if (messageText == "Расписание")
                {
                    ConfigWorker configWorker = new ConfigWorker();
                    Parser parser = new Parser();

                    // Exp how to use archiveConf
                    if (configWorker.GetFromArchive(DateTime.Today) is not null)
                    {
                        botResponse = configWorker.GetFromArchive(DateTime.Today).TextData;
                        await Console.Out.WriteLineAsync("Arc");
                    }
                    else
                    {
                        parser.ParseHTML();
                        ScheduleTable scheduleTable = configWorker.GetScheduleTable(DateTime.Today.DayOfWeek, configWorker.GetTableRowData().DayOfSchedule);
                        TableRowData tableRowData = configWorker.GetTableRowData();
                        botResponse = ScheduleBuilder.BuildScheduleTable(scheduleTable, tableRowData);
                        configWorker.SaveToArchive(botResponse, DateTime.Today);
                    }

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
            while (true)
            {
                DateTime currentTime = DateTime.Now;

                if (currentTime.Hour == hour && currentTime.Minute == 0)
                {
                    Parser parser = new Parser();
                    parser.ParseHTML();
                    await Console.Out.WriteLineAsync("Parsing!");
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

    }
}
