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
using System.Collections.Generic;
using System.IO;

//this class handles the level editor gui, which is the most complex gui in this project
public class LevelEditorGUI : MonoBehaviour
{

    public GUISkin[] currentSkins;
	//this array holds the thumbnails of every object in the editor object library, make sure to make the array the same lenght as the number of objects available in the library, otherwise out of range crashes will occur
    public Texture2D[] objectLibrayImages;
    public Transform root;
    private string[] fileNames;
    private string[] levelNames;
	private Texture2D[] buttonImgs;
    private string[] prefabListNames;
    private bool showHelp = false;
    private string tempLevelName = "";
	
    void Awake()
    {
		if(GameManager.levelObjects != null)
		{
			if(GameManager.levelObjects.Count != 0)
			{
				//this is a check to avoid accidental crashes at the time of setting the object library images from the editor inspector
				//checks if the array is smaller than the number of availabe objects and if it is, it fills the rest with blank textures to avoid an out of index crash
				//if the user just forgets to set up the images in the inspector, it creates an array of blank images of the same size of the available objects
				//this doesnt check if the image array is bigger, because bigger arrays dont cause an out of index crash, this is just to try to make it fool proof
				if(objectLibrayImages != null)
				{
					if(objectLibrayImages.Length == 0)
					{
						buttonImgs = new Texture2D[GameManager.levelObjects.Count];
					}
					else
					{
						if(objectLibrayImages.Length < GameManager.levelObjects.Count)
						{
							buttonImgs = new Texture2D[GameManager.levelObjects.Count];
							objectLibrayImages.CopyTo(buttonImgs,0);
						}
						else
						{
							buttonImgs = objectLibrayImages;
						}
					}
				}
				else
				{
					buttonImgs = new Texture2D[GameManager.levelObjects.Count];
				}
					
				//get the name of the objects in the object library, for the editor object list
				prefabListNames = new string[ GameManager.levelObjects.Count ];
				int i = 0;
				foreach (KeyValuePair<string, GameObject> pair in GameManager.levelObjects)
				{
					prefabListNames[i] = pair.Key;
					i++;
				}	
			}
			else
			{
				prefabListNames = null;
				buttonImgs = null;
			}
		}
		else
		{
			prefabListNames = null;
			buttonImgs = null;
		}	
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (showHelp)
            {
                showHelp = false;
            }
            else
            {
                showHelp = true;
            }
        }   
		
            if (Input.GetKeyDown(KeyCode.R))
            {
			//sets some initial values for the coordinate editor
                GameManager.edModes = GameManager.EditorModes.MOVE;
                if (GameManager.SelectedObject != null)
                {
                    GameManager.coordEdX = GameManager.SelectedObject.position.x.ToString();
                    GameManager.coordEdY = GameManager.SelectedObject.position.y.ToString();
                    GameManager.coordEdZ = GameManager.SelectedObject.position.z.ToString();
                }
                else
                {
                    GameManager.coordEdX = "0";
                    GameManager.coordEdY = "0";
                    GameManager.coordEdZ = "0";
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
			//sets some initial values for the coordinate editor
                GameManager.edModes = GameManager.EditorModes.ROTATE;
                if (GameManager.SelectedObject != null)
                {
                    GameManager.coordEdX = GameManager.SelectedObject.rotation.eulerAngles.x.ToString();
                    GameManager.coordEdY = GameManager.SelectedObject.rotation.eulerAngles.y.ToString();
                    GameManager.coordEdZ = GameManager.SelectedObject.rotation.eulerAngles.z.ToString();
                }
                else
                {
                    GameManager.coordEdX = "0";
                    GameManager.coordEdY = "0";
                    GameManager.coordEdZ = "0";
                }
            }
    }

