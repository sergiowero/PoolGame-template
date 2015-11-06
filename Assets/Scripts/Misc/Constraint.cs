using UnityEngine;
using System.Collections;

/// <summary>
/// A constraint that will constrain an object on an axis if the bool is enabled for that one.
/// </summary>
	public class Constraint : MonoBehaviour {
        public float adjustment
        {
            set
            {
                realMin = new Vector3(m_Min.x + value, m_Min.y, m_Min.z + value);
                realMax = new Vector3(m_Max.x - value, m_Max.y, m_Max.z - value);
                realFirstRoundMin = new Vector3(m_FirstRoundMin.x + value, m_FirstRoundMin.y, m_FirstRoundMin.z + value);
                realFirstRoundMax = new Vector3(m_FirstRoundMax.x - value, m_FirstRoundMax.y, m_FirstRoundMax.z - value);
            }
        }

        [SerializeField]
        private bool m_FirstRound;

        [SerializeField]
        private Vector3 m_Min = new Vector3(-12.63f,0,-2.5f);
        [SerializeField]
        private Vector3 m_Max = new Vector3(-2.56f, 0, 2.5f);
        [SerializeField]
        private Vector3 m_FirstRoundMin = new Vector3(-12.63f, 0, -2.5f);
        [SerializeField]
        private Vector3 m_FirstRoundMax = new Vector3(-2.56f, 0, 2.5f);

        public Vector3 min 
        { 
            get
            {
                if (Application.isPlaying)
                    return GameManager.Rules.firstRound ? realFirstRoundMin : realMin;
                else
                    return m_Min;
            } 
        }

        public Vector3 max 
        { 
            get
            {
                if (Application.isPlaying)
                    return GameManager.Rules.firstRound ? realFirstRoundMax : realMax;
                else
                    return m_Max;
            }
        }


        private Vector3 realMin;
        private Vector3 realMax;

        private Vector3 realFirstRoundMin;
        private Vector3 realFirstRoundMax;

		void OnDrawGizmos() {
			// Draw a yellow sphere at the transform's position
            if (m_FirstRound)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube((m_FirstRoundMin + m_FirstRoundMax) * .5f, new Vector3(m_FirstRoundMax.x - m_FirstRoundMin.x, m_FirstRoundMax.y - m_FirstRoundMin.y, m_FirstRoundMax.z - m_FirstRoundMin.z));
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube((m_Min + m_Max) * .5f, new Vector3(m_Max.x - m_Min.x, m_Max.y - m_Min.y, m_Max.z - m_Min.z));
            }
		}
	}
