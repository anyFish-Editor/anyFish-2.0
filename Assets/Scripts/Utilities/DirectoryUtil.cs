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
using System.IO;
using System;

public class DirectoryUtil : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Probably could have named this better, checks if the directory already exists, if it doesn't
	// creates one. The bool is optionally used to handle additional setup if files are expected.
	// For example if no project directory exists, recreate all needed xml files for saving states, etc.
	public static bool AssertDirectoryExistsOrRecreate(string directoryPath)
	{
		if(Directory.Exists(directoryPath))
			return true;
			
		Directory.CreateDirectory(directoryPath);
		return false;
	}
	
	public static DirectoryInfo[] getSubDirectoriesByParent(string directoryPath)
	{
		DirectoryInfo dir = new DirectoryInfo(directoryPath);
		DirectoryInfo[] subDirs = null;
		try
		{
			if(dir.Exists)
			{
				subDirs = dir.GetDirectories();				
				return(subDirs);	
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
			WarningSystem.addWarning("SubDirectory Error", "Error searching supplied path for subdirectories", Code.Error);
		}
		
		return subDirs;
	}
}
