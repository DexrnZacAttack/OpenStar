namespace OpenStar;

public class Constants
{
    public const string ConsoleOutputTemplate =
        "{Timestamp:HH:mm:ss} [{Level:u3} | {SourceContext}] {Message:lj}{NewLine}{Exception}";

    public const string FileOutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} | {SourceContext}] {Message:lj}{NewLine}{Exception}";
}