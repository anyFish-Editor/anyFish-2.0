using UnityEngine;
using System.Collections;

public class ShortenedInit : MonoBehaviour {
	public GameObject blendRigRoot;
	public GameObject blendRigTarget;
	// Use this for initialization
	void Start () {
		// We'll move the blendRig to the starting MotionCaptureTarget
		// In larger project the MotionCaptureTarget point is pulled through space
		// using a blend between keyframed and motioncapture coordinates
		// pulling the blendrig behind it.
		blendRigRoot.transform.position = transform.position;
			
		// Connect the blendrig nodes
		Rigidbody rb = blendRigTarget.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		FixedJoint fj =  blendRigTarget.GetComponent<FixedJoint>();
		fj.connectedBody = rigidbody;
	}
	
	void Awake()
	{
			
	}
	// Update is called once per frame
	void Update () {
	
	}
}
