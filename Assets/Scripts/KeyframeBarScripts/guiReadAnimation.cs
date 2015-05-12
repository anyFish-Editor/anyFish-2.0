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

public class guiReadAnimation : MonoBehaviour
{
	private char pathChar = "/"[0];
	private string pathToFileLabel = "";
	private MoCapAnimDataPlayer player;
	private bool isGuiVisible = true;
	public static int selGridInt = -1;
	public static int selShowMocapInt = -1;
    private string[] selStrings = new string[] {"PureMocap", "PureKeyframe", "MocapPosKeyedRot"};
	private string[] selShowLineRendererStrings = new string[] {"Show MocapPath", "Hide MocapPath"};
	
	void Awake()
	{
		player = GetComponent<MoCapAnimDataPlayer>();
		print(player.name + " found");
		
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) 
		{
			pathChar = "\\"[0];
		}	
		
		Messenger.AddListener("TurnOffGui", onTurnOffGui);
		Messenger.AddListener("TurnOnGui", onTurnOnGui);
		Messenger<string>.AddListener("MocapLoadedState", OpenFile);
	}
	
	private void onTurnOffGui()
	{
		isGuiVisible = false;	
	}
	private void onTurnOnGui()
	{
		isGuiVisible = true;	
	}
	
	void OnGUI () 
	{	
		if(isGuiVisible){
	// Make the first button.
		if (GUI.Button(new Rect (20,100,80,20), "Animation")) 
		{
			SendMessage("OpenFileWindow");
		}
		GUI.Label(new Rect (110,100,800,20), pathToFileLabel);
		if (GUI.Button(new Rect(20, 130, 80, 20), "Play"))
		{
			player.play();
		}
			selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3);
			
			selShowMocapInt = GUILayout.SelectionGrid(selShowMocapInt, selShowLineRendererStrings, 2);
		}
	}

	public void OpenFile(string pathToFile) 
	{
		SaveState.setPathMoCap(pathToFile);
		
		//print(pathToFile);
		
		List<MoCapAnimData> animData = FileIO.ImportAnimation(pathToFile);
		pathToFileLabel = pathToFile;
		print("Imported List");
		
		
		player.moCapClip = animData;
		Debug.Log("DEFAULT POSITION: " + (animData[0].Rotation));
		print("AnimInit - Clip enabled");
		Messenger.Broadcast("AnimInit");
		Messenger<List<MoCapAnimData>>.Broadcast("MocapDataLoaded", animData);
	}	
	
}
			
