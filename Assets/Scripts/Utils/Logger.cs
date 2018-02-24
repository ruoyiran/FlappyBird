using UnityEngine;

public class Logger {
    public static void Print(object msg)
    {
        Debug.Log(msg);
    }

    public static void Print(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.Log(log);
    }

    public static void Warning(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.LogWarning(log);
    }

    public static void Error(string format, params object[] args)
    {
        string log = string.Format(format, args);
        Debug.LogError(log);   
    }

    public static void Exception(System.Exception ex)
    {
        Debug.LogException(ex);
    }
}
