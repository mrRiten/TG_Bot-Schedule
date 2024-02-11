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
            string jsonText = JsonConvert.SerializeObject(rowData, Formatting.Indented);

            File.WriteAllText(tableConf, jsonText);
        }

        public TableRowData GetTableRowData()
        {
            string jsonText = File.ReadAllText(tableConf);
            var item = JsonConvert.DeserializeObject<TableRowData>(jsonText);

            return item;
        }

        private List<ArchiveSchedule> CheckDataInArchive(List<ArchiveSchedule> archives)
        {
            if (archives.Count >= 4)
            {
                archives.RemoveAt(0);
            }
            return archives;
        }

        private int FindIdInArchiveByDate(List<ArchiveSchedule> archives, DayOfWeek day)
        {
            int indOfUpdate = -1;
            for (int i = 0; i != archives.Count; i++)
            {
                if (archives[i].DateOfSchedule == day)
                {
                    indOfUpdate = i;
                }
            }
            return indOfUpdate;
        }

        public void SaveToArchive(string textData, DateTime dayOfSave)
        {

            var archives = CheckDataInArchive(GetAllArchive()); // clear archives

            ArchiveSchedule archiveSchedule = new ArchiveSchedule
            {
                TextData = textData,
                DateOfSchedule = dayOfSave.DayOfWeek
            };

            if (FindIdInArchiveByDate(archives, dayOfSave.DayOfWeek) != -1)
            {
                archives[FindIdInArchiveByDate(archives, dayOfSave.DayOfWeek)] = archiveSchedule;
            }
            else
            {
                archives.Add(archiveSchedule);
            }

            string jsonText = JsonConvert.SerializeObject(archives, Formatting.Indented);

            File.WriteAllText(archiveConf, jsonText);
        }

        public List<ArchiveSchedule> GetAllArchive()
        {
            string jsonText = File.ReadAllText(archiveConf);
            var archives = JsonConvert.DeserializeObject<List<ArchiveSchedule>>(jsonText);
            return archives;
        }

        public ArchiveSchedule GetFromArchive(DayOfWeek dayOfSave)
        {
            var archives = GetAllArchive();
            foreach (var record in archives)
            {
                if (record.DateOfSchedule == dayOfSave)
                {
                    return record;
                }
            }
            return null;
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
