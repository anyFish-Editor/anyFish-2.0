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

public class ReferencePlane : MonoBehaviour {
	private bool isGuiVisible = true;
	private int lookDirectionLoaded = 0;
	void Awake ()
	{
		Messenger.AddListener("TurnOffGui", onTurnOffGui);
		Messenger.AddListener("TurnOnGui", onTurnOnGui);
		Messenger.AddListener("LoadTop", onLoadTop);
		Messenger.AddListener("LoadFront", onLoadFront);
	}
	
	private void onTurnOffGui()
	{
		isGuiVisible = false;	
	}
	private void onTurnOnGui()
	{
		isGuiVisible = true;	
	}
	
	void onLoadFront()
	{
		lookDirectionLoaded = 0;
		SendMessage("OpenFileWindow");
	}
	void onLoadTop()
	{
		lookDirectionLoaded = 1;
		SendMessage("OpenFileWindow");
	}
	public void OnGUI()
    {
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)1.0 * Screen.width / 1280, (float)1.0 * Screen.height / 800, 1.0f));
        //GUI.skin = currentSkin;
		
		if(isGuiVisible){
			GUI.skin =  GuiManager.GetSkin();
			
			/* Functionality moved to AnimationEditorState
			if (GUI.Button(new Rect (20,70,120,30), "Frame Sequence")) 
			{
				//Messenger.Broadcast("OpenFileWindow");
				SendMessage("OpenFileWindow");
				lookDirectionLoaded = 0;
			}
			
			if (GUI.Button(new Rect (140,70,120,30), "Frame Sequence Top")) 
			{
				//Messenger.Broadcast("OpenFileWindow");
				SendMessage("OpenFileWindow");
				lookDirectionLoaded = 1;
			}
			*/
		}
	}
	
	public void OpenFile(string pathToFile)
	{
		Debug.Log("Test");
		Debug.Log(pathToFile);
		if(lookDirectionLoaded == 0)
		{
			Messenger<string>.Broadcast("ScrubberFrameAdvance0", pathToFile);
			SaveState.setPathToFront(pathToFile);
		}
		if(lookDirectionLoaded == 1)
		{
			Messenger<string>.Broadcast("ScrubberFrameAdvance1", pathToFile);
			SaveState.setPathToTop(pathToFile);
		}
	}
}
