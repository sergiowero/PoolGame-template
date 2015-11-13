using UnityEngine;
using System.Collections;

public class BombBall : PoolBall 
{
    private float m_TimeRemain;

    public static Delegate1Args<IPlayer> GameoverWithBoom;

    [SerializeField]
    private GameObject m_BombEffect;

    [SerializeField]
    private float m_TimeToGameOver = 1;

    private GameObject m_BombGO;

    private UnityEngine.UI.Text m_BombTickText;
    private RectTransform m_BombTickTextTransform;
    private Transform m_BombTrans;

    public override void Awake()
    {
        base.Awake();
        m_TimeRemain = ConstantData.MissionBombBallDuration;
        m_BombTickText = GetComponentInChildren<UnityEngine.UI.Text>();
        m_BombTickText.transform.SetParent(GameManager.CurrentUIRoot.transform);
        m_BombTickText.transform.SetAsFirstSibling();
        m_BombTickTextTransform = m_BombTickText.transform as RectTransform;
        m_BombTrans = transform;
    }

    public override void Update()
    {
        if (GameManager.Rules.State == GlobalState.GAMEOVER)
            return;

        if((m_state & (State.POTTED | State.HIDE)) != 0x0)
        {
            return;
        }

        base.Update();
        m_TimeRemain -= Time.deltaTime;
        if (m_TimeRemain < 0) m_TimeRemain = 0;
        m_BombTickText.text = ((int)m_TimeRemain).ToString();
        //m_BombTickTextTransform.position = MathTools.World2UI(m_BombTrans.position);
        m_BombTickTextTransform.position = MathTools.World2UI(m_BombTrans.position);
        if(m_TimeRemain <= 0)
        {
            Boom();
        }
    }

    void OnDestroy()
    {
        if (m_BombGO)
            Destroy(m_BombGO);
        if (m_BombTickText)
            Destroy(m_BombTickText.gameObject);
    }

    public override void OpenRenderer()
    {
        base.OpenRenderer();
        //m_BombTickText.gameObject.SetActive(true);
    }

    public override void CloseRenderer()
    {
        base.CloseRenderer();
        m_BombTickText.gameObject.SetActive(false);
    }

    private void Boom()
    {
        HOAudioManager.Explosion();
        m_BombGO = Instantiate(m_BombEffect, transform.position, Quaternion.identity) as GameObject;
        Invoke("GameOverWithBoom", m_TimeToGameOver);
        gameObject.SetActive(false);
    }

    private void GameOverWithBoom()
    {
        if(GameManager.Rules.State != GlobalState.GAMEOVER)
        {
            GameManager.Rules.State = GlobalState.GAMEOVER;
            if (GameoverWithBoom != null)
                GameoverWithBoom(null);
        }
        Destroy(gameObject);
    }
}
