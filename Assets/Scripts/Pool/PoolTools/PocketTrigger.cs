using UnityEngine;
using System.Collections;

public class PocketTrigger : MonoBehaviour 
{
    [SerializeField]
    private Transform m_RefTrans;


    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Contains("Ball"))
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
}
