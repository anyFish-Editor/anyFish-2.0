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
using System.Collections.Generic;

[RequireComponent (typeof (GameRegistry))]

// The Purpose of the warning System is to produce informational popups deliverable to the user
// in a visually pleasing manor while conveying information to help debugging
public class WarningSystem : MonoBehaviour {
	public int maxVisibleWarnings = 2;
	protected static Queue<WarningData> warningQueue = new Queue<WarningData>();
	protected List<WarningVisual> activeWarnings;
	
	// We gather these here, to provide to the WarningVisual component
	// we could hardcode the warning visuals but eventhough this is kind
	// of dirty it makes less things to change in editor
	public Rect WarningIcon;
	public Rect ErrorIcon;
	public Rect InfoIcon;
	public Rect SuccessIcon;
	public Rect Background;
	public Rect CloseIcon;
	
	public Texture2D ImageAtlas;
	// Use this for initialization
	protected Vector3 lerpPosition = Vector3.zero;
	public static GUIStyle headLineStyle = new GUIStyle();
	public static GUIStyle messageStyle = new GUIStyle();
	
	void Start () {
		warningQueue = new Queue<WarningData>();
		activeWarnings = new List<WarningVisual>();
		
		headLineStyle.fontSize = 20;
		headLineStyle.normal.textColor = Color.white;
		messageStyle.fontSize = 14;
		headLineStyle.fontStyle = FontStyle.Bold;
		messageStyle.normal.textColor = Color.white;
	}
	
	void Update () {
		// If we've got an open spot to put up another warning
		// and we have more warnings to put up
		if(activeWarnings.Count < maxVisibleWarnings && warningQueue.Count > 0)
		{
			WarningVisual newVisual = gameObject.AddComponent<WarningVisual>();
			newVisual.warning = warningQueue.Dequeue();
			
			newVisual.bg = Background;
			newVisual.closeIcon = CloseIcon;
			newVisual.WarningIcon = WarningIcon;
			newVisual.ErrorIcon = ErrorIcon;
			newVisual.SuccessIcon = SuccessIcon;
			newVisual.InfoIcon = InfoIcon;
			newVisual.imageAtlas = ImageAtlas;
			newVisual.offsetY = Screen.height - 80 - ((activeWarnings.Count + 1) * Background.height + 10 * activeWarnings.Count);
			activeWarnings.Add(newVisual);
		}
		
		// We'll check our active warnings to see if they should be removed
		if(activeWarnings.Count > 0)
		{
			for(int i = 0; i < activeWarnings.Count; i++)
			{
				if(activeWarnings[i] == null)
				{					
					
					activeWarnings.RemoveAt(i);
				}else
				{
					lerpPosition.y = Screen.height - 80 - ((i + 1) * Background.height + 10 * i);
					activeWarnings[i].offsetY = Mathf.Lerp(activeWarnings[i].offsetY, lerpPosition.y, Time.deltaTime * 6f);
				}
			}
		}
	}
	
	public static void addWarning(string headline, string warningDescription, Code errorCode)
	{
		WarningData newWarning = new WarningData(headline, warningDescription, errorCode);
		warningQueue.Enqueue(newWarning);		
	}
	
}
