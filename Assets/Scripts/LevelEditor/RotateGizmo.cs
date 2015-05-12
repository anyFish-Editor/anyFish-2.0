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
//this class handles the mechanism behind the rotate transform gizmo
public class RotateGizmo : MonoBehaviour
{
    public Transform[] rGizmo;
    private Vector2 mouseDelta = Vector2.zero;
    private Vector3 currentMousePos = Vector3.zero;
    private Vector3 lastMousePos = Vector3.zero;
	private float timeDelta;

    void Update()
    {

		//checks how much the mouse have moved since the last update
        currentMousePos = Input.mousePosition;
        mouseDelta = new Vector2(currentMousePos.x - lastMousePos.x, currentMousePos.y - lastMousePos.y);
        timeDelta = Time.fixedDeltaTime;
		Quaternion tempObjRot = Quaternion.identity;
    	Quaternion objRotation = Quaternion.identity;
        ActivateDeactivateGizmo();
		
        if (GameManager.isGizmoActive)
        {
			
            if (GameManager.edModes == GameManager.EditorModes.ROTATE)
            {                 
				if (GameManager.SelectedObject != null)
				{
					//check if the mouse is draggin instead of just normal selecting
					if (GameManager.wasDragging)
					{
						float translationSpeed = 0.003f;
						Vector2 resMultiplier = new Vector2((float)1 * Screen.width / 1280, (float)1 * Screen.height / 800);
						
						// if so , handle the object movement one axis at a time
	                   objRotation = GameManager.SelectedObject.rotation;
		
	                   if (GameManager.axes == GameManager.GizmoAxes.X)
	                   {
							if(!GameManager.is2DMode)
							{
								tempObjRot = Quaternion.AngleAxis(((mouseDelta.x * resMultiplier.x) * 6f) * (translationSpeed * timeDelta) * GameManager.selHitDistance, Vector3.right);
							}              
					    }
					
	                   if (GameManager.axes == GameManager.GizmoAxes.Y)
	                   {
							if(!GameManager.is2DMode)
							{
								tempObjRot = Quaternion.AngleAxis(((mouseDelta.x * resMultiplier.x) * 6f) * (translationSpeed * timeDelta) * GameManager.selHitDistance, Vector3.up);
							}                  
					   }
						//Z is the only axis which rotates in 2d mode
	                   if (GameManager.axes == GameManager.GizmoAxes.Z)
	                   {
	                       tempObjRot = Quaternion.AngleAxis((mouseDelta.x * 6f) * (translationSpeed * timeDelta) * GameManager.selHitDistance, Vector3.forward);
	                   }
						
						//updates on the fly the values of the coordinate editor
	                   if (GameManager.axes != GameManager.GizmoAxes.NONE)
	                   {
							//finally apply the combined rotation to the selected object
		                   	objRotation *= tempObjRot;
							
							if(GameManager.lockToGrid)
							{
								Vector3 eulerRot = objRotation.eulerAngles;
								eulerRot.x = Mathf.Round(objRotation.eulerAngles.x);
								eulerRot.y = Mathf.Round(objRotation.eulerAngles.y);
								eulerRot.z = Mathf.Round(objRotation.eulerAngles.z);
								objRotation.eulerAngles = eulerRot;
							}
							
		                   	GameManager.SelectedObject.rotation = objRotation;
							objRotation = Quaternion.identity;
							tempObjRot = Quaternion.identity;	
						    rGizmo[0].transform.rotation = GameManager.SelectedObject.rotation;
                           GameManager.coordEdX = GameManager.SelectedObject.rotation.eulerAngles.x.ToString();
                           GameManager.coordEdY = GameManager.SelectedObject.rotation.eulerAngles.y.ToString();
                           GameManager.coordEdZ = GameManager.SelectedObject.rotation.eulerAngles.z.ToString();
	                   }
	                   else
	                   {
	                       	objRotation = Quaternion.identity;
							tempObjRot = Quaternion.identity;
	                   }               
	                }
					else
	          		{
	                    objRotation = Quaternion.identity;
						tempObjRot = Quaternion.identity;
					}
				}
				else
	      		{
	                objRotation = Quaternion.identity;
					tempObjRot = Quaternion.identity;
				}
			}
			else
      		{
                objRotation = Quaternion.identity;
				tempObjRot = Quaternion.identity;
			}
		} 
		else
      	{
         	objRotation = Quaternion.identity;
			tempObjRot = Quaternion.identity;
		}
		lastMousePos = currentMousePos;
	}
	
	
	//this handles the gizmo activation and deactivation according if theres a selected object or not, also choose the appropriate transform gizmo for 2d or 3d mode
    void ActivateDeactivateGizmo()
    {
		//////////-------------------////////
		
		if(GameManager.is2DMode)
		{
			foreach (Transform child in rGizmo[0])
            {
                child.gameObject.active = false;
            }
            rGizmo[0].gameObject.active = false;
            rGizmo[0].position = Vector3.zero;
			
			//////////-------------------////////
			
        if (GameManager.SelectedObject != null && GameManager.edModes == GameManager.EditorModes.ROTATE) 
        {
            rGizmo[0].gameObject.active = true;
            foreach (Transform child in rGizmo[1])
            {
                child.gameObject.active = true;
            }
            rGizmo[1].position = GameManager.SelectedObject.position;
        }
        else
        {
            foreach (Transform child in rGizmo[1])
            {
                child.gameObject.active = false;
            }
            rGizmo[1].gameObject.active = false;
            rGizmo[1].position = Vector3.zero;
        }
			
        //////////-------------------//////// 
			
		}
		else
		{
			foreach (Transform child in rGizmo[1])
            {
                child.gameObject.active = false;
            }
            rGizmo[1].gameObject.active = false;
            rGizmo[1].position = Vector3.zero;
			
			//////////-------------------////////
			
        if (GameManager.SelectedObject != null && GameManager.edModes == GameManager.EditorModes.ROTATE) 
        {
            rGizmo[0].gameObject.active = true;
            foreach (Transform child in rGizmo[0])
            {
                child.gameObject.active = true;
            }
            rGizmo[0].position = GameManager.SelectedObject.position;
        }
        else
        {
            foreach (Transform child in rGizmo[0])
            {
                child.gameObject.active = false;
            }
            rGizmo[0].gameObject.active = false;
            rGizmo[0].position = Vector3.zero;
        }
        //////////-------------------//////// 
		}
    }
}