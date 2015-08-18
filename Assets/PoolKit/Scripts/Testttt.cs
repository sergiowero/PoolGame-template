﻿using UnityEngine;
using System.Collections;

public class Testttt : MonoBehaviour {

    PoolGameScript8Ball.State m_state;

    void OnGUI()
    {
        if(GUILayout.Button("Rolling"))
        {
            m_state = PoolGameScript.State.ROLLING;
        }
        if(GUILayout.Button("Done rolling"))
        {
            m_state = PoolGameScript.State.DONE_ROLLING;
        }
        if(GUILayout.Button("Drag white ball"))
        {
            m_state = PoolGameScript.State.DRAG_WHITEBALL;
        }
        if(GUILayout.Button("Idle"))
        {
            m_state = PoolGameScript.State.IDLE;
        }

        GUILayout.Label("Rolling & Done rolling: " + ((m_state & (PoolGameScript.State.ROLLING | PoolGameScript.State.DONE_ROLLING)) != 0));
        //GUILayout.Label("Done rolling : " + ((m_state & PoolGameScript.State.DONE_ROLLING) != 0));
        GUILayout.Label("Drag white ball & Idle : " + ((m_state & (PoolGameScript.State.DRAG_WHITEBALL | PoolGameScript.State.IDLE)) != 0));
        //GUILayout.Label("Idle : " + ((m_state & PoolGameScript.State.IDLE) != 0));
    }
}
