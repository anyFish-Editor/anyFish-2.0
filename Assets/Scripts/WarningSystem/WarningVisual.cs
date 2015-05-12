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

[RequireComponent (typeof (WarningSystem))]
public class WarningVisual : MonoBehaviour {
	protected WarningData data;
	protected float maxTimeAlive = 6.0f;
	protected float timeAlive = 0.0f;
	protected float fadeAlpha = 1.0f;
	protected float fadeOutTime = 1.0f;
	
	public Rect bg;
	public Rect WarningIcon;
	public Rect ErrorIcon;
	public Rect InfoIcon;
	public Rect SuccessIcon;
	public Rect closeIcon;
	public Texture2D imageAtlas;
	protected Texture2D activeIcon;
	
	protected Texture2D warningTexture;
	protected Texture2D errorTexture;
	protected Texture2D infoTexture;
	protected Texture2D successTexture;
	protected Texture2D bgTexture;		
	
	protected bool texturesPresent = false;
	protected bool isFadingOut = false;
	protected int closeIconOffsetX = 0;
	protected int iconOffsetX = 0;
	protected int bgOffsetX = 0;
	public float offsetY;
	
	protected Rect mouseOverArea;
	protected Rect closeIconArea;
		
	public WarningData warning
	{
		set{ data = value; }	
	}
	// Use this for initialization
	void Start () {
		// WE'll make sure all textures are present, kinda long statement
		if(imageAtlas)
		{			
			texturesPresent = true;
			warningTexture = new Texture2D((int)WarningIcon.width, (int)WarningIcon.height);
			warningTexture.SetPixels(0, 0, (int)WarningIcon.width, (int)WarningIcon.height, imageAtlas.GetPixels((int)WarningIcon.x, (int)WarningIcon.y, (int)WarningIcon.width, (int)WarningIcon.height));
			warningTexture.Apply();
			
			errorTexture = new Texture2D((int)ErrorIcon.width, (int)ErrorIcon.height);
			errorTexture.SetPixels(0, 0, (int)ErrorIcon.width, (int)ErrorIcon.height, imageAtlas.GetPixels((int)ErrorIcon.x, (int)ErrorIcon.y, (int)ErrorIcon.width, (int)ErrorIcon.height));
			errorTexture.Apply();
			
			infoTexture = new Texture2D((int)InfoIcon.width, (int)InfoIcon.height);
			infoTexture.SetPixels(0, 0, (int)InfoIcon.width, (int)InfoIcon.height, imageAtlas.GetPixels((int)InfoIcon.x, (int)InfoIcon.y, (int)InfoIcon.width, (int)InfoIcon.height));
			infoTexture.Apply();
			
			successTexture = new Texture2D((int)SuccessIcon.width, (int)SuccessIcon.height);
			successTexture.SetPixels(0, 0, (int)SuccessIcon.width, (int)SuccessIcon.height, imageAtlas.GetPixels((int)SuccessIcon.x, (int)SuccessIcon.y, (int)SuccessIcon.width, (int)SuccessIcon.height));
			successTexture.Apply();
			
			bgTexture = new Texture2D((int)bg.width, (int)bg.height);
			bgTexture.SetPixels(0, 0, (int)bg.width, (int)bg.height, imageAtlas.GetPixels((int)bg.x, (int)bg.y, (int)bg.width, (int)bg.height));
			bgTexture.Apply();
			//bgTexture;		
			bgOffsetX = (int)((Screen.width / 2) - (bg.width / 2));
			closeIconOffsetX = (int)((Screen.width / 2) + (bg.width / 2) - closeIcon.width * 3 / 2);
			
			mouseOverArea = new Rect(bgOffsetX, offsetY, bg.width, bg.height);
			closeIconArea = new Rect(closeIconOffsetX, offsetY + 7, closeIcon.width, closeIcon.height);
			defineIcon();
			
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		mouseOverArea.y = offsetY;
		closeIconArea.y = offsetY + 7;
		
		timeAlive += Time.deltaTime;
		if(timeAlive > maxTimeAlive)
			isFadingOut = true;
		
		if(isFadingOut)
		{
			fadeAlpha = (fadeOutTime - (timeAlive - maxTimeAlive))/ fadeOutTime;
			if(fadeAlpha <= 0.01f)
				Destroy(this);
		}
	}
	
	void OnGUI()
	{
		if(texturesPresent)
		{
			GUI.depth = 2;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, fadeAlpha);
			GUI.DrawTexture(new Rect(bgOffsetX, offsetY, bg.width, bg.height), bgTexture);
			//GUI.DrawTexture(new Rect(closeIconOffsetX, offsetY + 7, closeIcon.width, closeIcon.height), closeIcon);
			GUI.DrawTexture(new Rect(bgOffsetX + 7, offsetY + 7, activeIcon.width, activeIcon.height), activeIcon);
			
			GUI.Label(new Rect(bgOffsetX + 70, offsetY + 7, 350, 70), data.headline, WarningSystem.headLineStyle);
			GUI.Label(new Rect(bgOffsetX + 70, offsetY + 30, 400, 70), data.message, WarningSystem.messageStyle);
			
			if(mouseOverArea.Contains(Event.current.mousePosition))
			{
				// Prevent the warning from fading.
				timeAlive = 0.0f;
				
				// If the mouse is over the closeIconArea check for a click
				if(closeIconArea.Contains(Event.current.mousePosition))
					if(Input.GetMouseButtonDown(0))
						Destroy(this);
			}
		}
	}
	
	protected void defineIcon()
	{
		if(texturesPresent && data != null)
		{
			switch(data.codeType)
			{
			case Code.Error:
				activeIcon = errorTexture;
				break;
			case Code.Warning:
				activeIcon = warningTexture;
				break;
			case Code.Info:
				activeIcon = infoTexture;
				break;
			case Code.Success:
				activeIcon = successTexture;
				break;
			}
		}
	}
}
