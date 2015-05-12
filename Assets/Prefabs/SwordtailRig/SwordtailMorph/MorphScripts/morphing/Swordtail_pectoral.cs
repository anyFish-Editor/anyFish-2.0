using UnityEngine;
using System.Collections;
using System.IO;

public class Swordtail_pectoral : MonoBehaviour {

	public string rigName = "r_pectoral";
	public int connectTop_TPS = 39;
	public int connectBottom_TPS = 42;
	public int endTop_TPS = 40;
	public int endBottom_TPS = 41;
	
	//makePrivate
	private Transform[] jointsTop = new Transform[6];
	private Transform[] jointsBottom = new Transform[6];
	private Transform jointMid;
	private Vector3[] tpsData;
	private bool loaded = false;
	
	private WWW www;
	
		// ----- Function Updated by Mahmoud 02.10.14 -----
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 0)
		{	
			if(rigName == "r_pectoral") // Right Pectoral by default
				Messenger<float, float>.AddListener("AdjustRPectoralFin", OnPectoralUpdate);
			else
				Messenger<float, float>.AddListener("AdjustLPectoralFin", OnPectoralUpdate);	
			Messenger.AddListener("StateChange", onStateChange);
			
			
			Messenger.AddListener("UpdateFinTextures", updateTextures);	
		}
	}
	
	// ----- New Function Pasted by Mahmoud 02.10.14 -----
	void onStateChange()
	{
		if(rigName == "r_pectoral") // Right Pect by default
			Messenger<float, float>.RemoveListener("AdjustRPectoralFin", OnPectoralUpdate);
		else
			Messenger<float, float>.RemoveListener("AdjustLPectoralFin", OnPectoralUpdate);	
		Messenger.RemoveListener("StateChange", onStateChange);
	}
		// ----- New Function Pasted by Mahmoud 02.10.14 -----
	
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
	
	// --------- New Pasted Function by Mahmoud 02.10.2014 ------
	void OnPectoralUpdate (float hChange, float vChange)
	{
		//float maxAngle = 25.0f;
		Vector3 axis = new Vector3(hChange / 35.0f, vChange / 35.0f, 0.0f);
		
		//t+= Time.deltaTime * 5.0f;
		//float angle = Mathf.Sin(t) * maxAngle;
		//float angle = Mathf.Atan(vChange/hChange) * 100;
		float angle = ((Mathf.Abs(vChange)+Mathf.Abs(hChange)) / 70.0f) * 30.0f;
		if(rigName == "r_pectoral")
			axis = new Vector3(axis.x, axis.y * -1.0f, axis.z);
		//float angle = amount;
		for(int i = 2; i<6; i++)
		{
			jointsTop[i].localEulerAngles = axis * angle;
			jointsBottom[i].localEulerAngles = axis * angle;
			jointMid.localEulerAngles = axis * angle;
		}	
	}
	// --------- End of Pasted Function by Mahmoud 02.10.2014 ------

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
	}
	public void setWidth(float xValue)
	{
		
		Vector3 newPos = jointMid.position;
		newPos.x = xValue;
		jointMid.position = newPos;
	}	
	
}
