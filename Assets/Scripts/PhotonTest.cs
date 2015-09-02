using UnityEngine;
using System.Collections;

using ExitGames.Client.Photon;

public class PhotonTest : MonoBehaviour , IPhotonPeerListener
{
    public PhotonPeer peer;

    void Start()
    {
        peer = new PhotonPeer(this, ConnectionProtocol.Udp);
    }

    void Update()
    {
        peer.Service();
    }

    void OnGUI()
    {
        if(GUILayout.Button("Connect"))
        {
            peer.Connect("localhost:5055", "ServerDemo");
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }
    public void OnEvent(EventData eventData)
    {

    }
    public void OnOperationResponse(OperationResponse operationResponse)
    {

    }
    public void OnStatusChanged(StatusCode statusCode)
    {
        switch(statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connect Success");
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnect");
                break;
        }
    }

}
