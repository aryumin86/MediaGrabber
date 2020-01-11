using System;
using System.Globalization;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Helps to extract DateTime from string using different formats.
    /// </summary>
    public class DateTimeParseHelper
    {
        public DateTime TryExtractDate(string dateTimeString){
            DateTime res = default(DateTime);
            string[] dateFormats = new string[]{
                "ddd, d MMM yyyy HH:mm:ss", //Вс, 1 дек 2019 20:40:00 +0300
                "ddd, d MMM yyyy HH:mm:ss",
                "dddd, d MMMM yyyy HH:mm:ss",
                "dddd, d MMMM yyyy HH:mm:ss",
                "ddd, dd MMM yyyy hh:mm:ss",
                "ddd, dd MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy H:mm:ss",
                "ddd, d MMM yyyy H:mm:ss",
            };

            var attempt = DateTime.TryParse(dateTimeString, out res);
            if(attempt == false){
                attempt = DateTime.TryParseExact(dateTimeString, dateFormats, 
                    System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out res);
            }
            
            if(attempt == false){
                var ruRu = new CultureInfo("ru-RU"); 
                attempt = DateTime.TryParseExact(dateTimeString, "U", ruRu,
                    DateTimeStyles.None, out res);
            }

            if(attempt == false){
                var ruRu = new CultureInfo("ru-RU"); 
                attempt = DateTime.TryParseExact(dateTimeString, dateFormats, ruRu,
                    DateTimeStyles.None, out res);
            }

            return res;
        }
    }
}