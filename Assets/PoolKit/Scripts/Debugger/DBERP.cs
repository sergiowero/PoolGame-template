using UnityEngine;
using System.Collections;

namespace Debugger
{
    public enum DebuggerType
    {
        Drawer, Tracker
    }

    public class DBERP : MonoBehaviour
    {
        public static DBERP GetComponentWithType(DebuggerType t, GameObject o)
        {
            switch(t)
            {
                case DebuggerType.Drawer:
                    Drawer drawer = o.GetComponent<Drawer>();
                    if (drawer == null)
                        drawer = o.AddComponent<Drawer>();
                    return drawer;
                case DebuggerType.Tracker:
                    Tracker tracker = o.GetComponent<Tracker>();
                    if (tracker == null)
                        tracker = o.AddComponent<Tracker>();
                    return tracker;
            }
            return null;
        }
    }
}
