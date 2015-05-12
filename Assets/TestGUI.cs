using UnityEngine;
using System.Collections;
using System.IO;

public class TestGUI : MonoBehaviour {
	public GUISkin mySkin;
	public bool toggleTxt = false;
	public bool toggleTwo = false;
	public bool toggleThree = false;
	private Vector2 scrollViewVector = Vector2.zero;
	
	public ProjectData[] projects;
	// Use this for initialization
	void Start () {
		string path = Application.dataPath + "\\projects\\";
		bool previousProjects = DirectoryUtil.AssertDirectoryExistsOrRecreate(path);
		
		if(previousProjects)
		{
			DirectoryInfo[] subDirs = DirectoryUtil.getSubDirectoriesByParent(path);
			foreach(DirectoryInfo info in subDirs)
			{				
				ProjectData newProject = new ProjectData();
				newProject.projectFolderPath = info.FullName;
				print("exists");
				WarningSystem.addWarning("Project Found", "Path:" + newProject.projectFolderPath, Code.Info);
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		//if(mySkin != null)
			GUI.skin = GuiManager.GetSkin();
		
		// Launch Screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 75, Screen.height / 2 - 150, 250, 350));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

		// We'll make a box so you can see where the group is on-screen.
		GUI.Box (new Rect (0,0,250,350), "Anyfish Editor");
		if(GUI.Button (new Rect (65,40,120,32), "New Project"))
		{
			Directory.CreateDirectory(Application.dataPath + "\\projects\\test\\");
			WarningSystem.addWarning("New Project Folder", "New path:", Code.Info);
		}
		GUI.Button (new Rect (65,80,120,32), "Load Project");
		
		
		scrollViewVector = GUI.BeginScrollView (new Rect (15, 140, 220, 120), scrollViewVector, new Rect (0, 0, 200, 150));

			// Put something inside the ScrollView
			toggleTxt = GUI.Toggle(new Rect(10, 10, 20, 24), toggleTxt, ""); GUI.Label(new Rect(35, 10, 100, 24), "Stickleback");
			toggleTwo = GUI.Toggle(new Rect(10, 40, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 40, 100, 24), "Swordtail");
			toggleTwo = GUI.Toggle(new Rect(10, 70, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 70, 100, 24), "Blinky");
			toggleTwo = GUI.Toggle(new Rect(10, 100, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 100, 100, 24), "Loop Example");
		// End the ScrollView
		GUI.EndScrollView();
		
		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();	
		
		
		/*
		GUI.depth = 3;
		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 300, 500, 600));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

		// We'll make a box so you can see where the group is on-screen.
		GUI.Box (new Rect (0,0,500,600), "Project Editor");
		
		GUI.Label(new Rect(35, 25, 100, 30), "Tank: ");
		
		GUI.Label(new Rect(35, 50, 100, 30), "Width(MM) : ");
		GUI.TextField(new Rect(110, 50, 100, 25), "1000");
		GUI.Label(new Rect(35, 75, 100, 30), "Height(MM): ");
		GUI.TextField(new Rect(110, 75, 100, 25), "700");
		GUI.Label(new Rect(35, 100, 100, 30), "Depth(MM) : ");
		GUI.TextField(new Rect(110, 100, 100, 25), "1400");
		
		//GUI.Button (new Rect (65,40,120,32), "New Project");
		//GUI.Button (new Rect (65,80,120,32), "Load Project");
		
		GUI.Label(new Rect(35, 140, 100, 30), "Fish Models : ");
		scrollViewVector = GUI.BeginScrollView (new Rect (15, 155, 220, 120), scrollViewVector, new Rect (0, 0, 200, 150));

		// Put something inside the ScrollView
			toggleTwo = GUI.Toggle(new Rect(10, 10, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 10, 100, 24), "Jacob Park");
			toggleTxt = GUI.Toggle(new Rect(10, 40, 20, 24), toggleTxt, ""); GUI.Label(new Rect(35, 40, 100, 24), "Consensus");
		toggleTwo = GUI.Toggle(new Rect(10, 70, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 70, 100, 24), "Distorted Test");
		toggleTwo = GUI.Toggle(new Rect(10, 100, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 100, 100, 24), "Oyster");
		// End the ScrollView
		GUI.EndScrollView();
		
		
		GUI.Label(new Rect(260, 140, 100, 30), "Fish Paths : ");
		scrollViewVector2 = GUI.BeginScrollView (new Rect (260, 155, 220, 120), scrollViewVector2, new Rect (0, 0, 200, 150));

		// Put something inside the ScrollView
			toggleThree = GUI.Toggle(new Rect(10, 10, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 10, 100, 24), "Zig Zag");
			toggleTwo = GUI.Toggle(new Rect(10, 40, 20, 24), toggleTwo, ""); GUI.Label(new Rect(35, 40, 100, 24), "Loop");
		toggleThree = GUI.Toggle(new Rect(10, 70, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 70, 100, 24), "Nest Point");
		toggleThree = GUI.Toggle(new Rect(10, 100, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 100, 100, 24), "Idle Wander");
		// End the ScrollView
		GUI.EndScrollView();
		
		GUI.Button (new Rect (260,290,80,32), "New Path");
		GUI.Button (new Rect (365,290,80,32), "Edit Path");
		
		// End the group we started above. This is very important to remember!
		
		
		GUI.Label(new Rect(35, 340, 100, 30), "Textures : ");
		scrollViewVector2 = GUI.BeginScrollView (new Rect (15, 355, 220, 120), scrollViewVector2, new Rect (0, 0, 200, 150));

		// Put something inside the ScrollView
			toggleThree = GUI.Toggle(new Rect(10, 10, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 10, 100, 24), "1164");
			toggleThree = GUI.Toggle(new Rect(10, 40, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 40, 100, 24), "1169");
		toggleThree = GUI.Toggle(new Rect(10, 70, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 70, 100, 24), "1171");
		toggleThree = GUI.Toggle(new Rect(10, 100, 20, 24), toggleThree, ""); GUI.Label(new Rect(35, 100, 100, 24), "1175");
		// End the ScrollView
		GUI.EndScrollView();
		
		GUI.Button (new Rect (25,500,100,32), "Batch Render");
		// End the group we started above. This is very important to remember!
		
		GUI.Label(new Rect(260, 340, 100, 30), "Time Dialation: ");
		
		GUI.Label(new Rect(260, 365, 120, 30), "Dialation frames : ");
		GUI.TextField(new Rect(370, 365, 50, 25), "16");
		GUI.Label(new Rect(260, 390, 120, 30), "Render Every: ");
		GUI.TextField(new Rect(370, 390, 50, 25), "4");
		
		
		
		GUI.EndGroup ();
		*/
	}
}
