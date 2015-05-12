using UnityEngine;
using System.Collections;
using System.IO;

public class Swordtail_dorsal : MonoBehaviour {

 	public string rigName = "dorsal";
	public int connectFront_TPS = 5;
	public int connectFrontMid_TPS = 6;
	public int connectBackMid_TPS = 7;
	public int connectBack_TPS = 8;
	
	public int endFront_TPS = 24;	
	public int endFrontMid_TPS = 25;	
	public int endBackMid_TPS = 26;
	public int endBack_TPS = 27;
	
    //make private
	private Transform[] jointsFront = new Transform[7];
	private Transform[] jointsFrontMid = new Transform[7];
	private Transform[] jointsBackMid = new Transform[7];
	private Transform[] jointsBack = new Transform[7];
	private Vector3[] tpsData;
	private bool loaded = false;
	private WWW www;
	
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 0)
		{
			Messenger.AddListener("UpdateFinTextures", updateTextures);
		}
	}
	
	void OnEnable()
    {
		Debug.Log("OnEnable Fired");
		Messenger<float>.AddListener("AdjustDorsalFin", onDorsalFinUpdate);	
		Messenger.AddListener("StateChange", onStateChange);
		
	}
	void OnDisable ()
	{
		//Messenger doesn't currently check if the even exists and causes problems if it doesn't
		// Do this hacky ish
		Debug.Log("OnDisable Fired");
		Messenger<float>.AddListener("AdjustDorsalFin", onDorsalFinUpdate);	
		Messenger.AddListener("StateChange", onStateChange);
		Messenger.AddListener("UpdateFinTextures", updateTextures);
    	Messenger<float>.RemoveListener("AdjustDorsalFin", onDorsalFinUpdate);
		Messenger.RemoveListener("StateChange", onStateChange);
		Messenger.RemoveListener("UpdateFinTextures", updateTextures);
	}
	
	public void updateTextures()
	{
		Messenger.RemoveListener("UpdateFinTextures", updateTextures);
		
		if(PlayerPrefs.GetString("OverrideDorsal") != "default")
			LoadTexture(PlayerPrefs.GetString("OverrideDorsal"));
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject fin = GameObject.Find("dorsalFin");
			
			fin.renderer.material.mainTexture = www.texture;
			//renderer.material.SetTexture(
		}
	}
	
	public void LoadTexture(string fileName)
	{
		string fileNamefull = Path.GetFullPath(fileName); //Mohammad
		fileName = fileNamefull;  //Mohammad
		Debug.Log("Loading: " + fileName);
		www = new WWW ("file://" + fileName);
		StartCoroutine(waitForFrameLoaded());	
	}
	
	public void morph(Vector3 [] data)
	{
		tpsData = data;
		if (loaded == false)
			getTransforms();
		unParent();
		setTPSpoints();
	}
	
	void onStateChange()
	{
		Messenger<float>.RemoveListener("AdjustDorsalFin", onDorsalFinUpdate);
		Messenger.RemoveListener("StateChange", onStateChange);
	}
	
	void onDorsalFinUpdate (float amount) 
	{
		//float maxAngle = 25.0f;
		Vector3 axis = new Vector3(1.0f, 0.0f, 0.0f);
		
		//t+= Time.deltaTime * 5.0f;
		//float angle = Mathf.Sin(t) * maxAngle;
		float angle = amount;
		for(int i = 2; i<6; i++)
		{
			jointsFront[i].localEulerAngles = axis * angle;
			jointsBackMid[i].localEulerAngles = axis * angle;
			jointsFrontMid[i].localEulerAngles = axis * angle;
			jointsBack[i].localEulerAngles = axis * angle;
		}
	}
	
	private void getTransforms()
	{
		jointsFront[0] = transform.FindChild(rigName).FindChild("Root").FindChild("front_connect"); 
		jointsFrontMid[0] = transform.FindChild(rigName).FindChild("Root").FindChild("front_mid_connect"); 
		jointsBackMid[0] = transform.FindChild(rigName).FindChild("Root").FindChild("back_mid_connect");
		jointsBack[0] = transform.FindChild(rigName).FindChild("Root").FindChild("back_connect");
		
		jointsFront[1] = jointsFront[0].FindChild("front_rot"); 
		jointsFrontMid[1] = jointsFrontMid[0].FindChild("front_mid_rot");
		jointsBackMid[1] = jointsBackMid[0].FindChild("back_mid_rot");
		jointsBack[1] = jointsBack[0].FindChild("back_rot");
		
		
		
		float count=1;
		for (int i=2; i<7; i++)
		{
			jointsFront[i] = jointsFront[i-1].FindChild("front_joint"+count);
			jointsFrontMid[i] = jointsFrontMid[i-1].FindChild("front_mid_joint"+count);
			jointsBackMid[i] = jointsBackMid[i-1].FindChild("back_mid_joint"+count);	
			jointsBack[i] = jointsBack[i-1].FindChild("back_joint"+count);	
			
			count++;
		}
		loaded = true;
	}
	private void unParent()
	{
		jointsFront[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsFrontMid[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsBackMid[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsBack[0].parent = transform.FindChild(rigName).FindChild("Root");
	}
	
	
	private void setTPSpoints()
	{
		
		jointsFront[0].position = tpsData[connectFront_TPS];
		jointsFrontMid[0].position = tpsData[connectFrontMid_TPS];
		jointsBackMid[0].position = tpsData[connectBackMid_TPS];
		jointsBack[0].position = tpsData[connectBack_TPS];
		
		// In case the point lies in front of the lower bone, flip the UP direction to Down to maintain
		// same X direction spin
		if(jointsFront[1].transform.position.z - tpsData[endFront_TPS].z > 0)
			jointsFront[1].LookAt(tpsData[endFront_TPS], Vector3.down);
		else
			jointsFront[1].LookAt(tpsData[endFront_TPS], Vector3.up);
		if(jointsFrontMid[1].transform.position.z - tpsData[endFrontMid_TPS].z > 0)
			jointsFrontMid[1].LookAt(tpsData[endFrontMid_TPS], Vector3.down);
		else
			jointsFrontMid[1].LookAt(tpsData[endFrontMid_TPS], Vector3.up);
		if(jointsBackMid[1].transform.position.z - tpsData[endBackMid_TPS].z > 0)
			jointsBackMid[1].LookAt(tpsData[endBackMid_TPS], Vector3.down);
		else
			jointsBackMid[1].LookAt(tpsData[endBackMid_TPS], Vector3.up);
		if(jointsBack[1].transform.position.z - tpsData[endBack_TPS].z > 0)
			jointsBack[1].LookAt(tpsData[endBack_TPS], Vector3.down);
		else
			jointsBack[1].LookAt(tpsData[endBack_TPS], Vector3.up);
		
		float lengthFront = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[endFront_TPS])/4;
		float lengthFrontMid = Vector3.Distance(tpsData[connectFrontMid_TPS],  tpsData[endFrontMid_TPS])/4;
		float lengthBackMid = Vector3.Distance(tpsData[connectBackMid_TPS],  tpsData[endBackMid_TPS])/4;
		float lengthBack = Vector3.Distance(tpsData[connectBack_TPS],  tpsData[endBack_TPS])/4;
		
		
		for (int i=3; i<7; i++)
		{
			
			jointsFront[i].localPosition = new Vector3(0,0,lengthFront);
			jointsFrontMid[i].localPosition = new Vector3(0,0,lengthFrontMid);
			jointsBackMid[i].localPosition = new Vector3(0,0,lengthBackMid);
			jointsBack[i].localPosition = new Vector3(0,0,lengthBack);
		}
	}
	
	private Vector3 blendVertex(Vector3 p1, Vector3 p2, float blend) 
	{
		Vector3 v3; 	
		v3 = p1 + blend * (p2 - p1);
		return v3;
	}	
	
	public Transform getFrontChild()
	{
		return jointsFront[0];
		
	}
	public void setFrontParent(Transform parent)
	{
		jointsFront[0].parent = parent;
	}
	
	public Transform getFrontMidChild()
	{
		return jointsFrontMid[0];
		
	}
	public void setFrontMidParent(Transform parent)
	{
		jointsFrontMid[0].parent = parent;
	}
	public Transform getBackMidChild()
	{
		return jointsBackMid[0];
	}
	public void setBackMidParent(Transform parent)
	{
		jointsBackMid[0].parent = parent;
	}	
	public Transform getBackChild()
	{
		return jointsBack[0];
	}
	public void setBackParent(Transform parent)
	{
		jointsBack[0].parent = parent;
	}		
}
