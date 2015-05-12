using UnityEngine;
using System.Collections;
using System.IO;

public class Stickleback_dorsal : MonoBehaviour {

 	public string rigName = "dorsal";
	public int connectFront_TPS = 11;
	public int connectMid_TPS = 12;
	public int connectBack_TPS = 13;
	
	public int endFront_TPS = 50;	
	public int endBack_TPS = 51;
	
    //make private
	private Transform[] jointsFront = new Transform[6];
	private Transform[] jointsMid = new Transform[6];
	private Transform[] jointsBack = new Transform[6];
	private Vector3[] tpsData;
	private bool loaded = false;
	
	private WWW www;
	
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 1)
			Messenger.AddListener("UpdateFinTextures", updateTextures);
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
	
	private void getTransforms()
	{
		jointsFront[0] = transform.FindChild(rigName).FindChild("Root").FindChild("front_connect"); 
		jointsMid[0] = transform.FindChild(rigName).FindChild("Root").FindChild("mid_connect");
		jointsBack[0] = transform.FindChild(rigName).FindChild("Root").FindChild("back_connect");
		
		jointsFront[1] = jointsFront[0].FindChild("front_rot"); 
		jointsMid[1] = jointsMid[0].FindChild("mid_rot");
		jointsBack[1] = jointsBack[0].FindChild("back_rot");
		
		
		
		float count=1;
		for (int i=2; i<6; i++)
		{
			jointsFront[i] = jointsFront[i-1].FindChild("front_joint"+count);
			jointsMid[i] = jointsMid[i-1].FindChild("mid_joint"+count);
			jointsBack[i] = jointsBack[i-1].FindChild("back_joint"+count);	
				
			count++;
		}
		loaded = true;
	}
	private void unParent()
	{
		jointsFront[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsMid[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsBack[0].parent = transform.FindChild(rigName).FindChild("Root");
	}
	private void setTPSpoints()
	{
		
		jointsFront[0].position = tpsData[connectFront_TPS];
		jointsMid[0].position = tpsData[connectMid_TPS];
		jointsBack[0].position = tpsData[connectBack_TPS];

		jointsFront[1].LookAt(tpsData[endFront_TPS], Vector3.up);
		jointsBack[1].LookAt(tpsData[endBack_TPS], Vector3.up);
		
		float lengthFront = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[endFront_TPS])/3;
		float lengthBack = Vector3.Distance(tpsData[connectBack_TPS],  tpsData[endBack_TPS])/3;
		
		//Figure out MID look at point
		float lengthSpine = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[connectBack_TPS]);
		float lengthFrontToMid = Vector3.Distance(tpsData[connectFront_TPS],  tpsData[connectMid_TPS]);
		float blend = lengthFrontToMid/lengthSpine;
		Vector3 endMid_Point = blendVertex(tpsData[endFront_TPS],  tpsData[endBack_TPS], blend);
		
		jointsMid[1].LookAt(endMid_Point, Vector3.up);
		float lengthMid = Vector3.Distance(tpsData[connectMid_TPS],  endMid_Point)/3;
		
		for (int i=3; i<6; i++)
		{
			
			jointsFront[i].localPosition = new Vector3(0,0,lengthFront);
			jointsMid[i].localPosition = new Vector3(0,0,lengthMid);
			jointsBack[i].localPosition = new Vector3(0,0,lengthBack);
		}
	}
	
	private Vector3 blendVertex(Vector3 p1, Vector3 p2, float blend) 
	{
		Vector3 v3; 	
		//print ( p1 + " " + p2 + " " + blend);
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
	
	public Transform getMidChild()
	{
		return jointsMid[0];
	}
	public void setMidParent(Transform parent)
	{
		jointsMid[0].parent = parent;
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
