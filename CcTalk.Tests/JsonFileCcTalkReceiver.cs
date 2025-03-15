using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CcTalk.Tests;

public class JsonFileCcTalkReceiver(string path) : ICcTalkReceiver
{
    private readonly byte _destination = 2;
    private readonly byte _source = 1;
    private readonly string[] _lines = File.ReadAllLines(path);
    private int _index = 0;

    private static void Deserialize(Dictionary<string, JsonElement> dict, ref CcTalkDataBlock data)
    {
        var sum = 0;
        foreach(var keyValue in dict)
        {
            var value = keyValue.Value;
            if (value.ValueKind == JsonValueKind.Number)
            {
                sum = (byte)(sum + value.GetByte());
                if (keyValue.Key == "Header")
                {
                    data.Header = value.GetByte();
                }
            }
            if (value.ValueKind == JsonValueKind.Array)
            {
                data.Data = new byte[value.GetArrayLength()];
                for (var i = 0; i < value.GetArrayLength(); i++)
                {
                    sum = (byte)(sum + value[i].GetByte());
                    data.Data[i] = value[i].GetByte();
                }
            }
        }
        if (sum != 0)
        {
            throw new Exception("Checksum check failed");
        }
    }

    private string Serialize(CcTalkDataBlock data)
    {
        var checksum = 256 - (byte)(_destination + _source + data.Header + data.DataLength + data.Data.Select(b => (int)b).Sum());
        var dict = new Dictionary<string, object>()
        {
            {"Destination", _destination},
            {"Source", _source},
            {"Header", data.Header},
            {"DataLength", data.DataLength},
            {"Data", data.Data},
            {"Checksum", checksum}
        };
        return JsonSerializer.Serialize(dict);
    }

    public Task<CcTalkError?> TryReceiveAsync(CcTalkDataBlock command, ref CcTalkDataBlock reply)
    {
        try
        {
            var expectedCommandString = _lines[_index++];
            var actualCommandString = Serialize(command);
            if (expectedCommandString != actualCommandString)
            {
                throw new Exception($"Unexpected command {actualCommandString}");
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(_lines[_index++]) ?? throw new Exception("Null dict");
            Deserialize(dict, ref reply);
            return Task.FromResult<CcTalkError?>(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Task.FromResult<CcTalkError?>(CcTalkError.FromMessage(e.Message));
        }
    }
}