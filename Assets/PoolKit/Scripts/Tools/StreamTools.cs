using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StreamTools
{
    public static void SerializeObject(object o, string fileName)
    {
        string file = GetStreamingAssetsPath() + fileName;
        using (FileStream fs = new FileStream(file, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, o);
            }
            catch (System.NotImplementedException e)
            {
                Debug.LogException(e); 
            }
        }
    }

    public static T DeserializeObject<T>(byte[] buffer) where T : new ()
    {
        T t = new T();
        try
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                t = (T)formatter.Deserialize(ms);
            }
        }
        catch (System.Exception)
        {
            return default(T);
        }
        return t;
    }

    public static T DeserializeObject<T>(string fileName) where T : new()
    {
        T t = new T();
        string file = GetStreamingAssetsPath() + fileName;
        Debug.Log("Load file : " + file);
        try
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                if (fs.Read(buffer, 0, buffer.Length) != 0)
                {
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        t = (T)formatter.Deserialize(ms);
                    }
                }
                else
                {
                    Debug.LogError("File : " + file + " content is null");
                }
            }
        }
        catch (System.Exception)
        {
            return default(T);
        }
        return t;
    }

    public static string GetStreamingAssetsPath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
        return Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
        return "jar:file://" + Application.dataPath + "!/assets/";
#endif
    }

    public static T Clone<T>(T t) where T : new()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, t);
            ms.Seek(0, 0);
            return (T)formatter.Deserialize(ms);
        }
    }
}

