using UnityEngine;
using System.Collections;
using System.IO;

public class Stickleback_body : MonoBehaviour {

	public string rigName = "body";
	
	public float[] widthProfile = new float [] {.01f, 0.13265308f, 0.05f, .01f};
	public int width_snouth_TPS = 1;
	public int width_gil_meet_top_TPS = 5;
	public int width_base_of_dorsal_TPS = 13;
	public int width_base_caudial = 16;
	public int tail_end_top_TPS = 16;
	public int tail_end_bottom_TPS = 17;
	
	//make privae
	private Vector3[] tpsData;
	private Transform[] SpineJoints = new Transform[12];
	private Transform[] m_topJoints = new Transform[17];
	private Transform[] m_bottomJoints = new Transform[17];
	private Transform[] r_sideJoints = new Transform[17]; 
	private Transform[] l_sideJoints = new Transform[17]; 
	private Transform[] r_topJoints = new Transform[7]; 
	private Transform[] l_topJoints = new Transform[7]; 
	private Transform[] r_bottomJoints = new Transform[7]; 
	private Transform[] l_bottomJoints = new Transform[7]; 	
	private Transform[] m_midJoints = new Transform[2];
	private float[] widthData = new float[17];
	private bool loaded = false;
	private float stndLngth;
	
	private WWW www;
	
