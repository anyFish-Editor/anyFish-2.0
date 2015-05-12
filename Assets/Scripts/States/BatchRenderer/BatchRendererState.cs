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
using System.Text.RegularExpressions;

public class BatchRendererState : BasicState {
	public ProjectData activeProject;
	public List<string> pathDirs;
	public List<string> tpsDirs;
	public List<string> textureDirs;
	
	// We use initialized to prevent any updates before it is ready
	// canLoadNext sets up the delay to adjust fish to default position
	// isActivelyRecording uses own block
	private bool isInitialized = false;
	private bool canLoadNextSet = true;
	private bool isReadyToStartRecording = false;
	private bool isActivelyRecording = false;
	private int currentSetIndex = 0;
	private List<string[]> renderSets;
	
	private GameObject target;
	private List<MoCapAnimData> playbackKeys;
	private FishType fishModel = FishType.Poeciliid;
	
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
	
	void Awake()
	{
		loadActiveProject();
		playbackRate = activeProject.dialFrames;
		
		pathDirs = new List<string>();
		tpsDirs = new List<string>();
		textureDirs = new List<string>();
		
		string[] pathDirectoryList = PlayerPrefsX.GetStringArray("PathsDir");
		string[] pathTexturesList = PlayerPrefsX.GetStringArray("TexturesDir");
		string[] pathTPSList = PlayerPrefsX.GetStringArray("TPSDir");
		pathDirs.InsertRange(0, pathDirectoryList);
		textureDirs.InsertRange(0, pathTexturesList);
		//TPS might be empty
		tpsDirs.InsertRange(0, pathTPSList);
		
		
		setupAnimationRequirements();
		setupRenderSets();
		isInitialized = true;
	}
	
	void Start ()
	{
		checkFins();
		Messenger.Broadcast("TurnOffGui");
	}
	void checkFins()
	{
		Messenger.Broadcast("UpdateFinTextures");
		
	}
	
	void setupAnimationRequirements()
	{
		int fishType = PlayerPrefs.GetInt("FishType");
		if(fishType == 0)
		{
			fishModel = FishType.Poeciliid;
			GameObject.Find("EditorPrefab/Root/SticklebackRig").SetActiveRecursively(false);
			KeyframeBar holder = GameObject.Find("KeyframeBar/KeyframeGameRegistry").GetComponent<KeyframeBar>();
			Debug.Log("Does holder equal null " + holder);
			holder.keyedObject = (GameObject)GameObject.Find("EditorPrefab/Root/SwordtailRigCore");
			holder.motionCaptureTarget = (GameObject)GameObject.Find("EditorPrefab/Root/SwordtailRigCore/MotionCaptureTarget");
			target = holder.motionCaptureTarget;
			/*
			if(PlayerPrefs.GetString("MorphPath") != "default")
			{
				Debug.Log("MorphPath: " + PlayerPrefs.GetString("MorphPath"));
				Messenger<string>.Broadcast("OnMorph", PlayerPrefs.GetString("MorphPath"));
			}
			if(PlayerPrefs.GetString("TexturePath") != "default")
				Messenger<string>.Broadcast("SwapTexture", PlayerPrefs.GetString("TexturePath"));
			*/
		}else if(fishType == 1)
		{
			fishModel = FishType.Stickleback;
			GameObject.Find("SwordtailRigCore").SetActiveRecursively(false);
			KeyframeBar holder = GameObject.Find("KeyframeBar/KeyframeGameRegistry").GetComponent<KeyframeBar>();
			holder.keyedObject = (GameObject)GameObject.Find("EditorPrefab/Root/SticklebackRig");
			holder.motionCaptureTarget = (GameObject)GameObject.Find("EditorPrefab/Root/SticklebackRig/MotionCaptureTarget");
			target = holder.motionCaptureTarget;
		}
	}
	void setupRenderSets()
	{
		renderSets = new List<string[]>();
		
		foreach(string path in pathDirs)
		{
			foreach(string texture in textureDirs)
			{
				foreach(string tps in tpsDirs)
				{
					string[] set = {path, texture, tps};
					renderSets.Add(set);
				}
			}
		}
	}
	void loadMorph(string path)
	{
		Messenger<string>.Broadcast("OnMorph", path);
	}
	void loadTexture(string path)
	{
		Messenger<string>.Broadcast("SwapTexture", path);
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
		
		playbackKeys = GameObject.Find("KeyframeGameRegistry").GetComponent<KeyframeBar>().buildPlaybackData();
   	}
	
	
	private void loadActiveProject()
	{
		GameRegistry activeRegistry = gameObject.GetComponent<GameRegistry>();
		
		XmlSerializer deserializer = new XmlSerializer(typeof(ProjectData));
		string projectPath = activeRegistry.activeProjectDirectory;
		Debug.Log(projectPath.LastIndexOf("\\"));
		Debug.Log(projectPath);
		
		string projectName = projectPath.Substring(projectPath.LastIndexOf("\\"));
  		TextReader textReader = new StreamReader(projectPath + "\\" + projectName + ".xml");
		activeProject = (ProjectData)deserializer.Deserialize(textReader);
		activeRegistry.activeProjectData = activeProject;
		PlayerPrefs.SetString("ProjectPath", activeProject.projectFolderPath);
   		textReader.Close();		
	}
			
