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

public class LabelBarTexture {
	private Texture keyOn;
	private Texture keyOff;
	private int frameCount;
	
	public int x { get; set; }
	public int y { get; set; }
	
	// This class is used to simply seperate the top of the timeline from the actual keys
	// this builds a grey background with text to represent every increment of 5, and positions
	// them accordingly by size. It took a little guess/check for font choice and letter size
	// after the 5 is drawn, all subsiquent text areas are drawn slightly shifted right to center the multi-digit
	// numbers
	public LabelBarTexture(int totalFrames)
	{
		frameCount = totalFrames;
		x = 0;
		y = 0;
	}	
	
	public void draw()
	{
		for(int i = 0; i <= frameCount; i++)
		{
			if(i % 5 == 0)
			{
				int xPos = (i / 5) * 40;
				if(i > 5)
					xPos -= 3;
				GUI.Label(new Rect(xPos, y, 12 * frameCount, 200), i.ToString());
			}
		}
		//GUI.DrawTexture(new Rect(x, y, 8, 16), keyOff);
	}
}


