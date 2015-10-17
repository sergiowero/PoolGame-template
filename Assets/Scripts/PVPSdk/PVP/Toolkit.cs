using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;


namespace PVPSdk{
    public class Toolkit 
    {

    	/// <summary>
    	/// 返回向量r旋转角度degA后的向量
    	/// </summary>
    	/// <returns>The rotate a.</returns>
    	/// <param name="r">The red component.</param>
    	/// <param name="degA">Deg a.</param>
    	public static Vector2 RotateA(Vector2 r, float degA){
    		float radA = Mathf.Deg2Rad * degA;
    		float sinRadA = Mathf.Sin (radA);
    		float cosRadA = Mathf.Cos (radA);
    		return new Vector2 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA);
    	}

        /// <summary>
        /// 返回向量r旋转角度degA后的向量
        /// </summary>
        /// <returns>The rotate a.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="degA">Deg a.</param>
        public static Vector3 RotateA3(Vector2 r, float degA){
            float radA = Mathf.Deg2Rad * degA;
            float sinRadA = Mathf.Sin (radA);
            float cosRadA = Mathf.Cos (radA);
            //Debug.LogError (string.Format ("{0} {1} {2} {3} {4} ",r.x,r.y,degA, r.x * cosRadA - r.y * sinRadA, r.x * sinRadA + r.y * cosRadA));
            return new Vector3 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA, 0);
        }

        /// <summary>
        /// 只对X Y 生效
        /// </summary>
        /// <returns>The a3.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="degA">Deg a.</param>
        public static Vector3 RotateA3(Vector3 r, float degA){
            float radA = Mathf.Deg2Rad * degA;
            float sinRadA = Mathf.Sin (radA);
            float cosRadA = Mathf.Cos (radA);
            return new Vector3 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA, 0);
        }

        public static float GetAngle(Vector2 _from , Vector2 _to){
            if (Vector3.Cross (new Vector3 (_from.x, _from.y, 0), new Vector3 (_to.x, _to.y)).z > 0) {
                return Vector2.Angle (_from, _to);
            } else {
                return -Vector2.Angle (_from, _to);
            }


        }

        /// <summary>
        /// 忽略Z轴
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="_from">From.</param>
        /// <param name="_to">To.</param>
        public static float GetAngle(Vector3 _from, Vector3 _to){
            return GetAngle (new Vector2 (_from.x, _from.y), new Vector2 (_to.x, _to.y));
        }

        public static Vector2 GetVector2(float angle){
            float radA = Mathf.Deg2Rad * angle;
            return new Vector2 (Mathf.Cos (radA), Mathf.Sin (radA));
        }

        /// <summary>
        /// Gets the md5 hash.
        /// </summary>
        /// <returns>The md5 hash.</returns>
        /// <param name="input">Input.</param>
        /// <param name="encoding">Encoding.</param>
    	public static string GetMd5Hash(string input, string encoding)
    	{
    		MD5 md5Hash = MD5.Create ();
    		// Convert the input string to a byte array and compute the hash.
    		byte[] data = md5Hash.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(input));
    		StringBuilder sBuilder = new StringBuilder();
    		for (int i = 0; i < data.Length; i++)
    		{
    			sBuilder.Append(data[i].ToString("x2"));
    		}
    			
    		return sBuilder.ToString();
    	}

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	public static GameObject GetChild<T>(GameObject parent, string name) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		foreach (T g in gs) {
    			if (g.name.CompareTo(name) == 0) {
    				return g.gameObject;
    			}
    		}
    		return null;
    	}

        /// <summary>
        /// Gets the child t.
        /// </summary>
        /// <returns>The child t.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	public static T GetChildT<T>(GameObject parent, string name) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		foreach (T g in gs) {
    			if (g.name.CompareTo(name) == 0) {
    				return g;
    			}
    		}
    		return null;
    	}

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="names">Names.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	public static Dictionary<String, GameObject> GetChildren<T>(GameObject parent, List<String> names) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		Dictionary<String, GameObject> r = new  Dictionary<String, GameObject>  ();
    		foreach (T g in gs) {
    			if (names.Contains(g.name)) {
    				r [g.name] = g.gameObject;
     			}
    		}
    		return r;
    	}

        /// <summary>
        /// Gets the writable path.
        /// </summary>
        /// <returns>The writable path.</returns>
        public static string GetWritableRoot()
        {
            string path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
                path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents"; 

            } else if (Application.platform == RuntimePlatform.Android) {

                path = Application.persistentDataPath;

            }
            else {

                path = Application.dataPath;
            }

            return path + "/StreamingAssets/";
        }

