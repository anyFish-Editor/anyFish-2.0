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
//this is just an slightly modified version of the mouselook script that comes as standard asset with unity
//this scripts handles the camera panning on the 3d mode, while on 2d mode, the camera view angle is static


[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 7F;
	public float sensitivityY = 7F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationX = 0F;
	float rotationY = 0F;
	
	public GameObject followObject;
	
	private Quaternion startRotation;
	private Vector3 startPosition;
	Quaternion originalRotation;

	void Update ()
	{
		if(GameManager.is2DMode) 
		{
			transform.rotation = Quaternion.identity;
			transform.localRotation = Quaternion.identity;
		}
		
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.LeftAlt))
        {
            if(!GameManager.is2DMode) 
			{
            	if (axes == RotationAxes.MouseXAndY)
	            {
	            
	                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
	                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
	            
	                rotationX = ClampAngle(rotationX, minimumX, maximumX);
	                rotationY = ClampAngle(rotationY, minimumY, maximumY);
	            
	                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
	                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
	            
	                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
	            }
	            else if (axes == RotationAxes.MouseX)
	            {
	                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
	                rotationX = ClampAngle(rotationX, minimumX, maximumX);
	            
	                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
	                transform.localRotation = originalRotation * xQuaternion;
	            }
	            else
	            {
	                rotationY += Input.GetAxis("Mouse X") * sensitivityY;
	                rotationY = ClampAngle(rotationY, minimumY, maximumY);
	            
	                Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.back);
	                transform.localRotation = originalRotation * yQuaternion;
				}
            } 
        }
		/*
		if(Input.GetKey(KeyCode.Space) || Input.GetKey("1"))
		{
			rotationX = 0.0f;
			rotationY = 0.0f;
			gameObject.GetComponent<Camera>().orthographicSize = startSize;
			gameObject.transform.position = startPosition;
		}
		if(Input.GetKey("2"))
		{
			rotationX = 0.0f;
			rotationY = 0.0f;
			if(followObject != null)
			{
				Vector3 newPosition = new Vector3(followObject.transform.position.x, followObject.transform.position.y, gameObject.transform.position.z);
				gameObject.transform.position = newPosition;
				gameObject.GetComponent<Camera>().orthographicSize = 80;
			}
		}
		*/
	}
	
	void Start ()
	{
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}