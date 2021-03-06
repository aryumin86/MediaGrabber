using System;
using System.Runtime.InteropServices;
using MediaGrabber.Library.Helpers;
using Xunit;

namespace MediaGrabber.Library.Tests.Helpers
{
    public class DateTimeParseHelperTests
    {
        [Theory]
        [InlineData("1 дек 2019")]
        [InlineData("Вс, 1 дек 2019")]
        [InlineData("Вс, 1 дек 2019 20:40:00")]
        [InlineData("Вс, 1 дек 2019 20:40:00 +0300")]
        [InlineData("Sun, 12 Jan 2020 15:35:00 +0300")]
        [InlineData("1 дек 2019 20:40:00")]
        [InlineData("Tue, 14 Jan 2020 17:49:44 +0300")]
        public void TryExtractDate_IdentifyCorrectDateTime(string dateTimeString){
            var dtpHelper = new DateTimeParseHelper();
            var resDateTime = dtpHelper.TryExtractDate(dateTimeString);
            Assert.True(resDateTime.Year > 1970);
        }
    }
}