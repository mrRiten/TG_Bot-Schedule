using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG_Bot
{
    public class ArchiveSchedule
    {
        public string? TextData { get; set; }
        public DateTime DateOfSchedule { get; set; }
    }

    public class ScheduleTable
    {
        public Dictionary<int, string[]>? Lessons { get; set; }
        public DayOfWeek? Day { get; set; }
    }

    public class TableRowData
    {
        public int Index { get; set; }
        public string? Group { get; set; }
        public string? NumbersReplacementLessons { get; set; }
        public string? ScheduledLessons { get; set; }
        public string? ReplacementLessons { get; set; }
        public string? Auditorium { get; set; }
        public DayOfWeek? Day { get; set; }
        public string? DayOfSchedule { get; set; }
    }
}
