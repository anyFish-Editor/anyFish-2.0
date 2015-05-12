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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class KeyframeInfo: ISerializable {
	public float tx, ty, tz;
	public Quaternion rot;
	public Boolean isKeyed = false;     
	public Boolean isPectKey = false;    //mahmoud 3.4.
	public float dorsalAngle = 0.0f;
	public Vector2 lpelvicAngles = new Vector2(0.1f, 0.1f); //Not zero to prevent divide by zero
	public Vector2 rpelvicAngles = new Vector2(0.1f, 0.1f);
	public Vector2 lpectAngles = new Vector2(0.1f, 0.1f);
	public Vector2 rpectAngles = new Vector2(0.1f, 0.1f);
	public Vector2 analAngles = new Vector2(0.1f, 0.1f);
		
	public KeyframeInfo()
	{
		rot = new Quaternion(0,0,0,0);
	}
	
	public KeyframeInfo(SerializationInfo info, StreamingContext ctxt)
	{
    	this.tx = (float)info.GetValue("Tx", typeof(float));
    	this.ty = (float)info.GetValue("Ty", typeof(float));
		this.tz = (float)info.GetValue("Tz", typeof(float));
		this.rot = (Quaternion)info.GetValue("Rot", typeof(Quaternion));
		this.dorsalAngle = (float)info.GetValue("DAng", typeof(float));
		this.lpelvicAngles = (Vector2)info.GetValue("lpelAng", typeof(Vector2));
		this.rpelvicAngles = (Vector2)info.GetValue("rpelAng", typeof(Vector2));
		this.lpectAngles = (Vector2)info.GetValue("rpectAng", typeof(Vector2));
		this.rpectAngles = (Vector2)info.GetValue("rpectAng", typeof(Vector2));
		this.analAngles = (Vector2)info.GetValue("analAng", typeof(Vector2));
		
   	}
	
	public void position(Vector3 pos)
	{
		tx = pos.x;
		ty = pos.y;
		tz = pos.z;
	}
	
	public Vector3 getPosition()
	{
		return new Vector3(tx, ty, tz);	
	}
	public Quaternion getRotation()
	{
		return rot;
	}
	public void rotation(Quaternion newRotation)
	{
		rot = newRotation;
	}
	
	public void ObjectToSerialize(SerializationInfo info, StreamingContext ctxt)
   {
    	//this.tx = info.GetValue("Tx", typeof(float));
		//this.ty = info.GetValue("Ty", typeof(float));
		//this.tz = info.GetValue("Tz", typeof(float));
		//this.rx = info.GetValue("Rx", typeof(float));
		//this.ry = info.GetValue("Ry", typeof(float));
		//this.rz = info.GetValue("Rz", typeof(float));
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
   {
    	info.AddValue("Tx", this.tx);
    	info.AddValue("Ty", this.ty);
		info.AddValue("Tz", this.tz);
		info.AddValue("Rot", this.rot);
		info.AddValue("DAng", this.dorsalAngle);
		info.AddValue("lpelAng", this.lpelvicAngles);
		info.AddValue("rpelAng", this.rpelvicAngles);
		info.AddValue("analAng", this.analAngles);
   }
}
