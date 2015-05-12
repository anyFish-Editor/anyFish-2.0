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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveStateInfo : ISerializable {
	public string pathToFrontFrame;
	public string pathToTopFrame;
	public string pathToMocapData;
	
	// Use this for initialization
	public SaveStateInfo()
	{
		pathToTopFrame = null;
		pathToFrontFrame = null;
		pathToMocapData = null;
	}
	
	void Start () {
		
	}
	
	
	public SaveStateInfo(SerializationInfo info, StreamingContext ctxt)
	{
		this.pathToFrontFrame = (string)info.GetValue("pTFF", typeof(string));
		this.pathToTopFrame = (string)info.GetValue("pTTF", typeof(string));
		this.pathToFrontFrame = (string)info.GetValue("pTMD", typeof(string));
   	}
	
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
		info.AddValue("pTFF", this.pathToFrontFrame);
		info.AddValue("pTTF", this.pathToTopFrame);
		info.AddValue("pTMD", this.pathToMocapData);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
