using UnityEngine;
using System.Collections;
	//we are either going to use a 8 ball or 9 ball gamescript depending on which game we are playing. 
	public class PoolGameScript : MonoBehaviour 
	{
		//have we fired the ball
		protected bool m_firedBall=false;

        [System.Flags]
		public enum State
		{
			IDLE = 1 << 0,
			ROLLING = 1 << 1,
			DONE_ROLLING = 1 << 2,
            DRAG_WHITEBALL = 1 << 3,
            NONE = 1 << 4
		};

		//what state aer we in
		protected State m_state = State.IDLE;
        protected State m_PrevousState;
        public State GlobalState 
        {
            set { m_PrevousState = m_state; m_state = value; }
            get { return m_state; } 
        }
        public void ReturnPrevousState()
        {
            GlobalState = m_PrevousState;
        }

		//the currnet players turn.
		protected int m_playerTurn = 0;

		//did the white ball enter the pocket.
		protected bool m_whiteEnteredPocket;

		//have we "broken the balls"
		protected bool m_break=false;

		//the foul string
		protected string m_foulSTR;
		//the number of balls pocketed
		protected int m_ballsPocketed;

		//whos turn is it
		protected int m_turnCounter = 0;

		//a ref to the current play
		protected BasePlayer m_currentPlayer;

		//the minimum ball speed before the ball is considred stopepd
		public float minBallSpeed = 0.2f;

		//are in we in gameover state yet
		protected bool m_gameover=false;

		//a ref to the players
		protected BasePlayer[] m_players;

		//do we have a foul
		protected bool m_foul=false;

		void OnGameStart()
		{
			BasePlayer[] players = (BasePlayer[])GameObject.FindObjectsOfType(typeof(BasePlayer));
			m_players = new BasePlayer[players.Length];
//			Debug.Log ("onGameStart " + players.Length);
			if(m_players.Length>1)
			{
				for(int i=0; i<players.Length; i++)
				{
					int pi = players[i].playerIndex;

					m_players[pi] = players[i];
				}
			}else{
				m_players  = players;
			}//=
			BaseGameManager.NewRoundBegin(0);
		}
		void OnEnable()
		{
			BaseGameManager.onFireBall += OnFireBall;
			BaseGameManager.onBallEnterPocket	+= OnEnterPocket;
			BaseGameManager.onWhiteBallHitBall += OnWhiteBallHitWall;
			BaseGameManager.onGameStart += OnGameStart;
			BaseGameManager.onIsMyTurn += OnIsMyTurn;

		}
		void OnDisable()
		{
			BaseGameManager.onFireBall -= OnFireBall;
			BaseGameManager.onBallEnterPocket	-= OnEnterPocket;
			BaseGameManager.onWhiteBallHitBall -= OnWhiteBallHitWall;
			BaseGameManager.onIsMyTurn -= OnIsMyTurn;
			BaseGameManager.onGameStart-= OnGameStart;

		}

        public bool OnIsMyTurn(int playerID)
		{
			return m_currentPlayer.playerIndex == playerID;
		}

		void OnWhiteBallHitWall(bool hitBall,PoolBall ball)
		{
			//its our first hit
			if(hitBall==false)
			{
				HandleFirstBallHitByWhiteBall(ball);
			}
		}
		public virtual void HandleFirstBallHitByWhiteBall(PoolBall ball)
		{}

		void OnEnterPocket(string pocketIndex, PoolBall ball)
		{
			EnterPocket(ball);
		}
		public virtual void EnterPocket(PoolBall ball)
		{
		}

		void OnFireBall()
		{
			GlobalState = State.ROLLING;
		}
		void Update () {
            if (GlobalState == State.ROLLING)
			{
				if(m_gameover==false)
					CheckDoneRolling();
			}
		}

		void HandleWhiteInPocket()
		{
			if(m_whiteEnteredPocket)
			{
				m_whiteEnteredPocket=false;
                Pools.CueBall.Reset();
			}
		}
        
        //涉及多人模式，暂略
        public virtual void ChangeTurn(bool foul, int turn)
        {
            m_foul = foul;
            m_playerTurn = turn;
            m_currentPlayer = m_players[m_playerTurn];

            m_currentPlayer.foul = m_foul;
            WhiteBall whiteball = Pools.CueBall;
            Vector3 oldBallPos = Vector3.zero;
            bool hasOldBallpos = false;
            if (whiteball != null)
            {
                hasOldBallpos = true;
                oldBallPos = whiteball.transform.position;
                whiteball.gameObject.SetActive(false);
            }

            if (whiteball)
            {
                whiteball.gameObject.SetActive(true);
                if (hasOldBallpos)
                {
                    whiteball.transform.position = oldBallPos;
                }
                //whiteball.clear();
                whiteball.foul = m_foul;
            }

            //BaseGameManager.showTitleCard( m_players[m_playerTurn].playerName + " turn!");

            BaseGameManager.NewRoundBegin(m_playerTurn);
        }

	
		IEnumerator ChangeTurn(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			if(m_gameover==false)
			{
				ChangeTurn(m_foul,m_playerTurn);

			}

		}
		void CheckDoneRolling()
		{
			bool doneRolling = true;
            PoolBall[] balls = Pools.BallsArray;
            for (int i = 0; i < balls.Length; i++)
			{
                if (balls[i] && (balls[i].IsDoneRolling() == false &&
                   balls[i].gameObject.activeInHierarchy))
				{
					doneRolling = false;
                    break;
				}
			}
			if(doneRolling)
			{
				float waitTime=0;
				bool fouls = HandleFouls();
				//clear the wall hit.
				if(fouls)
				{
					BaseGameManager.foul ("");
					waitTime=2f;
				}


				BaseGameManager.ballStopped();
                GlobalState = State.DONE_ROLLING;
                HandleWhiteInPocket();

				//change turn!
				if(m_ballsPocketed==0 || m_whiteEnteredPocket || m_foul)
				{
					if(m_players.Length>1)
					{
						m_playerTurn^=1;

						m_currentPlayer = m_players[m_playerTurn];
					}

					StartCoroutine(ChangeTurn(waitTime));
				}else
				{
					StartCoroutine(ChangeTurn(waitTime));
				}
				m_ballsPocketed=0;
				m_turnCounter++;
			}
		}
		public virtual bool HandleFouls()
		{
			return true;
		}

		public void ClearWallHit()
		{
            PoolBall[] balls = Pools.BallsArray;
            for (int i = 0; i < balls.Length; i++)
			{
                if (balls[i])
                    balls[i].hitWall = false;
			}
		}
	}
