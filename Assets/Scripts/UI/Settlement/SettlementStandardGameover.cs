using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettlementStandardGameover : MonoBehaviour {

    [SerializeField]
    private Text m_WinText;


    public void SetWinner(string name)
    {
        m_WinText.text = name + " WIN!";
    }
    public void OnBack()
    {
        Application.LoadLevel(0);
    }

    public void OnTryAgain()
    {
        Application.LoadLevel(1);
    }

}
