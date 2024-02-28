using System.Collections.Generic;

namespace TG_Bot
{
    public class ScheduleBuilder
    {
        public static string BuildScheduleTable(ScheduleTable scheduleTable, List<TableRowData> tableRowData)
        {
            var numberRepLessons = new List<string>();
            foreach (TableRowData row in tableRowData)
            {
                if (row.NumbersReplacementLessons.Contains(','))
                {
                    string[] temp = row.NumbersReplacementLessons.Split(',');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        numberRepLessons.Add(temp[i]);
                    }
                }
                else if (row.NumbersReplacementLessons.Contains('-'))
                {
                    string[] temp = row.NumbersReplacementLessons.Split('-');
                    for (int i = int.Parse(temp[0]); i <= int.Parse(temp[temp.Length - 1]); i++)
                    {
                        numberRepLessons.Add(string.Join("", i));
                    }
                }
                else
                {
                    numberRepLessons.Add(row.NumbersReplacementLessons);
                }

                foreach (var number in numberRepLessons)
                {
                    scheduleTable.Lessons[int.Parse(number)] = [row.ReplacementLessons, row.Auditorium];
                }
            }

            return BuildSimpleSchedule(scheduleTable);
        }

        public static string BuildScheduleTable(ScheduleTable scheduleTable)
        {
            return BuildSimpleSchedule(scheduleTable);
        }

        private static string BuildBeautifulWeekday(ScheduleTable scheduleTable)
        {
            string weekday = String.Concat(scheduleTable.Day);

            switch (weekday)
            {
                case "Monday":
                    weekday = "𝐌𝐨𝐧𝐝𝐚𝐲";
                    break;
                case "Tuesday":
                    weekday = "𝐓𝐮𝐞𝐬𝐝𝐚𝐲";
                    break;
                case "Wednesday":
                    weekday = "𝐖𝐞𝐝𝐧𝐞𝐬𝐝𝐚𝐲";
                    break;
                case "Thursday":
                    weekday = "𝐓𝐡𝐮𝐫𝐬𝐝𝐚𝐲";
                    break;
                case "Friday":
                    weekday = "𝐅𝐫𝐢𝐝𝐚𝐲";
                    break;
                case "Saturday":
                    weekday = "𝐒𝐚𝐭𝐮𝐫𝐝𝐚𝐲";
                    break;
                case "Sunday":
                    weekday = "𝐒𝐮𝐧𝐝𝐚𝐲";
                    break;
                default:
                    break;
            }
            return $"{weekday}";
        }

        private static string BuildSimpleSchedule(ScheduleTable scheduleTable)
        {
            string weekday = BuildBeautifulWeekday(scheduleTable);
            string result = $"📑 Расписание на *{weekday}*\n";
            ConfigWorker configWorker = new();
            ScheduleTime scheduleTime = configWorker.GetScheduleTime();

            Dictionary<int, string[]> sc = scheduleTable.Lessons.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            scheduleTable.Lessons = sc;

            foreach (var item in scheduleTable.Lessons)
            {
                result += $"`{item.Key}` *{item.Value[0]}* - \\[{item.Value[1]}] _{scheduleTime.NumToLessonTime[item.Key]}_\n";
            }
            return result;
        }

    }
}
