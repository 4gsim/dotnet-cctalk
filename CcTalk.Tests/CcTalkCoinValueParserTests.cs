using CcTalk.Coin;
using NUnit.Framework;

namespace CcTalk.Tests;

public class CcTalkCoinValueParserTest
{
    [TestCase("CH5A", null, 0, false)]
    [TestCase("CH5M.A", null, 0, false)]
    [TestCase("......", null, 0, true)]
    [TestCase("CH5m0A", "CH", 0, true)]
    [TestCase("CH001A", "CH", 1, true)]
    [TestCase("CH2.5A", "CH", 2, true)]
    [TestCase("CH100A", "CH", 100, true)]
    [TestCase("CH1K0A", "CH", 1_000, true)]
    [TestCase("CH10KA", "CH", 10_000, true)]
    [TestCase("CHM25A", "CH", 250_000, true)]
    [TestCase("CH2M0A", "CH", 2_000_000, true)]
    [TestCase("CH50MA", "CH", 50_000_000, true)]
    [TestCase("CHG10A", "CH", 100_000_000, true)]
    public void Parse(string text, string expectedId, int expectedValue, bool expectedReturn)
    {
        var actualReturn = CcTalkCoinValueParser.TryParse(text, out CcTalkCoin actualValue);
        Assert.That(actualReturn, Is.EqualTo(expectedReturn));
        Assert.That(actualValue.Id, Is.EqualTo(expectedId));
        Assert.That(actualValue.IntValue, Is.EqualTo(expectedValue));
    }
}