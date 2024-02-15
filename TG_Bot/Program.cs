﻿using Telegram.Bot;
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
                new KeyboardButton[] { "📑 Расписание" },
                ["⬅️ Предыдущее", "Следующее ➡️"],
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
            await UpdateScheduleAsync(12, 0);

            Console.WriteLine($"Start listening for @{me.Username}");

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

                if (messageText == "📑 Расписание")
                {
                    botResponse = configWorker.GetFromArchive(DateTime.Today.DayOfWeek).TextData ?? "";
                }
                else if (messageText == "Следующее ➡️")
                {
                    var tableRowData = configWorker.GetTableRowData();

                    if (tableRowData.Count > 0)
                    {
                        ScheduleTable scheduleTable = configWorker.GetScheduleTable(DateTime.Now.AddDays(1).DayOfWeek, tableRowData[0].DayOfSchedule);
                        botResponse = ScheduleBuilder.BuildScheduleTable(scheduleTable, tableRowData);
                    }
                    else
                    {
                        botResponse = configWorker.GetFromArchive(DateTime.Today.AddDays(1).DayOfWeek).TextData ?? "Ошибка";
                    }
                }
                else if (messageText == "⬅️ Предыдущее")
                {
                    botResponse = configWorker.GetFromArchive(DateTime.Today.AddDays(-1).DayOfWeek).TextData ?? "";
                }
                else{
                    botResponse = "Я не знаю такой команды(";
                }


                // received message text
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: botResponse,
                    replyMarkup: replyKeyboardMarkup,
                    parseMode: ParseMode.Markdown,
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

        public static async Task UpdateScheduleAsync(int hour, int minute)
        {
            // default value: hour = 12, minute = 30
            while (true)
            {
                var currentTime = DateTime.Now;

                if (currentTime.Hour >= hour && currentTime.Minute >= minute)
                {
                    if (currentTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        CreateNewSchedule(2, currentTime);
                        await Console.Out.WriteLineAsync("Parsing!");
                    }
                    else
                    {
                        CreateNewSchedule(1, currentTime);
                        await Console.Out.WriteLineAsync("Parsing!");
                    }
                }
                else
                {
                    ConfigWorker configWorker = new();
                    configWorker.ClearTableConf();
                }

                await Task.Delay(TimeSpan.FromHours(1));
            }
        }

        static void CreateNewSchedule(int addDyasValue, DateTime currentTime)
        {
            ConfigWorker configWorker = new();
            Parser parser = new();
            parser.ParseHTML();
            var tableRowData = configWorker.GetTableRowData();
            if (tableRowData.Count > 0)
            {
                ScheduleTable scheduleTable = configWorker.GetScheduleTable(currentTime.AddDays(addDyasValue).DayOfWeek, tableRowData[0].DayOfSchedule);
                string resultText = ScheduleBuilder.BuildScheduleTable(scheduleTable, tableRowData);
                configWorker.SaveToArchive(resultText, currentTime.AddDays(addDyasValue));
            }
            else
            {
                Console.Out.WriteLine("Null");
            }
            
        }

    }
}
