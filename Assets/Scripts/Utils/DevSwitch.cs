using System.IO;
using UnityEngine;

public class DevSwitch 
{
    // 获取指定的开发者开关值,开关文件是一个KV形式的
    // A=B
    public static string GetDevSwitchValue(string key)
    {
        string value = "";

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        string filePath = Application.persistentDataPath + "/DevSwitch";
#else
        string filePath = System.Environment.CurrentDirectory + "/DevSwitch";
#endif

        if (!File.Exists(filePath))
        {
            return "";
        }
        else
        {
            string[] fileLines = File.ReadAllLines(filePath);
            for ( int i = 0; i < fileLines.Length; ++i )
            {
                int value_pos = fileLines[i].IndexOf(key + "=");
                if (0 <= value_pos)
                {
                    value_pos = value_pos + (key + "=").Length;
                    if (value_pos < fileLines[i].Length)
                    {
                        value = fileLines[i].Substring(value_pos);
                        break;
                    }
                }
            }
        }

        return value;
    }
	
}
