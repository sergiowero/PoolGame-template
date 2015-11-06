using UnityEngine;
using System.Collections;

public class BombBall : PoolBall 
{
    private float m_TimeRemain;

    public static Delegate1Args<IPlayer> GameoverWithBoom;

    [SerializeField]
    private GUISkin m_Skin;

    public bool showGUI;

    public override void Awake()
    {
        base.Awake();
        m_TimeRemain = ConstantData.MissionBombBallDuration;
        showGUI = true;
    }

    public override void Update()
    {
        base.Update();
        m_TimeRemain -= Time.deltaTime;
        if (m_TimeRemain < 0) m_TimeRemain = 0;
        if(m_TimeRemain <= 0)
        {
            Boom();
        }
    }

    //public override void OnNewTurn(int turn)
    //{
    //    if (turn >= TurnDuration)
    //        Boom();
    //    m_TimeRemain = TurnDuration - turn;
    //}

    public override void CloseRenderer()
    {
        base.CloseRenderer();
        showGUI = false;
    }

    public override void OpenRenderer()
    {
        base.OpenRenderer();
        showGUI = true;
    }

    private void Boom()
    {
        AudioHelper.m_Instance.onExplosion();
        if (GameoverWithBoom != null)
            GameoverWithBoom(null);
        Destroy(gameObject);
    }

    void OnGUI()
    {
        if (showGUI)
        {
            GUISkin t = GUI.skin;
            GUI.skin = m_Skin;
            Vector2 v = Pools.SceneCamera.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(v.x, Screen.height - v.y, 50, 50), ((int)m_TimeRemain).ToString());
            GUI.skin = t;
        }
    }
}
