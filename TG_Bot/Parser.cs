using HtmlAgilityPack;


namespace TG_Bot
{
    public class Parser
    {
        public void ParseHTML()
        {
            string url = "https://menu.sttec.yar.ru/timetable/rasp_first.html";

            // Создаем объект HtmlWeb и загружаем HTML страницу
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Geting Day of Schedule
            var divElements = doc.DocumentNode.SelectNodes("//div");
            string dayOfSchedule = divElements[3].InnerText;

            // Находим таблицу на странице
            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

            // Cписок для хранения данных строк таблицы
            var rowDataList = new List<TableRowData>();

            // Проверка таблицы
            if (table != null)
            {
                // Получаем все строки таблицы
                HtmlNodeCollection rows = table.SelectNodes(".//tr");

                // Перебираем каждую строку таблицы
                for (int i = 0; i < rows.Count; i++)
                {
                    // Получаем колонки текущей строки
                    HtmlNodeCollection cells = rows[i].SelectNodes(".//td");

                    // Проверяем, есть ли вторая ячейка и содержит ли она нужную нам надпись
                    if (cells.Count > 1)
                    {
                        if (cells[1].InnerText.Contains("ИС1-21"))
                        {

                            // Создаем объект для хранения данных строки
                            TableRowData rowData = new TableRowData
                            {
                                Index = i,
                                Group = cells[1].InnerText,
                                NumbersReplacementLessons = cells[2].InnerText,
                                ScheduledLessons = cells[3].InnerText,
                                ReplacementLessons = cells[4].InnerText,
                                Auditorium = cells[5].InnerText,
                                DayOfSchedule = dayOfSchedule
                            };

                            // Добавление в список
                            rowDataList.Add(rowData);
                            
                        }
                    }
                    else Console.WriteLine("Не найден 2 столбец");
                }

            }
            else Console.WriteLine("Таблица не найдена");

            ConfigWorker configWorker = new ConfigWorker();
            configWorker.SaveTableRowData(rowDataList);
        }
    }
}