using UnityEngine;
using System.Collections;
	public class SetFramerate : MonoBehaviour {

		//want to set the framerate for your game simply attach this script to any gameobject.
		public int framerate=5;
		// Use this for initialization
		void Start () {
			Application.targetFrameRate = framerate;
            Debug.Log("Set target frame : " + framerate);
		}
		

	}
