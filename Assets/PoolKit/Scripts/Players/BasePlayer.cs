using UnityEngine;
using System.Collections;
	//the base play for our bowling characters -- wether it be human or AI
	public class BasePlayer : MonoBehaviour 
	{
		//a scalar that effects how much our x-component will effect when firing the ball. The smaller it is the easier it should be to get a strike.
		public float xScalar = .25f;
		
		//the power we use to fire the ball
		public float power = 100f;

		//have we already fired the ball.
		protected bool m_fired = false;

		//do we have a gameover yet
		protected bool m_gameOver=false;

		//the player index for the player
		public int playerIndex = 0;

		//is it my turn.
		protected bool m_myTurn = false;

		//greater then 8?
		protected int m_greaterThen8 = 0;


		//the name of the player
		public string playerName = "Player 1";

		//do we have a foul
		public bool foul=true;

		public virtual void Start()
		{
			m_myTurn = playerIndex==0;
			onPlayerTurn(0);
		}
		

		public void onGameStart()
		{
			m_myTurn = playerIndex==0;
		}

		public virtual void OnEnable()
		{
			BaseGameManager.onGameOver 		+= onGameOver;
			BaseGameManager.onNewRoundBegin 	+= onPlayerTurn;
			BaseGameManager.onResetPlayer 	+= onResetPlayer;
			BaseGameManager.onSetStripesOrSolids += onSetStripesOrSolids;
			BaseGameManager.onGameStart 		+= onGameStart;
		}
		public virtual void OnDisable()
		{
			BaseGameManager.onSetStripesOrSolids -= onSetStripesOrSolids;
			BaseGameManager.onGameOver 		-= onGameOver;
			BaseGameManager.onNewRoundBegin 	-= onPlayerTurn;
			BaseGameManager.onResetPlayer 	-= onResetPlayer;
			BaseGameManager.onGameStart 		-= onGameStart;

		}
		public void onSetStripesOrSolids(int pi, bool greater8)
		{
			if(playerIndex==pi)
			{
				if(greater8)
				{
					m_greaterThen8=1;
				}else{
					m_greaterThen8=-1;
				}
			}
		}
		public bool areAllBallsDown()
		{

			bool allBallsDown = true;
			if(m_greaterThen8==0)
			{
				allBallsDown=false;
			}
			if(m_greaterThen8!=0)
			{
				PoolBall[] balls =Pools.BallsArray;
				for(int i=0; i<balls.Length; i++)
				{
					if(balls[i] && balls[i].pocketed==false)
					{
						bool isMyBall = false;
						if(m_greaterThen8==1)
						{
							isMyBall = balls[i].ballType == PoolBall.BallType.STRIPE;
						}
						if(m_greaterThen8==-1)
						{
							isMyBall = balls[i].ballType == PoolBall.BallType.SOLID;
						}
						if(isMyBall)
						{
							allBallsDown=false;
						}
					}
				}
			}
			return allBallsDown;
		}

		
		public virtual void notMyTurn()
		{
		}
		public virtual void onMyTurn()
		{
            Pools.Cue.gameObject.SetActive(true);
		}
		public virtual void onPlayerTurn(int pi)
		{
			myTurnRPC(pi);
		}
		public void myTurnRPC(int pi)
		{
			if(pi==playerIndex)
			{

				onMyTurn();
				m_fired=false;
				m_myTurn = true;
			}else{
				notMyTurn();
				m_myTurn = false;
			}

		}
		void onGameOver(string vic)
		{
			m_gameOver=true;
		}
		public void onResetPlayer(int pi)
		{
			if(playerIndex==pi)
				onMyTurn();
			reset ();
		}

		public virtual void reset()
		{
			m_fired=false;
		}
		void onShotExpires()
		{
			if(m_myTurn)
			{
				fireBall();
			}
		}
		public virtual void fireBall()
		{
		}
	}		

