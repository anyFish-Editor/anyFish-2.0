using UnityEngine;
using System.Collections;
using System.IO;

public class Stickleback_pectoral : MonoBehaviour {

	public string rigName = "r_pectoral";
	public int connectTop_TPS = 40;
	public int connectBottom_TPS = 43;
	public int endTop_TPS = 41;
	public int endBottom_TPS = 42;
	
	//makePrivate
	private Transform[] jointsTop = new Transform[6];
	private Transform[] jointsBottom = new Transform[6];
	private Transform jointMid;
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
		
		if(PlayerPrefs.GetString("OverridePectoral") != "default")
			LoadTexture(PlayerPrefs.GetString("OverridePectoral"));
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject fin = GameObject.Find("r_pectoralFin");
			
			fin.renderer.material.mainTexture = www.texture;
			
			fin = GameObject.Find("l_pectoralFin");
			
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
	
	private void getTransforms()
	{
		jointMid = transform.FindChild(rigName).FindChild("Root").Find("mid_connect"); 

		jointsTop[0] = jointMid.FindChild("top_connect");
		jointsTop[1] = jointsTop[0].FindChild("top_rot");
		
		jointsBottom[0]= jointMid.FindChild("bottom_connect");	
    	jointsBottom[1] = jointsBottom[0].FindChild("bottom_rot");
		
		float count=1;
		for (int i=2; i<6; i++)
		{
			
			jointsTop[i] = jointsTop[i-1].FindChild("top_joint"+count);	
			jointsBottom[i] = jointsBottom[i-1].FindChild("bottom_joint"+count);	
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
		jointMid.position= (tpsData[connectTop_TPS] + tpsData[connectBottom_TPS])/2f;
		jointsTop[0].position = tpsData[connectTop_TPS];
		jointsBottom[0].position = tpsData[connectBottom_TPS];
		
		jointsTop[1].LookAt(tpsData[endTop_TPS], Vector3.up);
		jointsBottom[1].LookAt(tpsData[endBottom_TPS], Vector3.up);
		
		float topLength = Vector3.Distance(tpsData[connectTop_TPS],  tpsData[endTop_TPS])/3;
		float bottomLength = Vector3.Distance(tpsData[connectBottom_TPS],  tpsData[endBottom_TPS])/3;
		for (int i=3; i<6; i++)
		{
			
			jointsTop[i].localPosition = new Vector3(0,0,topLength);
			jointsBottom[i].localPosition = new Vector3(0,0,bottomLength);
		}
	}
	
	public Transform getChild()
	{
		return jointMid;
	}
	
	public void setParent(Transform parent)
	{
		jointMid.parent = parent;
		jointMid.localPosition = new Vector3(0,0,0);
	}
	
	private float t = 0.0f;
	public void debugRotate()
	{
		
		float speed = 2f;
		float maxAngle = 10.0f;
		Vector3 axis = new Vector3 (0.0f, 1.0f, 0.0f);
		//float t = 0.0f;
		for (int i=3; i<6; i++)
		{
			t+= Time.deltaTime*speed;
			float angle = Mathf.Sin(t)*maxAngle;
			
			jointsTop[i].localEulerAngles = axis * angle;	
			jointsBottom[i].localEulerAngles = axis * angle;
		}
	}	
}
