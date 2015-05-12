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

// this class manages the movement of the camera on the level editor

public class MoveCamera : MonoBehaviour
{
	public GUISkin currentSkin;
	//speed of the camera movement and boolean to store the result of the check if its 2d or 3d mode
    public float speed = 5;
	private bool was2d = false;
	private bool forward = false;
	private bool backward = false;
	private bool left = false;
	private bool right = false;
	private bool zoomIn = false;
	private bool zoomOut = false;
	
    void Update()
    {
		forward = Input.GetKey(KeyCode.W);
		backward = Input.GetKey(KeyCode.S);
		right = Input.GetKey(KeyCode.D);
		left = Input.GetKey(KeyCode.A);
		//zoomIn = Input.GetKey(KeyCode.Q);
		//zoomOut = Input.GetKey(KeyCode.E);
		
		// reset the camera position to 2d mode default position when entering 2d mode
		if(GameManager.is2DMode && !was2d)
		{
			Vector3 tempPos = transform.position;
			tempPos.z = -10f;
			transform.position = tempPos;		
		}
		
		//checks if the load and save dialog are not active
        if (!GameManager.isLoadDialogActive && !GameManager.isSaveDialogActive)
        {
			//2d mode camera controls
            if(GameManager.is2DMode)
			{				
            	if (forward)
	            {
	                transform.Translate(Vector3.up * (speed * Time.deltaTime));
	            }
	            
	            if (backward)
	            {
	                transform.Translate(Vector3.down * (speed * Time.deltaTime));
	            }
	            
	            if (left)
	            {
	                transform.Translate(Vector3.left * (speed * Time.deltaTime));
	            }
	            
	            if (right)
	            {
	                transform.Translate(Vector3.right * (speed * Time.deltaTime));
	            }
	            
	            if (zoomIn)
	            {
	                transform.Translate(Vector3.forward * (speed * Time.deltaTime));
	            }
	            
	            if (zoomOut)
	            {
	                transform.Translate(Vector3.back * (speed * Time.deltaTime));
				}
            } 
			
			//3d mode camera controls
			else
			{
				if (forward)
	            {
	                transform.Translate(Vector3.forward * (speed * Time.deltaTime));
	            }
	            
	           if (backward)
	            {
	                transform.Translate(Vector3.back * (speed * Time.deltaTime));
	            }
	            
	            if (left)
	            {
	                transform.Translate(Vector3.left * (speed * Time.deltaTime));
	            }
	            
	            if (right)
	            {
	                transform.Translate(Vector3.right * (speed * Time.deltaTime));
	            }
			}
        }
		
		//store the current level editor mode state for later use
		was2d = GameManager.is2DMode;
    }
}