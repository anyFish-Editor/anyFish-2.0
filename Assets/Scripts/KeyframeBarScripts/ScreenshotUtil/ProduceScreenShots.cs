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
using System.IO;

public class ProduceScreenShots : MonoBehaviour {
	
	private static string folder = "benthic01";
	
	static int frameRate = 25;
	// Use this for initialization
	private static bool isRecordingActive = false;
	
	private static int startFrameCount = 1;
	
	void Awake()
	{
		//newFolderName(folder);
	}
	public static void newFolderName(string folderName)
	{
		folder = folderName;
		System.IO.Directory.CreateDirectory(PlayerPrefs.GetString("ProjectPath") + "\\" + folder);
		reset();
	}
	
	public static void reset(){
		startFrameCount = 1;	
	}
	public static void TakePic () {
		//gets the current position of the main camera
		Debug.Log("Take Pick");


		//Make a path from relative path to solid path (Mohammad)
		string othTry = PlayerPrefs.GetString("ProjectPath");
		string imagePath = PlayerPrefs.GetString("ProjectPath") + "\\" + folder;
		string imagePathFull = Path.GetFullPath(imagePath);
		
		
    	// Set the playback framerate! (real time doesn't influence time anymore)
		//var name2 = string.Format("{0}/{1:D05}shot.bmp", relative, startFrameCount );
		var name = string.Format("{0}/{1:D05}shot.bmp", imagePathFull, startFrameCount ); //Mohamamd
    	//var name = string.Format("{0}/{1:D05}shot.bmp", PlayerPrefs.GetString("ProjectPath") + "\\" + folder, startFrameCount ); //I changed this line to the above line (Mohammad)
		startFrameCount++;
    	// Capture the screenshot
	    Application.CaptureScreenshot (name);
	}
}


































