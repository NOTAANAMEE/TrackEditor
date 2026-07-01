using System.IO;

namespace TrackEditor.Operations;

internal class ParseCheck
{
    public bool All { get; private set; } = true;

    public bool Any { get; private set; }

    public string Message { get; private set; } = "";

    private ParseCheck Read(bool successful, string message)
    {
        All = All && successful;
        Any = Any || successful;
        if (!successful && string.IsNullOrEmpty(Message)) Message = message;
        return this;
    }

    public ParseCheck ParseInt(string? str, string valueName, out int ret)
        => Read(int.TryParse(str, out ret), $"Failed to parse {valueName} as an integer.");

    public ParseCheck ParseDouble(string? str, string valueName, out double ret)
        => Read(double.TryParse(str, out ret), $"Failed to parse {valueName} as a decimal.");

    public ParseCheck ParseFloat(string? str, string valueName, out float ret)
        => Read(float.TryParse(str, out ret), $"Failed to parse {valueName} as a decimal.");

    public ParseCheck ParseBool(string? str, string valueName, out bool ret)
        => Read(bool.TryParse(str, out ret), $"Failed to parse {valueName} as a boolean.");

    public ParseCheck ParseString(string? str, string valueName, out string ret)
    {
        ret = str ?? "";
        return Read(str != null, $"Failed to parse {valueName} as a string.");
    }

    public ParseCheck CheckPath(string? path, string valueName, out string ret)
    {
        ret = path ?? "";
        return Read(File.Exists(ret), $"{valueName} is not a valid path");
    }

    public ParseCheck ParseEnum<T>(string? str, string valueName, out T ret) where T : struct, Enum
        => Read(Enum.TryParse(str, out ret), 
            $"Failed to parse {valueName} as an enum of type {typeof(T).Name}.");
}
