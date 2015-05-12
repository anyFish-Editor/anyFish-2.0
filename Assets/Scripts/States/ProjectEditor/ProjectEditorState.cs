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

public class ProjectEditorState : BasicState {
	public ProjectData activeProject;
	
	public bool toggleTxt = false;
	public bool toggleTwo = false;
	public bool toggleThree = false;
	
	private Vector2 fishModelScrollVector = Vector2.zero;
	private Vector2 fishPathsScrollVector = Vector2.zero;
	private Vector2 fishTexturesScrollVec = Vector2.zero;
	private Vector2 tpsTexturesScrollVec = Vector2.zero;
	
	private FilenameSelector pathNames;
	private FilenameSelector textureNames;
	private FilenameSelector tpsNames;
	
	private bool isInitialized = false; // Bug where onGUI is called before AWAKE
	private bool isDisplayingPrompt = false;
	
	private string pathName = "";
	// Use this for initialization
	void Awake()
	{
		loadActiveProject();
		
		isInitialized = true;
		pathNames = gameObject.AddComponent<FilenameSelector>();
		tpsNames = gameObject.AddComponent<FilenameSelector>();
		textureNames = gameObject.AddComponent<FilenameSelector>();
		scanTextures();
		scanTPS();
		scanFinDir();
		loadPreviousPaths();
	}
	void Start () {
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
	
	private void loadPreviousPaths()
	{
		if(Directory.Exists(activeProject.projectFolderPath + "\\" + "paths"))
		{
			string [] fileEntries = Directory.GetFiles(activeProject.projectFolderPath + "\\" + "paths");
    		foreach(string fileName in fileEntries)
    		{
				
    		    if(fileName.LastIndexOf(".xml") != -1)
				{
					pathNames.addItem(fileName);
				}
    		   Debug.Log(fileName);
   			}
		}
		else
		{
			Directory.CreateDirectory(activeProject.projectFolderPath + "\\" + "paths");
		}
	}
	private void scanTextures()
	{
		if(Directory.Exists(activeProject.projectFolderPath + "\\" + "textures"))
		{
			string [] fileEntries = Directory.GetFiles(activeProject.projectFolderPath + "\\" + "textures");
    		foreach(string fileName in fileEntries)
    		{
				// If its an image file png/jpg add to the list.
				if(fileName.LastIndexOf(".png") != -1 || fileName.LastIndexOf(".jpg") != -1 || fileName.LastIndexOf(".PNG") != -1 || fileName.LastIndexOf(".JPG") != -1)
				{
					textureNames.addItem(fileName);
				}
    		    // do something with fileName
    		   	Debug.Log(fileName);
   			}
		}
		else
		{
			Directory.CreateDirectory(activeProject.projectFolderPath + "\\" + "textures");
		}
	}
	private void scanTPS()
	{
		if(Directory.Exists(activeProject.projectFolderPath + "\\" + "tps"))
		{
			string [] fileEntries = Directory.GetFiles(activeProject.projectFolderPath + "\\" + "tps");
    		foreach(string fileName in fileEntries)
    		{
				// If its an image file png/jpg add to the list.
				if(fileName.LastIndexOf(".tps") != -1 || fileName.LastIndexOf(".TPS") != -1)
				{
					tpsNames.addItem(fileName);
				}
    		    // do something with fileName
    		   	Debug.Log(fileName);
   			}
		}
		else
		{
			Directory.CreateDirectory(activeProject.projectFolderPath + "\\" + "tps");
		}
	}
	private void scanFinDir()
	{
		PlayerPrefs.SetString("OverrideDorsal", "default");
		PlayerPrefs.SetString("OverrideCaudal", "default");
		PlayerPrefs.SetString("OverrideAnal", "default");
		PlayerPrefs.SetString("OverridePelvic", "default");
		PlayerPrefs.SetString("OverridePectoral", "default");
		
		if(Directory.Exists(activeProject.projectFolderPath + "\\" + "fins"))
		{
			string [] fileEntries = Directory.GetFiles(activeProject.projectFolderPath + "\\" + "fins");
    		foreach(string fileName in fileEntries)
    		{
				// If its an image file png/jpg add to the list.
				if(fileName.LastIndexOf(".png") != -1)
				{
					if(fileName.Contains("dorsal"))
						PlayerPrefs.SetString("OverrideDorsal", fileName);
					if(fileName.Contains("caudal"))
						PlayerPrefs.SetString("OverrideCaudal", fileName);
					if(fileName.Contains("anal"))
						PlayerPrefs.SetString("OverrideAnal", fileName);
					if(fileName.Contains("pelvic"))
						PlayerPrefs.SetString("OverridePelvic", fileName);
					if(fileName.Contains("pectoral"))
						PlayerPrefs.SetString("OverridePectoral", fileName);
				}
    		    // do something with fileName
    		   	Debug.Log(fileName);
   			}
		}
		else
		{
			Directory.CreateDirectory(activeProject.projectFolderPath + "\\" + "fins");
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(isInitialized)
		{
			GUI.skin = GuiManager.GetSkin();
			GUI.depth = 3;
			
			if(isDisplayingPrompt)
				GUI.enabled = false;
			
			GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 300, 500, 600));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
	
			// We'll make a box so you can see where the group is on-screen.
			GUI.Box (new Rect (0,0,500,600), "Project Editor: " + activeProject.projectName);
			
			GUI.Label(new Rect(35, 25, 360, 30), "Tank:   Warning:Numbers only");
			
			// Unity textfields work by assigning a value on return, without the assignment they are uneditable
			// Decided to parse them back to floats on the fly.	
			
			//TODO: Limit to only number input Regex.Replace(text, @"[^0-9 ]", "");
			GUI.Label(new Rect(35, 50, 100, 30), "Width(MM) : ");
			activeProject.tankDimensions.x = float.Parse(GUI.TextField(new Rect(150, 50, 100, 25), activeProject.tankDimensions.x.ToString()));
			GUI.Label(new Rect(35, 75, 100, 30), "Height(MM): ");
			activeProject.tankDimensions.y = float.Parse(GUI.TextField(new Rect(150, 75, 100, 25), activeProject.tankDimensions.y.ToString()));
			GUI.Label(new Rect(35, 100, 100, 30), "Depth(MM) : ");
			activeProject.tankDimensions.z = float.Parse(GUI.TextField(new Rect(150, 100, 100, 25), activeProject.tankDimensions.z.ToString()));
			
			GUI.Label(new Rect(290, 25, 180, 30), "Background Color");
			GUI.Label(new Rect(295, 50, 30, 30), "R");
			activeProject.backgroundR = int.Parse(GUI.TextField(new Rect(330, 50, 35, 25), activeProject.backgroundR.ToString()));
			
			GUI.Label(new Rect(295, 75, 30, 30), "G");
			activeProject.backgroundG = int.Parse(GUI.TextField(new Rect(330, 75, 35, 25), activeProject.backgroundG.ToString()));
			
			GUI.Label(new Rect(295, 100, 30, 30), "B");
			activeProject.backgroundB = int.Parse(GUI.TextField(new Rect(330, 100, 35, 25), activeProject.backgroundB.ToString()));
			
			
			GUI.Label(new Rect(35, 140, 100, 30), "Fish Models : ");
			fishModelScrollVector = GUI.BeginScrollView (new Rect (15, 160, 220, 120), fishModelScrollVector, new Rect (0, 0, 200, 150));
			
			activeProject.type = GUILayout.SelectionGrid(activeProject.type, getPossibleFishModels(), 1, "toggle");
			
			//selectedModelIndex = 
			GUI.EndScrollView();
			
			
			GUI.Label(new Rect(260, 140, 100, 30), "Fish Paths : ");
			fishPathsScrollVector = GUI.BeginScrollView (new Rect (260, 155, 220, 120), fishPathsScrollVector, new Rect (0, 0, 200, 150));
				pathNames.render();
			
			GUI.EndScrollView();
		
			if(GUI.Button (new Rect (260,290,80,32), "New Path"))
			{
				PlayerPrefs.SetInt("FishType", activeProject.type);
				isDisplayingPrompt = true;
				//Application.LoadLevel(1);
				//GameRegistry activeRegistry = gameObject.GetComponent<GameRegistry>();
				//activeRegistry.switchState(States.AnimationEditor);
				//WarningSystem.addWarning("Write Failure", "Failed to write path data", Code.Error);
			}
				
			if(GUI.Button (new Rect (365,290,80,32), "Edit Path"))
			{
				if(pathNames.hasActive())
				{
					List<string> activePaths = pathNames.getActive();
					// Check that only one is selected
					if(activePaths.Count != 1)
						WarningSystem.addWarning("Please select a path.", "Please select only a single path for editing", Code.Warning);
					else{
						// Open the path
						PlayerPrefs.SetInt("FishType", activeProject.type);
						PlayerPrefs.SetString("PathDir", activePaths[0]);
						if(tpsNames.hasActive())
						{
							List<string> activeMorphs = tpsNames.getActive();
							if(activeMorphs.Count > 1)
								WarningSystem.addWarning("Please select a single TPS", "Please select just one morph for path editing", Code.Warning);
							else
							{
								if(textureNames.hasActive())
								{
									List<string> activeTextures = textureNames.getActive();
									if(activeTextures.Count > 1)
										WarningSystem.addWarning("Please select a single texture", "Please select a single texture for path editing", Code.Warning);
									else{
										PlayerPrefs.SetString("MorphPath", activeMorphs[0]);
										PlayerPrefs.SetString("TexturePath", activeTextures[0]);
										Application.LoadLevel(1);
									}
								}
								else{
									PlayerPrefs.SetString("MorphPath", activeMorphs[0]);
									PlayerPrefs.SetString("TexturePath", "default");
									Application.LoadLevel(1);
								}
							}
						}else
						{
							PlayerPrefs.SetString("TexturePath", "default");
							PlayerPrefs.SetString("MorphPath", "default");
							Application.LoadLevel(1);
						}
						
					}
				}else
					WarningSystem.addWarning("Select a path!", "First select an existing path or create a new one.", Code.Warning);
				
			}
			
			// End the group we started above. This is very important to remember!
			
			
			GUI.Label(new Rect(35, 340, 100, 30), "Textures : ");
			fishTexturesScrollVec = GUI.BeginScrollView (new Rect (15, 355, 220, 120), fishTexturesScrollVec, new Rect (0, 0, 200, 150));
				textureNames.render();
			// Put something inside the ScrollView
			//toggleThree = GUI.Toggle(new Rect(10, 10, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 10, 100, 24), "need swordtail tps");
			
				// End the ScrollView
			GUI.EndScrollView();
			//GUI.enabled = false;
			if(GUI.Button (new Rect (25,500,100,32), "Batch Render")){
				// Store FishType
				bool hasRequirementsToRender = true;
				
				PlayerPrefs.SetInt("FishType", activeProject.type);
				// Check if Paths	
				if(pathNames.hasActive())
				{
					List<string> activePaths = pathNames.getActive();
					PlayerPrefsX.SetStringArray("PathsDir", activePaths.ToArray());						
				}else{
					WarningSystem.addWarning("Select a path!", "First select an existing path or create a new one.", Code.Warning);
					hasRequirementsToRender = false;
				}
				// Check textures
				if(textureNames.hasActive())
				{
					List<string> activeTextures = textureNames.getActive();
					PlayerPrefsX.SetStringArray("TexturesDir", activeTextures.ToArray());						
				
				}else{
					WarningSystem.addWarning("Select a texture!", "Please select at least one existing texture.", Code.Warning);
					hasRequirementsToRender = false;
				}
				
				// Check TPS
				if(tpsNames.hasActive())
				{
					List<string> activeTPS = tpsNames.getActive();
					PlayerPrefsX.SetStringArray("TPSDir", activeTPS.ToArray());
				}else{
					PlayerPrefsX.SetStringArray("TPSDir", new string[]{"default"});
				}
				
				// Launch Renderer
				if(hasRequirementsToRender)
					Application.LoadLevel(2);
			}
			
			GUI.enabled = true;
			
			GUI.Label(new Rect(260, 340, 100, 30), "TPS : ");
			tpsTexturesScrollVec = GUI.BeginScrollView (new Rect (260, 355, 220, 120), tpsTexturesScrollVec, new Rect (0, 0, 200, 150));
				tpsNames.render();
			// Put something inside the ScrollView
			//toggleThree = GUI.Toggle(new Rect(10, 10, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 10, 100, 24), "need swordtail tps");
			
				// End the ScrollView
			GUI.EndScrollView();
			
			
			
			//GUI.Label(new Rect(260, 480, 100, 30), "Time Dilation: ");
			
			GUI.Label(new Rect(260, 485, 120, 30), "Frame Rate: ");
			activeProject.dialFrames = int.Parse(GUI.TextField(new Rect(390, 485, 50, 25), activeProject.dialFrames.ToString()));
			GUI.Label(new Rect(260, 510, 120, 30), "Render Every: ");
			activeProject.snapshotPer = int.Parse(GUI.TextField(new Rect(390, 515, 50, 25), activeProject.snapshotPer.ToString()));
			
			if(GUI.Button (new Rect (250,550,220,32), "Windows Only: Open Project Folder"))
			{
				try{
					string itemPath = activeProject.projectFolderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
	    			System.Diagnostics.Process.Start("explorer.exe", "/select,"+itemPath);
				}catch
				{
					WarningSystem.addWarning("Failed to spawn explorer", "Unable to open explorer, are you using a mac?", Code.Error);	
				}
			}
			
			GUI.enabled = true;
			
			if(isDisplayingPrompt)
			{
				GUI.BeginGroup(new Rect(120, 100, 500, 500));
				GUI.Box (new Rect (30,80,190,180), "Enter Path Name:");
				
				GUI.SetNextControlName("ProjectPathField");
				pathName = GUI.TextField(new Rect(50, 120, 150, 32), pathName, 40);
				GUI.FocusControl("ProjectPathField");
				
				if(GUI.Button(new Rect(50, 190, 150, 32), "Ok"))
				{
					isDisplayingPrompt = false;
					
					PlayerPrefs.SetInt("FishType", activeProject.type);
					PlayerPrefs.SetString("PathDir", activeProject.projectFolderPath + "\\paths\\" + pathName + ".xml");
					PlayerPrefs.SetString("PathName", pathName);
					Application.LoadLevel(1);
					
					//Directory.CreateDirectory(activeProject.projectFolderPath + "\\paths\\" + pathName);
					
					PathData newPathData = new PathData();
					newPathData.pathName = pathName;
					newPathData.keyframes = new List<KeyframeInfo>(0);
					saveNewPathData(newPathData);
					//GameRegistry activeRegistry = gameObject.GetComponent<GameRegistry>();
					//activeRegistry.switchState(States.AnimationEditor);
					
					// Create Project Directory and Add to the display list
				}
				GUI.EndGroup();
			}
			
			
			GUI.EndGroup ();
			
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
		
		XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(data.projectFolderPath + "\\" + data.projectName + ".xml");
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
	}
	
	private void saveNewPathData(PathData data)
	{
		
		XmlSerializer serializer = new XmlSerializer(typeof(PathData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(activeProject.projectFolderPath + "\\paths\\" + data.pathName + ".xml");
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
	}
	
	public override void SaveState ()
	{
		//TODO: Save Project State
		saveNewProjectData(activeProject);
	}
	
	// 
	public override void CloseState ()
	{
		Destroy(pathNames);
		//TODO: Cleanup State
		Destroy(this);
	}
}
