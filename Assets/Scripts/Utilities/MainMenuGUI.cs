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
//this class handles main menu
public class MainMenuGUI : MonoBehaviour 
{	
	public GUISkin currentSkin;
	public Texture2D projectLogo;
	public bool isMainMenuActive = false;
	
	void Start()
	{
		isMainMenuActive = true;
	}
	
	void OnGUI() 
	{
		//sets resolution indepence , nothing too fancy
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3((float)1.0 * Screen.width/1280, (float)1.0 * Screen.height/800, 1.0f));
		GUI.skin = currentSkin;
		GUI.depth = 3;
		
		if (isMainMenuActive) 
		{	
			//draws the project logo above the menu
			GUI.DrawTexture(new Rect(340, 100, 600,262), projectLogo, ScaleMode.ScaleToFit);
			
			GUI.Label(new Rect(10f,750f,100f,50f),"V2.1");
			
			GUI.enabled = false;
			if(GUI.Button(new Rect(640 -150,(800 - 25) - 320,300,50),"Play"))
			{
				Application.LoadLevel("LevelSelection");
			}
			GUI.enabled = true;

            if (GUI.Button(new Rect(640 - 150, (800 - 25) - 250, 300, 50), "Level Editor"))
            {
                Application.LoadLevel("LevelEditor");
            }
		
			//this next 2, call another gui windows, firs it enables the gui, then sets the active flag for that dialog and then sets where its being called from
			if(GUI.Button(new Rect(640 - 150,(800 - 25) - 180,300,50),"Controls"))
			{
				(GetComponent("ControlsGUI") as ControlsGUI).enabled = true;
				(GetComponent("ControlsGUI") as ControlsGUI).isControlsGUIActive = true;
				(GetComponent("ControlsGUI") as ControlsGUI).wasFromMenu = true;
				isMainMenuActive = false;
			}
			

			if(GUI.Button(new Rect(640 - 150,(800 - 25) - 110,300,50),"Video Options"))
			{
				(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).enabled = true;
				(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).isVideoOptionsActive = true;
				(GetComponent("VideoOptionsGUI") as VideoOptionsGUI).wasFromMenu = true;
				isMainMenuActive = false;
			}
			
			if(GUI.Button(new Rect(640 - 150,(800 - 25) - 40,300,50),"Quit"))
			{
				if(isMainMenuActive)
				{
					Application.Quit();
				}
			}	
		}	
	}
}
