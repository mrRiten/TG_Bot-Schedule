using Newtonsoft.Json;

namespace TG_Bot
{
    public class ConfigWorker
    {
        private readonly string tableConf = "tableConf.json";
        private readonly string scheduleConf = "scheduleConf.json";

        public void SaveTableRowData(TableRowData rowData)
        {
            string json = JsonConvert.SerializeObject(rowData, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(tableConf, json);
        }

        public TableRowData GetTableRowData()
        {
            string jsonText = File.ReadAllText(tableConf);
            var item = JsonConvert.DeserializeObject<TableRowData>(jsonText);

            return item;
        }

        public ScheduleTable GetScheduleTable(DayOfWeek day)
        {
            string jsonText = File.ReadAllText(scheduleConf);
            var records = JsonConvert.DeserializeObject<ScheduleTable[]>(jsonText);
            foreach (var record in records)
            {
                if (record.Day == day)
                {
                    return record;
                }
            }
            return null;
        }

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
    }

}
