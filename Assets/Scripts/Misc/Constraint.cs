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
                realMin = new Vector3(min.x + value, min.y, min.z + value);
                realMax = new Vector3(max.x - value, max.y, max.z - value);
            }
        }

        public Vector3 min = new Vector3(-12.63f,0,-2.5f);
        public Vector3 max = new Vector3(-2.56f, 0, 2.5f);

        private Vector3 realMin;
        private Vector3 realMax;

		void OnDrawGizmos() {
			// Draw a yellow sphere at the transform's position
			Gizmos.color = Color.red;

            Gizmos.DrawWireCube((min + max) * .5f, new Vector3(max.x - min.x, max.y - min.y, max.z - min.z));
		}

		void LateUpdate () {
			Vector3 pos = transform.position;
            pos = MathTools.Clamp3(pos, realMin, realMax);
			transform.position = pos;
		}

	}
