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
using System.Text.RegularExpressions;
//this class handles the selection mechanism which later the transform gizmos use to operate properly
public class SelectObject : MonoBehaviour
{
    private Transform mouseOverObject;
    private RaycastHit mouseOverHitinfo;
	private RaycastHit gizmoHitinfo;
    private bool isLeftMouseButtonPressed = false;
    private bool wasLeftMouseButtonPressed = false;
    private bool stillLeftMouseButtonPressed = false;
	private bool isRPressed = false;
	private bool wasRPressed = false;
    private int frameCounter = 0;
	private bool isOnObject = false;
	private bool isOnGizmo = false;
	
    void FixedUpdate()
    {
		//get the current state of R key
		//isRPressed = Input.GetKey(KeyCode.R);	
		
		//used to delete objects from the level
		if (GameManager.SelectedObject != null)
        {
			//the object to delete first is destroyed and then set to null
			//because all over the code is checked if the selected object is not null or if its null
            if (isRPressed && !wasRPressed)
            {
                Destroy(GameManager.SelectedObject.gameObject);
				GameManager.SelectedObject = null;
            }
        } 
		
		//this is used to disable mouse inputs when the mouse is over level editor gui
		//this is used to make resolution independent rectangles to check if the mouse is over this rectangles
		//the base resolution in which this project was develop, was 1280x800.. you can change it to yours, remember to also change that in the resolution independence part on every OnGUI in this project
        Vector2 resMultiplier = new Vector2((float)1 * Screen.width / 1280, (float)1 * Screen.height / 800);
		
		//this are the rectangles used to check if the mouse are over certain parts of the editor gui, to allow proper operation on the dialogs
		//you can obtain their dimensions browsing in the LevelEditorGUI.cs file, follow the comments
        Rect mainMenuRect = new Rect(0 * resMultiplier.x, 0f * resMultiplier.y, 1030 * resMultiplier.x, 50 * resMultiplier.y);
        Rect coordEdRect = new Rect(1120 * resMultiplier.x, 500f * resMultiplier.y, 150 * resMultiplier.x, 290 * resMultiplier.y);	
		
		//gets the main camera position, for later calculations at the time of selecting an object
        GameManager.camPos = Camera.mainCamera.transform;
		//Gets the mouse pointer screen coordinates
        isLeftMouseButtonPressed = Input.GetMouseButton(0);
		
		
        if (GameManager.edModes == GameManager.EditorModes.MOVE || GameManager.edModes == GameManager.EditorModes.ROTATE)
        {
			//this converts mouse coordinates to gui coordinates, by switching from bottom left to top left coordinates in the Y axis
			//this variable will be used later to switch the Y axis of the mouse coordinates
			Vector2 transformedMousePos = (Vector2)Input.mousePosition;
			transformedMousePos.y = -(transformedMousePos.y + (-(float)Screen.height));
			//////////-------------------////////
			//check the mouse is not over level editor gui
			//////////-------------------////////
            if (!coordEdRect.Contains(transformedMousePos) && !mainMenuRect.Contains(transformedMousePos) && !GameManager.isLoadDialogActive && !GameManager.isSaveDialogActive)
            {
				//convert mouse pointer screen coordinates to 3d world coordinates and then use that to shoot a ray from there to infinite units far away in straight direction
				//if that ray hits any object with a collider, it retrieves its infomation and stores it in mouseOverObject
                GameManager.mouseOverRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
				
				//retrieving the Gizmo Layer as explained in unity documentation
				//its set to ignore everything but the gizmo layer
				int gizmoLayerMask = 1 << 9;
				// inverting the layermask, so it ignores only the gizmo layer
				int  ObjectLayerMask = ~gizmoLayerMask;
				
				//shoot a ray to find out what object is under the mouse, but ignore the gizmo
                if (Physics.Raycast(GameManager.mouseOverRay, out mouseOverHitinfo, 9000f, ObjectLayerMask))
                {
					//set up the current object under the mouse and set to true the raycast hit success flag
                    mouseOverObject = mouseOverHitinfo.transform;
					isOnObject = true;
                }
                else
                {
					//otherwise clear the object under the mouse, but only do this if the user is not dragging
                    if (!GameManager.wasDragging)
                    {
						isOnObject = false;
                    	mouseOverObject = null;
                    }
                }
				
				//shoot a ray to know if the mouse is over any part of the gizmo ignoring everthing else, but make that check only if theres a currently selected object
				if(GameManager.SelectedObject != null)
				{
					if (Physics.Raycast (GameManager.mouseOverRay, out gizmoHitinfo, 9000f, gizmoLayerMask)) 
					{
						GameManager.isGizmoActive = true;
						isOnGizmo = true;
					} 
					else 
					{
						//if the mouse isnt over any gizmo part disable the gizmo, but only if the user is not dragging
						if(!GameManager.wasDragging)
						{
							GameManager.isGizmoActive = false;
							isOnGizmo = false;
						}
					}
				}
				else
				{
					//if the mouse isnt over any gizmo part disable the gizmo, but only if the user is not dragging
					if(!GameManager.wasDragging)
					{
						GameManager.isGizmoActive = false;
						isOnGizmo = false;
					}
				}
            
			
			//////////-------------------////////
            //checks if both raycasts are giving true, if its true, give priority to the gizmo to avoid command overlapping, 
			//to enable operation even when the gizmo is behind another objects
			if(isOnGizmo && isOnObject)
			{
				isOnObject = false;
			}
			
			//////////-------------------////////
			
			//straight forward, if theres no currently selected object, just disable the gizmo
            if (GameManager.SelectedObject == null)
            {
                GameManager.isGizmoActive = false;
            }

            //////////-------------------////////
			
			
            //////////-------------------////////
			//determine which axis of the gizmo active, but only if the gizmo is actually active
            if(GameManager.isGizmoActive)
			{
					
				//checks if the information given by the raycast is valid by checking if its not null
				if(gizmoHitinfo.transform != null)
				{
					//checks if the user is dragging, we dont want to switch axis in the middle of a move or rotation operation
					if (!GameManager.wasDragging) 
					{
						if (gizmoHitinfo.transform.name == "GizmoXAxis") 
						{
							GameManager.axes = GameManager.GizmoAxes.X;
						} 
						else if (gizmoHitinfo.transform.name == "GizmoYAxis") 
						{
							GameManager.axes = GameManager.GizmoAxes.Y;
						} 
						else if (gizmoHitinfo.transform.name == "GizmoZAxis") 
						{
							GameManager.axes = GameManager.GizmoAxes.Z;
						} 
						else 
						{
							//if the mouse is not over any valid part of the gizmo, just set to none
							GameManager.axes = GameManager.GizmoAxes.NONE;
						}
					}
				}
			}
            //////////-------------------////////
			
			//if a single click is received, select the object under the mouse
            if (isLeftMouseButtonPressed && !wasLeftMouseButtonPressed && !stillLeftMouseButtonPressed)
            {
				//check if the user isnt dragging any object
				if(!GameManager.wasDragging)
				{
					//checks if the click is made in middle of nowhere, and if so, clears out any selection
					if(!isOnGizmo && !isOnObject)
					{
						GameManager.SelectedObject = null;
					}
					
					//checks if both raycasts are giving true, if its true, give priority to the gizmo to avoid command overlapping, 
					//to enable operation even when the gizmo is behind another objects
					if(isOnGizmo && isOnObject)
					{
						isOnObject = false;
					}
					
					if(isOnObject)
					{
						//checks if the mouse is actually over an object
	               		if (mouseOverObject != null) 
						{
							//selectable objects should reside inside the Root GameObject, if the object doesnt have any parent, means its a no proper selectable object
		               		if (mouseOverObject.parent != null) 
							{
								//and if it does have a parent, check if its the right one
		               			if (mouseOverObject.parent.name == "Root" || mouseOverObject.parent.name == "AnyFishCore") 
								{
		               				//set the current selected object for later manipulation
		               				GameManager.SelectedObject = mouseOverObject;
		               				//set the distance between the object and the camera for later gizmo calculations
		               				GameManager.selHitDistance = mouseOverHitinfo.distance;
		               				
		               				//feed the selected object information to the coordinate editor
		               				if (GameManager.edModes == GameManager.EditorModes.MOVE) 
									{
										//if its move mode, feed object position to the coordinate editor
		               					GameManager.coordEdX = GameManager.SelectedObject.position.x.ToString ();
		               					GameManager.coordEdY = GameManager.SelectedObject.position.y.ToString ();
		               					GameManager.coordEdZ = GameManager.SelectedObject.position.z.ToString ();
		               				} 
									else if (GameManager.edModes == GameManager.EditorModes.ROTATE) 
									{
										//if its rotate mode feed  object rotation euler angles to the coordinate editor
		               					GameManager.coordEdX = GameManager.SelectedObject.rotation.eulerAngles.x.ToString ();
		               					GameManager.coordEdY = GameManager.SelectedObject.rotation.eulerAngles.y.ToString ();
		               					GameManager.coordEdZ = GameManager.SelectedObject.rotation.eulerAngles.z.ToString ();
		               					
		               					//if the editor its in rotate mode, make rotate gizmo adopt the same rotation of the selected object
		               					(GetComponent ("RotateGizmo") as RotateGizmo).rGizmo [0].transform.rotation = GameManager.SelectedObject.rotation;
									}
		               			}
	               			}
						}
					}					
				}
            }
			
			//////////-------------------////////
			
			//if the left mouse button has been pressed for 3 frames in a row, stablish that its currently dragging
            if (isLeftMouseButtonPressed && wasLeftMouseButtonPressed && stillLeftMouseButtonPressed)
            {
					
				//enable dragging only if there is a currently selected object
                if (GameManager.SelectedObject != null)
                {
						
					//also check if the gizmo is active
                    if (GameManager.isGizmoActive)
                    {
							//Debug.Log("GizmodActiveCheck");
						//and that the gizmo is in a proper axis mode
                        if (GameManager.axes != GameManager.GizmoAxes.NONE)
                        {
                            GameManager.wasDragging = true;
                        }          
                    }
                    else
                    {
                        GameManager.wasDragging = false;
                    }
                }
            }
            else
            {
                GameManager.wasDragging = false;
            }
			//disable the dragging otherwise
            //////////-------------------////////
			
        
		
				//////////-------------------////////
		        //this checks if the left mouse button has been pressed for at least 3 frames in a row
		        if (frameCounter != 5)
		        {
		            if (frameCounter == 2)
		            {
		                wasLeftMouseButtonPressed = isLeftMouseButtonPressed;
		            }
		            else if (frameCounter == 4)
		            {
		                stillLeftMouseButtonPressed = isLeftMouseButtonPressed;
		            }
		            frameCounter++;
		        }
		        else
		        {
		            frameCounter = 0;
		        } 
				
			}
        }
		
		//////////-------------------////////
		//this stores the previous state of the R key to enable one time pressing of the R key for object deletion
		wasRPressed = isRPressed;
    }
}