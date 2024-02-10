using Newtonsoft.Json;

namespace TG_Bot
{
    public class ConfigWorker
    {
        private readonly string archiveConf = "archiveConf.json";
        private readonly string tableConf = "tableConf.json";
        private readonly string firstScheduleConf = "firstScheduleConf.json";
        private readonly string secondScheduleConf = "secondScheduleConf.json";

        public void SaveTableRowData(TableRowData rowData)
        {
            string json = JsonConvert.SerializeObject(rowData, Formatting.Indented);

            File.WriteAllText(tableConf, json);
        }

        public TableRowData GetTableRowData()
        {
            string jsonText = File.ReadAllText(tableConf);
            var item = JsonConvert.DeserializeObject<TableRowData>(jsonText);

            return item;
        }

        public void SaveToArchive(string textData, DateTime dayOfSave)
        {
            ArchiveSchedule archiveSchedule = new ArchiveSchedule
            {
                TextData = textData,
                DateOfSchedule = dayOfSave,
            };
            string json = JsonConvert.SerializeObject(archiveSchedule, Formatting.Indented);

            File.WriteAllText(archiveConf, json);
        }

        public ArchiveSchedule GetFromArchive(DateTime dayOfSave)
        {
            string jsonText = File.ReadAllText(archiveConf);
            var item = JsonConvert.DeserializeObject<ArchiveSchedule>(jsonText);
            return item;
        }

        public ScheduleTable GetScheduleTable(DayOfWeek day, string dayOfSchedule)
        {
            if (dayOfSchedule == "(Знаменатель) Первая смена")
            {
                return ReadScgeduleTable(day, secondScheduleConf);
            }
            else
            {
                return ReadScgeduleTable(day, firstScheduleConf);
            }
            return null;
        }

        private ScheduleTable ReadScgeduleTable(DayOfWeek day, string path)
        {
            string jsonText = File.ReadAllText(path);
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

}
