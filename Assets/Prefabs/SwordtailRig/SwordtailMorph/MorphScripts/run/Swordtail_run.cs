using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Swordtail_run : MonoBehaviour {
	public String fileName = "./data/body1.tps";
	public Swordtail_morph_data data;
	public Swordtail_body body;
	public Swordtail_pelvic r_pelvic;
	public Swordtail_pelvic l_pelvic;
	public Swordtail_pectoral r_pectoral;
	public Swordtail_pectoral l_pectoral;
	public Swordtail_dorsal dorsal;
	public Swordtail_caudal caudal;
	public Swordtail_anal anal;
	
	// DEBUG TOOLS
	public string empty;
	public string Debug_Info = "DEBUG info Below";
	public DEBUG_swordtail_TPS tpsDebug;
	public bool doAutoLoad = false;
	public bool doDebugPrint = false;
	public bool doDebugTPS = false;
	public bool doDebugBody = false;
	public bool doDebugMotion = false;
	
	//private
	private Vector3[] tpsData;
	
	void Awake()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
  		if(fishType == 0)
		{
			Messenger<string>.AddListener("OnMorph", LoadFileOnce);
		}
	}
	// Use this for initialization
	void Start () {
		if (doDebugPrint)
			print("----- Loading Swordtail");
		if (doAutoLoad)
		{
			data.LoadFile(fileName);
			tpsData=data.tpsData;
			doMorph();
		}
	}
	
	public void LoadFileOnce(string file_Name)
	{
		LoadFile(file_Name);
		//Messenger<string>.RemoveListener("OnMorph", LoadFileOnce);
	}
	
	public void LoadFile(string file_Name)
	{
		fileName = file_Name;
		data.LoadFile(fileName);
		tpsData = data.tpsData;
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
		r_pectoral.setWidth(body.findWidth(r_pectoral.getChild()));
		r_pectoral.setParent(body.findSpineParent(r_pectoral.getChild()));
		if (doDebugPrint)
			print("----- r_pectoral Loaded");
		l_pectoral.morph(tpsData);
		l_pectoral.setWidth(-body.findWidth(l_pectoral.getChild()));
		l_pectoral.setParent(body.findSpineParent(l_pectoral.getChild()));
		if (doDebugPrint)
			print("----- l_pectoral Loaded");	
		
		
		dorsal.morph(tpsData);
		dorsal.setFrontParent(body.findTopParent(dorsal.getFrontChild()));
		dorsal.setFrontMidParent(body.findTopParent(dorsal.getFrontMidChild()));
		dorsal.setBackMidParent(body.findTopParent(dorsal.getBackMidChild()));
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
		anal.setFrontParent(body.findBottomParent(anal.getFrontChild()));
		anal.setBackParent(body.findBottomParent(anal.getBackChild()));
		if (doDebugPrint)
			print("----- anal Loaded");
	}
	
	// Update is called once per frame
	void Update () {
		
		if (doDebugMotion)
		{
			body.debugRotate();
		}
	
	}
}
