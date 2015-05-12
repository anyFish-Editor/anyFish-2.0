using UnityEngine;
using System.Collections;

public class RayGizmoDraw : MonoBehaviour {
	
	public Vector3 direction;
	public float length;
	public Vector3 offset;
	public Color color;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos ()
	{
		//Vector3 hold = new Vector3 (transform.rotation.x, transform.rotation.y, transform.rotation.z);
		
		Gizmos.color = color;
		
		 Vector3 calcDirection = transform.TransformDirection (direction) * length;
		 Gizmos.DrawRay (transform.position+offset, calcDirection); // hold.normalized );
	}
}
