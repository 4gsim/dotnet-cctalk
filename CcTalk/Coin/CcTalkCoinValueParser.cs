using System;
using System.Collections.Generic;

namespace CcTalk.Coin;

public class CcTalkCoinValueParser
{
    private static readonly Dictionary<char, double> Multipliers = new()
    {
        { 'm', 0.001 },
        { '.', 1 },
        { 'K', 1_000 },
        { 'M', 1_000_000 },
        { 'G', 1_000_000_000 }
    };

    public static bool TryParse(string text, out CcTalkCoin value)
    {
        if (text.Length < 5)
        {
            value = new CcTalkCoin();
            return false;
        }

        if ("......".Equals(text))
        {
            value = new CcTalkCoin(); // returns no coin
            return true;
        }

        var number = "";
        var multiplierValue = -1d;
        var isSet = false;
        for (var i = 2; i < 5; i++)
        {
            if (char.IsDigit(text[i]))
            {
                number += text[i];
                continue;
            }

            if (Multipliers.TryGetValue(text[i], out var multiplier) && !isSet)
            {
                multiplierValue = multiplier * Math.Pow(10, i - 4);
                isSet = true;
                continue;
            }

            value = new CcTalkCoin();
            return false;
        }

        if (!double.TryParse(number, out double val))
        {
            value = new CcTalkCoin();
            return false;
        }

        val = isSet ? val * multiplierValue : val;
        value = new CcTalkCoin
        {
            Id = text[..2],
            Value = val,
            IntValue = (int)val,
            IsValid = true
        };
        return true;
    }
}