using CcTalk.Bill;
using NUnit.Framework;

namespace CcTalk.Tests;

public class CcTalkBillValueParserTest
{
    [TestCase("CH5A", null, 0, false)]
    [TestCase(".......", null, 0, true)]
    [TestCase("CHxxxxA", null, 0, false)]
    [TestCase("CH0001A", "CH", 1, false)]
    [TestCase("CH5000A", "CH", 5000, false)]
    public void Parse(string text, string expectedCountryCode, int expectedScaledValue, bool expectedReturn)
    {
        var actualReturn = CcTalkBillValueParser.TryParse(text, out CcTalkBill actualValue);
        Assert.That(actualReturn, Is.EqualTo(expectedReturn));
        Assert.That(actualValue.CountryCode, Is.EqualTo(expectedCountryCode));
        Assert.That(actualValue.ScaledValue, Is.EqualTo(expectedScaledValue));
    }
}