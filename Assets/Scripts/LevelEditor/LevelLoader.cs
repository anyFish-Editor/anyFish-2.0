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
using System.Xml;
using System.IO;
using System.Collections.Generic;

//this class is the responsible for actually loading and saving the levels from external xml files
//this class works out of the box in iOS, but you need to set it up as Arm7 and full .NET 2.0 in the unity project settings, it has only been tested on a iPad 2 with iOS 4.3

public class LevelLoader : MonoBehaviour
{
	//set the root gameobject which will serve as a list of currently available scene objects
    public Transform root;

    void Awake()
    {
		//try to clean up all previous trash, to try to make more room for the new allocations
		System.GC.Collect();
		//load to memory all the objects that are in the collection of prefabs to an unity object array
        UnityEngine.Object[] listOfObjects = Resources.LoadAll("Actors", typeof(GameObject));	
		//create a dictionary that will store all the level objects in the objects array
		//a dictionary is used because that way you can look up objects by name instead of index,
		//removing a lot of problems related to the old index based system so its easier to add new objects to the library later in time
		Dictionary<string, GameObject> MapObjects = new Dictionary<string, GameObject>();
		
		//iterate through all the elements of the object array and add them to the dictionary
		for (int i = 0; i < listOfObjects.Length; i++)
        {
			MapObjects.Add(((GameObject)listOfObjects[i]).name, (GameObject)listOfObjects[i]);
		}	
		//send the library to the centralized gamemanager class so its available everywhere
		GameManager.levelObjects = MapObjects;
		
		//set the local array and dictionary to null, so the garbage collector can get rid of them
		listOfObjects = null;
		MapObjects = null;
		System.GC.Collect();
    }

    void Start()
    {
		//a check if the currently loaded level is the editor,as this scripts is loaded by both game and level editor scenes
		//this is done so editor only behaviour can be programmed to objects in the prefab collection
		//like a rigidbody object, you can check this to ensure it only enables physics when its loaded in the game scene but not in the editor
        if (Application.loadedLevelName == "LevelEditor")
        {
            GameManager.isEditor = true;
        }
        else
        {
            GameManager.isEditor = false;
        }
    }

    public void LoadLevel()
    {
        XmlDocument xmldoc = new XmlDocument();
		XmlNode rootNode;

		//checks if the intended level to load actually exists
        if (GameManager.currentLevel != "" && File.Exists(GameManager.currentLevel))
        {        
			//loads the level specified previously by a level selection dialog and then retrieves the root element in the xml file
            xmldoc.Load(GameManager.currentLevel);
            rootNode = xmldoc.FirstChild;
			
			//check for proper file header, if its ok, destroys all the current scene objects in the level to make space for new ones
            if (rootNode.Name == "level")
            {
                if (root.transform.childCount != 0)
                {
                    foreach (Transform child in root)
                    {
                        Destroy(child.gameObject);
                    }
                }          
				
				//if the file header has any child elements, process them acordingly
                if (rootNode.ChildNodes.Count != 0)
                {
                    Vector3 posVector = Vector3.zero;
                    Quaternion rotQuat = Quaternion.identity;
                    Vector3 rotVector = Vector3.zero;
					//check if this level should run in 2d or 3d mode
					if(rootNode.ChildNodes[0].Name == "levelmode")
					{
						if (rootNode.ChildNodes[0].Attributes[0].Name == "is2d")
						{
							if(bool.TryParse(rootNode.ChildNodes[0].Attributes[0].Value, out GameManager.is2DMode))
							{
								GameManager.is2DMode = bool.Parse(rootNode.ChildNodes[0].Attributes[0].Value);
							}
							else
							{
								GameManager.is2DMode = false;
							}
						}
						else
						{
							GameManager.is2DMode = false;
						}
					}
					
					
					//iterate through the file for actor elements where its where the actual object properties are stored in order to instantiate the proper objects in the scene
                    for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                    {
						//if this is an actor node, process the attributes
                        if (rootNode.ChildNodes[i].Name == "actor")
                        {
							GameObject currGameObj;
							//check what object type is this actor
                            if (rootNode.ChildNodes[i].Attributes[0].Name == "type")
                            {
								
								//this checks if the current type name exists in the dictionary of object types, if it exists, tries to retrieve it by name and store it in the currentGameObj variable
								if(GameManager.levelObjects.ContainsKey(rootNode.ChildNodes[i].Attributes[0].Value.ToString()))
								{
									currGameObj = new GameObject();
									GameManager.levelObjects.TryGetValue(rootNode.ChildNodes[i].Attributes[0].Value.ToString(), out currGameObj);
								}
								else
								{
									currGameObj = null;
								}
                            }
							else
							{
								currGameObj = null;
							}
							
							//this next 3 sections of code retrieve the position of the object by converting the string into a float, one axis at a time, if the data retrieved is somehow invalid, set it up to 0
                            if (rootNode.ChildNodes[i].Attributes[1].Name == "positionx")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[1].Value.ToString(), out posVector.x))
                                {
                                    posVector.x = float.Parse(rootNode.ChildNodes[i].Attributes[1].Value.ToString());
                                }
								else
								{
									posVector.x = 0f;
								}
                            }
                            else
                            {
                                posVector.x = 0f;
                            }

                            if (rootNode.ChildNodes[i].Attributes[2].Name == "positiony")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[2].Value.ToString(), out posVector.y))
                                {
                                    posVector.y = float.Parse(rootNode.ChildNodes[i].Attributes[2].Value.ToString());
                                }
								else
								{
									posVector.y = 0f;
								}
                            }
                            else
                            {
                                posVector.y = 0f;
                            }

