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
//this class handles the object insertion while its on the insert mode in the level editor
public class InsertObject : MonoBehaviour
{
	public Transform gridCollider;
	public Transform grid;
    public Transform root;
	
    private GameObject currGameObj;
	private bool isLeftMouseButtonPressed = false;
	private bool wasLeftMouseButtonPressed = false;
	
    void FixedUpdate()
    {
			isLeftMouseButtonPressed = Input.GetMouseButton(0);
			//gui resolution independece calculations
            Vector2 resMultiplier = new Vector2((float)1 * Screen.width / 1280, (float)1 * Screen.height / 800);
			//this is used to disable mouse inputs when the mouse is over level editor gui
			//this is used to make resolution independent rectangles to check if the mouse is over this rectangles
			//the base resolution in which this project was develop, was 1280x800.. you can change it to yours, remember to also change that in the resolution independence part on every OnGUI in this project
		
			//this are the rectangles used to check if the mouse are over certain parts of the editor gui, to allow proper operation on the dialogs
			//you can obtain their dimensions browsing in the LevelEditorGUI.cs file, follow the comments
            Rect mainMenuRect = new Rect(0 * resMultiplier.x, 0 * resMultiplier.y, 1030 * resMultiplier.x, 50 * resMultiplier.y);
            Rect objLibRect = new Rect(1030 * resMultiplier.x, 20 * resMultiplier.y, 240 * resMultiplier.x, 580 * resMultiplier.y);

            if (GameManager.edModes == GameManager.EditorModes.INSERT)
            {
				//activate the grid collider during insert mode
                gridCollider.gameObject.active = true;
				
				//if left mouse button is pressed and its not on top of the disabled mouse input zones, instantiate the selected object to the level
                if (!wasLeftMouseButtonPressed && isLeftMouseButtonPressed)
                {
					//this retrieves the mouse position and then inverts the Y axis so the top of the screen is coordinate 0
					Vector2 transformedMousePos = (Vector2)Input.mousePosition;
					transformedMousePos.y = -(transformedMousePos.y + (-(float)Screen.height));
                    if (!objLibRect.Contains(transformedMousePos) && !mainMenuRect.Contains(transformedMousePos) && !GameManager.isLoadDialogActive && !GameManager.isSaveDialogActive)
                    {
					
						//this converts the current mouse pointer screen coordinates to 3d world coordinates
						//but as mouse coordinates are 2d, you need to specify how deep the third coordinate need to be, in this case 10 units far away from the camera
            			Vector3 mouseToWorldProjection = Camera.main.transform.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
							
						//convert mouse pointer screen coordinates to 3d world coordinates and then use that to shoot a ray from there to 1000 units far away in straight direction
                        Ray mouseOverRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
						RaycastHit mouseOverHitinfo;
                        if (Physics.Raycast(mouseOverRay, out mouseOverHitinfo, 1000f))
                        {
							if (GameManager.currentObjType != null && GameManager.currentObjType != "") 
							{
								Vector3 insertPoint = mouseOverHitinfo.point;
								if(GameManager.lockToGrid)
								{
									insertPoint.y = Mathf.Round(insertPoint.y);
									insertPoint.x = Mathf.Round(insertPoint.x);
									insertPoint.z = Mathf.Round(insertPoint.z);
								}
								//instantiate the object where the ray hits an object and use the hit point as position of this new object
								if(GameManager.levelObjects.TryGetValue(GameManager.currentObjType, out currGameObj))
								{
									currGameObj = (GameObject)GameObject.Instantiate(currGameObj, insertPoint, Quaternion.identity);
								    currGameObj.transform.parent = root;
									currGameObj.name = GameManager.levelObjects[GameManager.currentObjType].name;	
								}	                     
							}
                        }
                        else
                        {
							if (GameManager.currentObjType != null && GameManager.currentObjType != "")
							{
								Vector3 insertPoint = mouseToWorldProjection;
								if(GameManager.lockToGrid)
								{
									insertPoint.y = Mathf.Round(insertPoint.y);
									insertPoint.x = Mathf.Round(insertPoint.x);
									insertPoint.z = Mathf.Round(insertPoint.z);
								}
								if (GameManager.levelObjects.TryGetValue(GameManager.currentObjType, out currGameObj)) 
								{
									//if the ray doesnt hit anything, create the object in mid air with the previously obtained mouse to world coordinate conversion
									currGameObj = (GameObject)GameObject.Instantiate(currGameObj, insertPoint, Quaternion.identity);
									currGameObj.transform.parent = root;
									currGameObj.name = GameManager.levelObjects[GameManager.currentObjType].name;
								}
							}
                        }
                    }
                }
            }
            else
            {
				//leaving insert mode, disable grid collider
                gridCollider.gameObject.active = false;
            } 
		
		wasLeftMouseButtonPressed = isLeftMouseButtonPressed;
    }
}