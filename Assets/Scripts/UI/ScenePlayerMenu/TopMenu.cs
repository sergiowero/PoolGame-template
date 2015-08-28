using UnityEngine;
using System.Collections;

public abstract class TopMenu : MonoBehaviour
{
    public abstract IPlayer[] GetPlayers();
}
