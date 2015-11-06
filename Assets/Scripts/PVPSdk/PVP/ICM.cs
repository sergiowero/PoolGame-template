using System;
using UnityEngine;

namespace PVPSdk.PVP
{
    internal class ICM
    {
        private static InternetClient _internetClient = null;
        internal static InternetClient internetClient{
            get{
                if (ICM._internetClient == null) {
                    GameObject go = new GameObject ();
                    ICM._internetClient = go.AddComponent<InternetClient> ();
                }
                return ICM._internetClient;
            }
        }
        private static HandlerRegister _handlerRegister = null;

        internal static HandlerRegister handlerRegister {
            get {
                if (_handlerRegister == null) {
                    _handlerRegister = new HandlerRegister ();
                }
                return _handlerRegister;
            }
        }
    }
}

