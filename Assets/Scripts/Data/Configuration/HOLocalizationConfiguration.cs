using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class HOLocalizationConfiguration : HOConfigurationReader
{
    private static HOLocalizationConfiguration m_Instance;
    public static HOLocalizationConfiguration Instance { get { return m_Instance; } }


    public TextAsset m_TextAsset;

    protected Dictionary<int, Hashtable> m_DicTable = new Dictionary<int, Hashtable>();

    public static string GetValue(int id)
    {
        if (Instance == null)
            return "";
        string value = Instance.m_DicTable[id][ConstantData.GameLanguage.ToString()].ToString();
        return value;
    }

    public virtual void ParseTextAsset()
    {
        XmlDocument doc = new XmlDocument();
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
        nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
        nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
        nsmgr.AddNamespace("html", "http://www.w3.org/TR/REC-html40");
        doc.LoadXml(m_TextAsset.text);

        //加载表头

        //在指定命名空间下找到Row节点的集合, 这个Row节点必须满足路径为Worksheet/Table/Row
        XmlNodeList rowNodeList = doc.DocumentElement.SelectNodes("//ss:Worksheet/ss:Table/ss:Row", nsmgr);
        //在当前节点下找到Cell集合
        XmlNodeList nameCellNodeList = rowNodeList[0].SelectNodes("ss:Cell", nsmgr);
        string[] attributeNameArray = new string[nameCellNodeList.Count];
        for (int i = 0, count = attributeNameArray.Length; i < count; i++)
        {
            //获得Cell节点的值
            attributeNameArray[i] = nameCellNodeList[i].SelectSingleNode("ss:Data", nsmgr).InnerText.Trim();
        }
        //加载表的值
        for (int i = 2; i < rowNodeList.Count; i++)
        {
            XmlNodeList cellNodeList = rowNodeList[i].SelectNodes("ss:Cell", nsmgr);
            Hashtable table = new Hashtable();
            int realCellIndex = 0;
            //读取该Row下的所有Cell的数据
            for (int j = 0; j < attributeNameArray.Length; ++j)
            {
                XmlNode node = cellNodeList[realCellIndex];

                //如果node直接为空表示此后的所有列均没有值
                if (node == null) continue;

                //如果能读取到Index属性,则代表这个node为之后列的,当前列的node是空的,需要填充数据
                if (node.Attributes["ss:Index"] != null)
                {
                    int k = j;
                    j = int.Parse(node.Attributes["ss:Index"].Value) - 1;
                    if (j < attributeNameArray.Length)
                    {
                        for (int m = k; m < j; m++)
                        {
                            table[attributeNameArray[m]] = "0";
                        }
                    }
                    else
                    {
                        for (int m = k; m < attributeNameArray.Length; m++)
                        {
                            table[attributeNameArray[m]] = "0";
                        }
                        break;
                    }
                }

                XmlNode dataNode = node.SelectSingleNode("ss:Data", nsmgr);
                string attributeName = attributeNameArray[j];
                string attributeValue = "";
                //为了保险起见,这里会再检测一下数据的安全性
                if (dataNode != null && dataNode.InnerText != "")
                {
                    attributeValue = dataNode.InnerText;
                }
                else
                {
                    attributeValue = "0";
                }
                table[attributeName] = attributeValue;
                realCellIndex++;
            }
            int id = int.Parse(table["ID"].ToString());
            m_DicTable.Add(id, table);
        }
        //PrintValue();
    }

    void PrintValue()
    {
        foreach (var v in m_DicTable)
        {
            string s = v.Key.ToString();
            foreach (DictionaryEntry de in m_DicTable[v.Key])
            {
                s += " :" + de.Key + "-" + de.Value + ": ";
            }
        }
    }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
            ParseTextAsset();
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnApplicationQuit()
    {
        m_Instance = null;
    }

}
