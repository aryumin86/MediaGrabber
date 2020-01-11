using System;
using MediaGrabber.Library.Helpers;
using Xunit;

namespace MediaGrabber.Library.Tests.Helpers
{
    public class DateTimeParseHelperTests
    {
        [Theory]
        [InlineData("Вс, 1 дек 2019 20:40:00")]
        //[InlineData("Вс, 1 дек 2019 20:40:00 +0300")]
        public void TryExtractDate_IdentifyCorrectDateTime(string dateTimeString){
            var dtpHelper = new DateTimeParseHelper();
            var resDateTime = dtpHelper.TryExtractDate(dateTimeString);
            Assert.True(resDateTime.Year > 1970);
        }
    }
}