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

public class KeyframeTexture {
	private Texture keyOn;
	private Texture keyOff;
	private Texture keySelected;
	private Texture keyOffFive;
	private Texture KeyPectmove;   // Mahmoud 3.4.14
	private Rect textureSize;
	
	public int x { get; set; }
	public int y { get; set; }
	public bool isKeyed { get; set; }
	public bool isPectKey { get; set; }   //Mahmoud 3.4.14
	public bool isMultipleFive {get; set;}
	public bool isSelected {get; set;}
	
	// isKeyed means that there is associated keyframe information translation/rotation
	// isMultiple of Five means that there is an alternate image used for visual readability
	// isSelected uses a blue image to display the current 'head' of the timeline scrubber.
	
	
	public KeyframeTexture()
	{
		x = 0;
		y = 0;
		isKeyed = false;
		isPectKey = false;   //Mahmoud 3.4.14
		isMultipleFive = false;
		isSelected = false;
		keyOffFive = (Texture)Resources.Load("keyframeMFive");
		keyOn = (Texture)Resources.Load("keyframeKeyed");
		keyOff = (Texture)Resources.Load("keyframeEmpty");
		keySelected = (Texture)Resources.Load("keyframeSelected");
		KeyPectmove = (Texture)Resources.Load("keyframePectmove");      //Mahmoud 3.4.14
	}
	
	public void OnMouseDown()
	{
		Debug.Log("TEST MOUSE DOWN");	
	}
	
	public void draw()
	{
		textureSize = new Rect(x, y, 8, 16);
		
		if(isSelected)
			GUI.DrawTexture(textureSize, keySelected);
		else if(isPectKey)
			GUI.DrawTexture(textureSize, KeyPectmove);
		else if(isKeyed)
			GUI.DrawTexture(textureSize, keyOn);
		else if(isMultipleFive)
			GUI.DrawTexture(textureSize, keyOffFive);
		else
			GUI.DrawTexture(textureSize, keyOff);
		
	}
	
	// When a keyframeTexture contains the Vector2 position we've been clicked
	// this method is necessary as opposed to the GUI onMouse because of using the 
	// drawTexture methods to display the graphics. DrawTextures don't report clicks
	public bool contains(Vector2 position)
	{
		if(textureSize.Contains(position))
		{
			isSelected = true;
			return true;
		}
		else 
		{
			isSelected = false;
			return false;
		}
		
	}
}
