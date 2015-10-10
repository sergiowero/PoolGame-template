using UnityEngine;
using System.Collections;
public class AudioHelper : MonoBehaviour
{

    public static AudioHelper m_Instance = null;

    //called when the ball enters a pocket.
    public AudioClip m_BallPotted;

    //called when the ball is fired
    public AudioClip m_BallFired;

    //called when the ball hits another ball
    public AudioClip m_BallHitBall;


    //called when the ball hits a wall.
    public AudioClip m_BallHitRail;

    public AudioClip m_Break;

    public AudioClip m_Explosion;

    public AudioSource m_Audio;


    void Awake()
    {
        m_Audio = audio;
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void onBallHitBall(Vector3 vel)
    {
        float v0 = Mathf.Max(Mathf.Abs(vel.x), Mathf.Abs(vel.y), Mathf.Abs(vel.z));
        if (m_Audio)
        {
            //v0 = Mathf.Clamp(v0, 0, 1f);
            v0 = Mathf.Clamp(v0, 0, 5);
            v0 = Mathf.Lerp(0, 1, v0 * .2f);
            m_Audio.PlayOneShot(m_BallHitBall, v0);
        }
    }

    public void onBallHitWall(Vector3 vel)
    {
        float v0 = Mathf.Max(Mathf.Abs(vel.x), Mathf.Abs(vel.y), Mathf.Abs(vel.z));
        if (m_Audio)
        {
            //v0 = Mathf.Clamp( v0 , 0, 1f);
            v0 = Mathf.Clamp(v0, 0, 5);
            v0 = Mathf.Lerp(.2f, 1, v0 * .2f);
            m_Audio.PlayOneShot(m_BallHitRail, v0);
        }
    }

    public void onFireBall()
    {
        if (m_Audio)
        {
            m_Audio.PlayOneShot(m_BallFired);
        }
    }

    public void onBallEnterPocket()
    {
        if (m_Audio)
        {
            m_Audio.PlayOneShot(m_BallPotted);
        }
    }

    public void onBreak()
    {
        if(m_Audio)
        {
            m_Audio.PlayOneShot(m_Break);
        }
    }

    public void onExplosion()
    {
        if(m_Audio)
        {
            m_Audio.PlayOneShot(m_Explosion);
        }
    }
}
