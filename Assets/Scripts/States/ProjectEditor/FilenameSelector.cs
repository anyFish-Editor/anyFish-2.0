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

public class FilenameSelector : MonoBehaviour {
	private List<string> fileNames;
	private List<bool> checkMarks;
	// Use this for initialization
	
	public void addItem(string aName)
	{
		if(fileNames == null)
		{			
			fileNames = new List<string>();
			checkMarks = new List<bool>();	
		}
		//Check if we have it already	
		if(!fileNames.Contains(aName))
		{
			// Add it	
			fileNames.Add(aName);
			checkMarks.Add(false); //Unchecked by default
		}
	}
	
	public void removeItem(string aName)
	{
		//Check if we have it already then remove
		if(fileNames.Contains(aName))
		{
			//Remove Logic	
		}else
		{
			WarningSystem.addWarning("Removal request failed", "Attempted to remove item that was a member of collection", Code.Warning);
		}
	}
	
	public bool hasActive()
	{
		bool hasActive = false;
		if(checkMarks != null)
		{
			foreach(bool check in checkMarks)
			{
				if(check == true)
					hasActive = true;
			}
		}
		
		return hasActive;
	}
	
	public List<string> getActive()
	{
		List<string> activeList = new List<string>();
		for(int i = 0; i < fileNames.Count; i++)
		{
			if(checkMarks[i] == true)
				activeList.Add(fileNames[i]);
		}
			
		return activeList;
	}
	public void render() {
		if(fileNames != null)
		{
			for(int i = 0; i < fileNames.Count; i++)
			{
				checkMarks[i] = GUI.Toggle(new Rect(10, 10+ 30*i, 20, 24), checkMarks[i], "");
				int nameStartIndex = fileNames[i].LastIndexOf(@"\") + 1;
				int lastIndex = fileNames[i].LastIndexOf(".");
				
				//Debug.Log("Start: " + nameStartIndex + ", End: " + lastIndex);
				GUI.Label(new Rect(35, 10 + 30*i, 100, 24), fileNames[i].Substring(nameStartIndex, lastIndex - nameStartIndex), "Label");
			}
		}
		
	}
}
