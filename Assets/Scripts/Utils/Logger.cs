using UnityEngine;

public class Logger {

    public static void Print(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.Log(log);
        LogFileWriter.Instance.Write(log);
    }

    public static void Warning(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.LogWarning(log);
        LogFileWriter.Instance.Write(log);
    }

    public static void Error(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.LogError(log);

        LogFileWriter.Instance.Write(log, true);
    }

    public static void Exception(System.Exception ex)
    {
        Debug.LogException(ex);
        LogFileWriter.Instance.Write(ex.Message, true);
    }
}
