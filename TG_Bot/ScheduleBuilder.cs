using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TG_Bot
{
    public class ScheduleBuilder
    {
        public static string BuildScheduleTable(ScheduleTable scheduleTable, TableRowData tableRowData)
        {
            string[] numberRepLessons;
            if (tableRowData.NumbersReplacementLessons.Length > 1)
            {
                numberRepLessons = tableRowData.NumbersReplacementLessons.Split(",");
            }
            else
            {
                numberRepLessons = [tableRowData.NumbersReplacementLessons];
            }

            foreach (var number in numberRepLessons)
            {
                scheduleTable.Lessons[int.Parse(number)] = [tableRowData.ReplacementLessons, tableRowData.Auditorium];
            }

            string result = $"Расписание на {scheduleTable.Day}\n";
            
            foreach(var item in scheduleTable.Lessons)
            {
                result += $"{item.Key} {item.Value[0]} - {item.Value[1]}\n";
            }
            return result;
        }

        public static string BuildScheduleTable(ScheduleTable scheduleTable)
        {
            return BuildSimpleSchedule(scheduleTable);
        }

        private static string BuildSimpleSchedule(ScheduleTable scheduleTable)
        {
            string result = $"Расписание на {scheduleTable.Day}\n";

            foreach (var item in scheduleTable.Lessons)
            {
                result += $"{item.Key} {item.Value[0]} - {item.Value[1]}\n";
            }
            return result;
        }

    }
}
