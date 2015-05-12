using UnityEngine;
using System.Collections;
using System.IO;

public class Swordtail_anal : MonoBehaviour {

 	public string rigName = "anal";
	public int connectFront_TPS = 17;
	public int connectBack_TPS = 16;
	public int endFront_TPS = 35;
	public int endBack_TPS = 34;
	
	//make private
	private Transform[] jointsBack = new Transform[6];
	private Transform[] jointsFront = new Transform[6];
	private Vector3[] tpsData;
	private bool loaded = false;
	private WWW www;
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 0)
		{
			Messenger.AddListener("UpdateFinTextures", updateTextures);	
		
			Messenger<float, float>.AddListener("AdjustAnalFin", OnAnalUpdate);	
			Messenger.AddListener("StateChange", onStateChange);
		}
	}
	
	void onStateChange()
	{
		Messenger<float, float>.RemoveListener("AdjustAnalFin", OnAnalUpdate);	
		Messenger.RemoveListener("StateChange", onStateChange);
	}
	public void updateTextures()
	{
		Debug.Log("----------------******UpdateTextures, Swordtail_anal");
		Messenger.RemoveListener("UpdateFinTextures", updateTextures);
		
		if(PlayerPrefs.GetString("OverrideAnal") != "default")
			LoadTexture(PlayerPrefs.GetString("OverrideAnal"));
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject fin = GameObject.Find("analFin");
			
			fin.renderer.material.mainTexture = www.texture;
			
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
	
	void OnAnalUpdate (float hChange, float vChange)
	{
		//float maxAngle = 25.0f;
		Vector3 axis = new Vector3(hChange / 35.0f, vChange / 35.0f, 0.0f);
		
		//t+= Time.deltaTime * 5.0f;
		//float angle = Mathf.Sin(t) * maxAngle;
		//float angle = Mathf.Atan(vChange/hChange) * 100;
		float angle = ((Mathf.Abs(vChange)+Mathf.Abs(hChange)) / 70.0f) * 30.0f;
		//float angle = amount;
		for(int i = 2; i<6; i++)
		{
			jointsBack[i].localEulerAngles = axis * angle;
			jointsFront[i].localEulerAngles = axis * angle;
		}	
	}
	
	public void morph(Vector3 [] data)
	{
		tpsData = data;
		if (loaded == false)
			getTransforms();
		unParent();
		setTPSpoints();
		
	}
	
	private void getTransforms()
	{

		jointsBack[0] = transform.FindChild(rigName).FindChild("Root").FindChild("back_connect");
		jointsBack[1] = jointsBack[0].FindChild("back_rot");
		
		jointsFront[0]= transform.FindChild(rigName).FindChild("Root").FindChild("front_connect");	
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
		jointsBack[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsFront[0].parent = transform.FindChild(rigName).FindChild("Root");
	}
	private void setTPSpoints()
	{

		jointsBack[0].position = tpsData[connectBack_TPS];
		jointsFront[0].position = tpsData[connectFront_TPS];
		
		jointsBack[1].LookAt(tpsData[endBack_TPS], Vector3.up);
		jointsFront[1].LookAt(tpsData[endFront_TPS], Vector3.up);
		
		float backLength = Vector3.Distance(tpsData[connectBack_TPS],  tpsData[endBack_TPS])/3;
		float frontLength = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[endFront_TPS])/3;
		for (int i=3; i<6; i++)
		{
			
			jointsBack[i].localPosition = new Vector3(0,0,backLength);
			jointsFront[i].localPosition = new Vector3(0,0,frontLength);
		}
	}
	public Transform getFrontChild()
	{
		return jointsFront[0];
		
	}
	public void setFrontParent(Transform parent)
	{
		jointsFront[0].parent = parent;
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
