using UnityEngine;
using System.Collections;
	//the player spawner -- will spawn the various players.
	public class PlayerSpawner : MonoBehaviour 
	{
		//the ai type that we want to spawn.
		public string aiToSpawn = "9Ball";

		//spawn the objects owned by the master client.
		public void Start()
		{
            //BaseGameManager.startGame();
		}

		void spawnPlayer (int i) 
		{
            string objectToSpawn = "HumanPlayer" + i;
            Instantiate(Resources.Load(objectToSpawn),
                                        Vector3.zero,
                                        Quaternion.identity);	
		}
		void spawnAI (int i) 
		{
			string objectToSpawn = aiToSpawn + "AIPlayer" + i;
			Instantiate(Resources.Load(objectToSpawn),
			                                          Vector3.zero,
			                                          Quaternion.identity);	
		}




	}
