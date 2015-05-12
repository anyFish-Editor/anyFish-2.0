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
using System.IO;

//this class handles the 3 listboxes used in the project, the resolution listbox, the level selection listbox and the object library listbox in the level editor
//their implementations are a little tricky, they are not completely reusable, thats why i set 3 different lists, they are static mainly for convenience to avoid create new objects every time it is needed
//however even though the implementations may look a bit hacky, it took me a lot of time to achieve this result and was one of the hardest parts of the project
//the implementation consists in just laying gui areas on top of another areas that are supposed to be used with guilayout elements but using regular gui elements
//the with the provided list items, it iterates through how many items the list should have and draws that many buttons or labels
//the scrolling feeling its achieved just moving the whole gui area up and down, and using the parent gui area as a clipping mask
//the proper scrolling movement by the scroll bars is calculated using the height of every element, the height of the space between element and the number of elements in the list
//that way the scroll bar knows how tall the scrolling space should be
//takes into account how tall the actual list box is , to determine if scroll bars necesary , calculating how many items tall(including space between items) is the listbox

public class ListControls
{
    static private float scrollbarValue = 0;
    static private float itemHeight = 33;
    static private float editorItemHeight = 55;
    static private int scrollbarItems = 9;

    static public void EditorListbox(Rect dimensions, string[] listContents,GUISkin[] listSkins, Texture2D[] btnImages)
    {
        GUI.skin = listSkins[0];
        GUILayout.BeginArea(dimensions, "", GUI.skin.box);
        scrollbarItems = (int)dimensions.height / (int)editorItemHeight;
        if (listContents.Length > scrollbarItems)
        {
            scrollbarValue = GUI.VerticalScrollbar(new Rect(2, 2, 80, dimensions.height - 5), scrollbarValue, 1, dimensions.height - 10, (listContents.Length * editorItemHeight) + 10);
        }
        else
        {
            scrollbarValue = dimensions.height - 10;
        }
		
        if (listContents != null)
        {
            GUI.skin = listSkins[1];
            if (listContents.Length > scrollbarItems)
            {
               GUILayout.BeginArea(new Rect(10, 10, dimensions.width - 10, dimensions.height - 20), "", GUI.skin.box);
            }
            else
            {
                GUILayout.BeginArea(new Rect(5, 10, dimensions.width - 5, dimensions.height - 20), "", GUI.skin.box);
            }		 
			
            GUILayout.BeginArea(new Rect(0, (dimensions.height - 10) - scrollbarValue, dimensions.width - 10, (listContents.Length * editorItemHeight) + 10), "", GUI.skin.box);
            GUI.skin = listSkins[2];
			if(listContents.Length != 0)
			{
	            for (int i = 0; i < listContents.Length; i++)
	            {
					//this where the list items are inserted
	                GUI.Label(new Rect(20, (0 + i) * editorItemHeight, 50, editorItemHeight - 5), btnImages[i]);
					
	                if (GUI.Button(new Rect(70, (0 + i) * editorItemHeight, dimensions.width - 100, editorItemHeight - 5), listContents[i]))
	                {
						GameManager.currentObjType = listContents[i];	
	                }
	            }
			}
            GUI.skin = listSkins[0];
            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    static public Vector2 ResListbox(Rect dimensions, string[] listContents, GUISkin[] listSkins,Vector2 resvec,Resolution[] resList)
    {
        GUI.skin = listSkins[0];
        GUILayout.BeginArea(dimensions, "", GUI.skin.box);
        scrollbarItems = (int)dimensions.height / (int)itemHeight;
        if (listContents.Length > scrollbarItems)
        {
            scrollbarValue = GUI.VerticalScrollbar(new Rect(2, 2, 80, dimensions.height - 5), scrollbarValue, 1, dimensions.height - 10, (listContents.Length * itemHeight) + 10);
        }
        else
        {
            scrollbarValue = dimensions.height - 10;
        }
        if (listContents != null)
        {
            GUI.skin = listSkins[1];
            if (listContents.Length > scrollbarItems)
            {
                GUILayout.BeginArea(new Rect(10, 10, dimensions.width - 10, dimensions.height - 20), "", GUI.skin.box);
            }
            else
            {
                GUILayout.BeginArea(new Rect(5, 10, dimensions.width - 5, dimensions.height - 20), "", GUI.skin.box);
            }
            GUILayout.BeginArea(new Rect(0, (dimensions.height - 10) - scrollbarValue, dimensions.width - 10, (listContents.Length * itemHeight) + 10), "", GUI.skin.box);
            GUI.skin = listSkins[2];
            for (int i = 0; i < listContents.Length; i++)
            {
				//this where the list items are inserted
                if (GUI.Button(new Rect(20, (0 + i) * itemHeight, dimensions.width - 50, 30), listContents[i]))
                {
                    resvec.x = resList[i].width;
                    resvec.y = resList[i].height;
                }

            }
            GUI.skin = listSkins[0];
            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
        return resvec;
    }

    static public void LevelListbox(Rect dimensions, string[] levelNames, string[] fileNames, GUISkin[] listSkins)
    {
        GUI.skin = listSkins[0];
        GUILayout.BeginArea(dimensions, "", GUI.skin.box);
        scrollbarItems = (int)dimensions.height / (int)itemHeight;
        if (fileNames.Length > scrollbarItems)
        {
            scrollbarValue = GUI.VerticalScrollbar(new Rect(2, 2, 80, dimensions.height - 5), scrollbarValue, 1, dimensions.height - 10, (fileNames.Length * itemHeight) + 10);
        }
        else
        {
            scrollbarValue = dimensions.height - 10;
        }
        if (fileNames != null)
        {
            GUI.skin = listSkins[1];
            if (fileNames.Length > scrollbarItems)
            {
            GUILayout.BeginArea(new Rect(10, 10, dimensions.width - 10, dimensions.height - 20), "", GUI.skin.box);
            }
            else
            {
                GUILayout.BeginArea(new Rect(5, 10, dimensions.width - 5, dimensions.height - 20), "", GUI.skin.box);
            }
            GUILayout.BeginArea(new Rect(0, (dimensions.height - 10) - scrollbarValue, dimensions.width - 10, (fileNames.Length * itemHeight) + 10), "", GUI.skin.box);
            GUI.skin = listSkins[2];
            for (int i = 0; i < fileNames.Length; i++)
            {
				//this where the list items are inserted
                if (GUI.Button(new Rect(20, (0 + i) * itemHeight, dimensions.width - 50, 30), levelNames[i]))
                {
                    GameManager.currentLevel = fileNames[i];
                }

            }
            GUI.skin = listSkins[0];
            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }
}