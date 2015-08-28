using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Debugger
{
    public class Drawer : DBERP
    {
        private List<Vector3> m_RayDirs = new List<Vector3>();
        private List<Color> m_RayColors = new List<Color>();
        private Transform m_TargetTransform;
        private Vector3 m_Point;

        void Update()
        {
            if (m_TargetTransform && m_RayDirs.Count > 0)
            {
                for (int i = 0, count = m_RayDirs.Count; i < count; i++)
                {
                    Color c;
                    if (m_RayColors.Count == 0)
                        c = Color.white;
                    else if (i >= m_RayColors.Count)
                        c = m_RayColors[m_RayColors.Count - 1];
                    else
                        c = m_RayColors[i];
                    Debug.DrawRay(m_TargetTransform.position, m_RayDirs[i], c);
                }
            }
        }

        public void SetLineColors(params Color[] color)
        {
            m_RayColors.Clear();
            foreach (Color c in color)
            {
                m_RayColors.Add(c);
            }
        }

        public void SetRays(params Vector3[] vs)
        {
            m_RayDirs.Clear();
            foreach (Vector3 v in vs)
            {
                m_RayDirs.Add(v.normalized);
            }
        }

        public void SetTransform(Transform t)
        {
            m_TargetTransform = t;
        }

        public void Clear()
        {
            m_TargetTransform = null;
            m_RayDirs.Clear();
            m_RayColors.Clear();
        }
    }
}
