using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Helps to extract DateTime from string using different formats.
    /// </summary>
    public class DateTimeParseHelper
    {
        public DateTime TryExtractDate(string dateTimeString){
            DateTime res = default(DateTime);
            var ruRu = new CultureInfo("ru-RU"); 
            var currentCulture = CultureInfo.CurrentCulture;
            string[] dateFormats = new string[]{
                
                "ddd, d MMM yyyy HH:mm:ss",
                "ddd, d MMM yyyy HH:mm:ss",
                "dddd, d MMMM yyyy HH:mm:ss",
                "dddd, d MMMM yyyy HH:mm:ss",
                "ddd, dd MMM yyyy hh:mm:ss",
                "ddd, dd MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy h:mm:ss",
                "ddd, d MMM yyyy H:mm:ss",
                "ddd, d MMM yyyy H:mm:ss",                
                "ddd, d MMM yyyy HH:mm:ss K",
                "d MMM yyyy HH:mm:ss K",
                "ddd, d MMM yyyy HH:mm:ss K",

            };

            var attempt = DateTime.TryParse(dateTimeString, out res);

            if(attempt == false){
                attempt = DateTime.TryParseExact(dateTimeString, 
                    CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern, 
                    ruRu, DateTimeStyles.None, out res);
            }

            if(attempt == false){
                attempt = DateTime.TryParseExact(dateTimeString, 
                    CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames, 
                    ruRu, DateTimeStyles.None, out res);
            }

            if(attempt == false){
                attempt = DateTime.TryParseExact(dateTimeString, dateFormats, 
                    ruRu, System.Globalization.DateTimeStyles.RoundtripKind, out res);
            }
            
            if(attempt == false){
                
                attempt = DateTime.TryParseExact(dateTimeString, "U", ruRu,
                    DateTimeStyles.None, out res);
            }

            if(attempt == false){
                attempt = DateTime.TryParseExact(dateTimeString, dateFormats, ruRu,
                    DateTimeStyles.None, out res);
            }

            if (attempt == false && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ruRu.DateTimeFormat.MonthNames =
                    "янв;фев;мар;апр;май;июн;июл;авг;сен;окт;ноя;дек;"
                    .Split(";")
                    .ToArray();

                attempt = DateTime.TryParse(dateTimeString, ruRu,
                    DateTimeStyles.None, out res);
            }

            if (attempt == false && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                attempt = DateTime.TryParseExact(dateTimeString, dateFormats, ruRu,
                    DateTimeStyles.None, out res);
            }

            return res;
        }
    }
}