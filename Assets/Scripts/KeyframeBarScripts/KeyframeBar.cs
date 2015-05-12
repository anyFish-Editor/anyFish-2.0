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
//using UnityEditor;

public class KeyframeBar : MonoBehaviour {
	private ArrayList keys;
	public List<KeyframeInfo> keyInfo;
	private LabelBarTexture labelBar;
	private Vector2 scrollViewVector = Vector2.zero;
	
	public int currentFrame = 0;
	public int targetFrame = 0;
	public int nOriginal = 0;
	
	public string Pectmove = "5";
	public string FirstKey = "0";
	public string LastKey = "90";
	
	private bool isGuiVisible = true;
	
	private Rect containingRect;
	public GameObject keyedObject;
	public GameObject motionCaptureTarget;
	public bool debugMode = false;
	
	public bool PositionToggle = true;
	public bool RotationToggle = true;
	public bool DorsalToggle = true;
	public bool OthersToggle = true;
	public bool StopPositionToggle = true;
	
	
	
	
	private int maxScrollLength = 56;
	private bool hasBeenResized = false;
	void Start () {
		//Messenger<List<KeyframeInfo>>.AddListener("KeyframeLoaded", onKeyframeLoaded);
		keys = new ArrayList();
		keyInfo = new List<KeyframeInfo>();
		
		updateFrameDisplay();
		labelBar = new LabelBarTexture(530);
		labelBar.y = 1;
		
		containingRect = new Rect((Screen.width / 2) - 250, Screen.height - 75, 500, 52);
		
		Messenger.AddListener("TurnOffGui", onTurnOffGui);
		Messenger.AddListener("TurnOnGui", onTurnOnGui);
		Messenger<List<KeyframeInfo>>.AddListener("KeyframeLoaded", onKeyframeLoaded);
		Messenger<int>.AddListener("KeyframeResize", respawnWithFrameCount);
	}
	
	// During the awake method we build a list of KeyframeTextures, and apply the isMultipleOfFive
	// boolean to have them shaded for easier recognition of frame breakdowns. All data under the commented
	// line is only meant to serve as "setup" variables for testing and can be removed
	void Awake() {
		Messenger<List<KeyframeInfo>>.AddListener("KeyframeLoaded", onKeyframeLoaded);
		Messenger<int>.AddListener("KeyframeResize", respawnWithFrameCount);
    }
	
	// UpdateFrameDisplay is used to toggle the current states of each frameTexture,
	// iterating through them and determining if it should be toggled. This fixes the removal
	// or addition of keyframes, and is called when a selection is changed.
	private void updateFrameDisplay()
	{
		for(int i = 0; i < keyInfo.Count; i++)
		{
			KeyframeTexture key = (KeyframeTexture)keys[i];
			if(keyInfo[i].isKeyed)				
				key.isKeyed = true;
			else
				key.isKeyed = false;				
		}
		
		for(int i = 0; i < keyInfo.Count; i++)
		{
			KeyframeTexture key = (KeyframeTexture)keys[i];
			if(keyInfo[i].isPectKey)				
				key.isPectKey = true;
			else
				key.isPectKey = false;				
		}
	}
	
