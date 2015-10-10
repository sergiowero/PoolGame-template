using UnityEngine;
using System.Collections;


public class Testttt : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_Q;
    [SerializeField]
    private float m_A;

    void Update()
    {
        //float sinx = Mathf.Sin(m_A);
        //float cosx = Mathf.Cos(m_A);
        //transform.rotation = new Quaternion(m_Q.x * sinx, m_Q.y * sinx, m_Q.z * sinx, cosx);
    }

    void OnGUI()
    {
        if(GUILayout.Button("Apply"))
        {
            Matrix4x4 m = GetRotateMatrix(m_A, m_Q);
            transform.position = m.MultiplyPoint(transform.position);
            Vector3 n = m_Q.normalized;
            transform.localEulerAngles = new Quaternion(n.x, n.y, n.z, m_A) * transform.localEulerAngles;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawRay(Vector3.zero, m_Q);
    }


    /// <summary>
    /// 得到一个旋转矩阵
    /// </summary>
    /// <param name="theta">旋转角度</param>
    /// <param name="v">旋转轴</param>
    /// <returns></returns>
    private Matrix4x4 GetRotateMatrix(float theta, Vector3 v)
    {
        v = v.normalized;
        theta = Mathf.Deg2Rad * theta;
        float x = v.x, y = v.y, z = v.z;
        float cost = Mathf.Cos(theta);
        float sint = Mathf.Sin(theta);
        Matrix4x4 mat = Matrix4x4.identity;
        mat[0, 0] = cost + x * x * (1 - cost); 
        mat[0, 1] = x * y * (1 - cost) - z * sint; 
        mat[0, 2] = x * z * (1 - cost) + y * sint;
        mat[1, 0] = x * y * (1 - cost) + z * sint; 
        mat[1, 1] = cost + y * y * (1 - cost); 
        mat[1, 2] = y * z * (1 - cost) - x * sint;
        mat[2, 0] = x * z * (1 - cost) - y * sint;
        mat[2, 1] = y * z * (1 - cost) + x * sint;
        mat[2, 2] = cost + z * z * (1 - cost);
        return mat;
    }
}