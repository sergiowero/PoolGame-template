using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopMenuQuickFire : TopMenu
{
    [SerializeField]
    protected QuickFirePlayer m_Player;

    public override IPlayer[] GetPlayers()
    {
        return new QuickFirePlayer[] { m_Player };
    }
}