	// A click on the timeline calls this, scrub by the current keyframe, if between frames find
	// and apply the lerp between the two
	private void gotoFrame(int i)
	{
		
		currentFrame = i;
		//MoCapAnimDataPlayer player = GameObject.Find("MotionCaptureTarget").GetComponent<MoCapAnimDataPlayer>();
		//player.getMocapFramePosition(i);
		//keyedObject.transform.position = motionCaptureTarget.transform.position;
		
		
		Debug.Log("GOTO " + i + " PureKeyframe transform "+ guiReadAnimation.selGridInt);
		/*
		if(guiReadAnimation.selGridInt == 0){
			keyedObject.transform.position = player.getMocapFramePosition(i);
			keyedObject.transform.eulerAngles = player.getMocapFrameRotation(i);
		}
		if(guiReadAnimation.selGridInt == 1){
		*/
		if(keyedObject){
			Debug.Log("goto keyframeRot transform");
			if(keyInfo[i].isKeyed)
			{
				keyedObject.transform.position = keyInfo[i].getPosition(); //new Vector3(keyInfo[i].tx, keyInfo[i].ty, keyInfo[i].tz); 
				keyedObject.transform.rotation = keyInfo[i].getRotation(); 
				AnimationEditorState.dorsalAmount = keyInfo[i].dorsalAngle;
				AnimationEditorState.lpelvicHAmount = keyInfo[i].lpelvicAngles.x;
				AnimationEditorState.lpelvicVAmount = keyInfo[i].lpelvicAngles.y;
				
				AnimationEditorState.rpelvicHAmount = keyInfo[i].rpelvicAngles.x;
				AnimationEditorState.rpelvicVAmount = keyInfo[i].rpelvicAngles.y;
				
				AnimationEditorState.lpectoralHAmount = keyInfo[i].lpectAngles.x;    //Mahmoud  02.17.14
				AnimationEditorState.lpectoralVAmount = keyInfo[i].lpectAngles.y;    // Mahmoud 02.17.14
				
				AnimationEditorState.analHAmount = keyInfo[i].analAngles.x;
				AnimationEditorState.analVAmount = keyInfo[i].analAngles.y;
			}else
			{
				Vector3 newPosition;
				Quaternion newRotation;
				float dorsalAmount;
				Vector2 newLPelvic;
				Vector2 newRPelvic;
				Vector2 newAnal;
				Vector2 newLPectoral;
				Vector2 newRPectoral;
				interpolateKeys(i, out newRotation, out newPosition, out dorsalAmount, out newLPelvic, out newRPelvic, out newLPectoral, out newRPectoral, out newAnal);
				keyedObject.transform.position = newPosition;				
				keyedObject.transform.rotation = newRotation;
				AnimationEditorState.dorsalAmount = dorsalAmount;
				AnimationEditorState.lpelvicHAmount = newLPelvic.x;
				AnimationEditorState.lpelvicVAmount = newLPelvic.y;
				
				AnimationEditorState.rpelvicHAmount = newRPelvic.x;
				AnimationEditorState.rpelvicVAmount = newRPelvic.y;
				
				AnimationEditorState.lpectoralHAmount = newLPectoral.x;   // Mahmoud 02.17.14
				AnimationEditorState.lpectoralVAmount = newLPectoral.y;   // Mahmoud 02.17.14
				
				AnimationEditorState.rpectoralHAmount = newRPectoral.x;   // Mahmoud 02.17.14
				AnimationEditorState.rpectoralVAmount = newRPectoral.y;   // Mahmoud 02.17.14
				
				AnimationEditorState.analHAmount = newAnal.x;
				AnimationEditorState.analVAmount = newAnal.y;
			}
		}
		
		
	}
	
	private void interpolateKeys(int i, out Quaternion rotation, out Vector3 position, out float dorsalAmount, out Vector2 lPelvic, out Vector2 rPelvic, out Vector2 lPectoral, out Vector2 rPectoral, out Vector2 anal)
	{
		KeyframeInfo lastKey = new KeyframeInfo();
		KeyframeInfo nextKey = new KeyframeInfo();
		int j = 0;
		int k = 0;
		// Check past keys
		for(j = i; j >= 0; j--)
		{
			if(keyInfo[j].isKeyed){
				lastKey = (KeyframeInfo)keyInfo[j];
				break;
			}
		}
		
		for(k = i; k < keyInfo.Capacity; k++)
		{
			if(keyInfo[k].isKeyed){
				nextKey = (KeyframeInfo)keyInfo[k];
				break;
			}
		}
			
		//Debug.Log("Last : " + j + " , next : " + k + ", and current: " + i + ", float: " + test);
		position = Vector3.Lerp(lastKey.getPosition(), nextKey.getPosition(), (float)(i - j)/(k - j));
		rotation = Quaternion.Slerp(lastKey.getRotation(), nextKey.getRotation(), (float)(i - j)/(k - j));	
		dorsalAmount = Mathf.Lerp(lastKey.dorsalAngle, nextKey.dorsalAngle, (float)(i-j)/(k-j));
		
		lPelvic.x = Mathf.Lerp(lastKey.lpelvicAngles.x, nextKey.lpelvicAngles.x, (float)(i-j)/(k-j));
		lPelvic.y = Mathf.Lerp(lastKey.lpelvicAngles.y, nextKey.lpelvicAngles.y, (float)(i-j)/(k-j));
		
		rPelvic.x = Mathf.Lerp(lastKey.rpelvicAngles.x, nextKey.rpelvicAngles.x, (float)(i-j)/(k-j));
		rPelvic.y = Mathf.Lerp(lastKey.rpelvicAngles.y, nextKey.rpelvicAngles.y, (float)(i-j)/(k-j));

		lPectoral.x = Mathf.Lerp(lastKey.lpectAngles.x, nextKey.lpectAngles.x, (float)(i-j)/(k-j));
		lPectoral.y = Mathf.Lerp(lastKey.lpectAngles.y, nextKey.lpectAngles.y, (float)(i-j)/(k-j));
		
		rPectoral.x = Mathf.Lerp(lastKey.rpectAngles.x, nextKey.rpectAngles.x, (float)(i-j)/(k-j));
		rPectoral.y = Mathf.Lerp(lastKey.rpectAngles.y, nextKey.rpectAngles.y, (float)(i-j)/(k-j));
		
		anal.x = Mathf.Lerp(lastKey.analAngles.x, nextKey.analAngles.x, (float)(i-j)/(k-j));
		anal.y = Mathf.Lerp(lastKey.analAngles.y, nextKey.analAngles.y, (float)(i-j)/(k-j));
	}
	
