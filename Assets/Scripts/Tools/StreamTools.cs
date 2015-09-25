using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class StreamTools
{
    public static IEnumerator LoadBytes<T>(string filePath, Delegate1Args<T> onloaded) where T : new()
    {
        WWW www = new WWW(filePath);
        Debug.Log("Loading file : " + filePath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Load file success : " + filePath);
            onloaded(StreamTools.DeserializeObject<T>(www.bytes));
        }
        else
        {
            Debug.Log(www.error);
            onloaded(default(T));
        }
    }

    public static FileInfo[] GetAllFile(string path, string pattern = null)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        List<FileInfo> fileInfos = new List<FileInfo>();
        if(dirInfo.Exists)
        {
            FileInfo[] infos;
            if (string.IsNullOrEmpty(pattern))
                infos = dirInfo.GetFiles();
            else
                infos = dirInfo.GetFiles(pattern); 
            //pass the .meta file
            foreach(var v in infos)
            {
                string name = v.Name;
                if (name.Substring(name.LastIndexOf(".") + 1).CompareTo("meta") != 0)
                    fileInfos.Add(v);
            }
            return fileInfos.ToArray();
        }
        return null;
    }

    public static void SerializeObject(object o, string fileName)
    {
        //string file = GetStreamingAssetsPath() + fileName;
        string dir = fileName.Substring(0, fileName.LastIndexOf('/'));
        if(!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string file = fileName;
        Debug.Log("Serialize object at : " + file);
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
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return default(T);
        }
        return t;
    }

    public static T DeserializeObject<T>(string fileName) where T : new()
    {
        T t = new T();
        //string file = GetStreamingAssetsPath() + fileName;
        string file = fileName;
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
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return default(T);
        }
        return t;
    }

    public static string GetStreamingAssetsPath(bool usePrefix = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string s = usePrefix ? "jar:file://" : "";
#else
        string s = usePrefix ? "file:///" : "";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return s + Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
        return s + Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
        return  s + Application.dataPath + "!/assets/";
#else 
        return s + Application.dataPath + "/StreamingAssets/";
#endif
    }

    public static string GetPersistentDataPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Application.persistentDataPath + "//";
#else
        return Application.persistentDataPath + "/";
#endif
    }

    public static string GetStreamingAssetsPathInEditor()
    {
        return "Assets/StreamingAssets/";
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

