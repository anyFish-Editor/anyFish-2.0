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

public class ScrubberTextureHolder : MonoBehaviour
	{	
	//private List<Texture2D> scrubTextures;
	private int currentFrame = 1;
	private int maxFrame = 1;
	private LineRenderer lRenderer;
	private string animationFilePath;
	private WWW www;
	public int LookDirection = 0;
	
	void Start () {
		
	}
	
	void Awake()
	{
		//scrubTextures = new List<Texture2D>();
		//scrubTextures.Add((Texture2D)Resources.Load("1.png", (Texture2D)));
		//setTextureByFrame(1);
		
	}
	
	void OnEnable()
	{
		if(LookDirection == 0)
			Messenger<string>.AddListener("ScrubberFrameAdvance0", setTextureTest);
		if(LookDirection == 1)
			Messenger<string>.AddListener("ScrubberFrameAdvance1", setTextureTest);
		
		Messenger<int>.AddListener("SetTextureByFrame", setTextureByFrame);
		
		//Messenger.AddListener("ScrubberFrameDecline", onFrameDecline);
	}
	
	void OnDisable()
	{
		//Messenger.RemoveListener("ScrubberFrameAdvance", onFrameAdvance);
		//Messenger.RemoveListener("ScrubberFrameDecline", onFrameDecline);
	}
	
	void setupMocapPreviewLine()
	{
		
		//List<MoCapAnimData> test = GetComponent<MoCapAnimDataPlayer>().getMocapClipList();
		//List<MoCapAnimData> test = MoCapAnimDataPlayer.getMocapClipList();
		//print(test.Count);
	}
	
	int getSequenceLength(string filePath)
	{
		int length = 1;
		int counter = 100;
		bool hasMore = true;
		
		while(hasMore)
		{
			/*
			//split it
			string newPath = filePath.Replace("1", length.ToString());
			Debug.Log("Testing path: " + newPath);
			if(System.IO.File.Exists(newPath))
			{
			   hasMore = true;
			   length++;
			}
			else
			   hasMore = false;
			*/
			
			if(verifyFileExists(filePath, length + counter))
			{			
	
				length += counter;
				if(counter < 0)
					counter *= -1;
				Debug.Log("Length is checking: " + length);
			}else
			{
				
				counter = counter / -2;
				Debug.Log("Fail: Counter changed to : " + counter);
				//Mathf.Round(counter);
			}
			
			if(counter <= 1 && counter >= -1)
			{
				if(counter < 0)
					if(verifyFileExists(filePath, length + counter))
						length++;
				
				
				hasMore = false;
			}
		}
		
		// We subtract one to account for starting at 1 instead of 0 for frame count
		return(length - 1);
	}
	
	bool verifyFileExists(string path, int fileNumber)
	{
		string newPath = path.Replace("1", fileNumber.ToString());
		if(System.IO.File.Exists(newPath))
			return true;
		
		return false;
	}
	void setTextureTest(string filePath)
	{
		Debug.Log("ScrubberTextureHolder::setTextureTest");
		animationFilePath = filePath;
		maxFrame = getSequenceLength(filePath);
		Debug.Log("TestTest" + filePath + ", maxframe: " + maxFrame);
		www = new WWW ("file://" + filePath);
		KeyframeGameRegistry.keyBar.respawnWithFrameCount(maxFrame);
    	// Wait for download to complete
		StartCoroutine(waitForFrameLoaded());
		//gameObject.transform.localScale += new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
		//renderer.material.SetTexture("_MainTex", www.texture);
	}
	
	public IEnumerator waitForFrameLoaded()
	{
		yield return www;
		
		if(www.isDone)
		{
			//gameObject.transform.localScale = new Vector3(www.texture.width / 10, 0, www.texture.height / 10) ;
			renderer.material.SetTexture("_MainTex", www.texture);
		}
	}
	void onFrameAdvance()
	{	
		//setupMocapPreviewLine();
		//currentFrame += 5;
		//GameRegistry.keyBar.currentFrame;
		setTextureByFrame(currentFrame);
	}
	
	void onFrameDecline()
	{
		//currentFrame = currentFrame - 5;
		setTextureByFrame(currentFrame);
	}
	
	void setTextureByFrame(int frameNumber)
	{
		currentFrame = KeyframeGameRegistry.keyBar.currentFrame;
		Debug.Log("SetTextureByFrame: " + frameNumber);
		if(frameNumber < maxFrame && frameNumber > 0)
		{
			string newPath = "file://" + animationFilePath.Replace("1", frameNumber.ToString());
			WWW www = new WWW (newPath);
			renderer.material.SetTexture("_MainTex", www.texture);
			Resources.UnloadUnusedAssets();
		}
	}
	
	void OnGUI () 
	{	
	// Make the first button.
		//if (GUI.Button(new Rect (500,300,80,20), " ---> ")) 
		//{
			//onFrameAdvance();
			//Messenger.Broadcast("ScrubberFrameAdvance");
			
			
			
		//}
		//if (GUI.Button(new Rect(20, 300, 80, 20), " <--- "))
		//{
		//	onFrameDecline();
			//Messenger.Broadcast("ScrubberFrameDecline");
		//}
	}
	
	// Update is called once per frame
	void Update () {
		//setTextureByFrame(currentFrame);
	}
}