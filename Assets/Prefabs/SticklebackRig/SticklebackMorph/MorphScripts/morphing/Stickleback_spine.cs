using UnityEngine;
using System.Collections;

public class Stickleback_spine : MonoBehaviour {

	public string rigName = "spine";	
	public int connectFront_TPS = 8;
	public int connectBack_TPS = 45;
	public int end_TPS = 44;
	
	//make private
	private Transform[] jointsBack = new Transform[6];
	private Transform[] jointsFront = new Transform[6];
	private Transform jointMid;
	private Vector3[] tpsData;
	private bool loaded = false;
	
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