	void Awake ()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 1)
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
		for (int i=1; i<17; i++)
		{
			// these points only go from 2-16
			if (i>= 2)
			{
				string jointName = "m_top_point_" + i;
				m_topJoints[i] = m_topJoints[0].FindChild(jointName);
				
				jointName = "m_bottom_point_" + i;
				m_bottomJoints[i] = m_topJoints[0].FindChild(jointName);
				
				jointName = "r_mid_point_" + i;
				r_sideJoints[i] = m_topJoints[0].FindChild(jointName);
				
				jointName = "l_mid_point_" + i;
				l_sideJoints[i] =m_topJoints[0].FindChild(jointName);
			}
			
		}		
		r_topJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		r_bottomJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		l_topJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		l_bottomJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		for (int i=1; i<6; i++)
		{
			
			if(i>=3)
			{
				string jointName = "r_top_point_" + i;
				r_topJoints[i] = r_topJoints[0].FindChild(jointName);
				
				jointName = "r_bottom_point_" + i;
				r_bottomJoints[i] = r_bottomJoints[0].FindChild(jointName);
				
				jointName = "l_top_point_" + i;
				l_topJoints[i] = l_topJoints[0].FindChild(jointName);
				
				jointName = "l_bottom_point_" + i;
				l_bottomJoints[i] = l_bottomJoints[0].FindChild(jointName);
			}
			
		}	
		m_midJoints[0] = transform.FindChild(rigName).FindChild("ROOT").FindChild("PLACEMENT");
		m_midJoints[1] = m_midJoints[0].FindChild("m_mid_point_1");
		loaded = true;
	}

	private void unParentTransfoms()
	{
		for (int i=1; i<12; i++)
			SpineJoints[i].parent = SpineJoints[0];
		
		for (int i=2; i<17; i++)
		{

			m_topJoints[i].parent = m_topJoints[0];
			m_bottomJoints[i].parent = m_bottomJoints[0];
			r_sideJoints[i].parent = r_sideJoints[0];
			l_sideJoints[i].parent = l_sideJoints[0];

			
		}		
		for (int i=3; i<6; i++)
		{
			r_topJoints[i].parent = r_topJoints[0];
			r_bottomJoints[i].parent = r_bottomJoints[0];
			l_topJoints[i].parent = l_topJoints[0];
			l_bottomJoints[i].parent = l_bottomJoints[0];
			
			
		}	
		m_midJoints[1].parent = m_midJoints[0];
			
	}
	
	private void setWidthData()
	{
		int widthPoint0_TPS = width_snouth_TPS;
		int widthPoint1_TPS = width_gil_meet_top_TPS;
		int widthPoint2_TPS = width_base_of_dorsal_TPS;
		int widthPoint3_TPS = width_base_caudial;
		
		widthData[widthPoint0_TPS] = widthProfile[0];
		widthData[widthPoint1_TPS] = widthProfile[1];
		widthData[widthPoint2_TPS] = widthProfile[2];
		widthData[widthPoint3_TPS] = widthProfile[3];
		
		float widthPoint0=0;
		if (widthPoint0_TPS==1)
			widthPoint0 =  m_midJoints[widthPoint0_TPS].position.z; 
		else
			widthPoint0 = m_topJoints[widthPoint0_TPS].position.z;
 		float widthPoint1 = m_topJoints[widthPoint1_TPS].position.z;
 		float widthPoint2 = m_topJoints[widthPoint2_TPS].position.z;
		float widthPoint3 = m_topJoints[widthPoint3_TPS].position.z;
		
		float blend = 0;
		for (int i=1;i<17; i++)
		{
			if (i<widthPoint0_TPS)
			{
				widthData[i]=widthData[widthPoint0_TPS];
			}
			else if (i>widthPoint0_TPS && i<widthPoint1_TPS)
			{
				blend = getBlend(widthPoint0, widthPoint1, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[widthPoint0_TPS], widthData[widthPoint1_TPS], blend);
			}	
			else if (i>widthPoint1_TPS && i<widthPoint2_TPS)
			{
				blend = getBlend(widthPoint1, widthPoint2, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[widthPoint1_TPS], widthData[widthPoint2_TPS], blend);
			}	
			else if (i>widthPoint2_TPS && i<widthPoint3_TPS)
			{
				blend = getBlend(widthPoint2, widthPoint3, m_topJoints[i].position.z);
				widthData[i] =  blendValue(widthData[widthPoint2_TPS], widthData[widthPoint3_TPS], blend);
			}				
			else if (i>widthPoint3_TPS)
			{
				widthData[i]=widthData[widthPoint3_TPS];
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
		stndLngth = (tpsData[tail_end_top_TPS].z + tpsData[tail_end_bottom_TPS].z)/2;
		int topCounter = 2;
		int bottomCounter = 16;
		float x;
		float y;
		float z;
		
		SpineJoints[0].position = new Vector3(0,0,0);
		for(int i =1; i<=31; i ++)
		{
			if (i==1)
			{
				m_midJoints[1].position = tpsData[i];	
			}
			else if (i<=16) // the top points
			{
				m_topJoints[topCounter].position = tpsData[i];	
				topCounter++;
			}
			else if(i<=31) // set the bottom points
			{
				m_bottomJoints[bottomCounter].position = tpsData[i];
				bottomCounter--;
			}

		}
		setWidthData();
		//seting the side joints up
		for (int i=2; i<=16; i++)
		{
			x = -widthData[i]* stndLngth;
			y = (m_topJoints[i].position.y + m_bottomJoints[i].position.y)/2;	
			z = (m_topJoints[i].position.z + m_bottomJoints[i].position.z)/2;
					
			r_sideJoints[i].position = new Vector3(x,y,z);
			x = widthData[i]* stndLngth;
			l_sideJoints[i].position = new Vector3(x,y,z);			
		}
		// setting up the eye joints

				
		r_sideJoints[3].position = tpsData[32];
		r_topJoints[3].position = tpsData[33];
		r_topJoints[4].position = tpsData[34];
		r_topJoints[5].position = tpsData[35];
		r_sideJoints[5].position = tpsData[36];
		r_bottomJoints[5].position = tpsData[37];
		r_bottomJoints[4].position = tpsData[38];
		r_bottomJoints[3].position = tpsData[39];
		
		x= 0;
		y= (r_topJoints[4].position.y + r_bottomJoints[4].position.y)/2;
		z= (r_topJoints[4].position.z + r_bottomJoints[4].position.z)/2;
		r_sideJoints[4].position = new Vector3(x,y,z);

		l_sideJoints[3].position = tpsData[32];
		l_topJoints[3].position = tpsData[33];
		l_topJoints[4].position = tpsData[34];
		l_topJoints[5].position = tpsData[35];
		l_sideJoints[5].position = tpsData[36];
		l_bottomJoints[5].position = tpsData[37];
		l_bottomJoints[4].position = tpsData[38];
		l_bottomJoints[3].position = tpsData[39];
		
		x= 0;
		y= (l_topJoints[4].position.y + l_bottomJoints[4].position.y)/2;
		z= (l_topJoints[4].position.z + l_bottomJoints[4].position.z)/2;
		l_sideJoints[4].position = new Vector3(x,y,z);	
		
		for (int i=3; i<=5; i++)
		{
			x = -widthData[i]* stndLngth;
			y = r_sideJoints[i].position.y;	
			z = r_sideJoints[i].position.z;
			r_sideJoints[i].position = new Vector3(x,y,z);
			
			x = -widthData[i]* stndLngth;
			y = r_topJoints[i].position.y;	
			z = r_topJoints[i].position.z;
			r_topJoints[i].position = new Vector3(x,y,z);		
			
			x = -widthData[i]* stndLngth;
			y = r_bottomJoints[i].position.y;	
			z = r_bottomJoints[i].position.z;
			r_bottomJoints[i].position = new Vector3(x,y,z);				
			
			x = widthData[i]* stndLngth;
			y = l_sideJoints[i].position.y;	
			z = l_sideJoints[i].position.z;
			l_sideJoints[i].position = new Vector3(x,y,z);	
			
			x = widthData[i]* stndLngth;
			y = l_topJoints[i].position.y;	
			z = l_topJoints[i].position.z;
			l_topJoints[i].position = new Vector3(x,y,z);

		    x = widthData[i]* stndLngth;
			y = l_bottomJoints[i].position.y;	
			z = l_bottomJoints[i].position.z;
			l_bottomJoints[i].position = new Vector3(x,y,z);
			
		}		
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
		
		for (int i=2; i<17; i++)
		{

			m_topJoints[i].parent = findSpineParent(m_topJoints[i]);
			m_bottomJoints[i].parent = findSpineParent(m_bottomJoints[i]);
			r_sideJoints[i].parent = findSpineParent(r_sideJoints[i]);
			l_sideJoints[i].parent = findSpineParent(l_sideJoints[i]);

			
		}		
		for (int i=3; i<6; i++)
		{
			r_topJoints[i].parent = findSpineParent(r_topJoints[i]);
			r_bottomJoints[i].parent = findSpineParent(r_bottomJoints[i]);
			l_topJoints[i].parent = findSpineParent(l_topJoints[i]);
			l_bottomJoints[i].parent = findSpineParent(l_bottomJoints[i]);
						
		}	
		m_midJoints[1].parent = findSpineParent(m_midJoints[1]);	
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
		for (int i=2; i<17; i++)	
		{
			if (i == 16)
				return m_bottomJoints[16];
			else if ((m_bottomJoints[i].position.z <= joint.position.z) && (joint.position.z <= m_bottomJoints[i+1].position.z))
				return m_bottomJoints[i];
		}
		return m_bottomJoints[16];
	}
	public Transform findTopParent(Transform joint)
	{
		for (int i=2; i<17; i++)	
		{
			if (i == 16)
				return m_topJoints[16];
			else if ((m_topJoints[i].position.z <= joint.position.z) && (joint.position.z <= m_topJoints[i+1].position.z))
				return m_topJoints[i];
		}
		return m_bottomJoints[16];
	}	

	public Transform findRightParent(Transform joint)
	{
		for (int i=2; i<17; i++)	
		{
			if (i == 16)
				return r_sideJoints[16];
			else if ((r_sideJoints[i].position.z <= joint.position.z) && (joint.position.z <= r_sideJoints[i+1].position.z))
				return r_sideJoints[i];
		}
		return r_sideJoints[16];
	}
	public Transform findLeftParent(Transform joint)
	{
		for (int i=2; i<17; i++)	
		{
			if (i == 16)
				return l_sideJoints[16];
			else if ((l_sideJoints[i].position.z <= joint.position.z) && (joint.position.z <= l_sideJoints[i+1].position.z))
				return l_sideJoints[i];
		}
		return r_sideJoints[16];
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

}