    void OnGUI()
    {
			//this file is very long and may be confusing without proper separation and organization, so thats why each dialog and function of the GUI is separated by comments
		
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)1.0 * Screen.width / 1280, (float)1.0 * Screen.height / 800, 1.0f));
            GUI.skin = currentSkins[0];
            GUI.depth = 5;
            GUI.skin = currentSkins[1];
		
            
            GUI.skin = currentSkins[0];
		/*
			GUILayout.BeginArea(new Rect(10, 0, 1260, 60), "", GUI.skin.box);
            // MAIN MENU GUI  ----------------------------------------------
			// this the gui on top of the level editor which has the basic functions and modes of the editor
			/*
            if (GUI.Button(new Rect(0, 15, 65, 30), "NEW"))
            {
                GameManager.currentLevel = "";
                GameManager.levelName = "";
                if (root.transform.childCount != 0)
                {
                    foreach (Transform child in root)
                    {
                        Destroy(child.gameObject);
                    }
                }    
            }

            if (GUI.Button(new Rect(70, 15, 65, 30), "OPEN"))
            {
                GameManager.isLoadDialogActive = true;
                tempLevelName = GameManager.currentLevel;
            }

            if (GUI.Button(new Rect(140, 15, 65, 30), "SAVE"))
            {
                if (GameManager.currentLevel != "")
                {
                    (GetComponent("LevelLoader") as LevelLoader).SaveLevel();
                }
                else
                {
                    GameManager.isSaveDialogActive = true;
                }
            }

            if (GUI.Button(new Rect(210, 15, 90, 30), "SAVE AS"))
            {
                GameManager.levelName = "";
                GameManager.isSaveDialogActive = true;
            }

            if (GUI.Button(new Rect(305, 15, 65, 30), "EXIT"))
            {
                Application.LoadLevel("MainMenu");
            }
		
		 	if (GameManager.edModes == GameManager.EditorModes.MOVE || GameManager.edModes == GameManager.EditorModes.ROTATE) 
			{
		 		if (GUI.Button(new Rect(570, 15, 80, 30), "DELETE"))
		 		{
 		       		if (GameManager.SelectedObject != null)
 		    		{
		 				Destroy(GameManager.SelectedObject.gameObject);
		 				GameManager.SelectedObject = null;
		 			}
				}
		 	}

            if (GUI.Button(new Rect(655, 15, 80, 30), "INSERT"))
            {
                GameManager.edModes = GameManager.EditorModes.INSERT;          
            }

            if (GUI.Button(new Rect(740, 15, 65, 30), "MOVE"))
            {
			//sets some initial values for the coordinate editor
                GameManager.edModes = GameManager.EditorModes.MOVE;
                if (GameManager.SelectedObject != null)
                {
                    GameManager.coordEdX = GameManager.SelectedObject.position.x.ToString();
                    GameManager.coordEdY = GameManager.SelectedObject.position.y.ToString();
                    GameManager.coordEdZ = GameManager.SelectedObject.position.z.ToString();
                }
                else
                {
                    GameManager.coordEdX = "0";
                    GameManager.coordEdY = "0";
                    GameManager.coordEdZ = "0";
                }
            }

            if (GUI.Button(new Rect(810, 15, 80, 30), "ROTATE"))
            {
			//sets some initial values for the coordinate editor
                GameManager.edModes = GameManager.EditorModes.ROTATE;
                if (GameManager.SelectedObject != null)
                {
                    GameManager.coordEdX = GameManager.SelectedObject.rotation.eulerAngles.x.ToString();
                    GameManager.coordEdY = GameManager.SelectedObject.rotation.eulerAngles.y.ToString();
                    GameManager.coordEdZ = GameManager.SelectedObject.rotation.eulerAngles.z.ToString();
                }
                else
                {
                    GameManager.coordEdX = "0";
                    GameManager.coordEdY = "0";
                    GameManager.coordEdZ = "0";
                }
            }
		
			GameManager.is2DMode = GUI.Toggle(new Rect(375, 15, 75, 30),GameManager.is2DMode,"2D/3D");
			if(GameManager.edModes == GameManager.EditorModes.MOVE || GameManager.edModes == GameManager.EditorModes.INSERT ) 
			{
				GameManager.lockToGrid = GUI.Toggle(new Rect(455, 15, 115, 30),GameManager.lockToGrid,"Grid Snap");
			}
			else
			{
				GameManager.lockToGrid = GUI.Toggle(new Rect(455, 15, 115, 30),GameManager.lockToGrid,"Deg Snap");
			}
            GUI.Label(new Rect(895, 15, 150, 50), "MODE:" + GameManager.edModes.ToString()); 
            GUILayout.EndArea();
            
            */
		// MAIN MENU GUI END  ----------------------------------------------

            // OBJECT LIBRARY GUI  ----------------------------------------------
            if (GameManager.edModes == GameManager.EditorModes.INSERT)
            {
                GUILayout.BeginArea(new Rect(1030, 20, 240, 580), "LIBRARY", GUI.skin.window);
			//this calls the object library listbox, check ListControls class to know more about it
			if(prefabListNames != null && buttonImgs != null)
			{
                ListControls.EditorListbox(new Rect(5, 55, 230, 510), prefabListNames, currentSkins, buttonImgs);
			}
			else
			{
				GUI.Label(new Rect(5, 55, 230, 510),"NO OBJECTS IN LIBRARY");
			}
                GUILayout.EndArea();
            }// OBJECT LIBRARY GUI END  ----------------------------------------------


            // POSITION/ROTATION EDITOR GUI ----------------------------------------------
            else if (GameManager.edModes == GameManager.EditorModes.MOVE || GameManager.edModes == GameManager.EditorModes.ROTATE)
            {
                if (GameManager.SelectedObject != null)
                {
                    if (GameManager.edModes == GameManager.EditorModes.MOVE)
                    {
                        GUILayout.BeginArea(new Rect(1120, 500, 150, 290), "Position", GUI.skin.window);
					
						GUI.Label(new Rect(10, 50, 40, 40), "X: ");
	                    GameManager.coordEdX = GUI.TextField(new Rect(30, 45, 100, 40), GameManager.coordEdX, 6);
	
	                    GUI.Label(new Rect(10, 90, 40, 40), "Y: ");
	                    GameManager.coordEdY = GUI.TextField(new Rect(30, 85, 100, 40), GameManager.coordEdY, 6);
						
						if(!GameManager.is2DMode)
						{
							GUI.Label(new Rect(10, 130, 40, 40), "Z: ");
	                    	GameManager.coordEdZ = GUI.TextField(new Rect(30, 125, 100, 40), GameManager.coordEdZ, 6);
						}
					
                    }
                    else if (GameManager.edModes == GameManager.EditorModes.ROTATE)
                    {
                        GUILayout.BeginArea(new Rect(1120, 500, 150, 290), "Rotation", GUI.skin.window);
						if(!GameManager.is2DMode)
						{
							GUI.Label(new Rect(10, 50, 40, 40), "X: ");
	                    	GameManager.coordEdX = GUI.TextField(new Rect(30, 45, 100, 40), GameManager.coordEdX, 6);
	
	                    	GUI.Label(new Rect(10, 90, 40, 40), "Y: ");
	                    	GameManager.coordEdY = GUI.TextField(new Rect(30, 85, 100, 40), GameManager.coordEdY, 6);
						
							GUI.Label(new Rect(10, 130, 40, 40), "Z: ");
	                    	GameManager.coordEdZ = GUI.TextField(new Rect(30, 125, 100, 40), GameManager.coordEdZ, 6);
						}
						else
						{
							GUI.Label(new Rect(10, 130, 40, 40), "Z: ");
	                    	GameManager.coordEdZ = GUI.TextField(new Rect(30, 125, 100, 40), GameManager.coordEdZ, 6);
						}
                    }

                    
					//here regular expresions are used, to prevent the user types characters instead of numbers, only numbers and the minus sign are allowed
					//the replacement is done on the fly , so theres no way to type characters in the coordinate text fields
                    GameManager.coordEdX = Regex.Replace(GameManager.coordEdX, @"[^\d\.@-]", String.Empty);
                    GameManager.coordEdY = Regex.Replace(GameManager.coordEdY, @"[^\d\.@-]", String.Empty);
					GameManager.coordEdZ = Regex.Replace(GameManager.coordEdZ, @"[^\d\.@-]", String.Empty);
                    
                    
                    if (GUI.Button(new Rect(30, 170, 80, 30), "APPLY"))
                    {

                        Vector3 tempvec = Vector3.zero;
					//here regular expressions are used again just to make sure theres no characters in the coordinate editor
					//as the typed numbers will later be converted to actual floats, having a character in here would cause a big crash
                        GameManager.coordEdX = Regex.Replace(GameManager.coordEdX, @"[^\d\.@-]", String.Empty);
                        GameManager.coordEdY = Regex.Replace(GameManager.coordEdY, @"[^\d\.@-]", String.Empty);
                        GameManager.coordEdZ = Regex.Replace(GameManager.coordEdZ, @"[^\d\.@-]", String.Empty);
                       //convert the values to floats
                        if (float.TryParse(GameManager.coordEdX, out tempvec.x))
                        {
                            tempvec.x = float.Parse(GameManager.coordEdX);
                        }
                        if (float.TryParse(GameManager.coordEdY, out tempvec.y))
                        {
                            tempvec.y = float.Parse(GameManager.coordEdY);
                        }
                        if (float.TryParse(GameManager.coordEdZ, out tempvec.z))
                        {
                            tempvec.z = float.Parse(GameManager.coordEdZ);
                        }
					//apply the values to the selected object
                        if (GameManager.edModes == GameManager.EditorModes.MOVE)
                        {
                            GameManager.SelectedObject.transform.position = tempvec;
                        }

                        else if (GameManager.edModes == GameManager.EditorModes.ROTATE)
                        {
                            Quaternion tempQuat = Quaternion.identity;
                            tempQuat.eulerAngles = tempvec;
                            GameManager.SelectedObject.transform.rotation = tempQuat;
                        }
                    }

                    
                    if (GUI.Button(new Rect(30, 210, 80, 30), "ROUND"))
                    {
                        Vector3 tempvec = Vector3.zero;
					//here regular expressions are used once again just to make sure theres no characters in the coordinate editor
					//as the typed numbers will later be converted to actual floats, having a character in here would cause a big crash
                        GameManager.coordEdX = Regex.Replace(GameManager.coordEdX, @"[^\d\.@-]", String.Empty);
                        GameManager.coordEdY = Regex.Replace(GameManager.coordEdY, @"[^\d\.@-]", String.Empty);
                        GameManager.coordEdZ = Regex.Replace(GameManager.coordEdZ, @"[^\d\.@-]", String.Empty);
                        if (float.TryParse(GameManager.coordEdX, out tempvec.x))
                        {
                            tempvec.x = float.Parse(GameManager.coordEdX);
                        }
                        if (float.TryParse(GameManager.coordEdY, out tempvec.y))
                        {
                            tempvec.y = float.Parse(GameManager.coordEdY);
                        }
                        if (float.TryParse(GameManager.coordEdZ, out tempvec.z))
                        {
                            tempvec.z = float.Parse(GameManager.coordEdZ);
                        }
                        tempvec.x = Mathf.Round(tempvec.x);
                        tempvec.y = Mathf.Round(tempvec.y);
                        tempvec.z = Mathf.Round(tempvec.z);
						//this sets the values only to the coordinate editor text fields , you need to press apply to actually apply the changes
						if (GameManager.edModes == GameManager.EditorModes.MOVE)
						{
							GameManager.coordEdX = tempvec.x.ToString();
                        	GameManager.coordEdY = tempvec.y.ToString();
							if(!GameManager.is2DMode)
							{
								GameManager.coordEdZ = tempvec.z.ToString();
							}
						}
						else if (GameManager.edModes == GameManager.EditorModes.ROTATE)
						{	
							GameManager.coordEdZ = tempvec.z.ToString();
							if(!GameManager.is2DMode)
							{
								GameManager.coordEdX = tempvec.x.ToString();
                        		GameManager.coordEdY = tempvec.y.ToString();
							}
						}      
                    }
				 //the transforms are screwed beyond repair or just simply want to return everything to factory state?. use RESET!
                    if (GUI.Button(new Rect(30, 250, 80, 30), "RESET"))
                    {
						if (GameManager.edModes == GameManager.EditorModes.MOVE)
						{
							GameManager.coordEdX = "0";
                        	GameManager.coordEdY = "0";
							if(!GameManager.is2DMode)
							{
								GameManager.coordEdZ = "0";
							}
						}
						else if (GameManager.edModes == GameManager.EditorModes.ROTATE)
						{
							GameManager.coordEdZ = "0";	
							if(!GameManager.is2DMode)
							{
								GameManager.coordEdX = "0";
                        		GameManager.coordEdY = "0";
							}
						}
					}
                    GUILayout.EndArea();
                }
            }//POSITION/ROTATION EDITOR GUI END -------------------------------------------- 

            //// HELP POPUP ------------
            GUI.skin = currentSkins[1];
            GUILayout.BeginArea(new Rect(20, 80, 600, 700), "", GUI.skin.box);
        	if (showHelp)
            {
            	if(GameManager.is2DMode)
            	{
            		GUI.Label(new Rect(0, 0, 600, 50), "                    CONTROLS");
                 	GUI.Label(new Rect(0, 50, 600, 50), "WASDQE to move the camera");
                 	GUI.Label(new Rect(0, 100, 600, 50), "use Left Mouse Button to SELECT and INSERT  objects into the level");
                 	GUI.Label(new Rect(0, 150, 600, 50), "use Left Mouse Button to MOVE or ROTATE objects");
                 	GUI.Label(new Rect(0, 200, 600, 50), "you can enter the position and rotation values manually");
                 	GUI.Label(new Rect(0, 250, 600, 50), "in the coord editor or choose Round or Reset and then press apply");
                 	GUI.Label(new Rect(0, 300, 600, 50), "Press Delete or R key to remove the Selected Object");
            		GUI.Label(new Rect(0, 350, 600, 50), "Level will be saved in 2d or 3d mode depending on the current active mode");
                	GUI.Label(new Rect(0, 400, 600, 50), "Press H to hide help");
            	}
            	else
            	{
            		GUI.Label(new Rect(0, 0, 600, 50), "                    CONTROLS");
                 	GUI.Label(new Rect(0, 50, 600, 50), "WASD to move the camera");
                 	GUI.Label(new Rect(0, 100, 600, 50), "Press Right Mouse Button");
                 	GUI.Label(new Rect(0, 150, 600, 50), "or ALT to pan the view with the Mouse");
                 	GUI.Label(new Rect(0, 200, 600, 50), "use Left Mouse Button to SELECT and INSERT  objects into the level");
                 	GUI.Label(new Rect(0, 250, 600, 50), "if Mouse is over the Grid they will appear there");
                 	GUI.Label(new Rect(0, 300, 600, 50), "otherwise objects will appear at position of the mouse in mid air");
                 	GUI.Label(new Rect(0, 350, 600, 50), "use Left Mouse Button to MOVE or ROTATE objects");
                 	GUI.Label(new Rect(0, 400, 600, 50), "you can enter the position and rotation values manually");
                 	GUI.Label(new Rect(0, 450, 600, 50), "in the coord editor or choose Round or Reset and then press apply");
                 	GUI.Label(new Rect(0, 500, 600, 50), "Press R key to remove the Selected Object");
            		GUI.Label(new Rect(0, 550, 600, 50), "Level will be saved in 2d or 3d mode depending on the current active mode");
                 	GUI.Label(new Rect(0, 600, 600, 50), "Press H to hide help");
            	}
			}
			else
        	{
            	//GUI.Label(new Rect(0, 0, 600, 50), "Press H to show Controls");
        	}
            
            GUILayout.EndArea(); 
        //// HELP POPUP END-------------
        
            GUI.skin = currentSkins[0];

        //// SAVE AS DIALOG ------------
            if (GameManager.isSaveDialogActive)
            {
                GUILayout.BeginArea(new Rect((1280 / 2) - 300, (800 / 2) - 90, 600, 180), "Save As Level", GUI.skin.window);
                GameManager.levelName = GUI.TextField(new Rect(50, 50, 500, 50), GameManager.levelName, 25);
                if (GUI.Button(new Rect(50, 110, 200, 30), "Save New Level"))
                {
					//use the name typed in the text field, add it the .xml extension and the path to the levels folder and call the SaveLevel function
                    if (GameManager.levelName != "")
                    {
                        GameManager.currentLevel = GameManager.levelsPath.FullName + "/" + GameManager.levelName + ".xml";
                        (GetComponent("LevelLoader") as LevelLoader).SaveLevel();
                    }
                    GameManager.isSaveDialogActive = false;
                }
                if (GUI.Button(new Rect(350, 110, 200, 30), "CANCEL"))
                {
                    GameManager.levelName = "";
                    GameManager.isSaveDialogActive = false;
                }
                GUILayout.EndArea();
            }//// SAVE AS DIALOG END ------------

        //// OPEN LEVEL DIALOG ----------
            if (GameManager.isLoadDialogActive)
            {
                GUILayout.BeginArea(new Rect((1280 / 2) - 400, (800 / 2) - 300, 800, 600), "Level Selection", GUI.skin.window);

                //same as with the level selection dialog,
				//checks if the level folder exists, then retrieve all the xml files in there
				// and extract the file names from the path to display in the list
                if (Directory.Exists(GameManager.levelsPath.FullName))
                {
                    fileNames =System.IO.Directory.GetFiles(GameManager.levelsPath.FullName,"*.xml");
                    if (fileNames != null)
                    {
                        levelNames = new string[fileNames.Length];

                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            levelNames[i] = Path.GetFileName(fileNames[i]);
                        }
                    }
                }

                ListControls.LevelListbox(new Rect(25, 50, 800 - 50, 410), levelNames,fileNames, currentSkins);

                GUI.Label(new Rect(25, 500, 750, 50),"Selected Level: " + Path.GetFileName(GameManager.currentLevel));

                if (GUI.Button(new Rect((400 - 75) - 300, 600 - 65, 150, 50), "Cancel"))
                {
                    GameManager.currentLevel = tempLevelName;
                    GameManager.isLoadDialogActive = false;
                }

                if (GUI.Button(new Rect((400 - 75) + 300, 600 - 65, 150, 50), "Load Level"))
                {
                    if (GameManager.currentLevel != "")
                    {
                        (GetComponent("LevelLoader") as LevelLoader).LoadLevel();
                    }
                    GameManager.isLoadDialogActive = false;
                }
                GUILayout.EndArea(); 
            }
        //// OPEN LEVEL DIALOG END ----------

        ////CURRENT LEVEL INDICATOR ---------
            if (GameManager.currentLevel != "")
            {
                GUI.Label(new Rect(0, 750, 900, 50), "Current Level: " + Path.GetFileName(GameManager.currentLevel)); 
            }
            else
            {
               // GUI.Label(new Rect(0, 750, 900, 50), "Current Level: UNSAVED LEVEL" ); 
            }
        ////CURRENT LEVEL INDICATOR END ---------
    }
}