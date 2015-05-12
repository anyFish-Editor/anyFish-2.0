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

public class KeyframeGameRegistry : MonoBehaviour {
	public static string animationFilePath = "";
	public static string motionCapFilePath = "";
	public static string morphDataFilePath = "";
	public static string rigTextureFilePath = "";
	
	public static KeyframeBar keyBar;
	
	public bool isBatchRenderer = false;
	public int keyFrameBarX = 20;
	public int keyFrameBarY = 20;
	public GameObject keyableGameObject;
	public GameObject MotionCaptureTarget;
	
	public bool isDebugModeActive = false;
	
	// Use this for initialization
	void Awake () {
		keyBar = gameObject.AddComponent<KeyframeBar>();
		keyBar.keyedObject = keyableGameObject;
		keyBar.debugMode = isDebugModeActive;
		keyBar.motionCaptureTarget = MotionCaptureTarget;
		//TODO moveable keyframeBar
		//keyBar.transform.Translate((float)keyFrameBarX, (float)keyFrameBarY, 0.0f);
		
		// Switch the state
		GameRegistry activeRegistry = GameObject.Find("EditorApplication").GetComponent<GameRegistry>();
		if(isBatchRenderer)
			activeRegistry.switchState(States.BatchRenderer);
		else
			activeRegistry.switchState(States.AnimationEditor);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.P))
    	{
			GameObject.Find("FrontViews").renderer.enabled = false;
			GameObject.Find("TopViews").renderer.enabled = false;
			Messenger.Broadcast("TurnOffGui");
		}
		if (Input.GetKeyDown(KeyCode.L))
    	{
			GameObject.Find("FrontViews").renderer.enabled = true;
			GameObject.Find("TopViews").renderer.enabled = true;
			Messenger.Broadcast("TurnOnGui");
		}
	}
	
}
