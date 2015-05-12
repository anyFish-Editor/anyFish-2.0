using UnityEngine;
using System.Collections;
using System.IO;


public class Stickleback_caudal : MonoBehaviour {

 	public string rigName = "caudal";
	public int connectTop_TPS = 15;
	public int connectBottom_TPS = 18;
	
	public int endTop_TPS = 52;	
	public int endBottom_TPS = 53;
	
    //make private
	private Transform[] jointsTop = new Transform[7];
	private Transform[] jointsMid = new Transform[7];
	private Transform[] jointsBottom = new Transform[7];
	private Vector3[] tpsData;
	private bool loaded = false;
	private WWW www;
	
	
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 1)
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
		
		jointsTop[1] = jointsTop[0].FindChild("top_rot"); 
		jointsMid[1] = jointsMid[0].FindChild("mid_rot");
		jointsBottom[1] = jointsBottom[0].FindChild("bottom_rot");
		
		
		
		float count=1;
		for (int i=2; i<7; i++)
		{
			jointsTop[i] = jointsTop[i-1].FindChild("top_joint"+count);
			jointsMid[i] = jointsMid[i-1].FindChild("mid_joint"+count);
			jointsBottom[i] = jointsBottom[i-1].FindChild("bottom_joint"+count);	
				
			count++;
		}
		loaded = true;
	}
	private void unParent()
	{
		jointsTop[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsMid[0].parent = transform.FindChild(rigName).FindChild("Root");
		jointsBottom[0].parent = transform.FindChild(rigName).FindChild("Root");
	}
	private void setTPSpoints()
	{
		
		jointsTop[0].position = tpsData[connectTop_TPS];
		jointsBottom[0].position = tpsData[connectBottom_TPS];
		jointsMid[0].position = (tpsData[connectTop_TPS] + tpsData[connectBottom_TPS])/2;
		
		jointsTop[1].LookAt(tpsData[endTop_TPS], Vector3.up);
		jointsBottom[1].LookAt(tpsData[endBottom_TPS], Vector3.up);
		Vector3 endMid_Point = (tpsData[endTop_TPS] + tpsData[endBottom_TPS])/2;
		
		jointsMid[1].LookAt(endMid_Point, Vector3.up);
		
		float lengthTop = Vector3.Distance(tpsData[connectTop_TPS],  tpsData[endTop_TPS])/4;
		float lengthBottom = Vector3.Distance(tpsData[connectBottom_TPS],  tpsData[endBottom_TPS])/4;
		float lengthMid = Vector3.Distance(jointsMid[0].position,  endMid_Point)/4;
		
		for (int i=3; i<7; i++)
		{
			
			jointsTop[i].localPosition = new Vector3(0,0,lengthTop);
			jointsMid[i].localPosition = new Vector3(0,0,lengthMid);
			jointsBottom[i].localPosition = new Vector3(0,0,lengthBottom);
		}
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
