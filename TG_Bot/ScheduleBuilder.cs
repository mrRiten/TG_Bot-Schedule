namespace TG_Bot
{
    public class ScheduleBuilder
    {
        public static string BuildScheduleTable(ScheduleTable scheduleTable, List<TableRowData> tableRowData)
        {
            string[] numberRepLessons;
            foreach (TableRowData row in tableRowData)
            {
                if (row.NumbersReplacementLessons.Length > 1)
                {
                    numberRepLessons = row.NumbersReplacementLessons.Split(",");
                }
                else
                {
                    numberRepLessons = [row.NumbersReplacementLessons];
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

        private static string BuildSimpleSchedule(ScheduleTable scheduleTable)
        {
            string result = $"📑 Расписание на {scheduleTable.Day}\n";
            ConfigWorker configWorker = new();
            ScheduleTime scheduleTime = configWorker.GetScheduleTime();

            foreach (var item in scheduleTable.Lessons)
            {
                result += $"`{item.Key}` *{item.Value[0]}* - \\[{item.Value[1]}] _{scheduleTime.NumToLessonTime[item.Key]}_\n";
            }
            return result;
        }

    }
}
