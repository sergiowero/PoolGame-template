using UnityEngine;
using System.Collections;
namespace PoolKit
{
	public class AudioHelper : MonoBehaviour {

		//called when the ball enters a pocket.
		public AudioClip onBallEnterPocketAC;

		//called when the ball is fired
		public AudioClip onBallFiredAC;

		//called when the ball hits another ball
		public AudioClip onBallHitBallAC;

		
		//called when the ball hits a wall.
		public AudioClip onBallHitWallAC;

		
		//called when there is a foul.
		public AudioClip onFoulAC;
		public void OnEnable()
		{
			BaseGameManager.onBallEnterPocket	+= onBallEnterPocket;
			BaseGameManager.onFireBall			+= onFireBall;
			BaseGameManager.onBallHitBall		+= onBallHitBall;
			BaseGameManager.onBallHitWall		+= onBallHitWall;
			BaseGameManager.onFoul		+= onFoul;


		}
		public void OnDisable()
		{
			BaseGameManager.onBallHitBall		-= onBallHitBall;
			BaseGameManager.onBallHitWall		-= onBallHitWall;
			BaseGameManager.onFoul				-= onFoul;

			BaseGameManager.onBallEnterPocket  -= onBallEnterPocket;
			BaseGameManager.onFireBall			-= onFireBall;

		}
		public void onFoul(string onFoul)
		{
			if(audio)
			{
				audio.PlayOneShot( onFoulAC,0.25f );
			}
		}
		



		public void onBallHitBall(Vector3 vel)
		{
			float v0 = Mathf.Max(vel.x,vel.y,vel.z);
			if(audio)
			{
				audio.PlayOneShot( onBallHitBallAC,v0*0.1f );
			}
		}

		public void onBallHitWall(Vector3 vel)
		{
			float v0 = Mathf.Max(vel.x,vel.y,vel.z);

			if(audio)
			{
				audio.PlayOneShot( onBallHitWallAC ,v0*0.1f);
			}
		}

		public void onFireBall()
		{
			if(audio)
			{
				audio.PlayOneShot( onBallFiredAC );
			}
		}

		public void onBallEnterPocket(string pocketID,PoolBall ball)
		{
			if(audio)
			{
				audio.PlayOneShot( onBallEnterPocketAC );
			}
		}


	}
}