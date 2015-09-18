using UnityEngine;
using System.Collections;

public class TopMenuMission : TopMenu {

    [SerializeField]
    protected MissionPlayer m_Player;

    public override IPlayer[] GetPlayers()
    {
        return new IPlayer[1] { m_Player };
    }
}