	public KeyframeInfo getKeyinfoByFrame(int frame)
	{
		if(keyInfo != null)
		{
			if(keyInfo[frame].isKeyed)
				return keyInfo[frame];
		}
		
		return null;
	}
	private void setKey()
	{
		Debug.Log("KeyframeBar::setKey ");
		keyInfo[currentFrame] = new KeyframeInfo();
		keyInfo[currentFrame].isKeyed = true;
		//KeyframeInfo test = (KeyframeInfo)keyInfo[currentFrame];
		keyInfo[currentFrame].position(keyedObject.transform.position);
		keyInfo[currentFrame].rotation(keyedObject.transform.rotation);
		Debug.Log(keyedObject.transform.position);
		//test.position(new Vector3(pos.x, pos.y, pos.z)); 
		if(AnimationEditorState.dorsalAmount != 0.0f)
		keyInfo[currentFrame].dorsalAngle = AnimationEditorState.dorsalAmount;
		keyInfo[currentFrame].lpelvicAngles = new Vector2(AnimationEditorState.lpelvicHAmount, AnimationEditorState.lpelvicVAmount);
		keyInfo[currentFrame].rpelvicAngles = new Vector2(AnimationEditorState.rpelvicHAmount, AnimationEditorState.rpelvicVAmount);
		keyInfo[currentFrame].lpectAngles = new Vector2(AnimationEditorState.lpectoralHAmount, AnimationEditorState.lpectoralVAmount);  // Mahmoud 02.17.14
		keyInfo[currentFrame].rpectAngles = new Vector2(AnimationEditorState.rpectoralHAmount, AnimationEditorState.rpectoralVAmount);  // Mahmoud 02.17.14
		keyInfo[currentFrame].analAngles = new Vector2(AnimationEditorState.analHAmount, AnimationEditorState.analVAmount);
		updateFrameDisplay();
	}
	
	private void deleteKey()
	{
		//if(keyInfo[currentFrame].isKeyed)
		//	keyInfo[currentFrame].isKeyed = false;
		
		for(int i = 0; i < keyInfo.Count; i++)
		{
			for(int j=0; j<selectedKeys.Count; j++)
			{
				if(i==selectedKeys[j])
					keyInfo[i].isKeyed = false;
					keyInfo[i].isPectKey = false;  //Mahmoud 4.4.14
			}			
		}
		
		InitFrameSelection();
		updateFrameDisplay();
		UpdateKeyInfoList();
	}
	
	private void pasteKeys()
	{
		int diff = Mathf.Abs (startFrame - copiedKeys[0]);
		switch(checkFrames())
		{
			case 1:	
				WarningSystem.addWarning("Paste frames",  "Select keyframes with Shift key \tand click 'copy' button before paste it", Code.Info);
				return;
			case 2:
				WarningSystem.addWarning("Paste frames",  "Not enough space(free frames) to paste copied frames", Code.Info);
				return;
			default:				
				break;
		}
		
		//paste frames		

		for(int i=0; i<keyInfo.Count; i++)
			for (int j=0; j<copiedKeys.Count; j++)
				if(i == copiedKeys[j] && keyInfo[i].isKeyed)
				{
					//currentFrame = diff+i;
					//setKey ();
					duplicateKeys(i,diff+i);
				}

		InitFrameSelection();
		UpdateKeyInfoList();
		
	}
	
	void saveKeyframeData()
	{
		
		XmlSerializer serializer = new XmlSerializer(typeof(List<KeyframeInfo>));
		//string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
  		TextWriter textWriter = new StreamWriter(@".\test.xml");
  		serializer.Serialize(textWriter, keyInfo);
  		textWriter.Close();
	}
	
	void loadKeyframeData()
	{
		XmlSerializer deserializer = new XmlSerializer(typeof(List<KeyframeInfo>));
  		TextReader textReader = new StreamReader(@".\test.xml");
   		List<KeyframeInfo> newKeys;
   		newKeys = (List<KeyframeInfo>)deserializer.Deserialize(textReader);
   		textReader.Close();
		
		keyInfo = newKeys;
		updateFrameDisplay();
   	}
	
