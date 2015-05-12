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
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animation))]
[RequireComponent (typeof (Rigidbody))]	

public class MoCapAnimDataPlayer : MonoBehaviour {
	
	public List<MoCapAnimData> moCapClip = null;
	public List<MoCapAnimData> keyFrameData = null;
	
	public float fps = 50.0f;
	private float targetDelta;// = 1/fps;
	private float currDelta = 0;
	
	public float timeDialationMultiplier = 3;
	private float countToMultiplier = 1;
	
	private int frame;
	
	private bool playMode = false;
	private bool stopMode = false;
	

	// Use this for initialization
	void Awake () 
	{
		//Messenger.AddListener("ScrubberFrameAdvance", onFrameAdvance);
		//Messenger.AddListener("ScrubberFrameDecline", onFrameDecline);
		targetDelta = 1/fps;
	}
	
	void OnEnable()
	{
		//Messenger.AddListener("ScrubberFrameAdvance", onFrameAdvance);
		//Messenger.AddListener("ScrubberFrameDecline", onFrameDecline);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(moCapClip != null && stopMode != true) //Don't track frames or do anything unless a clip is present
		{
			updateDelta();
			
			if(stopMode == true && currDelta != 0)	//Stop play loop actions and reset the delta counter;
			{
				resetDelta();	
			}
			
			if(playMode == true)	//Don't check Delta unless play is enabled
			{
				// Attempt to smooth into rotation of new frame
				
				if(currDelta >= targetDelta)	//Allows updates at the desire targetDela/Framerate
				{
					// New switchbased method
					
					switch(guiReadAnimation.selGridInt)
					{
					case 0:	
						Debug.Log("Rotation: " + moCapClip[frame].Rotation);
						transform.position = moCapClip[frame].Position;
						transform.localEulerAngles = moCapClip[frame].Rotation;
						break;
					case 1:
						transform.position = Vector3.Lerp(keyFrameData[frame - 1].Position, keyFrameData[frame].Position, countToMultiplier/timeDialationMultiplier);
						//transform.localEulerAngles = Vector3.Lerp(keyFrameData[frame - 1].Rotation, keyFrameData[frame].Rotation, countToMultiplier/timeDialationMultiplier);				
						transform.rotation = Quaternion.Lerp(keyFrameData[frame - 1].QRotation, keyFrameData[frame].QRotation, countToMultiplier/timeDialationMultiplier);
						break;
					case 2:
						transform.position = Vector3.Lerp(moCapClip[frame - 1].Position, moCapClip[frame].Position, countToMultiplier/timeDialationMultiplier);
						transform.localEulerAngles = Vector3.Lerp(keyFrameData[frame - 1].Rotation, keyFrameData[frame].Rotation, countToMultiplier/timeDialationMultiplier);				
						
						break;
					}
					
					
					//if(countToMultiplier % 8 == 0)
					//	ProduceScreenShots.TakePic();
					
					
					// Old method
					
					Debug.Log("Rotation ------ : " + moCapClip[frame].Rotation.x);
					
					Debug.Log("Frame advance: " + countToMultiplier);
					
					if(countToMultiplier >= timeDialationMultiplier)
					{
						Debug.Log("Frame advance");
						++frame;
						resetDelta();
					}
					countToMultiplier++;
					if(countToMultiplier > timeDialationMultiplier)
						countToMultiplier = 1f;
				}
				
			}
			
			if(frame == moCapClip.Count-1)
			{
				Messenger.Broadcast("MoCapStop");
				stopMode = true;
				//ProduceScreenShots.Stop();
			}
		}
		
	}
	
	void onFrameAdvance()
	{
		//
		//
		if(stopMode != true) //Don't track frames or do anything unless a clip is present
		{
			//updateDelta(1);
					
				//if(currDelta >= targetDelta)	//Allows updates at the desire targetDela/Framerate
				//{
					transform.localEulerAngles = moCapClip[frame].Rotation;
					//transform.rotation = moCapClip[frame].Rotation;
					transform.position = moCapClip[frame].Position;
					++frame;
				//	//resetDelta();
				//}	
			
			print("Frame Advance");
			if(frame == moCapClip.Count-1)
			{
				Messenger.Broadcast("MoCapStop");
				stopMode = true;
			}
		}
	}
	
	void onFrameDecline()
	{
		if(moCapClip != null && stopMode != true) //Don't track frames or do anything unless a clip is present
		{
			updateDelta(-1);
			if(playMode == true)	//Don't check Delta unless play is enabled
			{		
				if(Math.Abs(currDelta) >= targetDelta)	//Allows updates at the desire targetDela/Framerate
				{
					transform.localEulerAngles = moCapClip[frame].Rotation;
					//transform.rotation = moCapClip[frame].Rotation;
					transform.position = moCapClip[frame].Position;
					++frame;
					resetDelta();
				}				
			}
			
			if(frame == moCapClip.Count-1)
			{
				Messenger.Broadcast("MoCapStop");
				stopMode = true;
				frame = 1;
			}
		}
	}
	
	public List<MoCapAnimData> getMocapClipList()
	{
		return moCapClip;
	}
	
	void updateDelta(float sign)
	{
		currDelta += sign * Time.deltaTime;			
	}
	
	void updateDelta()
	{
		currDelta += Time.deltaTime;			
	}
	
	public void setMocapFrame(int gotoFrame)
	{
		if(moCapClip != null)
		{
			transform.localEulerAngles = moCapClip[gotoFrame].Rotation;;
			transform.position = moCapClip[gotoFrame].Position;
			//moCapClip[gotoFrame];
		}
	}
	
	public Vector3 getMocapFramePosition(int lookupFrame)
	{
		if(moCapClip != null)
			return moCapClip[lookupFrame].Position;
		else 
			return new Vector3();
	}
	
	public Vector3 getMocapFrameRotation(int lookupFrame)
	{
		if(moCapClip != null)
			return moCapClip[lookupFrame].Rotation;
		else 
			return new Vector3();
	}
	
	void resetDelta()
	{
		currDelta = 0f;	
	}
	
	public void play()
	{
		
		playMode = true;	
		stopMode = false;
		keyFrameData = KeyframeGameRegistry.keyBar.buildPlaybackData();
		frame = 1;
		
		GameObject keyObject = GameObject.Find("Stickleback_KeyframeAble");
		//if(keyObject != null)
		//{
		keyObject.SetActiveRecursively(false);
		//keyObject.active = false;
			//Destroy(keyObject);
		//}
		
		GameObject.Find("FrontViews").renderer.enabled = false;
		GameObject.Find("TopViews").renderer.enabled = false;
		Messenger.Broadcast("TurnOffGui");
		//ProduceScreenShots.Start();
		
		// Get and convert keyframeInfo
		/*
		List<KeyframeInfo> keys = GameRegistry.keyBar.keyInfo;
		moCapClip = new List<MoCapAnimData>();
		for(int i = 0; i < keys.Count; i++)
		{
			//moCapClip
			if(keys[i] != null)
			{
			MoCapAnimData temp = new MoCapAnimData();
			temp.Position = new Vector3();
			temp.Rotation = new Quaternion();
			Debug.Log(keys[i].getPosition());
			temp.Position = keys[i].getPosition();
			temp.Rotation = keys[i].getRotation();
			
			moCapClip.Add(temp);
			}
			//moCapClip[i].Rotation = keys[i].getRotation();
		}
		*/
		
		Messenger.Broadcast("AnimInit");
	}
}
