using UnityEngine;
using System.Collections;

namespace Debugger
{
    [RequireComponent(typeof(CanvasGroup))]
    [ExecuteInEditMode]
    public class CanvasBuggerFix : MonoBehaviour
    {

        CanvasGroup m_CanvasGroup;

        [SerializeField]
        private bool m_Interactable;

        [SerializeField]
        private bool m_BlocksRaycasts;

        [SerializeField]
        private bool m_IgnoreParentGroup;

        void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            ResetField();
        }

        void OnEnable()
        {
            ResetField();
        }

        void OnDisable()
        {
            ResetField();
        }


        void ResetField()
        {
            m_CanvasGroup.interactable = m_Interactable;
            m_CanvasGroup.blocksRaycasts = m_BlocksRaycasts;
            m_CanvasGroup.ignoreParentGroups = m_IgnoreParentGroup;
        }
    }
}
