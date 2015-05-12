using UnityEngine;
using System;
using System.Collections;

public class PhysicsLocomotion : MonoBehaviour {
	
	public bool enablePhysics = true;
	public bool enableMotionEquation = true;
	
	//public GameObject motionTest;

	//public float var1 = 1.0f;
	//public float var2 = 1.0f;
	//public float var3 = 1.0f;
	
	public bool useMotionTarget;
	public FixedJoint motionTarget;						//	Sets the connected body for physics driven motion
	public GameObject motionTargetGroup;

	public GameObject[] joints;
	public GameObject root;								//	This is the root for the physics parenting
	public int motorJointIndex = 3;						//	This is the joint that will drive the motion & needs to be set to kinematic
	
	//Joint Settings
	
	public Vector3 axis;								//	Sets the axis about which the physics operates
	public float jointDrag = 0.1f;						//  Sets each joints Drag
	public float jointMass = 0.1f;                   	//  Sets each joints Mass
	public float taperMin = 0.10f;						//	Sets the final percentage
	public float spring = 0.0f;
	public float damper = 0.0f;
	
	//Tail/Anchor Settings
	public int tailIndex = -1;
	public float tailJointDrag = 0.1f;						//  Sets each joints Drag
	public float tailJointMass = 0.1f;                   	//  Sets each joints Mass;						
	public float tailSpring = 0.0f;
	public float tailDamper = 0.0f;
				
	private float[] offsets;
	
	//public Vector3[] offsets;

	//private StartRIG sr;
	private bool runMotion = true;
	private bool motionCaptureInit = true;
	private bool splineBuilt;


	void Awake()

	{
		//BuildSpine();  //Used for physics, disabled currently
		Array.Resize<float>(ref offsets, joints.Length);
	}
	
	void OnEnable()
	{
		//Messenger.AddListener("RunMotion", OnRunMotion);
		//Messenger.AddListener("MotionCaptureInit", OnMotionCaptureInit);
	}
	
	void OnDisable()
	{
		//Messenger.RemoveListener("RunMotion", OnRunMotion);
		//Messenger.RemoveListener("MotionCaptureInit", OnMotionCaptureInit);
	}

	void Update()
	{
		
	}
		                      
	void LateUpdate()
	{
		if( runMotion && splineBuilt == false && useMotionTarget==true && motionCaptureInit)
		{
			InitPosition();
			BuildSpine();
		}
	}
	
	void InitPosition()
	{
		root.transform.position = motionTargetGroup.transform.position;
	}

	void BuildSpine()
	{
		print("Spline Build Started");
		CalcOffsets();

		for(int s=0;s < joints.Length;s++)
		{	
		
			if(enablePhysics)
			{
				joints[s].transform.parent = root.transform;
				AddJointPhysics(s);
			}
		}
		splineBuilt = true;
		
		print("Splind Build Complete");
		//Messenger.Broadcast("PhysicsInit");
		// Attach the joints to the target object and parent it to this object   
		
	}
	

	void CalcOffsets()
	{
		for(int i = 0; i < joints.Length - 1; i++)
		{
			offsets[i] = Vector3.Distance(joints[i].transform.position, joints[i + 1].transform.position);
		}
	}

	void AddJointPhysics( int n )		//Not currently used
	{
		
		Rigidbody rigid = joints[n].AddComponent<Rigidbody>();
		
		HingeJoint ph = joints[n].AddComponent<HingeJoint>();
		
		ph.axis = axis;
		ph.useSpring = true;
		ph.anchor = Vector3.zero;
		JointSpring tempSpring = new JointSpring();
		
		float range = joints.Length - 1;
		
		float seg = ( range - (float) n ) / range;
		
		if(n == tailIndex)
		{
			tempSpring.spring = tailSpring;
			tempSpring.damper = tailDamper;
			rigid.drag = tailJointDrag;
			rigid.mass = tailJointMass;
			ph.spring = tempSpring;
		}
		else
		{
			tempSpring.spring = spring  * (seg * ( 1 - taperMin ) + taperMin);
			tempSpring.damper = damper;
			rigid.drag = jointDrag * (seg * ( 1 - taperMin ) + taperMin);
			rigid.mass = jointMass * (seg * ( 1 - taperMin ) + taperMin);
		}
			
		tempSpring.targetPosition = 0.0f;
		ph.spring = tempSpring;
		

		
		if( n <= motorJointIndex && n != 0 )
		{
			joints[n-1].hingeJoint.connectedBody = rigid;
		}
		
		if( n == motorJointIndex)
		{
			rigid.isKinematic = true;
			
			GameObject helperJoint = new GameObject( joints[n].name + "_HelperJoint" );
			
			helperJoint.transform.parent = root.transform;
			helperJoint.transform.position = joints[n].transform.position;
			helperJoint.transform.rotation = joints[n].transform.rotation;
			
			Rigidbody hrb = helperJoint.AddComponent<Rigidbody>();
			HingeJoint hph = helperJoint.AddComponent<HingeJoint>();
			
			hrb.useGravity = false;
			hrb.mass = rigid.mass;
			hrb.drag = rigid.drag;
			hrb.angularDrag = 0.0f;
			hrb.isKinematic = false;
			
			hph.axis = ph.axis;
			hph.useSpring = ph.useSpring;
			hph.anchor = ph.anchor;
			hph.spring = ph.spring;
			
			hph.connectedBody = rigid;
			
			GameObject fixedJoint = new GameObject( joints[n].name + "_FixedJoint" );
			
			fixedJoint.transform.parent = root.transform;
			fixedJoint.transform.position = joints[n].transform.position;
			fixedJoint.transform.rotation = joints[n].transform.rotation;
			
			joints[n].transform.parent = fixedJoint.transform;
			
			Rigidbody rb = fixedJoint.AddComponent<Rigidbody>();
			FixedJoint fj = fixedJoint.AddComponent<FixedJoint>();
			
			rb.useGravity = false;
			rb.mass = 0.001f;
			rb.drag = 0.0f;
			rb.angularDrag = 0.0f;
			rb.isKinematic = false;
			
			if(n != 0)
			{
				fj.connectedBody = hrb;
			}
			
			if(useMotionTarget)
			{
				print(motionTarget.name);				
				motionTarget.connectedBody = rb;
			}
		}
		
		else
		{
			rigid.isKinematic = false;
		}
		rigid.useGravity = false;

		if(n > motorJointIndex)
		{      
			ph.connectedBody = joints[n-1].rigidbody;   
		}
	}
	
	void OnDrawGizmos ()
	{
		
	}

}