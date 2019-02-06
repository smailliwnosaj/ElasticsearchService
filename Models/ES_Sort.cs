

using System.Collections.Generic;

namespace ElasticsearchService.Models
{
    public class ES_Sort
    {
        public enum By
        {
            scoreDESC,
            scoreASC,
            dateTimeDESC,
            dateTimeASC,
            dayOfWeekDESC,
            dayOfWeekASC,
            timeOfDayDESC,
            timeOfDayASC,
            //categoryDESC,
            //categoryASC,
            durationDESC,
            durationASC,
            numberDESC,
            numberASC,
            fromNumberDESC,
            fromNumberASC,
            toNumberDESC,
            toNumberASC
        }

        public static List<object> GetQuerySort(By by)
        {
            var sort = new List<object>();

            switch (by)
            {

                case ES_Sort.By.scoreDESC:
                    sort.Add(new ES_Sort.ScoreDesc());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.scoreASC:
                    sort.Add(new ES_Sort.ScoreAsc());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                case ES_Sort.By.dateTimeASC:
                    sort.Add(new ES_Sort.DateTimeAsc());
                    break;
                case ES_Sort.By.dateTimeDESC:
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                //case ES_Sort.By.categoryDESC:
                //    sort.Add(new ES_Sort.CategoryDESC());
                //    sort.Add(new ES_Sort.DateTimeDesc());
                //    break;
                //case ES_Sort.By.categoryASC:
                //    sort.Add(new ES_Sort.CategoryASC());
                //    sort.Add(new ES_Sort.DateTimeDesc());
                //    break;

                case ES_Sort.By.dayOfWeekDESC:
                    sort.Add(new ES_Sort.DayOfWeekDESC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.dayOfWeekASC:
                    sort.Add(new ES_Sort.DayOfWeekASC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                case ES_Sort.By.durationDESC:
                    sort.Add(new ES_Sort.DurationDESC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.durationASC:
                    sort.Add(new ES_Sort.DurationASC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                case ES_Sort.By.fromNumberDESC:
                    sort.Add(new ES_Sort.FromNumberDESC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.fromNumberASC:
                    sort.Add(new ES_Sort.FromNumberASC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                case ES_Sort.By.numberDESC:
                    sort.Add(new ES_Sort.NumberDESC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.numberASC:
                    sort.Add(new ES_Sort.NumberASC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                case ES_Sort.By.timeOfDayDESC:
                    sort.Add(new ES_Sort.TimeOfDayDESC());
                    break;
                case ES_Sort.By.timeOfDayASC:
                    sort.Add(new ES_Sort.TimeOfDayASC());
                    break;

                case ES_Sort.By.toNumberDESC:
                    sort.Add(new ES_Sort.ToNumberDESC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;
                case ES_Sort.By.toNumberASC:
                    sort.Add(new ES_Sort.ToNumberASC());
                    sort.Add(new ES_Sort.DateTimeDesc());
                    break;

                default:
                    break;
            }

            return sort;
        }

        public class ScoreDesc
        {
            public string _score { get { return "desc"; } }
        }
        public class ScoreAsc
        {
            public string _score { get { return "asc"; } }
        }

        public class DateTimeDesc
        {
            public string UtcDateTime { get { return "desc"; } }
        }
        public class DateTimeAsc
        {
            public string UtcDateTime { get { return "asc"; } }
        }

        public class DayOfWeekDESC
        {
            public string Day { get { return "desc"; } }
        }
        public class DayOfWeekASC
        {
            public string Day { get { return "asc"; } }
        }

        public class TimeOfDayDESC
        {
            public string Time { get { return "desc"; } }
        }
        public class TimeOfDayASC
        {
            public string Time { get { return "asc"; } }
        }

        //public class CategoryDESC
        //{
        //    public string Categories { get { return "desc"; } }
        //}
        //public class CategoryASC
        //{
        //    public string Categories { get { return "asc"; } }
        //}

        public class DurationDESC
        {
            public string Duration { get { return "desc"; } }
        }
        public class DurationASC
        {
            public string Duration { get { return "asc"; } }
        }

        public class NumberDESC
        {
            public string Number { get { return "desc"; } }
        }
        public class NumberASC
        {
            public string Number { get { return "asc"; } }
        }

        public class FromNumberDESC
        {
            public string FromNumber { get { return "desc"; } }
        }
        public class FromNumberASC
        {
            public string FromNumber { get { return "asc"; } }
        }

        public class ToNumberDESC
        {
            public string ToNumber { get { return "desc"; } }
        }
        public class ToNumberASC
        {
            public string ToNumber { get { return "asc"; } }
        }
    }
}
