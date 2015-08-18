using UnityEngine;
using System.Collections;
	//our human player script
	public class HumanPlayer : BasePlayer 
	{
        private static HumanPlayer m_Instance = null;

        public void Awake()
        {
            if(m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

		public override void Start()
		{
			base.Start();
		}

		public override void onMyTurn()
		{
			base.onMyTurn();
		}
	}
