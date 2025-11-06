namespace CcTalk.Bill;

public static class CcTalkBillValueParser
{
    public static bool TryParse(string text, out CcTalkBill value)
    {
        if (text.Length < 7)
        {
            value = new CcTalkBill();
            return false;
        }

        if (".......".Equals(text))
        {
            value = new CcTalkBill(); // returns no coin
            return true;
        }

        if (!int.TryParse(text.Substring(2, 4), out var scaledValue))
        {
            value = new CcTalkBill();
            return false;
        }

        value = new CcTalkBill()
        {
            CountryCode = text[..2],
            ScaledValue = scaledValue
        };
        return true;
    }
}