//        public static string GetBattleJsonPath()
//        {
//            string path = null;
//            if (Application.platform == RuntimePlatform.IPhonePlayer) {
//                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
//                path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents" + GlobalData.battle_json_file_path_section; 
//
//
//            } else if (Application.platform == RuntimePlatform.Android) {
//
//                path = Application.persistentDataPath + GlobalData.battle_json_file_path_section;
//
//            }
//            else {
//
//                path = Application.dataPath + GlobalData.battle_json_file_path_section;
//            }
//
//            return path;
//        }
//
//        /// <summary>
//        /// Gets the writable path.
//        /// </summary>
//        /// <returns>The writable path.</returns>
//        public static string GetWritablePath()
//        {
//            string path = null;
//            if (Application.platform == RuntimePlatform.IPhonePlayer) {
//                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
//                path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents" + GlobalData.json_file_path_section; 
//                  
//            } else if (Application.platform == RuntimePlatform.Android) {
//
//                path = Application.persistentDataPath + GlobalData.json_file_path_section;
//
//            }
//            else {
//
//    			path = Application.dataPath + GlobalData.json_file_path_section;
//            }
//
//            return path;
//        }
//
//        /// <summary>
//        /// Saves the file.
//        /// </summary>
//        /// <returns>The file.</returns>
//        /// <param name="file_name">File name.</param>
//        /// <param name="bytes">Bytes.</param>
//        /// <param name="save_path">Save path.</param>
//        public static string SaveFile(string file_name, byte[] bytes, string save_path = "")
//        {
//            if (string.IsNullOrEmpty(save_path))
//                save_path = Toolkit.GetWritablePath ();
//            string save_file_path = save_path + file_name;
//            string temp_file_path = save_path + file_name + ".tmp";
//
//            Debug.Log ("Toolkit:Save = " + save_file_path);
//
//            if (!Directory.Exists(save_path))
//                Directory.CreateDirectory(save_path);
//
//            if (File.Exists(temp_file_path))
//                File.Delete(temp_file_path);
//
//            File.WriteAllBytes (temp_file_path, bytes);
//
//            if (File.Exists(save_file_path))
//                File.Delete(save_file_path);
//
//            File.Move (temp_file_path, save_file_path);
//
//            Debug.Log ("=========>Toolkit:Save Finish = " + save_file_path);
//
//            return save_file_path;
//        }
//
//        /// <summary>
//        /// Saves the file.
//        /// </summary>
//        /// <param name="file_name">File name.</param>
//        /// <param name="data">Data.</param>
//        /// <param name="save_path">Save path.</param>
//        /// <typeparam name="T">The 1st type parameter.</typeparam>
//        public static string SaveFile<T>(string file_name, T data, string save_path = "")
//        {
//    //        string save_path = Application.dataPath + "/user/";
//            if (string.IsNullOrEmpty(save_path))
//                save_path = Toolkit.GetWritablePath ();
//
//            string save_file_path = save_path + file_name;
//            string temp_file_path = save_path + file_name + ".tmp";
//
//    //        Debug.Log ("Toolkit:Save = " + save_file_path);
//
//            if (!Directory.Exists(save_path))
//                Directory.CreateDirectory(save_path);
//
//            if (File.Exists(temp_file_path))
//                File.Delete(temp_file_path);
//
//            File.WriteAllText(temp_file_path, JsonConvert.SerializeObject(data));
//
//            if (File.Exists(save_file_path))
//                File.Delete(save_file_path);
//
//            File.Move (temp_file_path, save_file_path);
//
//    //        Debug.Log ("======>Toolkit:Save Finish = " + save_file_path);
//
//            return save_file_path;
//        }
//
//        
//
//        /// <summary>
//        /// Loads the file.
//        /// </summary>
//        /// <returns>The file.</returns>
//        /// <param name="file_name">File name.</param>
//        /// <param name="save_path">Save path.</param>
//        /// <typeparam name="T">The 1st type parameter.</typeparam>
//    	public static T LoadFile<T>(string file_name, string save_path = null)
//        {
//    //        string save_path = Toolkit.GetWritablePath ();
//    		if ( string.IsNullOrEmpty(save_path) ) 
//    		{
//    			save_path = Toolkit.GetWritablePath ();
//    		}
//            string save_file_path = save_path + file_name;
//    //        Debug.Log ("Toolkit:Load = " + save_file_path);
//            string data = File.ReadAllText (save_file_path);
//            return JsonConvert.DeserializeObject<T>(data);
//        }
//
//        public static bool FileExists(string file_name, string path = "") {
//    //        return File.Exists (Application.dataPath + "/user/" + file_name);
//            if (string.IsNullOrEmpty(path))
//                path = GetWritablePath ();
//            string save_path = path + file_name;
//            return File.Exists (save_path);
//        }
//
//        public static void DeleteFile(string file_name, string path = "") {
//
//            if (string.IsNullOrEmpty(path))
//                path = GetWritablePath ();
//                
//            string save_path = path + file_name;
//            Debug.Log ("========Toolkit::DeleteFile " + save_path);
//            if (File.Exists(save_path))
//                File.Delete(save_path);
//        }
//
//    	/// <summary>
//    	/// Loads the zip file.
//    	/// </summary>
//    	/// <returns>The zip file.</returns>
//    	/// <param name="file_name">File_name.</param>
//    	/// <param name="pwd">Pwd.</param>
//    	/// <param name="path">Path.</param>
//    	/// <typeparam name="T">The 1st type parameter.</typeparam>
//    	public static T LoadZipFile<T> (string file_name, string pwd = "", string path = "")
//    	{
//    		if (string.IsNullOrEmpty(path))
//    			path = GetWritablePath ();
//    		string file_path = path + file_name;
//    		
//    		//TODO:json 解析不通过
//            string data = UnZipFileData (file_name, file_path, pwd);
//    		if (!string.IsNullOrEmpty(data)) {
//    			return JsonConvert.DeserializeObject<T>(data);
//    		} else {
//    			return default(T);
//    		}
//    	}
//
//    	/// <summary>
//    	/// Uns the zip file data.
//    	/// </summary>
//    	/// <returns>The zip file data.</returns>
//    	/// <param name="file">File.</param>
//    	/// <param name="pwd">Pwd.</param>
//    	public static string UnZipFileData(string fileName, string file, string pwd = "") {
//    		if (!File.Exists(file))
//    		{
//    			Debug.Log ("UnZipFileData File not Exists: "+ file);
//    			return string.Empty;
//    		}
//    		
//    		StringBuilder stringBuilder = new StringBuilder ();
//    		using (ZipInputStream s = new ZipInputStream(File.OpenRead(file))) {
//    			if (!string.IsNullOrEmpty(pwd)) {
//    				s.Password = pwd;
//    			}
//    			
//    			try {
//    				ZipEntry theEntry;
//    				while ((theEntry = s.GetNextEntry()) != null) {
//                        if(theEntry.Name + ".zip" != fileName){
//                            continue;
//                        }
//    					while (true) {
//    						byte[] data = new byte[theEntry.Size];
//    						int size = s.Read(data, 0, (int)theEntry.Size);
//    						if (size > 0) {
//    							string text = System.Text.Encoding.UTF8.GetString (data);
//    							stringBuilder.Append (text);
//    						} else {
//    							break;
//    						}
//    					}
//    				}
//    			} catch (Exception) {
//    				
//    			}
//    			
//    		}
//    		return stringBuilder.ToString();
//    	}
//
//    	/// <summary>
//    	/// Uns the zip file data.
//    	/// </summary>
//    	/// <returns>The zip file data.</returns>
//    	/// <param name="fileBuf">File buffer.</param>
//    	/// <param name="pwd">Pwd.</param>
//    	public static string UnZipFileData(byte[] fileBuf, string pwd = "") {
//    		
//    		MemoryStream stream = new MemoryStream (fileBuf);
//    		StringBuilder stringBuilder = new StringBuilder ();
//    		using (ZipInputStream s = new ZipInputStream(stream)) {
//    			if (!string.IsNullOrEmpty(pwd)) {
//    				s.Password = pwd;
//    			}
//    			
//    			try {
//    				ZipEntry theEntry;
//    				while ((theEntry = s.GetNextEntry()) != null) {
//    					while (true) {
//    						byte[] data = new byte[theEntry.Size];
//    						int size = s.Read(data, 0, (int)theEntry.Size);
//    						if (size > 0) {
//    							string text = System.Text.Encoding.UTF8.GetString (data);
//    							stringBuilder.Append (text);
//    						} else {
//    							break;
//    						}
//    					}
//    				}
//    			} catch (Exception) {
//    				
//    			}
//    			
//    		}
//    		return stringBuilder.ToString();
//    	}
//    	
//    	/// <summary>
//    	/// Uns the zip file.
//    	/// </summary>
//    	/// <returns><c>true</c>, if zip file was uned, <c>false</c> otherwise.</returns>
//    	/// <param name="file">File.</param>
//    	/// <param name="extract">Extract.</param>
//    	public static bool UnZipFile(string file , string extract, string pwd = "")
//    	{
//    		if (!File.Exists(file))
//    		{
//    			Debug.Log("file not exit!");
//    			return false;
//    		}
//    		
//    		using (ZipInputStream s = new ZipInputStream(File.OpenRead(file))) {
//    			if (!string.IsNullOrEmpty(pwd)) {
//    				//                Debug.Log ("UnZipFile pwd = " + pwd);
//    				s.Password = pwd;
//    			}
//    			
//    			ZipEntry theEntry;
//    			while ((theEntry = s.GetNextEntry()) != null) {
//    				string directoryName = extract;
//    				string fileName      = Path.GetFileName(theEntry.Name);
//    				
//    				// create directory
//    				if ( directoryName.Length > 0 ) {
//    					Directory.CreateDirectory(directoryName);
//    				}
//    				
//    				if (fileName != String.Empty) {
//    					using (FileStream streamWriter = File.Create(directoryName + theEntry.Name)) {
//    						int size = 2048;
//    						byte[] data = new byte[2048];
//    						while (true) {
//    							size = s.Read(data, 0, data.Length);
//    							if (size > 0) {
//    								streamWriter.Write(data, 0, size);
//    							} else {
//    								break;
//    							}
//    						}
//    					}
//    				}
//    			}
//    		}
//    		
//    		return true;
//    		
//    	}
//
//    	/// <summary>
//    	/// Uns the zip file.
//    	/// </summary>
//    	/// <returns><c>true</c>, if zip file was uned, <c>false</c> otherwise.</returns>
//    	/// <param name="fileBuf">File buffer.</param>
//    	/// <param name="extract">Extract.</param>
//    	/// <param name="pwd">Pwd.</param>
//    	public static bool UnZipFile(byte[] fileBuf , string extract, string pwd = "")
//    	{
//    		MemoryStream stream = new MemoryStream (fileBuf);		
//    		using (ZipInputStream s = new ZipInputStream(stream)) {
//    			if (!string.IsNullOrEmpty(pwd)) {
//    				s.Password = pwd;
//    			}
//    			ZipEntry theEntry;
//    			while ((theEntry = s.GetNextEntry()) != null) {
//    				if (theEntry.IsDirectory) {
//    					Debug.Log ("extract = " + extract);
//    					Debug.Log ("theEntry.Name = " + theEntry.Name);
//    					Directory.CreateDirectory(extract + "/" + theEntry.Name);
//    					continue;
//    				}
//
//    				string directoryName = extract;
//    				string fileName      = Path.GetFileName(theEntry.Name);
//
//    				// create directory
//    				if ( directoryName.Length > 0 ) {
//    					Directory.CreateDirectory(directoryName);
//    				}
//    				if (fileName != String.Empty) {
//    					using (FileStream streamWriter = File.Create(directoryName + theEntry.Name)) {
//    						int size = 2048;
//    						byte[] data = new byte[2048];
//    						while (true) {
//    							size = s.Read(data, 0, data.Length);
//    							if (size > 0) {
//    								streamWriter.Write(data, 0, size);
//    							} else {
//    								break;
//    							}
//    						}
//    					}
//    				}
//    			}
//    		}
//    		
//    		return true;
//    		
//    	}
//    	
//    	public static bool ZipFiles(string[] files, string dest, string pwd) {
//    		using (ZipOutputStream o = new ZipOutputStream(File.Open(dest, FileMode.Create))) {
//    			if (!string.IsNullOrEmpty(pwd)) {
//    				o.Password = pwd;
//    			}
//    			
//    			foreach(string file in files) {
//    				if (!File.Exists(file)) {
//    					return false;
//    				}
//    				
//    				FileInfo item = new FileInfo(file);  
//    				FileStream fs = File.OpenRead(item.FullName);  
//    				byte[] buffer = new byte[fs.Length];  
//    				fs.Read(buffer, 0, buffer.Length);  
//    				
//    				ZipEntry entry = new ZipEntry(item.Name);  
//    				o.PutNextEntry(entry);  
//    				o.Write(buffer, 0, buffer.Length);  
//    			}
//    			
//    			if (o != null) {
//    				o.Finish();
//    				o.Close();
//    			}
//    		}
//    		
//    		return true;
//    	}
//
        private static DateTime _starTime = new DateTime(1970, 1, 1);
    	/// <summary>
        /// Gets the unix time.
        /// Unix时间戳格式
        /// </summary>
        /// <returns>The unix time.</returns>
    	public static int GetUnixTime() 
    	{ 
            TimeSpan t = (DateTime.UtcNow - _starTime);
            return (int)t.TotalSeconds;
    	}

        /// <summary>
        /// Strings the convert list.
        /// </summary>
        /// <returns>The convert list.</returns>
        /// <param name="separator">Separator.</param>
        /// <param name="str">String.</param>
        /// <param name="list">List.</param>
        public static ArrayList StringConvertList (char separator, string str, ArrayList list)
    	{
            list.Clear();
            if (!string.IsNullOrEmpty(str))
    		{
                string[] ss = str.Split (separator);
    			foreach(string s in ss)
    				list.Add (s);
    		}

    		return list;
    	}

        /// <summary>
        /// Converts the int date time.
        /// </summary>
        /// <returns>The int date time.</returns>
        /// <param name="d">D.</param>
        public static System.DateTime ConvertIntDateTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(d);
            return time;
        }

        /// <summary>
        /// Shuffles the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            System.Random rnd = new System.Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        /// <summary>
        /// Gets the local streaming assets path.
        /// </summary>
        /// <returns>The local streaming assets path.</returns>
        /// <param name="file_name">File name.</param>
        static public string GetLocalStreamingAssetsPath(string file_name)
        {
            string url_path = System.IO.Path.Combine(Application.streamingAssetsPath, file_name);
            
            if (Application.platform != RuntimePlatform.Android)
            {
                if (!url_path.Contains ("://")) {
                    
                    url_path = "file://" + url_path;
                }
            }
            
            return url_path;
        }

