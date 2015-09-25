using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public abstract class HOConfigurationReader : MonoBehaviour
{

    public virtual Hashtable ParseData(TextAsset info, Hashtable table)
    {
        // TODO Toto: 解密info
        XmlDocument doc = new XmlDocument();
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
        nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
        nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
        doc.LoadXml(info.text);
        XmlNodeList rowNodeList = doc.DocumentElement.SelectNodes("//ss:Worksheet/ss:Table/ss:Row", nsmgr);
        // load attribute
        XmlNodeList nameCellNodeList = rowNodeList[0].SelectNodes("ss:Cell", nsmgr);
        string[] attributeNameArray = new string[nameCellNodeList.Count];
        for (int i = 0; i < attributeNameArray.Length; ++i)
        {
            attributeNameArray[i] = nameCellNodeList[i].SelectSingleNode("ss:Data", nsmgr).InnerText.Trim();
        }
        // load value
        for (int i = 2; i < rowNodeList.Count; ++i)
        {
            XmlNodeList cellNodeList = rowNodeList[i].SelectNodes("ss:Cell", nsmgr);
            Hashtable subTable = new Hashtable();
            int realCellIndex = 0;      // 表格列索引，记录<Cell>索引
            for (int j = 0; j < attributeNameArray.Length; ++j)     // 读取并处理行数据
            {
                if (realCellIndex >= cellNodeList.Count)       // 末尾为空
                {
                    for (int m = j; m < attributeNameArray.Length; ++m)     // 填充末尾数据
                    {
                        ParseRepeatKey(subTable, attributeNameArray[m], "0");
                    }
                    break;
                }
                if (cellNodeList[realCellIndex].Attributes["ss:Index"] != null)     // 有空格
                {
                    int fillStartJ = j;
                    j = int.Parse(cellNodeList[realCellIndex].Attributes["ss:Index"].Value) - 1;
                    if (j < attributeNameArray.Length)      // 防止行末有格子但无数据
                    {
                        for (int m = fillStartJ; m < j; ++m)     // 在当前和跳格之间填充空格数据
                        {
                            ParseRepeatKey(subTable, attributeNameArray[m], "0");
                        }
                    }
                    else
                    {
                        for (int m = fillStartJ; m < attributeNameArray.Length; ++m)     // 在当前和跳格之间填充空格数据
                        {
                            ParseRepeatKey(subTable, attributeNameArray[m], "0");
                        }
                        Debug.LogWarning("--Toto-- HOConfigurationReader->ParseData: Cell index overstep.");
                        break;
                    }
                }
                XmlNode dataNode = cellNodeList[realCellIndex].SelectSingleNode("ss:Data", nsmgr);
                string attributeName = attributeNameArray[j];
                string attributeValue = "";
                if (dataNode != null && dataNode.InnerText != "")
                {       // 本格有数据
                    attributeValue = dataNode.InnerText;
                }
                else
                {       // 本格是空
                    attributeValue = "0";
                }
                ParseRepeatKey(subTable, attributeName, attributeValue);

                ++realCellIndex;
            }
            ProcessData(table, subTable);
        }

        return table;
    }

    /// <summary>
    /// 单元格数据处理
    /// </summary>
    /// <param name="bigTable">整体表</param>
    /// <param name="subTable">子表</param>
    protected virtual void ProcessData(Hashtable bigTable, Hashtable subTable)
    {
        if ((string)subTable["ID"] == "0")
        {
            Debug.LogWarning("--Toto-- HOConfigurationReader->ProcessData: subTable id is 0.");
            return;
        }
        bigTable.Add(subTable["ID"], subTable);
    }

    /// <summary>
    /// 解析成string。“0”作为“”
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    protected static string ParseTableValueToString(object val)
    {
        string result = (string)val;
        if (result == "0")
            return "";

        return result;
    }

    protected static int ParseTableValueToInt(object val)
    {
        return int.Parse((string)val);
    }

    protected static float ParseTableValueToFloat(object val)
    {
        return float.Parse((string)val);
    }

    protected static bool ParseTableValueToBoolean(object val)
    {
        return System.Convert.ToBoolean(ParseTableValueToInt(val));
    }

    /// <summary>
    /// 解析重复key
    /// </summary>
    /// <param name="table"></param>
    /// <param name="attName"></param>
    /// <param name="attValue"></param>
    protected void ParseRepeatKey(Hashtable table, string attName, string attValue)
    {
        if (table.ContainsKey(attName))
            attValue = table[attName] + "&&" + attValue;     // && 作为分隔符
        table[attName] = attValue;
    }

    /// <summary>
    /// 序列化表数据
    /// </summary>
    public static void SerializeTable(object obj, string binaryAssetFileName)
    {
        string filePath = Application.dataPath + "/Data/Bin/" + binaryAssetFileName + ".bytes";
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, obj);
            }
            catch (System.NotImplementedException ex)
            {
                Debug.LogError("serialize exception : message = " + ex.Message + ", tostring = " + ex.ToString());
            }
        }
    }

    /// <summary>
    /// 反序列化表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="binarayAssetTable"></param>
    /// <returns></returns>
    public static T DeSerializeTable<T>(TextAsset binarayAssetTable) where T : new()
    {
        T t = new T();
        using (MemoryStream ms = new MemoryStream(binarayAssetTable.bytes))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            t = (T)formatter.Deserialize(ms);
        }
        return t;
    }
}

/* 分隔符等级说明
 * 最大级
 * $$
 * &&
 * 最小级
 * 例子：123&&456$$789&&123
*/ 