	// When new data is loaded in, rebuild the frames and the text labels over the timeline
	// to appropriate size
	public void respawnWithFrameCount(int count)
	{
		
		UpdateKeyInfoList();
		
		if(count == 0)
			count = 55;
		
		if(hasBeenResized == false)
		{
			hasBeenResized = true;		
		}
		
		//Updated by Chengde to reserve keyframe data when changing total number of frames
		ArrayList oldKeys = new ArrayList();
		List<KeyframeInfo> oldKeyInfo = new List<KeyframeInfo>();
		/////////////// 12.09.2013 //////////////////////
		
		if(keys != null)
		{
			//saveKeyframeData()
			
			//Updated by CHENGDE to reserve keyframe data when changing total number of frames
			for(int i = 0; i < keys.Count; i++)
				oldKeys.Add(keys[i]);					
			/////////////// 12.09.2013 //////////////////////
		
			maxScrollLength = count;
			for(int i = 0; i < keys.Count; i++)
			{				
				keys[i] = null;
			}
		}
		
		//Updated by CHENGDE to reserve keyframe data when changing total number of frames
		if(keyInfo.Count > 0)
			for(int i=0; i<keyInfo.Count; i++)
				oldKeyInfo.Add (keyInfo[i]);
		/////////////// 12.09.2013 //////////////////////
		
		keyInfo = new List<KeyframeInfo>(count);
		
		keys = new ArrayList();
		for(int i = 0; i < count; i++)
		{
			KeyframeTexture keyTexture = new KeyframeTexture();
			keyTexture.x = i * 8;
			keyTexture.y = 20;
			if(i % 5 == 0)
				keyTexture.isMultipleFive = true;
			
			keys.Add(keyTexture);
			keyInfo.Add(new KeyframeInfo());
		}
		
		
		currentFrame = 1;
		labelBar = new LabelBarTexture(count);
		
		keyInfo[0] = new KeyframeInfo();
		keyInfo[0].position(new Vector3(150f, 50f, 0f));
		keyInfo[0].rotation(new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
		keyInfo[0].isKeyed = true;
		keyInfo[count-1 ] = new KeyframeInfo();
		keyInfo[count-1].position(new Vector3(0f, 0f, 0f));
		keyInfo[count-1].rotation(new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
		keyInfo[count-1].isKeyed = true;
		
		//Updated by CHENGDE to reserve keyframe data when changing total number of frames
		if(keys != null)
		{
			int keyInfoCnt = (oldKeyInfo.Count<keyInfo.Count)?oldKeyInfo.Count:keyInfo.Count;
			for(int i=0; i<keyInfoCnt; i++)
				keyInfo[i] = oldKeyInfo[i];	
			
			int keyCnt = (oldKeys.Count<keys.Count)? oldKeys.Count: keys.Count;
			for(int i=0; i<keyCnt; i++)
				keys[i] = oldKeys[i];
		}
		/////////////// 12.09.2013 //////////////////////
		
		
		gotoFrame(1);
		updateFrameDisplay();
		//Debug.Log("Rebuilding the Keyframebar took: " + (startTime - endTime) + " ms");
	}
	
	void Update() 
	{
	}
	
	// Update is called once per frame
	void OnGUI()
	{
		GUI.depth = 2;
		if(isGuiVisible){
			GUI.skin =  GuiManager.GetSkin();
		scrollViewVector = GUI.BeginScrollView(containingRect, scrollViewVector, new Rect (0, 0, maxScrollLength * 8, 36));
		
		foreach(KeyframeTexture key in keys)
		{
			key.draw();			
		}
		//GUI.Label(new Rect(0, 0, 200, 200), "150");
		GUI.DrawTexture(new Rect(0,0, 12 * maxScrollLength, 20), (Texture)Resources.Load("keyframeBarTop"));
		labelBar.draw();
		
		
		// End the ScrollView
		GUI.EndScrollView();
		
		if (Event.current.type == EventType.MouseDown)
            OnMouseClick();
		
		if (GUI.Button(new Rect((Screen.width / 2) + 250 + 30, Screen.height - 76, 90, 30), "Set Key"))
            setKey();
		if (GUI.Button(new Rect((Screen.width / 2) + 250 + 130, Screen.height - 76, 90, 30), "Delete Key"))
            deleteKey();
		if (GUI.Button(new Rect((Screen.width / 2) + 250 + 30, Screen.height - 36, 90, 30), "Copy Keys"))
            copyKeys();
		if (GUI.Button(new Rect((Screen.width / 2) + 250 + 130, Screen.height - 36, 90, 30), "Paste Keys"))
            pasteKeys();
			
	//	if (GUI.Button(new Rect((Screen.width / 2) + 250 + 175, Screen.height - 240, 45, 30), "Stop"))
    //        StopPosition();
			
		GUI.Box (new Rect ((Screen.width / 2) + 255 + 20,Screen.height - 290,195,85), "Pectoral Auto Movement");
		
		GUI.Label(new Rect(new Rect((Screen.width / 2) + 250 + 35, Screen.height - 265, 35, 35)), "Start");
		FirstKey = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 35, Screen.height - 240, 40, 25), FirstKey);	
		
		GUI.Label(new Rect(new Rect((Screen.width / 2) + 250 + 85, Screen.height - 265, 35, 35)), "End");
		LastKey = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 85, Screen.height - 240, 40, 25), LastKey);
		
