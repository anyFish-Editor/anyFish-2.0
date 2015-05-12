using UnityEngine;
using System.Collections;
using System.IO;

public class Swordtail_caudal : MonoBehaviour {

 	public string rigName = "caudal";
	public int connectTop_TPS = 11;
	public int connectBottom_TPS = 12;
	public int connectSwordTop_TPS = 30;
	public int connectSwordBottom_TPS = 33;
	
	public int endTop_TPS = 28;	
	public int endMid_TPS = 29;	
	public int endBottom_TPS = 33;
	public int endSwordTop_TPS = 31;	
	public int endSwordBottom_TPS = 32;
	
    //make private
	private Transform[] jointsTop = new Transform[7];
	private Transform[] jointsMid = new Transform[7];
	private Transform[] jointsBottom = new Transform[7];
	private Transform[] jointsSwordTop= new Transform[7];
	private Transform[] jointsSwordBottom = new Transform[7];
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
	public void updateTextures()
	{
		Messenger.RemoveListener("UpdateFinTextures", updateTextures);
		
		if(PlayerPrefs.GetString("OverrideCaudal") != "default")
			LoadTexture(PlayerPrefs.GetString("OverrideCaudal"));
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject fin = GameObject.Find("caudalFin");
			
			fin.renderer.material.mainTexture = www.texture;
			//renderer.material.SetTexture(
		}
	}
	
	public void LoadTexture(string fileName)
	{
		string fileNamefull = Path.GetFullPath(fileName); //Mohammad
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
	
	private void getTransforms()
	{
		jointsTop[0] = transform.FindChild(rigName).FindChild("Root").FindChild("top_connect"); 
		jointsMid[0] = transform.FindChild(rigName).FindChild("Root").FindChild("mid_connect");
		jointsBottom[0] = transform.FindChild(rigName).FindChild("Root").FindChild("bottom_connect");
		jointsSwordTop[0] = transform.FindChild(rigName).FindChild("Root").FindChild("top_sword_connect"); 
		jointsSwordBottom[0] = transform.FindChild(rigName).FindChild("Root").FindChild("bottom_sword_connect");
		
		jointsTop[1] = jointsTop[0].FindChild("top_rot"); 
		jointsMid[1] = jointsMid[0].FindChild("mid_rot");
		jointsBottom[1] = jointsBottom[0].FindChild("bottom_rot");
		
		jointsSwordTop[1] = jointsSwordTop[0].FindChild("top_sword_rot"); 
		jointsSwordBottom[1] = jointsSwordBottom[0].FindChild("bottom_sword_rot");
		
		float count=1;
		for (int i=2; i<7; i++)
		{
			jointsTop[i] = jointsTop[i-1].FindChild("top_joint"+count);
			jointsMid[i] = jointsMid[i-1].FindChild("mid_joint"+count);
			jointsBottom[i] = jointsBottom[i-1].FindChild("bottom_joint"+count);	
			jointsSwordTop[i] = jointsSwordTop[i-1].FindChild("top_sword_joint"+count);
			jointsSwordBottom[i] = jointsSwordBottom[i-1].FindChild("bottom_sword_joint"+count);
				
			count++;
		}
		loaded = true;
	}
	private void unParent()
	{
		jointsTop[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsMid[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsBottom[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsSwordTop[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsSwordBottom[0].parent = transform.FindChild(rigName).FindChild("Root");
	}
	private void setTPSpoints()
	{
		
		jointsTop[0].position = tpsData[connectTop_TPS];
		jointsBottom[0].position = tpsData[connectBottom_TPS];
		jointsMid[0].position = (tpsData[connectTop_TPS] + tpsData[connectBottom_TPS])/2;
		jointsSwordTop[0].position = tpsData[connectSwordTop_TPS];
		jointsSwordBottom[0].position = tpsData[connectSwordBottom_TPS];
		
		jointsTop[1].LookAt(tpsData[endTop_TPS], Vector3.up);
		jointsBottom[1].LookAt(tpsData[endBottom_TPS], Vector3.up);
		jointsMid[1].LookAt(tpsData[endMid_TPS], Vector3.up);
		jointsSwordTop[1].LookAt(tpsData[endSwordTop_TPS], Vector3.up);
		jointsSwordBottom[1].LookAt(tpsData[endSwordBottom_TPS], Vector3.up);
		
		float lengthTop = Vector3.Distance(tpsData[connectTop_TPS],  tpsData[endTop_TPS])/4;
		float lengthBottom = Vector3.Distance(tpsData[connectBottom_TPS],  tpsData[endBottom_TPS])/4;
		float lengthMid = Vector3.Distance(jointsMid[0].position,  tpsData[endMid_TPS])/4;
		
		float lengthSwordTop = Vector3.Distance(tpsData[connectSwordTop_TPS],  tpsData[endSwordTop_TPS])/4;
		float lengthSwordBottom = Vector3.Distance(tpsData[connectSwordBottom_TPS],  tpsData[endSwordBottom_TPS])/4;
		
		for (int i=3; i<7; i++)
		{
			
			jointsTop[i].localPosition = new Vector3(0,0,lengthTop);
			jointsMid[i].localPosition = new Vector3(0,0,lengthMid);
			jointsBottom[i].localPosition = new Vector3(0,0,lengthBottom);
			jointsSwordTop[i].localPosition = new Vector3(0,0,lengthSwordTop);
			jointsSwordBottom[i].localPosition = new Vector3(0,0,lengthSwordBottom);
		}
		
		jointsSwordTop[0].parent = jointsBottom[6];
		jointsSwordBottom[0].parent = jointsBottom[6];
	}
	
	
	public Transform getTopChild()
	{
		return jointsTop[0];
		
	}
	public void setTopParent(Transform parent)
	{
		jointsTop[0].parent = parent;
	}
	
	public Transform getMidChild()
	{
		return jointsMid[0];
	}
	public void setMidParent(Transform parent)
	{
		jointsMid[0].parent = parent;
	}	
	public Transform getBottomChild()
	{
		return jointsBottom[0];
	}
	public void setBottomParent(Transform parent)
	{
		jointsBottom[0].parent = parent;
	}	
}
