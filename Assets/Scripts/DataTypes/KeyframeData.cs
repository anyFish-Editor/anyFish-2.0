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

public class KeyframeData {
	private Vector3 pos;
	private Vector3 rot;
	
	public KeyframeData()
	{
		
	}
	
	public KeyframeData(Vector3 newPosition, Vector3 newRotation)
	{
		pos = newPosition;
		rot = newRotation;
	}
	
	public Quaternion rotationAsQuaternion()
	{
		Quaternion returnQuat = Quaternion.identity;
		returnQuat.eulerAngles = rot;
		return returnQuat;
	}
	
	public Vector3 position
	{
		get{ return pos; }
		set{ pos = value; }
	}
	public Vector3 rotation
    {
        get { return rot; }
        set { rot = value; }
    }
}