		GUI.Label(new Rect(new Rect((Screen.width / 2) + 250 + 135, Screen.height - 265, 35, 35)), "F/C");
		Pectmove = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 135, Screen.height - 240, 40, 25), Pectmove);

			
			
			if (GUI.Button(new Rect((Screen.width / 2) + 250 + 180, Screen.height - 240, 30, 25), "Go!"))
			PectAutoMove ();
		
			//Pectmove = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 130, Screen.height - 240, 35, 30), Pectmove);
			//FirstKey = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 130, Screen.height - 240, 35, 30), FirstKey);
			//LastKey = GUI.TextArea(new Rect((Screen.width / 2) + 250 + 130, Screen.height - 240, 35, 30), LastKey);
		//Paste special box and toggles
			
		GUI.Box (new Rect ((Screen.width / 2) + 255 + 20,Screen.height - 206,195,130), "Paste Special");
			
		PositionToggle= GUI.Toggle (new Rect((Screen.width / 2) + 250 + 30,Screen.height - 180,20,20), PositionToggle,"Position");
		
		RotationToggle= GUI.Toggle (new Rect((Screen.width / 2) + 250 + 30,Screen.height - 157,20,20),RotationToggle,"Rotation");

		DorsalToggle= GUI.Toggle (new Rect((Screen.width / 2) + 250 + 30,Screen.height - 134,20,20),DorsalToggle,"Dorsal fin");
		
		OthersToggle= GUI.Toggle (new Rect((Screen.width / 2) + 250 + 30,Screen.height - 111,20,20),OthersToggle,"Others");
			
		//StopPositionToggle = GUI.Toggle (new Rect((Screen.width / 2) + 250 + 30,Screen.height - 220,20,20),StopPositionToggle,"Stop Position");
			
		//if (GUI.Button(new Rect((Screen.width / 2) + 250 + 120, Screen.height - 36, 90, 30), "Load Keys"))
        //    loadKeyframeData();
		}
	}
	
	private void onTurnOffGui()
	{
		isGuiVisible = false;	
	}
	private void onTurnOnGui()
	{
		isGuiVisible = true;	
	}
	
	private void onKeyframeLoaded(List<KeyframeInfo> newKeys)
	{
		Debug.Log("onKeyframeLoaded");
		if(newKeys.Count > 1)
		{
			Debug.Log("onKeyframeLoaded: Respawn with count: " + newKeys.Count);
			respawnWithFrameCount(newKeys.Count);
			keyInfo = newKeys;
		}else
		{
			//Populate it with empty stuff
			respawnWithFrameCount(45);
		}
		updateFrameDisplay();
	}
	public List<MoCapAnimData> buildPlaybackData()
	{
		List<MoCapAnimData> playbackList = new List<MoCapAnimData>();
		/*
		if(guiReadAnimation.selGridInt == 1){
			Debug.Log("goto keyframeRot transform");
			if(keyInfo[i].isKeyed)
			{
				keyedObject.transform.position = keyInfo[i].getPosition(); //new Vector3(keyInfo[i].tx, keyInfo[i].ty, keyInfo[i].tz); 
				keyedObject.transform.rotation = keyInfo[i].getRotation(); 
			}else
			{
				Vector3 newPosition;
				Quaternion newRotation;
				interpolateKeys(i, out newRotation, out newPosition);
				keyedObject.transform.position = newPosition;
				keyedObject.transform.rotation = newRotation;
			}
		}
		*/
		// Use this
		
		for(int i = 0; i < keyInfo.Count; i++)
		{
			if(keyInfo[i].isKeyed){
				//Debug.Log("Testing move");
				MoCapAnimData newKey;// = new MoCapAnimData();
				newKey.Position = keyInfo[i].getPosition();
				newKey.Rotation = keyInfo[i].getRotation().eulerAngles;
				newKey.QRotation = keyInfo[i].getRotation();
				newKey.LPecAmount = new Vector2(keyInfo[i].lpelvicAngles.x, keyInfo[i].lpelvicAngles.y);
				newKey.RPecAmount = new Vector2(keyInfo[i].rpelvicAngles.x, keyInfo[i].rpelvicAngles.y);
				newKey.LPectAmount = new Vector2(keyInfo[i].lpectAngles.x, keyInfo[i].lpectAngles.y);  // Mahmoud 02.17.14
				newKey.RPectAmount = new Vector2(keyInfo[i].rpectAngles.x, keyInfo[i].rpectAngles.y);  // Mahmoud 02.17.14
				newKey.AnalAmount = new Vector2(keyInfo[i].analAngles.x, keyInfo[i].analAngles.y);
				//newKey.LPectAmount = new Vector2(1f,1f);
				if(keyInfo[i].dorsalAngle != 0f)
					newKey.DorsalAmount = keyInfo[i].dorsalAngle;
				else
					newKey.DorsalAmount = 0.0f;
				playbackList.Add(newKey);
			}else
			{
				Vector3 newPosition;
				Quaternion newRotation;
				float dorsalAmount;
				Vector2 newLPelvic;
				Vector2 newRPelvic;
				Vector2 newAnal;
				Vector2 newLPect;
				Vector2 newRPect;
				interpolateKeys(i, out newRotation, out newPosition, out dorsalAmount, out newLPelvic, out newRPelvic, out newLPect, out newRPect, out newAnal);
				//keyedObject.transform.position = newPosition;
				//keyedObject.transform.rotation = newRotation;
				//Debug.Log("Blended Position: " + newPosition);
				MoCapAnimData newKey;// = new MoCapAnimData();
				newKey.Position = newPosition;
				newKey.Rotation = newRotation.eulerAngles;
				newKey.QRotation = newRotation;
				newKey.DorsalAmount = dorsalAmount;
				newKey.LPecAmount = newLPelvic;
				newKey.RPecAmount = newRPelvic;
				newKey.AnalAmount = newAnal;
				newKey.LPectAmount = newLPect; 				// Mohammed 02.12.14
				newKey.RPectAmount = newRPect; 				// Mohammed 02.12.14


				playbackList.Add(newKey);
			}
		}
		
		return playbackList;
	}
	
	private int startFrame=1;
	private int endFrame = -1;
	private List<int> selectedKeys = new List<int>();
	private List<int> copiedKeys;
	private bool ifCopied = false;
	
	public void OnMouseClick()
	{
		if(containingRect.Contains(Event.current.mousePosition))
		{
			Vector2 mosPos = new Vector2(Event.current.mousePosition.x- containingRect.x + scrollViewVector.x, Event.current.mousePosition.y - containingRect.y);
			Debug.Log("InsideTest");
			for(int i = 0; i < keys.Count; i++)
			{
				if((keys[i] as KeyframeTexture).contains(mosPos))
				{
					Event e = Event.current;
					if (e.shift) {
						//Only works one keyframes
						if(keyInfo[startFrame].isKeyed==false || keyInfo[i].isKeyed==false)
						{
							WarningSystem.addWarning("Warning",  "Only keyframes should be selected", Code.Info);
							startFrame = i;
							return;
						}
						if(startFrame > i)
						{
							WarningSystem.addWarning("Warning",  "Select frames from left to right", Code.Info);
							startFrame = i;
							break;
						}
						endFrame = i;	
						WarningSystem.addWarning("Select Frames",  "Frame " +startFrame+" to " + endFrame + " is selected.", Code.Info);
						
						
						selectedKeys.Clear ();
						int cnt = 0;
						foreach (KeyframeTexture key in keys)
						{
							if(cnt>=startFrame && cnt<=endFrame)
								selectedKeys.Add(cnt);

							cnt++;
						}
						
						startFrame = i;
					}
					else if(e.control)
					{
						//Only works one keyframes
						if(keyInfo[startFrame].isKeyed==false || keyInfo[i].isKeyed==false)
						{
							WarningSystem.addWarning("Select Frames",  "Only keyframes should be selected", Code.Info);
							startFrame = i;
							return;
						}
						
						int cnt = 0;
						foreach (KeyframeTexture key in keys)
						{
							if(cnt==i)
							{
								selectedKeys.Add(cnt);
								break;
							}
							cnt++;
						}
						
						startFrame = i;
						
					}
					else{
						selectedKeys.Clear ();
						selectedKeys.Add(i);

						startFrame =i;
						gotoFrame(i);
						Messenger<int>.Broadcast("SetTextureByFrame", i);
					}
					selectedKeys.Sort ();
					UpdateSelectedKeys();
					
				}
			}
		}
	}
	
	private void duplicateKeys(int nOriginal, int targetFrame)
	{

		keyInfo[targetFrame].isKeyed = true;

		Debug.Log(keyedObject.transform.position);

		
		if (PositionToggle == true){
			keyInfo[targetFrame].position(keyInfo[nOriginal].getPosition ());}
		
		if (RotationToggle == true) {
			keyInfo[targetFrame].rotation(keyInfo[nOriginal].getRotation ());}
		
		if (DorsalToggle == true) {
			keyInfo[targetFrame].dorsalAngle = keyInfo[nOriginal].dorsalAngle;}
		
		if (OthersToggle == true) {
			keyInfo[targetFrame].lpelvicAngles = keyInfo[nOriginal].lpelvicAngles;
			keyInfo[targetFrame].rpelvicAngles = keyInfo[nOriginal].rpelvicAngles;
			keyInfo[targetFrame].lpectAngles = keyInfo[nOriginal].lpectAngles;
			keyInfo[targetFrame].rpectAngles = keyInfo[nOriginal].rpectAngles;
			keyInfo[targetFrame].analAngles = keyInfo[nOriginal].analAngles;}
		
			
		updateFrameDisplay();
		//interpolateKeys (targetFrame);
		//gotoFrame(1);

		//needs to interpolate the keys here!!!!
			
		
	}
	
	
	/*
	private void duplicateKeys(int nOriginal, int targetFrame)
	{
		keyInfo[targetFrame] = new KeyframeInfo();
		keyInfo[targetFrame].isKeyed = true;
		keyInfo[targetFrame].position(keyInfo[targetFrame].getPosition ());
		keyInfo[targetFrame].rotation(keyInfo[nOriginal].getRotation ());
		keyInfo[targetFrame].dorsalAngle = keyInfo[nOriginal].dorsalAngle;
		keyInfo[targetFrame].lpelvicAngles = keyInfo[nOriginal].lpelvicAngles;
		keyInfo[targetFrame].rpelvicAngles = keyInfo[nOriginal].rpelvicAngles;
		keyInfo[targetFrame].analAngles = keyInfo[nOriginal].analAngles;
		
		updateFrameDisplay();
	}
	*/
	
	private int checkFrames()
	{
		if(ifCopied == false)
			return 1;

		int copiedFrameCnt = copiedKeys[copiedKeys.Count -1] - copiedKeys[0] + 1;
		int freeFrameCnt = keys.Count - startFrame;
		if(freeFrameCnt<copiedFrameCnt)
			return 2;
		
		return 3;
	}
	
	private void copyKeys()
	{
		//UpdateKeyInfoList();
		
		if(selectedKeys.Count==0)
		{
			WarningSystem.addWarning("Copy Frames",  "No key frames to copy", Code.Info);
			return;
		}
		copiedKeys = new List<int>();
		for(int i=0; i<selectedKeys.Count; i++)
		{
			copiedKeys.Add (selectedKeys[i]);
		}
		ifCopied = true;
		WarningSystem.addWarning("Copy Frames",  "Selected frames have been copied", Code.Info);
	}
	
	private void InitFrameSelection()
	{
		selectedKeys.Clear ();
		ifCopied = false;
	}
	
	private void UpdateSelectedKeys()
	{
		for(int i = 0; i < keyInfo.Count; i++)
		{
			KeyframeTexture key = (KeyframeTexture)keys[i];
			key.isSelected = false;
			for(int j=0; j<selectedKeys.Count; j++)
			{
				if (i==selectedKeys[j])
					key.isSelected = true;
			}
		}
	}
	/*
	private void StopPosition ()
	{
		
		// Controls the fin rotation for a Stop Position 
		
			keyInfo[currentFrame] = new KeyframeInfo();
			keyInfo[currentFrame].isKeyed = true;
			
			keyInfo[currentFrame].position(keyedObject.transform.position);
			keyInfo[currentFrame].rotation(keyedObject.transform.rotation);
			keyInfo[currentFrame].dorsalAngle = 0;
			keyInfo[currentFrame].lpelvicAngles = new Vector2 (-25.0f,-8.0f);
			keyInfo[currentFrame].rpelvicAngles = new Vector2(-8.0f,-25.0f);
			keyInfo[currentFrame].lpectAngles = new Vector2(-15.0f,-15.0f);  //Mahmoud 02.17.14
			keyInfo[currentFrame].analAngles = new Vector2(0.1f,0.1f);

			keyInfo[currentFrame-1] = new KeyframeInfo();
			
			keyInfo[currentFrame-1].position(keyedObject.transform.position);
			keyInfo[currentFrame-1].rotation(keyedObject.transform.rotation);
			keyInfo[currentFrame-1].dorsalAngle = 0;
			keyInfo[currentFrame-1].lpelvicAngles = new Vector2 (0.0f,0.0f);
			keyInfo[currentFrame-1].rpelvicAngles = new Vector2(0.0f,0.0f);
			keyInfo[currentFrame-1].analAngles = new Vector2(0.1f,0.1f);
		
		updateFrameDisplay();
		UpdateKeyInfoList();
			
	}
	*/
	/* PectAytoMove()
	 		The function controls the cycle movement of the pectoral fins. It provides several
	 		user defined parameters such as the cycle speed and ...
	 		*/
	
	private void PectAutoMove ()       // Mahmoud 3.4.2014
	{
		int j = int.Parse(FirstKey);
		int k = int.Parse(LastKey);   //gets a user deifined value for the speed of the movement
		int l = int.Parse(Pectmove);   //gets a user deifined value for the speed of the movement
		
		for(int i=j; i+(l-1)<k; i+=l)
		{
			pectautomove(i,l);        // calls this function
		}
		
		
		InitFrameSelection();
		updateFrameDisplay();
		UpdateKeyInfoList();

	}
	
	private void pectautomove(int i, int l)     // Mahmoud 3.4.2014
	{
		if (keyInfo[i].isKeyed == true)      //the only reason of the If functions is to avoid overwritting
			                                 // of the previous key texture (red) with the pectkey (green)
		{	
			keyInfo[i].isKeyed = true;
			//keyInfo[i].isPectKey = true ;
			keyInfo[i].lpectAngles = new Vector2(0.0f,0.0f);       //pectoral fin in closed position
			keyInfo[i].rpectAngles = keyInfo[i].lpectAngles;
			
			if (keyInfo[i+(l-1)].isKeyed == true)
			{
				keyInfo[i+(l-1)].isKeyed = true;
				//keyInfo[i+(j-1)].isPectKey = true ;
				keyInfo[i+(l-1)].lpectAngles = new Vector2(0.0f,40.0f);        //pectoral fin in open position this value controls the max opening of the fin
				keyInfo[i+(l-1)].rpectAngles = keyInfo[i+(l-1)].lpectAngles;
			}
			else 
			{
				keyInfo[i+(l-1)].isKeyed = true;
				keyInfo[i+(l-1)].isPectKey = true ;
				keyInfo[i+(l-1)].lpectAngles = new Vector2(0.0f,40.0f);
				keyInfo[i+(l-1)].rpectAngles = keyInfo[i+(l-1)].lpectAngles;
			}
		}
		else
		{
			keyInfo[i].isKeyed = true;
			keyInfo[i].isPectKey = true ;
			keyInfo[i].lpectAngles = new Vector2(0.0f,0.0f);
			keyInfo[i].rpectAngles = keyInfo[i].lpectAngles;
		
			if (keyInfo[i+(l-1)].isKeyed == true)
			{
				keyInfo[i+(l-1)].isKeyed = true;
				//keyInfo[i+(j-1)].isPectKey = true ;
				keyInfo[i+(l-1)].lpectAngles = new Vector2(0.0f,40.0f);
				keyInfo[i+(l-1)].rpectAngles = keyInfo[i+(l-1)].lpectAngles;
			}
			else 
			{
				keyInfo[i+(l-1)].isKeyed = true;
				keyInfo[i+(l-1)].isPectKey = true ;
				keyInfo[i+(l-1)].lpectAngles = new Vector2(0.0f,40.0f);
				keyInfo[i+(l-1)].rpectAngles = keyInfo[i+(l-1)].lpectAngles;
			}
		}
		
	}
	
	
	
	
	private void UpdateKeyInfoList()
	{
		for(int i=0; i<keyInfo.Count; i++)
		{
			
			Vector3 newPosition;
			Quaternion newRotation;
			float dorsalAmount;
			Vector2 newLPelvic;
			Vector2 newRPelvic;
			Vector2 newAnal;
			Vector2 newLPect;   //Mahmoud
			Vector2 newRPect;   //Mahmoud
			if(keyInfo[i].isKeyed ==false)
			{
				interpolateKeys(i, out newRotation, out newPosition, out dorsalAmount, out newLPelvic, out newRPelvic, out newRPect, out newLPect, out newAnal);			
				keyInfo[i].position (newPosition);	
				keyInfo[i].rotation (newRotation);	
				
				keyInfo[i].dorsalAngle  = dorsalAmount;
				
				keyInfo[i].lpelvicAngles.x = newLPelvic.x;
				keyInfo[i].lpelvicAngles.y = newLPelvic.y;
				
				keyInfo[i].rpelvicAngles.x = newRPelvic.x;
				keyInfo[i].rpelvicAngles.y = newRPelvic.y;
				
				keyInfo[i].rpectAngles.x = newRPect.x;        //Mahmoud
				keyInfo[i].rpelvicAngles.y = newRPect.y;      //Mahmoud
				
				keyInfo[i].rpectAngles.x = newRPect.x;        //Mahmoud
				keyInfo[i].rpectAngles.y = newRPect.y;        //Mahmoud
				
			}
		}
	}
}