//        static public List<PrizeShowData> ParseIconStr(string icon_str) {
//            List<PrizeShowData> data_list = new List<PrizeShowData> ();
//    		if (!string.IsNullOrEmpty(icon_str)) {
//    			string []icon_array = icon_str.Split('|');
//    			foreach (string s in icon_array) {
//    				
//    				string[] triple = s.Split (',');
//    				data_list.Add (new PrizeShowData(
//    					int.Parse(triple[0]), 
//    					triple[1], 
//    					int.Parse(triple[2]))); 
//    				
//    			}
//    		}
//            return data_list;
//        }        
//
//    	public static Vector3 GetAncestorPosition(Transform tranform, Transform ancestor){
//    		Vector3 position = tranform.localPosition;
//    		while (tranform.parent != null && !tranform.parent.Equals (ancestor)) {
//    			tranform = tranform.parent;
//    			position += tranform.localPosition;
//    		}
//    		return position;
//    	}
//
//        /// <summary>
//        /// 相对tranform 的位置是 position，求相对 ancestor 的位置 
//        /// </summary>
//        /// <returns>The relative position.</returns>
//        /// <param name="tranform">Tranform.</param>
//        /// <param name="position">Position.</param>
//        /// <param name="ancestor">Ancestor.</param>
//        public static Vector3 GetRelativePosition(Transform tranform, Vector3 position, Transform ancestor){
//            return position + GetAncestorPosition (tranform, ancestor);
//        }
//
//    	public static void GetAncestorTransformInfo(Transform tranform, Transform ancestor, ref Vector3[] transformInfo, ref Quaternion rotation){
//    		transformInfo[0] = tranform.localPosition;
//    		rotation = tranform.localRotation;
//    		transformInfo [1] = tranform.localScale;
//    		while (tranform.parent != null && !tranform.parent.Equals (ancestor)) {
//    			tranform = tranform.parent;
//    			transformInfo[0] += tranform.localPosition;
//    			rotation *= tranform.localRotation;
//    			transformInfo [1] += tranform.localScale;
//    		}
//    	}
//
//
//        public static void SetEquipAddProp(UILabel label, int rare) {
//            if (label != null) {
//                int add = 0;
//                if (rare >= 5 && rare < 8) {
//                    add = rare - 4;
//                } else if (rare >= 9 && rare < 12) {
//                    add = rare - 8;
//                } 
//                
//                if (add > 0) {
//                    NGUITools.SetActive(label.gameObject, true);
//                    string text = string.Format("+{0}", add);
//                    if (rare == 11) {//MAX
//                        text = "MAX";
//                    } 
//                    label.text = text;
//                } else {
//                    NGUITools.SetActive(label.gameObject, false);
//                }
//            }
//        }
//
//    //	public static string GetCopyJsonFile()
//    //	{
//    //		string file = "/StreamingAssets/copy_json.zip";
//    //		string path = null;
//    //		if (Application.platform == RuntimePlatform.IPhonePlayer) {
//    //			path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
//    //			path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents" + file; 
//    //			
//    //		} else if (Application.platform == RuntimePlatform.Android) {
//    //			
//    //			path = Application.persistentDataPath + file;
//    //			
//    //		}
//    //		else {
//    //			
//    //			path = Application.dataPath + file;
//    //		}
//    //		
//    //		return path;
//    //	}
//
//    	public static void ClearItems(UIGrid grid)
//    	{
//    		if (grid != null) {
//    			for (int i = 0; i < grid.transform.childCount; i++) {
//    				MonoBehaviour.Destroy (grid.transform.GetChild(i).gameObject);
//    			}
//    		}
//    	}

        public static int CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2048];
            int totoal_read = 0;
            int read;
            while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
                totoal_read += read;
                output.Write (buffer, 0, read);
            }

            return totoal_read;
        }

    	public static int ConvertVerToInt(string ver) {
    		string str = ver.Replace( ".", "" );
    		int assetVer = 0;
    		if (!int.TryParse(str.Trim(), out assetVer)) {
    			Debug.LogError("ConvertAssetVerToInt error ver = " + ver);
    		}
    		return assetVer;
    	}
    }

}