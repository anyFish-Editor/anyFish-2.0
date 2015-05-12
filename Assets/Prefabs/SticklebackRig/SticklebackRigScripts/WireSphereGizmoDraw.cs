using UnityEngine;
using System.Collections;

public class WireSphereGizmoDraw : MonoBehaviour {
	
	public float radius;
	public Color color;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos ()
	{
		Gizmos.color = color;
		 Gizmos.DrawWireSphere(transform.position, radius);
	}
}
