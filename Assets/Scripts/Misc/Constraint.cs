using UnityEngine;
using System.Collections;

/// <summary>
/// A constraint that will constrain an object on an axis if the bool is enabled for that one.
/// </summary>
	public class Constraint : MonoBehaviour {
		/// <summary>
		/// The constraint x.
		/// </summary>
		public bool constraintX = false;

		/// <summary>
		/// Do we want to use a y-constraint.
		/// </summary>
		public bool constraintY = false;
		
		public bool constraintZ = false;
		/// <summary>
		/// The x axis.
		/// </summary>
		public Vector2 xAxis = new Vector2(-10,10);
		
		/// <summary>
		/// The y axis constraint
		/// </summary>
		public Vector2 yAxis = new Vector2(0,40);
		/// <summary>
		/// The y axis constraint
		/// </summary>
		public Vector2 zAxis = new Vector2(0,40);

        [System.NonSerialized]
        public float adjustment;

        [System.NonSerialized]
        public Vector3 min = new Vector3();
        [System.NonSerialized]
        public Vector3 max = new Vector3();

        public bool PointInTheArea(Vector3 v)
        {
            Vector3 min = new Vector3(xAxis.x + adjustment, yAxis.x + adjustment, zAxis.x + adjustment),
            max = new Vector3(xAxis.y - adjustment, yAxis.y - adjustment, zAxis.y - adjustment);
            return v.x > min.x && v.x < max.x && v.z > min.z && v.z < max.z;
        }

		void OnDrawGizmos() {
			// Draw a yellow sphere at the transform's position
			Gizmos.color = Color.red;
			Gizmos.DrawLine(new Vector3(xAxis.y,0,zAxis.x),new Vector3(xAxis.y,0,zAxis.y));
			Gizmos.DrawLine(new Vector3(xAxis.x,0,zAxis.x), new Vector3(xAxis.x,0,zAxis.y));
			Gizmos.DrawLine(new Vector3(xAxis.x,0,zAxis.x),new Vector3(xAxis.y,0,zAxis.x));
			Gizmos.DrawLine(new Vector3(xAxis.x,0,zAxis.y),new Vector3(xAxis.y,0,zAxis.y));
		}

		void LateUpdate () {
			Vector3 pos = transform.position;
            min.Set(xAxis.x + adjustment, pos.y, zAxis.x + adjustment);
            max.Set(xAxis.y - adjustment, pos.y, zAxis.y - adjustment);

            pos = MathTools.Clamp3(pos, min, max);

            //if(constraintX)
            //{
            //    pos.x = Mathf.Clamp(pos.x, xAxis.x + adjustment, xAxis.y - adjustment);
            //}
            //if(constraintY)
            //{
            //    pos.y = Mathf.Clamp(pos.y, yAxis.x + adjustment, yAxis.y - adjustment);
            //}
            //if(constraintZ)
            //{
            //    pos.z = Mathf.Clamp(pos.z, zAxis.x + adjustment, zAxis.y - adjustment);
            //}
			
			transform.position = pos;
		}

	}
