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

public class ProjectSelectorState : BasicState {
	
	protected List<ProjectData> projects;
	
	int projectSelectionIndex = -1;
	
	private Vector2 scrollViewVector = Vector2.zero;
	private bool isDisplayingNamePrompt = false;
	private string projectName = "";
	
	// Use this for initialization
	void Awake()
	{
		Messenger.resetAllMessengers();
		PlayerPrefs.DeleteAll();
		projects = new List<ProjectData>();	
	}
	void Start () {
		// Build a list of all projects
		string path = Application.dataPath + "\\projects\\";
		bool previousProjects = DirectoryUtil.AssertDirectoryExistsOrRecreate(path);
		
		if(previousProjects)
		{
			DirectoryInfo[] subDirs = DirectoryUtil.getSubDirectoriesByParent(path);
			foreach(DirectoryInfo info in subDirs)
			{				
				ProjectData newProject = new ProjectData();
				newProject.projectFolderPath = info.FullName;
				newProject.projectName = info.Name;
				//WarningSystem.addWarning("New Project Folder", "New path:" + newProject.projectFolderPath, Code.Info);
				projects.Add(newProject);
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		//GuiManager guiManager = GameObject.Find("GuiManagerObject").GetComponent<GuiManager>();
		GUI.skin =  GuiManager.GetSkin();
		GUI.depth = 10;	
		// Launch Screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 175, Screen.height / 2 - 250, 250, 380));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

		// We'll make a box so you can see where the group is on-screen.
		GUI.Box (new Rect (0,0,250,380), "Anyfish Editor");
		
		
		if(GUI.Button (new Rect (65,40,120,32), "New Project"))
		{
			isDisplayingNamePrompt = true;
		}
		
		
		if(GUI.Button (new Rect (65,80,120,32), "Load Project"))
		{
			if(projectSelectionIndex != -1)
			{
				GameRegistry activeRegistry = gameObject.GetComponent<GameRegistry>();
				activeRegistry.activeProjectDirectory = projects[projectSelectionIndex].projectFolderPath;
				activeRegistry.switchState(States.ProjectEditor);
			}
			else
			{
				WarningSystem.addWarning("Select a Project", "Please selection a project before loading.", Code.Warning);	
			}
		}
		
		if(isDisplayingNamePrompt)
			GUI.enabled = false;
		scrollViewVector = GUI.BeginScrollView (new Rect (15, 140, 220, 120), scrollViewVector, new Rect (0, 0, 200, 1000));
		
		projectSelectionIndex = GUILayout.SelectionGrid(projectSelectionIndex, getProjectNames(), 1, "toggle");
		
		
		// End the ScrollView
		GUI.EndScrollView();
		GUI.enabled = true;
		if(isDisplayingNamePrompt)
		{
			
			GUI.Box (new Rect (30,80,190,180), "Enter Project Name:");
			
			GUI.SetNextControlName("ProjectNameField");
			projectName = GUI.TextField(new Rect(50, 120, 150, 32), projectName, 40);
			GUI.FocusControl("ProjectNameField");
			
			if(GUI.Button(new Rect(50, 190, 150, 32), "Ok"))
			{
				isDisplayingNamePrompt = false;
				
				// Create Project Directory and Add to the display list
				Directory.CreateDirectory(Application.dataPath + "\\projects\\" + projectName);
				ProjectData newProject = new ProjectData();
				
				//Creates a relative path for the project which is used in the xml file later. (Mohammad)
				string relative= "";
				if (Directory.Exists(Application.dataPath))
				{
					string p = Application.dataPath;
					string parpath1 = Directory.GetParent(Application.dataPath).FullName;
					string parpath2 = Directory.GetParent(parpath1).FullName;
					string parpath3 = Directory.GetParent(parpath2).FullName;
					relative = "../../" + Application.dataPath.Remove(0,parpath3.Length)+ "\\projects\\" + projectName;
				}

					
				//newProject.projectFolderPath = Application.dataPath + "\\projects\\" + projectName;
				newProject.projectFolderPath = relative;
				newProject.projectName = projectName;
				newProject.tankDimensions.x = 1000;
				newProject.tankDimensions.z = 700;
				newProject.tankDimensions.y = 1000;
				createDirectoryIfItDoesntExist("tps", newProject.projectFolderPath);
				createDirectoryIfItDoesntExist("fins", newProject.projectFolderPath);
				createDirectoryIfItDoesntExist("textures", newProject.projectFolderPath);
				createDirectoryIfItDoesntExist("paths", newProject.projectFolderPath);
				Debug.Log("Project path: " + newProject.projectFolderPath);
					saveNewProjectData(newProject);
				projects.Add(newProject);
				WarningSystem.addWarning("New Project Folder", "New path:" + newProject.projectFolderPath, Code.Info);
				
			}
		}
		if(GUI.Button (new Rect (65,290,120,32), "About"))
		{
			WarningSystem.addWarning("Version Info",  "Version: 1.1 \t\tReleased: Jan 1, 2014 \nUpdates: Keyframe multi-selection, version info etc. ", Code.Info);
		}
		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();
		
	}
	
	private void createDirectoryIfItDoesntExist(string directoryName, string path)
	{		
		if(!Directory.Exists(path + "\\" + directoryName))
		{
			Debug.Log("Folder Created at: " + path);
			Directory.CreateDirectory(path + "\\" + directoryName);
		}	
	}
	private void saveNewProjectData(ProjectData data)
	{
		
		XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(data.projectFolderPath + "\\" + data.projectName + ".xml");
  		serializer.Serialize(textWriter, data);
  		textWriter.Close();
	}
	
	private string[] getProjectNames()
	{
		string[] returnList = new string[projects.Count];
		for(int i = 0; i < projects.Count; i++)
		{
			returnList[i] = projects[i].projectName;	
		}
		return returnList;
	}
	public override void SaveState ()
	{
		
	}
	
	// If projectSelector is the current state than closing it is the equivilent of quitting
	// the unity application
	public override void CloseState ()
	{
		Destroy(this);
		//Application.Quit();
	}
}
