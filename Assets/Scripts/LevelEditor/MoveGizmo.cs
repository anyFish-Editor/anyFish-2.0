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

//this class handles the mechanism behind the move transform gizmo
public class MoveGizmo : MonoBehaviour
{
    public Transform[] mGizmo;

    private Vector2 mouseDelta = Vector2.zero;
    private Vector3 currentMousePos = Vector3.zero;
    private Vector3 lastMousePos = Vector3.zero;
    private Vector3 camAndObjPos = Vector3.zero;
    private bool negX = false;
    private bool negZ= false;
	private int frameCounter = 0;
	private float timeDelta;

    void Update()
    {
			//checks how much the mouse have moved since the last update
            currentMousePos = Input.mousePosition;
            mouseDelta = new Vector2(currentMousePos.x - lastMousePos.x, currentMousePos.y - lastMousePos.y);
			timeDelta = Time.fixedDeltaTime;
            //check where its the camera in reference to the gizmo
			//as the object movement through the gizmo is obtained via the mouse screen coordinates
			//and as mouse coordinates are 2d and the view is 3d, the direction and projection changes depending from what angle are you looking it
			//this code switches the directions of the movement depending if the camera is on positive or negative world coordinates
            if (GameManager.SelectedObject != null)
            {
                camAndObjPos = GameManager.camPos.position - GameManager.SelectedObject.transform.position;
                if (camAndObjPos.z > 0)
                {
                    negX = false;
                }
                else
                {
                    negX = true;
                }

                if (camAndObjPos.x > 0)
                {
                    negZ = false;
                }
                else
                {
                    negZ = true;
                }
            }////--
					
            ActivateDeactivateGizmo();
		
			Vector3 tempObjPos = Vector3.zero;
			Vector3 objMovement = Vector3.zero;

            if (GameManager.isGizmoActive)
            {
                if (GameManager.edModes == GameManager.EditorModes.MOVE)
                {
                    if (GameManager.SelectedObject != null) 
					{
                    	tempObjPos = GameManager.SelectedObject.position;
	                    //check if the mouse is draggin instead of just normal selecting
	                    if (GameManager.wasDragging)
	                    {	
							
	                    // if so , handle the object movement one axis at a time
							Vector2 resMultiplier = new Vector2((float)1 * Screen.width / 1280, (float)1 * Screen.height / 800);
							float translationSpeed = 0.003f;
						
							// a weird hacky way to make this part of the code only execute every 6 frames
							//this is done in order to be able to get decent performance on the lock to grid feature
							//this is because the move gizmo works with mouse delta instead of mouse projection
							//but to date i havent found a stable and reliable way to implement 3D mouse projection gizmo movement
							if(GameManager.lockToGrid)
							{
								 if (frameCounter != 5)
		        				{
		           					if (frameCounter == 4)
		            				{
										float deltaMultiplier;

										deltaMultiplier = 28f;
									
										//this is to limit extreme movement of the mouse, it kills movement spikes, and keeps it at a more reasonable level
										mouseDelta.x = Mathf.Clamp(mouseDelta.x,-3f, 3f);
										mouseDelta.y = Mathf.Clamp(mouseDelta.y,-3f, 3f);

										if (GameManager.axes == GameManager.GizmoAxes.X)
		                        		{		
											if (!negX)
		                            		{
		                                		objMovement.x = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier)));
		                            		}
		                            		else
		                            		{
		                                		objMovement.x = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier));
		                            		} 	                            
		                        		}
	                    
				                        if (GameManager.axes == GameManager.GizmoAxes.Y)
				                        {
				                            objMovement.y = ((mouseDelta.y * resMultiplier.y) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier));
				                        }
	                    
				                        if (GameManager.axes == GameManager.GizmoAxes.Z)
				                        {
						                    //check if its 2d mode, if so allow movement only to the proper direction according to the camera projection
						                    if(GameManager.is2DMode)
						                    {
							                    if (!negX)
												{
							                        objMovement.x = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier)));
							                    }
							                    else
							                    {
							                        objMovement.x = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier));
							                    }
							                     objMovement.y = ((mouseDelta.y * resMultiplier.y) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier));
						                    }
						                    else
						                    {
							                    if (negZ)
						                    	{
						                       		objMovement.z = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier)));
						                    	}
						                    	else
						                    	{
						                        	objMovement.z = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * (timeDelta * deltaMultiplier));
						                    	}  
						                    }                             
				                        }
		            				}
		            				frameCounter++;
		        				}
		        				else
		        				{
		            				frameCounter = 0;
		        				} 
							}
							else
							{
							
								if (GameManager.axes == GameManager.GizmoAxes.X)
		                        {		
									if (!negX)
		                            	{
		                                	objMovement.x = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta));
		                            	}
		                            	else
		                            	{
		                                	objMovement.x = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta);
		                            	} 	                            
		                        }
		                    
		                        if (GameManager.axes == GameManager.GizmoAxes.Y)
		                        {
		                            objMovement.y = ((mouseDelta.y * resMultiplier.y) * GameManager.selHitDistance) * (translationSpeed * timeDelta);
		                        }
		                    
		                        if (GameManager.axes == GameManager.GizmoAxes.Z)
		                        {
				                    //check if its 2d mode, if so allow movement only to the proper direction according to the camera projection
				                    if(GameManager.is2DMode)
				                    {
					                    if (!negX)
										{
					                        objMovement.x = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta));
					                    }
					                    else
					                    {
					                        objMovement.x = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta);
					                    }
					                     objMovement.y = ((mouseDelta.y * resMultiplier.y) * GameManager.selHitDistance) * (translationSpeed * timeDelta);
				                    }
				                    else
				                    {
					                    if (negZ)
				                    	{
				                       		objMovement.z = -(((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta));
				                    	}
				                    	else
				                    	{
				                        	objMovement.z = ((mouseDelta.x * resMultiplier.x) * GameManager.selHitDistance) * (translationSpeed * timeDelta);
				                    	}  
				                    }                             
		                        }
							}     
	                    }
	                    else
	                    {
	                        objMovement = Vector3.zero;
	                    }
					
					 	//updates on the fly the values of the coordinate editor
                        if (GameManager.axes != GameManager.GizmoAxes.NONE)
                        {
							//finally apply the combined movement to the object
	                    	tempObjPos += objMovement;
						
							if(GameManager.lockToGrid)
							{
								tempObjPos.x = Mathf.Round(tempObjPos.x);
								tempObjPos.y = Mathf.Round(tempObjPos.y);
								tempObjPos.z = Mathf.Round(tempObjPos.z);
							}							
					
							GameManager.SelectedObject.position = tempObjPos;
							tempObjPos = Vector3.zero;
							objMovement = Vector3.zero;
                            GameManager.coordEdX = GameManager.SelectedObject.position.x.ToString();
                            GameManager.coordEdY = GameManager.SelectedObject.position.y.ToString();
                            GameManager.coordEdZ = GameManager.SelectedObject.position.z.ToString();
                        }
						else
						{
							objMovement = Vector3.zero;
							tempObjPos = Vector3.zero;
						}
						
                    }
					else
					{
						objMovement = Vector3.zero;
						tempObjPos = Vector3.zero;
					}
                }
				else
				{
					objMovement = Vector3.zero;
					tempObjPos = Vector3.zero;
				}
                
            }
			else
			{
				objMovement = Vector3.zero;
				tempObjPos = Vector3.zero;
			}
	lastMousePos = currentMousePos;
    }
	
	//this handles the gizmo activation and deactivation according if theres a selected object or not, also choose the appropriate transform gizmo for 2d or 3d mode
    void ActivateDeactivateGizmo()
    {
		
		if(GameManager.is2DMode)
		{
			foreach (Transform child in mGizmo[0])
            {
                child.gameObject.active = false;
            }
            mGizmo[0].gameObject.active = false;
            mGizmo[0].position = Vector3.zero;
			
			//////////-------------------////////
			
	        if (GameManager.SelectedObject != null && GameManager.edModes == GameManager.EditorModes.MOVE) 
	        {
	            mGizmo[0].gameObject.active = true;
	            foreach (Transform child in mGizmo[1])
	            {
	                child.gameObject.active = true;
	            }
	            mGizmo[1].position = GameManager.SelectedObject.position;
	        }
	        else
	        {
	            foreach (Transform child in mGizmo[1])
	            {
	                child.gameObject.active = false;
	            }
	            mGizmo[1].gameObject.active = false;
	            mGizmo[1].position = Vector3.zero;
	        }
			
        //////////-------------------//////// 
			
		}
		else
		{
			foreach (Transform child in mGizmo[1])
            {
                child.gameObject.active = false;
            }
            mGizmo[1].gameObject.active = false;
            mGizmo[1].position = Vector3.zero;
			
			//////////-------------------////////
			
        if (GameManager.SelectedObject != null && GameManager.edModes == GameManager.EditorModes.MOVE) 
        {
            mGizmo[0].gameObject.active = true;
            foreach (Transform child in mGizmo[0])
            {
                child.gameObject.active = true;
            }
            mGizmo[0].position = GameManager.SelectedObject.position;
        }
        else
        {
            foreach (Transform child in mGizmo[0])
            {
                child.gameObject.active = false;
            }
            mGizmo[0].gameObject.active = false;
            mGizmo[0].position = Vector3.zero;
        }
        //////////-------------------//////// 
		}    
	}
}