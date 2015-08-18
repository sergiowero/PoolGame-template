using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	public class PoolGameScript8Ball : PoolGameScript 
	{
        private static PoolGameScript8Ball m_Instance = null;
        public static PoolGameScript8Ball Instance { get { return m_Instance; } }
        protected void Awake()
        {
            if (m_Instance)
            {
                Debug.LogError("two " + gameObject + " in then scene");
                return;
            }
            m_Instance = this;
        }

        void Start()
        {
            Shuffle();
        }

        private void Shuffle()
        {
            Dictionary<int, PoolBall> d = Pools.Balls;
            PoolBall[] balls = Pools.BallsArray;
            
            for (int j = 0; j < 3; j++)//we shuffle 3 times
            {
                for (int i = 1, length = balls.Length; i < length; i++)
                {
                    if (i == 8) //black 8 continue
                        continue;
                    int r = Random.Range(1, 15);
                    while (r == 8)
                        r = Random.Range(1, 15);

                    Vector3 v = d[r].transform.position;
                    d[r].transform.position = d[i].transform.position;
                    d[i].transform.position = v;
                }
            }
        }


		public override void HandleFirstBallHitByWhiteBall(PoolBall ball)
		{
			m_foul = false;
		}
		public override  void EnterPocket(PoolBall ball)
		{
			enterPocketRPC(ball.name,m_playerTurn);
		}
		void enterPocketRPC(string name,int playerTurn)
		{
			m_playerTurn = playerTurn;
			GameObject go = GameObject.Find(name);
			PoolBall ball = null;
			if(go)
			{
				ball = go.GetComponent<PoolBall>();
			}


			//we sunk the 8 ball
			if(ball && 
			   ball.ballType == PoolBall.BallType.BLACK)
			{
				m_gameover=true;

				//we got all the balls down.
				if(m_players[m_playerTurn].areAllBallsDown()==false)
				{	
					BaseGameManager.gameover(m_players[m_playerTurn].playerName + " Loses!");
				}else{
					BaseGameManager.gameover( m_players[m_playerTurn].playerName + " Wins!");
				}
			}

			if(ball && ball == Pools.CueBall)
			{
				m_whiteEnteredPocket = true;
			}else if(ball && ball.pocketed==false)
			{
				m_ballsPocketed++;
			}
		}

		public override void ChangeTurn(bool foul,
		                                   int turn)
		{
			base.ChangeTurn(foul,turn);
		}


		//handle the fouls for 8-ball.
		public override bool HandleFouls()
		{
			bool fouls = false;
			int wallHit = 0;
            PoolBall[] ball = Pools.BallsArray;
            for (int i = 0; i < ball.Length; i++)
			{
                if (ball[i] && ball[i].pocketed == false && ball[i].hitWall)
				{
					wallHit++;
				}
			}

            //string foulSTR = null;

			if(m_whiteEnteredPocket)
			{
                //BaseGameManager.showTitleCard("FOUL - White ball pocketed!");
                BaseUIController.foulText.gameObject.SetActive(true);
				fouls=true;
			}
			
			if(m_foul)
			{
				BaseGameManager.showTitleCard(m_foulSTR);
				
			}

			if(m_break==false)
			{
				if(wallHit<4)
				{
					//it was a foul ball.
					m_break = true;
                    //BaseGameManager.showTitleCard("FOUL - At least 4 balls must hit the wall after a break!");
                    //fouls=true;
				}else{
					m_break=true;
				}
			}
			
			if(wallHit==0 && m_ballsPocketed==0)
			{
                //BaseGameManager.showTitleCard("FOUL - No balls hit wall, or were pocketed!");
				fouls=true;
				
			}
			m_foul = fouls;
			
			
			ClearWallHit();
			return fouls;
		}
	}
