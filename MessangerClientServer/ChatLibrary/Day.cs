using System;
using System.Collections;

namespace ChatLibrary
{
    public static class Day
    {
        private static Hashtable monthsOfNumbers;

        static Day()
        {
            monthsOfNumbers = new Hashtable()
            {
                { "01", "Января" },
                { "02", "Февраля" },
                { "03", "Марта" },
                { "04", "Апреля" },
                { "05", "Мая" },
                { "06", "Июня" },
                { "07", "Июля" },
                { "08", "Августа" },
                { "09", "Сентября" },
                { "10", "Октября" },
                { "11", "Ноября" },
                { "12", "Декабря" }
            };
        }

        public static string GetParsedDate(string date)
        {
            string[] data = date.Split(new char[] {'.', ' ', ':'});
            if (Convert.ToInt32(data[2]) == DateTime.Now.Year
                && Convert.ToInt32(data[1]) == DateTime.Now.Month
                && Convert.ToInt32(data[0]) == DateTime.Now.Day)
            {
                return $"Сегодня в {data[3]}:{data[4]}";
            }

            if (Convert.ToInt32(data[2]) == DateTime.Now.Year
                && Convert.ToInt32(data[1]) == DateTime.Now.Month
                && Convert.ToInt32(data[0]) == DateTime.Now.Day - 1)
            {
                return $"Вчера в {data[3]}:{data[4]}";
            }

            if (Convert.ToInt32(data[2]) == DateTime.Now.Year)
            {
                return $"{data[0]} {monthsOfNumbers[data[1]]} в {data[3]}:{data[4]}";
            }

            return $"{data[0]} {monthsOfNumbers[data[1]]} {data[2]} в {data[3]}:{data[4]}";
        }
    }
}