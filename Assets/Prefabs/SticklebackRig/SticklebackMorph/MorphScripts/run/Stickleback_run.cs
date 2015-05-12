using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Stickleback_run : MonoBehaviour {
	public String fileName = "./data/testData.tps";
	public Stickleback_morph_data data;
	public Stickleback_body body;
	public Stickleback_pelvic r_pelvic;
	public Stickleback_pelvic l_pelvic;
	public Stickleback_pectoral r_pectoral;
	public Stickleback_pectoral l_pectoral;
	public Stickleback_spine spine1;
	public Stickleback_spine spine2;
	public Stickleback_spine spine3;
	public Stickleback_dorsal dorsal;
	public Stickleback_caudal caudal;
	public Stickleback_anal anal;
	
	// DEBUG TOOLS
	public string empty;
	public string Debug_Info = "DEBUG info Below";
	public DEBUG_stickleback_TPS tpsDebug;
	public bool doAutoLoad = false;
	public bool doDebugPrint = false;
	public bool doDebugTPS = false;
	public bool doDebugBody = false;
	public bool doDebugMotion = false;
	
	//private
	private Vector3[] tpsData;
	
	//Chengde, 06062013
	void Awake()
	 {	
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 1)
		{
	  		Messenger<string>.AddListener("OnMorph", LoadFileOnce);	
		}
	 }
	
	
	// Use this for initialization
	void Start () {
		if (doDebugPrint)
			print("----- Loading Stickleback");
		if (doAutoLoad)
		{
			data.LoadFile(fileName);
			tpsData=data.tpsData;
			doMorph();
		}
	}
	
	//Chengde, 06062013
	 public void LoadFileOnce(string file_Name)
	 {	
	  	LoadFile(file_Name);	
	  	//Messenger<string>.RemoveListener("OnMorph", LoadFileOnce);	
	 }
	
	public void LoadFile(string file_Name)
	{
		fileName = file_Name;
		data.LoadFile(fileName);
		tpsData = data.tpsData;	//Chengde, 06062013
		doMorph();
	}
	public void LoadData(Vector3 [] data)
	{		
		tpsData=data;
		doMorph();		
	}
	
	private void doMorph()
	{
		body.morph(tpsData);
		if (doDebugPrint)
			print("----- Body Loaded");
		if (doDebugTPS)
		{
			tpsDebug.setTransforms(tpsData);
			print("----- DEBUG TPS Loaded");
		}		
		r_pelvic.morph(tpsData);
		r_pelvic.setParent(body.findBottomParent(r_pelvic.getChild()));
		if (doDebugPrint)
			print("----- r_pelvic Loaded");
		l_pelvic.morph(tpsData);
		l_pelvic.setParent(body.findBottomParent(l_pelvic.getChild()));
		if (doDebugPrint)
			print("----- l_pelvic Loaded");	
		
		r_pectoral.morph(tpsData);
		r_pectoral.setParent(body.findRightParent(r_pectoral.getChild()));
		if (doDebugPrint)
			print("----- r_pectoral Loaded");
		l_pectoral.morph(tpsData);
		l_pectoral.setParent(body.findLeftParent(l_pectoral.getChild()));
		if (doDebugPrint)
			print("----- l_pectoral Loaded");	
		
		spine1.morph(tpsData);
		spine1.setParent(body.findTopParent(spine1.getChild()));
		if (doDebugPrint)
			print("----- spine1 Loaded");
		
		spine2.morph(tpsData);
		spine2.setParent(body.findTopParent(spine2.getChild()));
		if (doDebugPrint)
			print("----- spine2 Loaded");
		
		spine3.morph(tpsData);
		spine3.setParent(body.findTopParent(spine3.getChild()));
		if (doDebugPrint)
			print("----- spine3 Loaded");
		
		dorsal.morph(tpsData);
		dorsal.setFrontParent(body.findTopParent(dorsal.getFrontChild()));
		dorsal.setMidParent(body.findTopParent(dorsal.getMidChild()));
		dorsal.setBackParent(body.findTopParent(dorsal.getBackChild()));
		if (doDebugPrint)
			print("----- dorsal Loaded");
		
		caudal.morph(tpsData);
		caudal.setTopParent(body.findTopParent(caudal.getTopChild()));
		caudal.setMidParent(body.findSpineParent(caudal.getMidChild()));
		caudal.setBottomParent(body.findBottomParent(caudal.getBottomChild()));
		if (doDebugPrint)
			print("----- caudal Loaded");
		
		anal.morph(tpsData);
		anal.setFrontParent(body.findTopParent(anal.getFrontChild()));
		anal.setMidParent(body.findTopParent(anal.getMidChild()));
		anal.setBackParent(body.findTopParent(anal.getBackChild()));
		if (doDebugPrint)
			print("----- anal Loaded");
		
		//gameObject.transform.localScale = new Vector3(.37f, .37f, .37f);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (doDebugMotion)
		{
			body.debugRotate();
		}
	
	}
}
