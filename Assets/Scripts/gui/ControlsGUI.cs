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


// this class shows game controls from main menu and pause menu
public class ControlsGUI : MonoBehaviour 
{
	public GUISkin currentSkin;
	public bool isControlsGUIActive = false;
	public bool wasFromMenu = false;

	void OnGUI () 
	{
		
		//handles gui resolution independence and prints the game controls to the dialog on screen
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3((float)1.0 * Screen.width/1280, (float)1.0 * Screen.height/800, 1.0f));
		GUI.skin = currentSkin;
		GUI.depth = 0;
		GUILayout.BeginArea(new Rect((1280 / 2) - 250,(800 / 2) - 200,500,400),"CONTROLS",GUI.skin.window);
		GUILayout.Label("");
        GUILayout.Label("WASD to move the Ball");
        GUILayout.Label("Space or Right Control to Jump");
        GUILayout.Label("Pan the view with the Mouse");
        GUILayout.Label("ESC to Quit");
		GUILayout.Label("");
		
		//checks if the controls dialog was called from main menu or from pause menu, to know what window should it return to
		if(GUILayout.Button("Back"))
			{
				isControlsGUIActive = false;
				if(!wasFromMenu)
				{
					
					(GetComponent("PauseMenuGUI") as PauseMenuGUI).isPauseMenuActive = true;
					wasFromMenu = false;
				}
				else
				{
					(GetComponent("MainMenuGUI") as MainMenuGUI).isMainMenuActive = true;
					wasFromMenu = false;
				}
				
				(GetComponent("ControlsGUI") as ControlsGUI).enabled = false;	
				
			}
		GUILayout.EndArea();	
	}
}
