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
using System.IO;
using System.Collections.Generic;

//this static class serves as a global storage place to store the different states of things of the project, basically they handle settings used by the editor

public class GameManager
{
	//this enum is used to find out which editor mode is currently active
	public enum EditorModes { INSERT = 0, MOVE = 1, ROTATE = 2 }
    static public EditorModes edModes = EditorModes.ROTATE;
	//this enum is used to know which of the axes of the transform gizmo is currently active
    public enum GizmoAxes { X = 0, Y = 1, Z = 2, NONE = 3 }
    static public GizmoAxes axes = GizmoAxes.NONE;
	static public bool isEditor = false;
    static public Transform SelectedObject;
	//camPos along with selHitDistance are used to determine how far is the selected object from camera and thus move it faster if its far away or slower if its very close to the camera
    static public Transform camPos;
	static public Ray mouseOverRay;
	static public float selHitDistance = 0f;
	static public bool lockToGrid = false;
    static public bool isGizmoActive = false;
    static public bool wasDragging = false;
    static public string coordEdX = "";
    static public string coordEdY = "";
    static public string coordEdZ = "";
	//this where the collection of available prefabs is stored after loading them
    static public Dictionary<string, GameObject> levelObjects;
    static public string currentLevel = "";
    static public DirectoryInfo levelsPath;
	//if you are using OSX this should be the folder where the levels are shipped with the game, defaults to the resource folder inside the app bundle
	static public DirectoryInfo osxbundledlevelspath;
    static public string levelName = "";
    static public string currentObjType = "";
    static public bool isLoadDialogActive = false;
    static public bool isSaveDialogActive = false;
    static public bool is2DMode = false;
	
	//IF YOU ARE USING MAC OSX YOU SHOULD CHANGE THIS 2 VARIABLES TO THE NAME OF YOUR COMPANY AND PROJECT, ITS IMPORTANT
	static public string companyName = "TAMU";
	static public string productName = "anyFish";
}

