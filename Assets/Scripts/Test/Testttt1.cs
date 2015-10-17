using UnityEngine;
using System.Collections;

public class Testttt1 : MonoBehaviour
{
    [SerializeField]
    private Texture2D m_Texture;


    [SerializeField]
    private Color m_TextureColor;

    void OnGUI()
    {
        GUI.color = m_TextureColor;
        GUI.DrawTexture(new Rect(0, 0, m_Texture.width, m_Texture.height), m_Texture);
    }
}