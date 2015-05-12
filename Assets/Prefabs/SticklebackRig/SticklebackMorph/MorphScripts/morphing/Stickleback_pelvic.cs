using UnityEngine;
using System.Collections;
using System.IO;

public class Stickleback_pelvic : MonoBehaviour {
	
	public string rigName = "pelvic";
	public int connectFront_TPS = 25;
	public int connectBack_TPS = 55;
	public int end_TPS = 56;
	
	//make private
	private Transform[] jointsBack = new Transform[6];
	private Transform[] jointsFront = new Transform[6];
	private Transform jointMid;
	private Vector3[] tpsData;
	private bool loaded = false;
	private WWW www;
	
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 1)
		{
		if(rigName == "pelvic") // Right Pelvic by default
			Messenger<float, float>.AddListener("AdjustRPelvicFin", OnPelvicUpdate);
		else
			Messenger<float, float>.AddListener("AdjustLPelvicFin", OnPelvicUpdate);	
		Messenger.AddListener("StateChange", onStateChange);	
		
		Messenger.AddListener("UpdateFinTextures", updateTextures);
		}
	}
	
	void onStateChange()
	{
		if(rigName == "pelvic") // Right Pelvic by default
			Messenger<float, float>.RemoveListener("AdjustRPelvicFin", OnPelvicUpdate);
		else
			Messenger<float, float>.RemoveListener("AdjustLPelvicFin", OnPelvicUpdate);	
		Messenger.RemoveListener("StateChange", onStateChange);
	}
	
	public void updateTextures()
	{
		Messenger.RemoveListener("UpdateFinTextures", updateTextures);
		
		if(PlayerPrefs.GetString("OverridePelvic") != "default")
			LoadTexture(PlayerPrefs.GetString("OverridePelvic"));
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject fin = GameObject.Find("lpelvicFin");
			
			fin.renderer.material.mainTexture = www.texture;
			
			fin = GameObject.Find("rpelvicFin");
			
			fin.renderer.material.mainTexture = www.texture;
			//renderer.material.SetTexture(
		}
	}
	
	public void LoadTexture(string fileName)
	{
		string fileNamefull = Path.GetFullPath(fileName);//Mohammad
		fileName = fileNamefull; //Mohammad
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
	
	void OnPelvicUpdate (float hChange, float vChange)
	{
		//float maxAngle = 25.0f;
		Vector3 axis = new Vector3(hChange / 35.0f, vChange / 35.0f, 0.0f);
		
		//t+= Time.deltaTime * 5.0f;
		//float angle = Mathf.Sin(t) * maxAngle;
		//float angle = Mathf.Atan(vChange/hChange) * 100;
		float angle = ((Mathf.Abs(vChange)+Mathf.Abs(hChange)) / 70.0f) * 30.0f;
		if(rigName == "pelvic")
			axis = new Vector3(axis.x, axis.y * -1.0f, axis.z);
		//float angle = amount;
		for(int i = 2; i<6; i++)
		{
			jointsBack[i].localEulerAngles = axis * angle;
			jointsFront[i].localEulerAngles = axis * angle;
			jointMid.localEulerAngles = axis * angle;
		}	
	}
	
	private void getTransforms()
	{
		jointMid = transform.FindChild(rigName).FindChild("Root").Find("mid_connect"); 

		jointsBack[0] = jointMid.FindChild("back_connect");
		jointsBack[1] = jointsBack[0].FindChild("back_rot");
		
		jointsFront[0]= jointMid.FindChild("front_connect");	
    	jointsFront[1] = jointsFront[0].FindChild("front_rot");
		
		float count=1;
		for (int i=2; i<6; i++)
		{
			
			jointsBack[i] = jointsBack[i-1].FindChild("back_joint"+count);	
			jointsFront[i] = jointsFront[i-1].FindChild("front_joint"+count);	
			count++;
		}
		loaded = true;
	}
	private void unParent()
	{
		jointMid.parent = transform.FindChild(rigName).FindChild("Root");
	}
	private void setTPSpoints()
	{
		jointMid.position= (tpsData[connectBack_TPS] + tpsData[connectFront_TPS])/2f;
		jointsBack[0].position = tpsData[connectBack_TPS];
		jointsFront[0].position = tpsData[connectFront_TPS];
		
		jointsBack[1].LookAt(tpsData[end_TPS], Vector3.up);
		jointsFront[1].LookAt(tpsData[end_TPS], Vector3.up);
		
		float backLength = Vector3.Distance(tpsData[connectBack_TPS],  tpsData[end_TPS])/3;
		float frontLength = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[end_TPS])/3;
		for (int i=3; i<6; i++)
		{
			
			jointsBack[i].localPosition = new Vector3(0,0,backLength);
			jointsFront[i].localPosition = new Vector3(0,0,frontLength);
		}
	}
	
	public Transform getChild()
	{
		return jointMid;
	}
	
	public void setParent(Transform parent)
	{
		jointMid.parent = parent;
	}
}
