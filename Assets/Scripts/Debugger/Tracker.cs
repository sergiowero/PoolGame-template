using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Debugger
{
    public class Tracker : DBERP
    {
        private Transform m_TargetTransform;
        private Vector3 m_LastTargetPoint = new Vector3(0, 0, 0);
        private int m_trackFrameCount = 5;
        private List<Vector3> m_Points = new List<Vector3>();
        private Color m_LineColor = new Color(1, 1, 1);

        void Update()
        {
            if (Time.frameCount % m_trackFrameCount == 0)
            {
                if (m_LastTargetPoint != m_TargetTransform.position)
                {
                    m_Points.Add(m_TargetTransform.position);
                    m_LastTargetPoint = m_TargetTransform.position;
                }
            }
            if (m_Points.Count > 2)
            {
                for (int i = 1; i < m_Points.Count; i++)
                {
                    Debug.DrawLine(m_Points[i - 1], m_Points[i], m_LineColor);
                }
            }
        }

        public void SetTransform(Transform t)
        {
            m_TargetTransform = t;
        }

        public void SetTrackFrameCount(int frameCount)
        {
            m_trackFrameCount = frameCount;
        }

        public void SetColor(Color c)
        {
            m_LineColor = c;
        }

        public void Clear()
        {
            m_TargetTransform = null;
            m_Points.Clear();
        }
    }
}
