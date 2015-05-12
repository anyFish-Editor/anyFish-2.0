using UnityEngine;
using System.Collections;
using System.IO;

public class Swordtail_body : MonoBehaviour {

	public string rigName = "body";
	public int width_snout_TPS = 2; 
	public int width_base_pectorials_TPS = 3;
	public int width_posterior_dorsal_TPS = 8;
	public int width_caudal_pedunacle_TPS = 11;
	public int tail_end_top_TPS = 11;
	public int tail_end_bottom_TPS = 12;
	
	public float[] widthProfile = new float [] {0.069924683f, 0.089924683f, 0.049924683f, 0.009253282f};
	private Transform[] SpineJoints = new Transform[12];
	private Transform[] m_topJoints = new Transform[12];
	private Transform[] m_bottomJoints = new Transform[12];
	private Transform[] r_sideJoints = new Transform[12]; 
	private Transform[] l_sideJoints = new Transform[12]; 
	private Vector3[] tpsData;
	private float[] widthData = new float[12];
	private bool loaded = false;
	private float stndLngth;
	
	private WWW www;
	
	void Awake ()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 0)
		{
			Messenger<string>.AddListener("SwapTexture", LoadTexture);
			//var www = new WWW("file:///" + pathToFile);
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
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			Debug.Log("------------------ Texture Found");
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			GameObject test = GameObject.Find("fishSkin");
			Debug.Log("test = " + test);
			
			GameObject.Find("fishSkin").renderer.material.mainTexture = www.texture;
			//renderer.material.SetTexture(
		}
	}
	
	public void morph(Vector3 [] data)
	{
		
		tpsData = data;
		if (loaded== false)
			getTransforms();
		unParentTransfoms();
		setTPSpoints();
			
		setSpine();
		parent();
		
	}
	

	
	private void getTransforms()
	{
		
		
		SpineJoints[0] = transform.FindChild(rigName).FindChild("ROOT");
		
		for (int i=1; i<12; i++)
		{

			string jointName = "spine" + i + "JNT";
			
			SpineJoints[i] = SpineJoints[i-1].FindChild(jointName);
			
		}
		
		m_topJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		m_bottomJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		r_sideJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		l_sideJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		for (int i=1; i<12; i++)
		{
			// these points only go from 1-11
			if (i>= 1)
			{
				string jointName = "m_top_point_" + i;
				m_topJoints[i] = m_topJoints[0].FindChild(jointName);
				
				jointName = "m_bottom_point_" + i;
				m_bottomJoints[i] = m_topJoints[0].FindChild(jointName);
			}
			
			// these points only go from 2-11
			if (i>= 2)
			{			
				string jointName = "r_mid_point_" + i;
				r_sideJoints[i] = m_topJoints[0].FindChild(jointName);
				
				jointName = "l_mid_point_" + i;
				l_sideJoints[i] =m_topJoints[0].FindChild(jointName);
			}
			
		}		
	
		loaded = true;
	}

	private void unParentTransfoms()
	{
		for (int i=1; i<12; i++)
			SpineJoints[i].parent = SpineJoints[0];
		
		for (int i=1; i<12; i++)
		{

			m_topJoints[i].parent = m_topJoints[0];
			m_bottomJoints[i].parent = m_bottomJoints[0];
			if (i>=2)
			{
				
				r_sideJoints[i].parent = r_sideJoints[0];
				l_sideJoints[i].parent = l_sideJoints[0];
			}
		}		

			
	}
	
	private void setWidthData()
	{
		int snoutTPS = width_snout_TPS;
		int baseOfPecTPS = width_base_pectorials_TPS;
		int posDorsalTPS = width_posterior_dorsal_TPS;
		int caudPedTPS = width_caudal_pedunacle_TPS;
		//snout
		widthData[snoutTPS] = widthProfile[0];
 		//base of pectorials
		widthData[baseOfPecTPS] = widthProfile[1];
		//posterior insertiion of dorsal
		widthData[posDorsalTPS] = widthProfile[2];
		//caudal pedunacle
		widthData[caudPedTPS] = widthProfile[3];

 	
 		float snout = m_topJoints[snoutTPS].position.z;
 		float baseOfPec = m_topJoints[baseOfPecTPS].position.z;
 		float posDorsal=m_topJoints[posDorsalTPS].position.z;
 		float caudPed=m_topJoints[caudPedTPS].position.z;   
		
		float blend = 0;
		for (int i=1;i<12; i++)
		{
			if (i<snoutTPS)
			{
				widthData[i]=widthData[snoutTPS];
			}
			else if (i>snoutTPS && i<baseOfPecTPS)
			{
				blend = getBlend(snout, baseOfPec, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[snoutTPS], widthData[baseOfPecTPS], blend);
			}
			else if (i>baseOfPecTPS && i<posDorsalTPS)
			{
				blend = getBlend(baseOfPec, posDorsal, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[baseOfPecTPS], widthData[posDorsalTPS], blend);				
			}
			else if (i>posDorsalTPS && i<caudPedTPS)
			{
				blend = getBlend(posDorsal, caudPed, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[posDorsalTPS], widthData[caudPedTPS], blend);				
			}		
			else if (i>caudPedTPS)
			{
				widthData[i]=widthProfile[caudPedTPS];
			}
		}
		
	}
	private float getBlend(float p1, float p2, float p3)  
	{
		float blend ;	
		blend = (p3- p1)/(p2-p1);	
		return blend;
	}
	private float blendValue(float p1, float p2, float blend) 
	{
		float v3; 	
		v3 = p1 + blend * (p2 - p1);
		return v3;
	}
	
	private void setTPSpoints()
	{
		Debug.Log("tpsData[tail_end_top_TPS].z: " + tpsData[tail_end_top_TPS].z);
		stndLngth = (tpsData[tail_end_top_TPS].z + tpsData[tail_end_bottom_TPS].z)/2;
		int topCounter = 1;
		int bottomCounter = 11;
		float x;
		float y;
		float z;
		
		SpineJoints[0].position = new Vector3(0,0,0);
		for(int i =1; i<=22; i ++)
		{

			if (i<=11) // the top points
			{
				m_topJoints[topCounter].position = tpsData[i];	
				topCounter++;
			}
			else if(i<=22) // set the bottom points
			{
				m_bottomJoints[bottomCounter].position = tpsData[i];
				bottomCounter--;
			}

		}
		setWidthData();	
		
		//seting the side joints up
		for (int i=2; i<=11; i++)
		{
			x = -widthData[i] * stndLngth;
			y = (m_topJoints[i].position.y + m_bottomJoints[i].position.y)/2;	
			z = (m_topJoints[i].position.z + m_bottomJoints[i].position.z)/2;
					
			r_sideJoints[i].position = new Vector3(x,y,z);
			x = widthData[i]* stndLngth;
			l_sideJoints[i].position = new Vector3(x,y,z);			
		}
		
		//setting up the eye
	
		x = -widthData[2]* stndLngth;
		y = tpsData[23].y;
		z = tpsData[23].z;
				
		r_sideJoints[2].position = new Vector3(x,y,z);
		x = widthData[2]* stndLngth;
		l_sideJoints[2].position = new Vector3(x,y,z);			
		
	
	}

	private void setSpine()
	{
		
		float spineLength = stndLngth/10;
		float SpinePlacement = 0;
		for( int i =1; i<= 11; i++)
		{			
			Vector3 vec = SpineJoints[i].position;
			vec.z = SpinePlacement;
			SpineJoints[i].position = vec;
			SpinePlacement = SpinePlacement + spineLength;		
		}
		
	}	

	private void parent()
	{
		for (int i=2; i<12; i++)
			SpineJoints[i].parent = SpineJoints[i-1];	
		
		for (int i=1; i<12; i++)
		{

			m_topJoints[i].parent = findSpineParent(m_topJoints[i]);
			m_bottomJoints[i].parent = findSpineParent(m_bottomJoints[i]);
			if (i>=2)
			{
				r_sideJoints[i].parent = findSpineParent(r_sideJoints[i]);
				l_sideJoints[i].parent = findSpineParent(l_sideJoints[i]);
			}
			
		}			
	}
	
	public Transform findSpineParent(Transform joint)
	{
		for (int i=1; i<12; i++)
		{
			if (i == 11)
				return SpineJoints[11];
			else if ((SpineJoints[i].position.z <= joint.position.z) && (joint.position.z <= SpineJoints[i+1].position.z))
				return SpineJoints[i];
		}
		return SpineJoints[11];
		
	}
	
	public Transform findBottomParent(Transform joint)
	{
		for (int i=1; i<12; i++)	
		{
			if (i == 11)
				return m_bottomJoints[11];
			else if ((m_bottomJoints[i].position.z <= joint.position.z) && (joint.position.z <= m_bottomJoints[i+1].position.z))
				return m_bottomJoints[i];
		}
		return m_bottomJoints[11];
	}
	public Transform findTopParent(Transform joint)
	{
		for (int i=1; i<12; i++)	
		{
			if (i == 11)
				return m_topJoints[11];
			else if ((m_topJoints[i].position.z <= joint.position.z) && (joint.position.z <= m_topJoints[i+1].position.z))
				return m_topJoints[i];
		}
		return m_bottomJoints[11];
	}	

	public Transform findRightParent(Transform joint)
	{
		for (int i=2; i<12; i++)	
		{
			if (i == 11)
				return r_sideJoints[11];
			else if ((r_sideJoints[i].position.z <= joint.position.z) && (joint.position.z <= r_sideJoints[i+1].position.z))
					return r_sideJoints[1];
		}
		return r_sideJoints[11];
	}
	public Transform findLeftParent(Transform joint)
	{
		for (int i=2; i<12; i++)	
		{
			if (i == 11)
				return l_sideJoints[11];
			else if ((l_sideJoints[i].position.z <= joint.position.z) && (joint.position.z <= l_sideJoints[i+1].position.z))
				return l_sideJoints[i];
		}
		return r_sideJoints[11];
	}
	private float t = 0.0f;
	public void debugRotate()
	{
		
		float speed = 0.20f;
		float maxAngle = 5.0f;
		Vector3 axis = new Vector3 (0.0f, 1.0f, 0.0f);
		//float t = 0.0f;
		for (int i=1; i<12; i++)
		{
			t+= Time.deltaTime*speed;
			float angle = Mathf.Sin(t)*maxAngle;
			
			SpineJoints[i].localEulerAngles = axis * angle;		
		}
	}
	public float findWidth(Transform joint)
	{
		for (int i=2; i<12; i++)	
		{
			if (i == 11)
				return l_sideJoints[11].position.z;
			else if ((l_sideJoints[i].position.z <= joint.position.z) && (joint.position.z <= l_sideJoints[i+1].position.z))
			{
				float blend = getBlend(l_sideJoints[i].position.z, l_sideJoints[i+1].position.z, joint.position.z);
				return blendValue(r_sideJoints[i].position.x, r_sideJoints[i+1].position.x, blend);
			}
		}
		return r_sideJoints[11].position.z;		
		
	}
}
