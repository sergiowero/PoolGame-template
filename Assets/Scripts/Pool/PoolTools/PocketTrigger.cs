using UnityEngine;
using System.Collections;

public class PocketTrigger : MonoBehaviour 
{
    [SerializeField]
    private Transform m_RefTrans;
    [SerializeField]
    private PocketIndexes m_PocketIndex;
    public PocketIndexes PocketIndex { get { return m_PocketIndex; } }

    private SpriteRenderer m_Punishment;
    private SpriteRenderer m_Reward;
    private GameObject m_Block;
    private Transform m_RealPosition;

    public static PocketIndexes PunitivePocket;
    public static PocketIndexes RewardPocket;

    [SerializeField]
    private bool m_debugmode = false;

    [SerializeField]
    private Transform m_PointerTransform;

    public Vector3 pointerPosition { get { return m_PointerTransform.position; } }

    [SerializeField]
    private bool m_DrawGizmo;

    void Awake()
    {
        m_Punishment = transform.FindChild("PunishmentSprite").GetComponent<SpriteRenderer>();
        m_Reward = transform.FindChild("RewardSprite").GetComponent<SpriteRenderer>();
        m_Block = transform.FindChild("BlockOff").gameObject;
        m_RealPosition = transform.FindChild("RealPosition");
        m_Punishment.gameObject.SetActive(false);
        m_Reward.gameObject.SetActive(false);
        if (!m_debugmode)  m_Block.gameObject.SetActive(false);
        m_RefTrans.GetComponent<PoolRecycler>().SetTrigger(this);
    }

    public void Clear()
    {
        m_Punishment.gameObject.SetActive(false);
        m_Reward.gameObject.SetActive(false);
    }

    public void Punishment()
    {
        if (m_Block.activeInHierarchy) return;

        m_Reward.gameObject.SetActive(false);
        m_Punishment.gameObject.SetActive(true);
    }

    public void Reward()
    {
        if (m_Block.activeInHierarchy) return;

        m_Reward.gameObject.SetActive(true);
        m_Punishment.gameObject.SetActive(false);
    }

    public void BlockOff()
    {
        m_Block.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    [ContextMenu("BlockOff")]
    public void BlockOffEditor()
    {
        transform.FindChild("BlockOff").gameObject.SetActive(true);
        m_debugmode = true;
    }

    [ContextMenu("Collapse")]
    public void CollapseEditor()
    {
        transform.FindChild("BlockOff").gameObject.SetActive(false);
        m_debugmode = false;
    }
#endif //#if UNITY_EDITOR
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ball"))
        {
            AudioHelper.m_Instance.onBallEnterPocket();
            BallInPocket pocket = collision.gameObject.GetComponent<BallInPocket>();
            collision.transform.GetComponent<PoolBall>().CloseRenderer();
            if(pocket == null)
            {
                pocket = collision.gameObject.AddComponent<BallInPocket>();
                pocket.SetRefTrans(m_RefTrans);
            }
        }
    }

    public Vector3 GetRealPosition()
    {
        return m_RealPosition.position;
    }

    public static void MarkPocketType(PocketIndexes punishmentIndex, PocketIndexes rewardIndex)
    {
        for(int i = 0, length = Pools.PocketTriggers.Count; i < length; i++)
        {
            PocketTrigger trigger = Pools.PocketTriggers[i];
            if((trigger.PocketIndex & punishmentIndex) != 0x0)
                trigger.Punishment();
            else if((trigger.PocketIndex & rewardIndex) != 0x0)
                trigger.Reward();
            else
                trigger.Clear();
        }
        PunitivePocket = punishmentIndex;
        RewardPocket = rewardIndex;
    }

    public static void Block(PocketIndexes pockets)
    {
        for (int i = 0, length = Pools.PocketTriggers.Count; i < length; i++)
        {
            PocketTrigger trigger = Pools.PocketTriggers[i];
            if ((trigger.PocketIndex & pockets) != 0x0)
                trigger.BlockOff();
        }
    }

    public static PocketTrigger GetPocketWithIndexes(PocketIndexes index)
    {
        for(int i = 0, count = Pools.PocketTriggers.Count; i < count; i++)
        {
            if((Pools.PocketTriggers[i].m_PocketIndex & index) != 0x0)
                return Pools.PocketTriggers[i];
        }
        return null;
    }

    public void OnDrawGizmos()
    {
        if(m_DrawGizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(pointerPosition, Vector3.one * .2f);
            Gizmos.color = Color.white;
        }
    }
}