                            if (rootNode.ChildNodes[i].Attributes[3].Name == "positionz")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[3].Value.ToString(), out posVector.z))
                                {
                                    posVector.z = float.Parse(rootNode.ChildNodes[i].Attributes[3].Value.ToString());
                                }
								else
								{
									posVector.z = 0f;
								}
                            }
                            else
                            {
                                posVector.z = 0f;
                            }
							
							
							// the same goes with rotation, if the data retrieved is invalid set it to no rotation
                            if (rootNode.ChildNodes[i].Attributes[4].Name == "rotationx")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[4].Value.ToString(), out rotVector.x))
                                {
                                    rotVector.x = float.Parse(rootNode.ChildNodes[i].Attributes[4].Value.ToString());
                                }
								else
								{
									rotVector.x = 0f;
								}
                            }
                            else
                            {
                                rotVector.x = 0f;
                            }

                            if (rootNode.ChildNodes[i].Attributes[5].Name == "rotationy")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[5].Value.ToString(), out rotVector.y))
                                {
                                    rotVector.y = float.Parse(rootNode.ChildNodes[i].Attributes[5].Value.ToString());
                                }
								else
								{
									rotVector.y = 0f;
								}
                            }
                            else
                            {
                                rotVector.y = 0f;
                            }

                            if (rootNode.ChildNodes[i].Attributes[6].Name == "rotationz")
                            {

                                if (float.TryParse(rootNode.ChildNodes[i].Attributes[6].Value.ToString(), out rotVector.z))
                                {
                                    rotVector.z = float.Parse(rootNode.ChildNodes[i].Attributes[6].Value.ToString());
                                }
								else
								{
									rotVector.z = 0f;
								}
                            }
                            else
                            {
                                rotVector.z = 0f;
                            }
							
							//checks if we could actually retrieve the object from the dictionary
							if(currGameObj != null)
							{
								//set the rotation in eular angles to the rotation quaternion
	                            rotQuat.eulerAngles = rotVector;
								//once that all the required data is obtained, the instantiation of the objects is done
	                            currGameObj = (GameObject)GameObject.Instantiate(currGameObj, posVector, rotQuat);
								//then object its placed inside the root gameobject which serves as a list of all available objects in scene
	                            currGameObj.transform.parent = root;
								//this removes the (Clone) part of the name, that gets added when you instantiate an object
								//as objects are retrieved by name in the dictionary, its of vital importance that the instantiated objects have the exact same name as the original prefabs they were once
								currGameObj.name = currGameObj.name.Remove(currGameObj.name.Length -7);		
							}						
                        }
                    }
                }
            }      
        }
		System.GC.Collect();
    }

    public void SaveLevel()
    {
		//create the level file headers
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml("<level></level>");
		
		//sets if its a 2d or 3d level
        XmlNode docRoot = xmldoc.DocumentElement;
		XmlElement levelMode = xmldoc.CreateElement("levelmode");
		XmlAttribute modeAttr = xmldoc.CreateAttribute("is2d");
		modeAttr.Value = GameManager.is2DMode.ToString();
		levelMode.Attributes.Append(modeAttr);
		docRoot.AppendChild(levelMode);
		
		if(root.childCount != 0 )
		{
			// iterate through all the objects inside the root gameobject and create actor elements from its information
	        foreach (Transform child in root)
	        {
	            XmlElement elem = xmldoc.CreateElement("actor");
	            XmlAttribute typeAttr = xmldoc.CreateAttribute("type");
	            XmlAttribute posXAttr = xmldoc.CreateAttribute("positionx");
	            XmlAttribute posYAttr = xmldoc.CreateAttribute("positiony");
	            XmlAttribute posZAttr = xmldoc.CreateAttribute("positionz");
	            XmlAttribute rotXAttr = xmldoc.CreateAttribute("rotationx");
	            XmlAttribute rotYAttr = xmldoc.CreateAttribute("rotationy");
	            XmlAttribute rotZAttr = xmldoc.CreateAttribute("rotationz");
	            typeAttr.Value = child.name;
	            posXAttr.Value = child.position.x.ToString();
	            posYAttr.Value = child.position.y.ToString();
	            posZAttr.Value = child.position.z.ToString();
	            rotXAttr.Value = child.rotation.eulerAngles.x.ToString();
	            rotYAttr.Value = child.rotation.eulerAngles.y.ToString();
	            rotZAttr.Value = child.rotation.eulerAngles.z.ToString();
	            elem.Attributes.Append(typeAttr);
	            elem.Attributes.Append(posXAttr);
	            elem.Attributes.Append(posYAttr);
	            elem.Attributes.Append(posZAttr);
	            elem.Attributes.Append(rotXAttr);
	            elem.Attributes.Append(rotYAttr);
	            elem.Attributes.Append(rotZAttr);
	            docRoot.AppendChild(elem);
	        }
			//at last check if the levels folder actually exists, and if so save the level with the generated xml data
	        if (GameManager.currentLevel != "" && Directory.Exists(Path.GetDirectoryName(GameManager.currentLevel)))
	        {
	            xmldoc.Save(GameManager.currentLevel);
	        }
		}
		System.GC.Collect();
    }
}