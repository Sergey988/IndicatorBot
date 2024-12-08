namespace IndicatorBot.Helpers;

public abstract class LogHelper
{
    private static bool _loggerEnabled = true;

    public static void SetLoggingEnabled(bool isEnabled)
    {
        _loggerEnabled = isEnabled;
    }

    public static void Log(string message)
    {
        if (_loggerEnabled)
        {
            Console.WriteLine(message);
        }
    }
}