	// Update is called once per frame
	void Update () {
		if(isInitialized)
		{
			if(canLoadNextSet)
			{
				if(currentSetIndex > renderSets.Count - 1)
				{
					Debug.Log("************** Done rendering all sets");
					isActivelyRecording = false;
					canLoadNextSet = false;
					isReadyToStartRecording = false;
					return;
				}
				
				int startIndex = 0;
				int endindex = 0;
				endindex = renderSets[currentSetIndex][0].LastIndexOf(".");
				startIndex = renderSets[currentSetIndex][0].LastIndexOf("\\");
				string pathName = renderSets[currentSetIndex][0].Substring(startIndex+1, (endindex-startIndex-1));
				
				endindex = renderSets[currentSetIndex][1].LastIndexOf(".");
				startIndex = renderSets[currentSetIndex][1].LastIndexOf("\\");
				string tpsName = renderSets[currentSetIndex][1].Substring(startIndex+1, (endindex-startIndex-1));
				
				endindex = renderSets[currentSetIndex][2].LastIndexOf(".");
				startIndex = renderSets[currentSetIndex][2].LastIndexOf("\\");
				string textureName = renderSets[currentSetIndex][2].Substring(startIndex+1, (endindex-startIndex-1));
				
				pathName = Regex.Replace(pathName, @"\t|\n|\r", "");
				tpsName = Regex.Replace(tpsName, @"\t|\n|\r", "");
				textureName = Regex.Replace(textureName, @"\t|\n|\r", "");
				Debug.Log("path: " + pathName + ", tpsName: " + tpsName);
				string name = pathName + tpsName + textureName;
				ProduceScreenShots.newFolderName(name);
				// We'll start by positioning the first fish and waiting a few seconds before recording
				loadMorph(renderSets[currentSetIndex][2]);
				loadTexture(renderSets[currentSetIndex][1]);
				loadStateData(renderSets[currentSetIndex][0]);
				Messenger<int>.Broadcast("SwitchCamera", 1);
				
				canLoadNextSet = false;
				isReadyToStartRecording = true;
				isActivelyRecording = true;
				currentFrame = 0;
				playbackClock = 0;
			}else if(isReadyToStartRecording)
			{
				playFrame();	
			}else if (isActivelyRecording == false)
			{
				GameRegistry activeRegistry = gameObject.GetComponent<GameRegistry>();
				activeRegistry.switchState(States.ProjectSelector);
			}
		}
		/*
		 * if(fishModel == FishType.Poeciliid)
				{
					Messenger<float>.Broadcast("AdjustDorsalFin", dorsalAmount);
					Messenger<float, float>.Broadcast("AdjustLPelvicFin", lpelvicHAmount, lpelvicVAmount);
					Messenger<float, float>.Broadcast("AdjustRPelvicFin", rpelvicHAmount, rpelvicVAmount);
					Messenger<float, float>.Broadcast("AdjustAnalFin", analHAmount, analVAmount);
				}*/
	}
	
	void playFrame()
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
		analHAmount = playbackKeys[currentFrame].AnalAmount.x;
		analVAmount = playbackKeys[currentFrame].AnalAmount.y;
		
		if(fishModel == FishType.Poeciliid)
		{
			Messenger<float>.Broadcast("AdjustDorsalFin", dorsalAmount);
			Messenger<float, float>.Broadcast("AdjustLPelvicFin", lpelvicHAmount, lpelvicVAmount);
			Messenger<float, float>.Broadcast("AdjustRPelvicFin", rpelvicHAmount, rpelvicVAmount);
			Messenger<float, float>.Broadcast("AdjustAnalFin", analHAmount, analVAmount);
		}
		
		playbackClock += Time.deltaTime;
		if(currentFrame == 0){
			if(playbackClock > 7)
			{
				ProduceScreenShots.TakePic();
				currentFrame++;
			}
		}else if(playbackClock > (1.0f / (float)playbackRate))
		{
			//Debug.Log("Playback clock: " + playbackClock);
			playbackClock = 0.0f;	
			
			
			
			
			//yeild WaitForEndOfFrame;
			
			ProduceScreenShots.TakePic();
			if(currentFrame == playbackKeys.Count -1)
			{
				isReadyToStartRecording = false;
				canLoadNextSet = true;
				currentFrame = 0;
				currentSetIndex++;
				target.transform.position = playbackKeys[0].Position;
				target.transform.rotation = playbackKeys[0].QRotation;
				Debug.Log("One Batch Finished");
			}
			currentFrame++;
		}
	}

	private string[] getPossibleFishModels()
	{
		// TODO: Better way to do this
		string[] returnList = new string[2];
		returnList[(int)FishType.Poeciliid] = "Poeciliid";
		returnList[(int)FishType.Stickleback] = "Stickleback";
		return returnList;
	}
	
	private void saveNewProjectData(ProjectData data)
	{
		WarningSystem.addWarning("Attempt to Save ProjectData", "Shouldn't be called in render", Code.Error);
		/*
		XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(data.projectFolderPath + "\\" + data.projectName + ".xml");
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
  		*/
	}
	
	private void saveNewPathData(PathData data)
	{
		WarningSystem.addWarning("Attempt to Save Pathdata", "Shouldn't be called in render", Code.Error);
		/*
		XmlSerializer serializer = new XmlSerializer(typeof(PathData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(activeProject.projectFolderPath + "\\paths\\" + data.pathName + ".xml");
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
  		*/
	}
	
	public override void SaveState ()
	{
		//TODO: Save Project State
		saveNewProjectData(activeProject);
	}
	
	// 
	public override void CloseState ()
	{
		Messenger.Broadcast("StateChange");	
		Application.LoadLevel(0);
		Destroy(this);
	}
}
