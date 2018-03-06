using System;
using System.IO;
using System.Text;

public class LogFileWriter : Utils.Singleton<LogFileWriter> {
    private StreamWriter _logWriter = null;

    public void Write(string log)
    {
        if (_logWriter == null)
        {
            _logWriter = CreateStreamWriter();
            _logWriter.AutoFlush = true;
        }
        if(_logWriter != null)
        {
            _logWriter.WriteLine(GetFormatLogStrWithTime(log));
        }
    }

    private StreamWriter CreateStreamWriter()
    {
        string logPath = GetLogPath();
        if (!File.Exists(logPath))
        {
            File.Create(logPath).Close();
        }
        return new StreamWriter(logPath, true, Encoding.UTF8);
    }

    private string GetLogPath()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        string logPath = Application.persistentDataPath + "/Log/";
#else
        string logPath = System.Environment.CurrentDirectory + "/Log/";
#endif
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        return logPath + GetFormatDateStr() + ".log";
    }

    private string GetFormatDateStr()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    private string GetFormatLogStrWithTime(string log)
    {
        return string.Format("[{0}] {1}", DateTime.Now.TimeOfDay.ToString(), log);
    }
}
