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

//this class handles pause and pause menu in the game
public class PauseMenuGUI : MonoBehaviour 
{
	public GUISkin currentSkin;
	public bool isPaused = false;
	public bool isPauseMenuActive = false;
		
	void Update () 
	{	
		
		//if pause key is pressed , freeze the game by setting timeScale to 0, GUI still responds to user input even if the updating its completely frozen
		//also check if the game is already paused, if so unfreeze the game by setting timeScale back to 1
		//remember that the update method only works if timeScale is set to anything but 0
		// also all timers in unity depend on the timeScale setting, to if something its set to happen in 1 second, and timeScale is set to 0.25 it will happen in 4 seconds
		 if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))		
         {
			if(isPaused && isPauseMenuActive)
			{
				isPaused = false;
			}
			else
			{
				isPaused = true;
			}
				
		}
		
		if(isPaused)
		{	
			isPaused = true;
			if(!(GetComponent("ControlsGUI") as ControlsGUI).isControlsGUIActive && !(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).isVideoOptionsActive)
			{
				isPauseMenuActive = true;
			}
			Time.timeScale = 0f;
		}
		else
		{
			isPaused = false;
			isPauseMenuActive = false;
			Time.timeScale = 1f;
		}
	}
	
	void OnGUI()
	{	
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3((float)1.0 * Screen.width/1280, (float)1.0 * Screen.height/800, 1.0f));
		GUI.skin = currentSkin;		
		
		if(isPaused && isPauseMenuActive)
		{		
			//this tells unity to draw this gui after everything else, so it can pop out
			GUI.depth = 0;
			GUILayout.BeginArea(new Rect((1280 / 2) - 150,(800 / 2) - 150,300,280),"Game Paused",GUI.skin.window);
			
			
			if(GUILayout.Button("Resume Game"))
			{
				//unfreeze game
				Time.timeScale = 1f;
				isPaused = false;
			}
			
			//this next 2, call other gui windows, and gui still responds even when a game is frozen, theres no need to unfreeze
			if(GUILayout.Button("Controls"))
			{
                (GetComponent("ControlsGUI") as ControlsGUI).enabled = true;
                (GetComponent("ControlsGUI") as ControlsGUI).isControlsGUIActive = true;
                isPauseMenuActive = false;
			}
			

			if(GUILayout.Button("Video Options"))
			{
				(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).enabled = true;
				(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).isVideoOptionsActive = true;
				isPauseMenuActive = false;
			}

			
			//as this next 3 call different scenes, you need to unfreeze the game before switching the scene otherwise the game will not respond
            if (Application.loadedLevelName == "LevelEditor")
            {
                if (GUILayout.Button("Go to Level Selection"))
                {
                    Time.timeScale = 1f;
                    Application.LoadLevel("LevelSelection");
                }
            }
            else
            {
                if (GUILayout.Button("Go to Level Editor"))
                {
                    Time.timeScale = 1f;
                    Application.LoadLevel("LevelEditor");
                }
            }          

            if (GUILayout.Button("Return to Main Menu"))
            {
                Time.timeScale = 1f;
                Application.LoadLevel("MainMenu");
            }
			
			if(GUILayout.Button("Quit Game"))
			{
				Application.Quit();
			}
			GUILayout.EndArea();
		}
	}
}
