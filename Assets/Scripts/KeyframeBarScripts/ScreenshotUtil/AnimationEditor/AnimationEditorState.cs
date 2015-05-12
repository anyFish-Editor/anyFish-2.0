//AnyFish program is used to study fish behavior using simulated virtual fish as stimuli.
//For details of the software, please visit:
//http://swordtail.tamu.edu/anyfish/Main_Page

//Copyright (C) <2014>  <AnyFish development team>

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class AnimationEditorState : BasicState {
	private bool isGuiVisible = true;
	public static int selGridInt = 1;
	public static int selShowMocapInt = 1;
    private string[] selStrings = new string[] {"Mocap", "Keyframe", "Blended"};
	private string[] selShowLineRendererStrings = new string[] {"Show MocapPath", "Hide MocapPath"};
	private string frameField = "90";
	private char pathChar = "/"[0];
	
	private bool isPlaying = false;
	private bool isFirstRun = true;
	private int currentFrame;
	private int subframeCount = 0;
	private int playbackRate = 15;
	private float playbackClock = 0.0f;
	public static float dorsalAmount = 0.0f;
	public static float lpelvicHAmount = 0.1f;
	public static float lpelvicVAmount = 0.1f;
	public static float rpelvicHAmount = 0.1f;
	public static float rpelvicVAmount = 0.1f;
	public static float analHAmount = 0.1f;
	public static float analVAmount = 0.1f;
	public static float lpectoralHAmount = 0.1f; //new
	public static float lpectoralVAmount = 0.1f; //new
	public static float rpectoralHAmount = 0.1f; //new
	public static float rpectoralVAmount = 0.1f; //new
	
	public bool FrontToggle = true;
	public bool TopToggle = true;
	public Terrain terrain = Terrain.activeTerrain;

	
	private GameObject target;
	private List<MoCapAnimData> playbackKeys;
	private FishType fishModel = FishType.Poeciliid;
	
	void Awake ()
	{
		
		Messenger.AddListener("TurnOffGui", onTurnOffGui);
		Messenger.AddListener("TurnOnGui", onTurnOnGui);
		int fishType = PlayerPrefs.GetInt("FishType");
		Debug.Log("AnimationEditorState fishtype: " + fishType);
		
		if(fishType == 0)
		{
			fishModel = FishType.Poeciliid;
			GameObject.Find("EditorPrefab/Root/SticklebackRig").SetActiveRecursively(false);
			KeyframeBar holder = GameObject.Find("KeyframeBar/KeyframeGameRegistry").GetComponent<KeyframeBar>();
			Debug.Log("Does holder equal null " + holder);
			holder.keyedObject = (GameObject)GameObject.Find("EditorPrefab/Root/SwordtailRigCore");
			holder.motionCaptureTarget = (GameObject)GameObject.Find("EditorPrefab/Root/SwordtailRigCore/MotionCaptureTarget");
			target = holder.motionCaptureTarget;
			if(PlayerPrefs.GetString("MorphPath") != "default")
			{
				Debug.Log("MorphPath: " + PlayerPrefs.GetString("MorphPath"));
				Messenger<string>.Broadcast("OnMorph", PlayerPrefs.GetString("MorphPath"));
			}
			if(PlayerPrefs.GetString("TexturePath") != "default")
				Messenger<string>.Broadcast("SwapTexture", PlayerPrefs.GetString("TexturePath"));
			
		}else if(fishType == 1)	// changed "else" to "else if" by Chengde, 06132013
		{
			fishModel = FishType.Stickleback;
			GameObject.Find("SwordtailRigCore").SetActiveRecursively(false);
			KeyframeBar holder = GameObject.Find("KeyframeBar/KeyframeGameRegistry").GetComponent<KeyframeBar>();
			holder.keyedObject = (GameObject)GameObject.Find("EditorPrefab/Root/SticklebackRig");
			holder.motionCaptureTarget = (GameObject)GameObject.Find("EditorPrefab/Root/SticklebackRig/MotionCaptureTarget");
			target = holder.motionCaptureTarget;	
			
			//pasted following two if statement by Chengde, 06132013. 
			if(PlayerPrefs.GetString("MorphPath") != "default")
			{
				Debug.Log("MorphPath: " + PlayerPrefs.GetString("MorphPath"));
				Messenger<string>.Broadcast("OnMorph", PlayerPrefs.GetString("MorphPath"));
			}
			if(PlayerPrefs.GetString("TexturePath") != "default")
				Messenger<string>.Broadcast("SwapTexture", PlayerPrefs.GetString("TexturePath"));
		}
		
		
		//string path = PlayerPrefs.GetString("PathDir");
		
		//loadStateData(path);
	}
	
	void Start ()
	{
		checkFins();
	}
	private void onTurnOffGui()
	{
		isGuiVisible = false;	
	}
	private void onTurnOnGui()
	{
		isGuiVisible = true;	
	}
		
	void firstRun()
	{
		string path = PlayerPrefs.GetString("PathDir");
		Debug.Log("pathDir: " + path);
		loadStateData(path);
		isFirstRun = false;
	}
	
	void checkFins()
	{
		Messenger.Broadcast("UpdateFinTextures");
		
	}
	// Update is called once per frame
	IEnumerator playFrame()
	{
		KeyframeBar holder = GameObject.Find("KeyframeBar/KeyframeGameRegistry").GetComponent<KeyframeBar>();
			//Debug.Log("HOLDER TARGET---------------------" + holder.motionCaptureTarget.transform.parent);
			
		target.transform.position = playbackKeys[currentFrame].Position;
		target.transform.rotation = playbackKeys[currentFrame].QRotation;//subframeCount++;
		dorsalAmount = playbackKeys[currentFrame].DorsalAmount;
		lpelvicHAmount = playbackKeys[currentFrame].LPecAmount.x;
		lpelvicVAmount = playbackKeys[currentFrame].LPecAmount.y;
		rpelvicHAmount = playbackKeys[currentFrame].RPecAmount.x;
		rpelvicVAmount = playbackKeys[currentFrame].RPecAmount.y;
		lpectoralHAmount = playbackKeys[currentFrame].LPectAmount.x;      // Mahmoud 02.17.14
		lpectoralVAmount = playbackKeys[currentFrame].LPectAmount.y;      // Mahmoud 02.17.14
		rpectoralHAmount = playbackKeys[currentFrame].RPectAmount.x;      // Mahmoud 02.17.14
		rpectoralVAmount = playbackKeys[currentFrame].RPectAmount.y;      // Mahmoud 02.17.14
		analHAmount = playbackKeys[currentFrame].AnalAmount.x;
		analVAmount = playbackKeys[currentFrame].AnalAmount.y;
		
		playbackClock += Time.deltaTime;
		Debug.Log("playbackClock: " + playbackClock);
		Messenger.Broadcast("TurnOffGui");
		yield return new WaitForSeconds(2.0f);
		//Vector3 position = Vector3.Lerp(playbackKeys[currentFrame].Position, playbackKeys[currentFrame + 1].Position, (float)subframeCount / playbackRate);
		//Quaternion rotation = Quaternion.Slerp(playbackKeys[currentFrame].QRotation, playbackKeys[currentFrame + 1].QRotation, (float)subframeCount / playbackRate);	
		
		//target.transform.position = position;
		//target.transform.rotation = rotation;
		subframeCount++;
		
		if(playbackClock > (1.0f / (float)playbackRate))
		{
			//Debug.Log("playClock: " + playbackClock);
			playbackClock = 0.0f;	
			target.transform.position = playbackKeys[currentFrame].Position;
			target.transform.rotation = playbackKeys[currentFrame].QRotation;//subframeCount++;
			dorsalAmount = playbackKeys[currentFrame].DorsalAmount;
			//if(subframeCount == playbackRate)
			//{
				subframeCount = 1;
				currentFrame++;
			//ProduceScreenShots.TakePic();
			if(currentFrame == playbackKeys.Count -1)
			{
				isPlaying = false;
				yield return new WaitForSeconds(1.0f);
				target.transform.position = playbackKeys[0].Position;
				target.transform.rotation = playbackKeys[0].QRotation;
			Debug.Log("Test");
				Messenger.Broadcast("TurnOnGui");
				//Reset motioncaption's parent the actual fish. Fish is moved for keyframing
				// motioncapture for playback
				//Transform fish = target.transform.parent;
				//fish.position = Vector3.zero;
				//fish.rotation = Quaternion.identity;
			}
		}
	}
	
	void Update () {
		if(isFirstRun)
			firstRun();		
		if(fishModel == FishType.Poeciliid)
		{
			Messenger<float>.Broadcast("AdjustDorsalFin", dorsalAmount);
			Messenger<float, float>.Broadcast("AdjustLPelvicFin", lpelvicHAmount, lpelvicVAmount);
			Messenger<float, float>.Broadcast("AdjustRPelvicFin", rpelvicHAmount, rpelvicVAmount);
			Messenger<float, float>.Broadcast("AdjustLPectoralFin", lpectoralHAmount, lpectoralVAmount);
			Messenger<float, float>.Broadcast("AdjustRPectoralFin", rpectoralHAmount, rpectoralVAmount);
			Messenger<float, float>.Broadcast("AdjustAnalFin", analHAmount, analVAmount);
		}
		if(isPlaying)
		{
			StartCoroutine("playFrame");
		}
				//	WarningSystem.addWarning("Warning",  analHAmount.ToString (), Code.Info);
	}
	
	
	void OnGUI()
	{
		GUI.skin = GuiManager.GetSkin();
		GUI.depth = 3;
		
		/*
		if(isGuiVisible){
			// Make the first button.
			if (GUI.Button(new Rect (20,100,80,20), "Animation")) 
			{
				SendMessage("OpenFileWindow");
			}
			GUI.Label(new Rect (110,100,800,20), pathToFileLabel);
			
			selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3);
				
			selShowMocapInt = GUILayout.SelectionGrid(selShowMocapInt, selShowLineRendererStrings, 2);
		}*/
		
		if(isGuiVisible)
		{
			GUI.Box(new Rect(15, 15, 250, 625), "Path Properties");
			GUI.BeginGroup(new Rect(30, 50, 235, 560));
				GUI.Label(new Rect(0, 0, 98, 30), "Frame Count");
				frameField = GUI.TextArea(new Rect(100, 0, 50, 25), frameField);
				if(GUI.Button(new Rect(160, 0, 50, 30), "Set"))
				{
					Messenger<int>.Broadcast("KeyframeResize", int.Parse(frameField) );
				}	
			
			GUI.Label(new Rect(50, 30, 150, 30), "Motion Blend Type");
			selGridInt = GUI.SelectionGrid(new Rect(0, 55, 220, 30), selGridInt, selStrings, 3);
			
			GUI.enabled = false; //TODO on mocap loaded
			selShowMocapInt = GUI.SelectionGrid(new Rect(0, 90, 220, 30), selShowMocapInt, selShowLineRendererStrings, 2);
			//Debug.Log("selGridInt set: " + selGridInt);
			GUI.enabled = true;
			GUI.Label(new Rect(35, 120, 180, 30), "Load Guide Sequences");
			
			GUI.Label(new Rect(0, 145, 60, 30), "Front");	
			//FrontToggle= GUI.Toggle (new Rect(40, 145, 30, 20),FrontToggle,"");
			//if (FrontToggle == false)
			//HideFront();
			if(GUI.Button(new Rect(50, 145, 60, 30), "Load"))
				{
					Messenger.Broadcast("LoadFront");
				}
			
			GUI.Label(new Rect(120, 145, 50, 30), "Top");
			//TopToggle= GUI.Toggle (new Rect(150, 145, 30, 20),TopToggle,"");
			//if (TopToggle == false)
			//HideTop();
			if(GUI.Button(new Rect(160, 145, 60, 30), "Load"))
				{
					Messenger.Broadcast("LoadTop");
				}	
			
			if (GUI.Button(new Rect(0, 175, 100, 30), "Terrain On/Off"))
			{
				HideTerrain();
			}
		
			if (GUI.Button (new Rect(120, 175, 100, 30),"Guides On/Off"))
			{
				TopOnOff();
				FrontOnOff();
			}
				
			//{
			//	ShowAll();
			//}
			
			//if(GUI.Button(new Rect(180, 145, 40, 30), "Load"))
			//	{
			//		Messenger.Broadcast("LoadTop");
			//	}	
			//
			GUI.Label(new Rect(50, 200, 180, 30), "Fish Fins Control");
			// Pelvics are kept at just greater than zero to prevent division by zero in later calc
			
			GUI.Label(new Rect(25, 220, 180, 30), "Pelvic");
			GUI.Label(new Rect(5, 240, 180, 30), "L-H");
			GUI.Label(new Rect(5, 255, 180, 30), "L-V");
			lpelvicHAmount = GUI.HorizontalSlider(new Rect(40, 245, 60, 15), lpelvicHAmount, -35.0f, 35.0f);
			lpelvicVAmount = GUI.HorizontalSlider(new Rect(40, 260, 60, 15), lpelvicVAmount, -35.0f, 35.0f);
			
			GUI.Label(new Rect(5, 280, 180, 30), "R-H");
			GUI.Label(new Rect(5, 295, 180, 30), "R-V");
			rpelvicHAmount = GUI.HorizontalSlider(new Rect(40, 285, 60, 15), rpelvicHAmount, -35.0f, 35.0f);
			rpelvicVAmount = GUI.HorizontalSlider(new Rect(40, 300, 60, 15), rpelvicVAmount, -35.0f, 35.0f);
			
			// Mahmoud 17.2.2014 ...........................................................
			
			GUI.Label(new Rect(140, 220, 180, 30), "Pectoral");
			GUI.Label(new Rect(120, 240, 180, 30), "L-H");
			GUI.Label(new Rect(120, 255, 180, 30), "L-V");
			lpectoralHAmount = GUI.HorizontalSlider(new Rect(155, 245, 60, 15), lpectoralHAmount, -35.0f, 35.0f);
			lpectoralVAmount = GUI.HorizontalSlider(new Rect(155, 260, 60, 15), lpectoralVAmount, -0.1f, 50.0f);
			
			GUI.Label(new Rect(120, 280, 180, 30), "R-H");
			GUI.Label(new Rect(120, 295, 180, 30), "R-V");
			rpectoralHAmount = GUI.HorizontalSlider(new Rect(155, 285, 60, 15), rpectoralHAmount, -35.0f, 35.0f);
			rpectoralVAmount = GUI.HorizontalSlider(new Rect(155, 300, 60, 15), rpectoralVAmount, -0.1f, 50.0f);
			
			//End ..........................................................................
						
			GUI.Label(new Rect(25, 315, 180, 30), "Anal");
			GUI.Label(new Rect(5, 335, 180, 30), "H");
			GUI.Label(new Rect(5, 350, 180, 30), "V");
			analHAmount = GUI.HorizontalSlider(new Rect(40, 340, 60, 10), analHAmount, -35.0f, 35.0f);
			analVAmount = GUI.HorizontalSlider(new Rect(40, 355, 60, 10), analVAmount, -35.0f, 35.0f);
			
			GUI.Label(new Rect(140, 315, 180, 30), "Dorsal");
			
			if(fishModel != FishType.Poeciliid)
				GUI.enabled = false;
			dorsalAmount = GUI.HorizontalSlider(new Rect(155, 340, 60, 10), dorsalAmount, -15.0f, 35.0f);
			
			GUI.enabled = true;
			
			
			
			GUI.Label(new Rect(45, 370, 180, 30), "Primary Camera");
				if(GUI.Button(new Rect(0, 395, 70, 30), "Front(1)"))
				{
					Messenger<int>.Broadcast("SwitchCamera",0);
				}
				if(GUI.Button(new Rect(70, 395, 70, 30), "Free(2)"))
				{
					Messenger<int>.Broadcast("SwitchCamera", 1);
				}
			GUI.enabled = false;
				if(GUI.Button(new Rect(140, 395, 70, 30), "Top(3)"))
				{
					//Messenger.Broadcast("LoadTop");
				}
			GUI.enabled = true;
			GUI.Label(new Rect(35, 425, 180, 30), "Playback Settings");
			GUI.Label(new Rect(0, 455, 80, 30), "Framerate");
			playbackRate = int.Parse(GUI.TextField(new Rect(80, 455, 35, 30), playbackRate.ToString()));
			if(GUI.Button(new Rect(140, 455, 70, 30), "Play"))
			{
				//ProduceScreenShots.newFolderName(PlayerPrefs.GetString("PathName"));
				playbackKeys = GameObject.Find("KeyframeGameRegistry").GetComponent<KeyframeBar>().buildPlaybackData();
				currentFrame = 0;
				isPlaying = true;
			}
			
			if(GUI.Button(new Rect(30, 490, 150, 30), "Save and Exit"))
			{
				PathData data = new PathData();
				CameraManager cameras = GameObject.Find("Cameras").GetComponent<CameraManager>();
				data.setCameraPosition(cameras.getActiveCameraPosition());
				data.setCameraRotation(cameras.getActiveCameraRotation());
				data.pathName = "test";
				data.keyframes = GameObject.Find("KeyframeGameRegistry").GetComponent<KeyframeBar>().keyInfo;
				saveNewPathData(data);
				GameRegistry activeRegistry = GameObject.Find("EditorApplication").GetComponent<GameRegistry>();
				activeRegistry.switchState(States.ProjectSelector);
			}
			
			if(GUI.Button(new Rect(30, 530, 150, 30), "Exit Without Saving"))
			{
				Application.Quit();
			}
			GUI.EndGroup();
		}
	}
	
	public void OpenFile(string pathToFile) 
	{
		//SaveState.setPathMoCap(pathToFile);
		
		//print(pathToFile);
		
		List<MoCapAnimData> animData = FileIO.ImportAnimation(pathToFile);
		print("Imported List");
		
		
		//player.moCapClip = animData;
		Debug.Log("DEFAULT POSITION: " + (animData[0].Rotation));
		print("AnimInit - Clip enabled");
		//Messenger.Broadcast("AnimInit");
		//Messenger<List<MoCapAnimData>>.Broadcast("MocapDataLoaded", animData);
	}
	
	private void saveNewPathData(PathData data)
	{
		//GameRegistry activeRegistry = GameObject.Find("EditorApplication").GetComponent<GameRegistry>();
		
		XmlSerializer serializer = new XmlSerializer(typeof(PathData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(PlayerPrefs.GetString("PathDir"));
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
	}
	
	private void loadStateData(string pathName)
	{
		//GameRegistry activeRegistry = GameObject.Find("EditorApplication").GetComponent<GameRegistry>();
		XmlSerializer deserializer = new XmlSerializer(typeof(PathData));
  		TextReader textReader = new StreamReader(pathName);
   		PathData data;
   		data = (PathData)deserializer.Deserialize(textReader);
   		textReader.Close();
		
		Debug.Log("LOADING PATH DATA : AES");
		Messenger<List<KeyframeInfo>>.Broadcast("KeyframeLoaded", data.keyframes);
		CameraManager cameras = GameObject.Find("Cameras").GetComponent<CameraManager>();
		cameras.setActiveCameraPosition(data.getCameraPosition());
		cameras.setActiveCameraRotation(data.getCameraRotation());
   	}
	
	public override void CloseState ()
	{
		Messenger.Broadcast("StateChange");	
		Application.LoadLevel(0);
		isFirstRun = true;
		Destroy(this);
	}
	
	private void FrontOnOff ()   //Mahmoud 05.04.14
	{	
		GameObject Front;
		Front =  GameObject.Find("FrontViews"); 
		Front.renderer.enabled = !Front.renderer.enabled;
		//Destroy(Front.gameObject);
	}
	
	private void TopOnOff ()
	{	
		GameObject Top;
		Top =  GameObject.Find("TopViews"); 
		Top.renderer.enabled = !Top.renderer.enabled;
		//Destroy(Top.gameObject);
	}
	
	private void HideTerrain ()
	{			
		//TerrainData terrainData = (TerrainData)Resources.Load("Terrain");		
		terrain.enabled = !terrain.enabled;		
//		GameObject terrain = GameObject.GetComponent(Terrain);		
		//terrain.renderer.enabled = false;
		//Destroy(terrain.gameObject);
	}

	
	public override void SaveState ()
	{
		//TODO Save State
	}
}
