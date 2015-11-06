using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
//all fields and method at the base class
[CustomEditor(typeof(HOAudioPlayer))]
public class AudioPlayer_Inspector : Editor 
{
    public override void OnInspectorGUI()
    {
        HOAudioPlayer audioPlayer = target as HOAudioPlayer;
        audioPlayer.m_PlayWithKey = EditorGUILayout.Toggle("勾上使用音效ID", audioPlayer.m_PlayWithKey);
        if(audioPlayer.m_PlayWithKey)
        {
            EditorGUILayout.LabelField("音效ID,用逗号连接多个ID号:");
        }
        else
        {
            EditorGUILayout.LabelField("音效名称,用逗号连接多个名称:");
        }
        audioPlayer.m_AudioName = EditorGUILayout.TextField(audioPlayer.m_AudioName);
        if (audioPlayer.m_PlayWithKey)
        {
            EditorGUILayout.LabelField("需要重复的音效ID,用逗号隔开:");
        }
        else
        {
            EditorGUILayout.LabelField("需要重复播放的音效名称,用逗号隔开:");
        }
        audioPlayer.m_LoopIDs = EditorGUILayout.TextField(audioPlayer.m_LoopIDs);
    }
}
