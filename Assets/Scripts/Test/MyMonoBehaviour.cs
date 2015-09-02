using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour
{
    private List<DataObject> m_Data = new List<DataObject>();
    private Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);

    void Awake()
    {
        m_Data.Add(new DataObject("One", 1, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        m_Data.Add(new DataObject("Two", 2, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        m_Data.Add(new DataObject("Three", 3, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        m_Data.Add(new DataObject("Four", 4, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
        m_Data.Add(new DataObject("Five", 5, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
    }

    public void OnGUI()
    {
        DataObject toFront, dropDead;
        Color color;

        GUI.Box(dropTargetRect, "Die");

        toFront = dropDead = null;
        foreach (DataObject data in m_Data)
        {
            color = GUI.color;

            if (data.Dragging)
            {
                GUI.color = dropTargetRect.Contains(Event.current.mousePosition) ? Color.red : color;
            }

            data.OnGUI();

            GUI.color = color;

            if (data.Dragging)
            {
                if (m_Data.IndexOf(data) != m_Data.Count - 1)
                {
                    toFront = data;
                }
            }
        }

        if (toFront != null)
        // Move an object to front if needed
        {
            m_Data.Remove(toFront);
            m_Data.Add(toFront);
        }
    }
}