using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopMenuStandard : TopMenu
{
    [SerializeField]
    protected BasePlayer m_Player1;
    [SerializeField]
    protected BasePlayer m_Player2;
    [SerializeField]
    protected Text m_Chip;

    public BasePlayer Player1 { get { return m_Player1; } }
    public BasePlayer Player2 { get { return m_Player2; } }

    public int ChipPrize
    {
        get { return int.Parse(m_Chip.text); }
        set { m_Chip.text = value.ToString(); }
    }

    public override IPlayer[] GetPlayers()
    {
        return new BasePlayer[] { m_Player1, m_Player2 };
    }

}
