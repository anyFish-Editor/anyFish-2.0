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

//this class handles the dialog where the screen resolution can be changed

public class VideoOptionsGUI : MonoBehaviour 
{
    public GUISkin currentSkin;
    public GUISkin listSkin;
    public GUISkin skinLess;
    public GUISkin[] currentSkins;
	public bool isVideoOptionsActive = false;
	public bool wasFromMenu = false;

    private Resolution[] resList;
    private string[] reses;
	private float qualitySliderValue = 3f;
	private int intQualitySliderValue = 3;
	private string selectedQuality = "";
	private bool wantFullScreen = true;
	private Vector2 selectedResolution = Vector2.zero;
	private QualityLevel levelOfQuality;
	
	void Start () 
	{
		//get the current quality setting
		selectedQuality = QualitySettings.currentLevel.ToString();
		
		//get a list of all supported resolutions
        resList = Screen.resolutions;
        reses = new string[resList.Length];
        for (int i = 0; i < resList.Length; i++)
        {
            reses[i] = resList[i].width.ToString() + " X " + resList[i].height.ToString();
        }
	}
	
	void Update () 
	{	
		
		//check what quality level is specified by the slider
		switch( intQualitySliderValue)
		{
		case 1:
			levelOfQuality = QualityLevel.Good;
			selectedQuality = levelOfQuality.ToString();
			break;
			
		case 2:
			levelOfQuality = QualityLevel.Beautiful;
			selectedQuality = levelOfQuality.ToString();
			break;
			
		case 3:
			levelOfQuality = QualityLevel.Fantastic;
			selectedQuality = levelOfQuality.ToString();
			break;
		}
	}
	
	void OnGUI()
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3((float)1.0 * Screen.width/1280, (float)1.0 * Screen.height/800, 1.0f));
		GUI.skin = currentSkin;
		GUI.depth = 0;
		
		if (isVideoOptionsActive) 
		{
	        GUILayout.BeginArea(new Rect((1280 / 2) - 360,(800 / 2) - 250,720,500),"VIDEO OPTIONS",GUI.skin.window);
			

			GUI.Label(new Rect(20, 50, 400, 30), "Select Your Screen Resolution");
            selectedResolution = ListControls.ResListbox(new Rect(20, 100, 400, 380), reses, currentSkins, selectedResolution, resList);

			GUI.Label(new Rect(440, 100, 250, 35),"Current Screen Resolution");
			GUI.Label(new Rect(440, 130, 200, 35),Screen.width.ToString() + " X " + Screen.height.ToString());
            
			//prints the selected resolution, if there none selected, show an empty string
            GUI.Label(new Rect(440, 165, 250, 35), "Selected Screen Resolution");
            if (selectedResolution.x == 0 || selectedResolution.y == 0)
            {
                GUI.Label(new Rect(440, 195, 200, 35), "");
            }
            else
            {
                GUI.Label(new Rect(440, 195, 200, 35), selectedResolution.x.ToString() + " X " + selectedResolution.y.ToString());
            }

            wantFullScreen = GUI.Toggle(new Rect(440, 240, 180, 20), wantFullScreen, "Full Screen?");
                      
            GUI.Label(new Rect(440, 280, 180, 35), "Graphics Quality");
            qualitySliderValue = GUI.HorizontalSlider(new Rect(440, 310, 250, 35), qualitySliderValue, 1, 3);
			intQualitySliderValue = (int)qualitySliderValue;
            GUI.Label(new Rect(440, 320, 180, 35), selectedQuality);

            if (GUI.Button(new Rect(440, 405, 250, 35), "Apply Changes"))
            {
                if (selectedResolution != Vector2.zero)
                {
                    if (selectedResolution.x > 639 && selectedResolution.y > 479)
                    {
                        Screen.SetResolution((int)selectedResolution.x, (int)selectedResolution.y, wantFullScreen);
                    }            
                }            
                QualitySettings.currentLevel = levelOfQuality;
            }
			

			
			//check if it was called from the main menu or the pause menu, to know what window should it return to
            if (GUI.Button(new Rect(440, 445, 250, 35), "Back"))
            {
                isVideoOptionsActive = false;
                if (!wasFromMenu)
                {

                    (GetComponent("PauseMenuGUI") as PauseMenuGUI).isPauseMenuActive = true;
                    wasFromMenu = false;
                }
                else
                {
                    (GetComponent("MainMenuGUI") as MainMenuGUI).isMainMenuActive = true;
                    wasFromMenu = false;
                }
                (GetComponent("VideoOptionsGUI") as VideoOptionsGUI).enabled = false;
            }
			GUILayout.EndArea();
		}	
	}	
}
