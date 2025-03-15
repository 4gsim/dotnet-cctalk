using System;

namespace CcTalk;

public readonly struct CcTalkError
{
    public string Messaage { get; }

    private CcTalkError(string message)
    {
        Messaage = message;
    }

    public static CcTalkError FromMessage(string message)
    {
        return new CcTalkError(message);
    }

    public static CcTalkError FromException(Exception e)
    {
        return new CcTalkError(e.Message);
    }

    public override string ToString()
    {
        return Messaage;
    }
}