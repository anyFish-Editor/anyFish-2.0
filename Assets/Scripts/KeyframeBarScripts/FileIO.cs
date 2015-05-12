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
using System.IO;
using System.Collections.Generic;

public class FileIO : MonoBehaviour
{
	public static  List<MoCapAnimData> ImportAnimation( string path  )
	{
		//float valueScale = 10;
		
		//Open File stream
		//StreamReader sr0 = new StreamReader(path);
		
		string[] fileLines = File.ReadAllLines(path);
		//Define space as a delimiter
		char[] noDelimeter;
		if(path.Contains(".csv"))
			noDelimeter = new char[] {','};
		else
			noDelimeter = new char[] { };
		
		//Set up temporary storage variables
		Vector3 tempHead = Vector3.zero;
		Vector3 tempTail = Vector3.zero;
		float tempX = 0;
		float tempY = 0;
		float tempZ = 0;
		
		float scaleFactor = 623f;
		//Vector3 upVector = ( 0, 1, 0);
		
		//Transform tempTransform = new Transform();
		MoCapAnimData tempData= new MoCapAnimData();
		Vector3 tempPosition = Vector3.zero;
		
		List<MoCapAnimData> mocapData = new List<MoCapAnimData>();
		
		//Read file until no more new lines
		//while (sr0.Peek() >= 0)
		float startTime = Time.realtimeSinceStartup;
		foreach(string line in fileLines)
		{
			//print(sr0.ReadLine());
			string numberLine;
			
 
				//numberLine = sr0.ReadLine();
			numberLine = line;
			
            string[] numberArray = numberLine.Split(noDelimeter); 
            
			//Set first set of 3 numbers after frame number to Head Vector, converting from Z up to Unity Y up
			//float.TryParse(numberArray[2], out tempX);
			//float.TryParse(numberArray[6], out tempY);
			//float.TryParse(numberArray[4], out tempZ);
			
			if(!path.Contains(".csv")){
			float.TryParse(numberArray[1], out tempX);
			float.TryParse(numberArray[3], out tempY);
			float.TryParse(numberArray[2], out tempZ);

			tempHead = new Vector3 ( tempX, tempY, tempZ ) * scaleFactor;
			tempHead.z *= 2.4f;
			//Store the initial value as position
			tempPosition = tempHead;
			//print("Head Position : " + tempHead);
			
			//Set second set 3 numbers after frame number to Head Vector, , converting from Z up to Unity Y up
			//tempTail = new Vector3( Convert.ToFloat32(numberArray[4]), Convert.ToFloat32(numberArray[6]), Convert.ToFloat32(numberArray[5]) );
			//float.TryParse(numberArray[10], out tempX);
			//float.TryParse(numberArray[12], out tempY);
			//float.TryParse(numberArray[8], out tempZ);
			float.TryParse(numberArray[5], out tempX);
			float.TryParse(numberArray[6], out tempY);
			float.TryParse(numberArray[4], out tempZ);
			
			//tempTail = new Vector3( tempX, tempY + 90, -tempZ);
			tempTail = new Vector3( -tempX, (-tempY + 90), -tempZ);
			//print("Tail Position: " + tempTail);
			}else
			{
				float.TryParse(numberArray[0], out tempX);
				float.TryParse(numberArray[2], out tempY);
				float.TryParse(numberArray[1], out tempZ);
	
				tempHead = new Vector3 ( tempX, tempY, tempZ ) * scaleFactor;
				tempHead.z *= 2.4f;
				//Store the initial value as position
				tempPosition = tempHead;
				//print("Head Position : " + tempHead);
			
			//Set second set 3 numbers after frame number to Head Vector, , converting from Z up to Unity Y up
			//tempTail = new Vector3( Convert.ToFloat32(numberArray[4]), Convert.ToFloat32(numberArray[6]), Convert.ToFloat32(numberArray[5]) );
			//float.TryParse(numberArray[10], out tempX);
			//float.TryParse(numberArray[12], out tempY);
			//float.TryParse(numberArray[8], out tempZ);
				float.TryParse("0", out tempX);
				float.TryParse("0", out tempY);
				float.TryParse("0", out tempZ);
			
				//tempTail = new Vector3( tempX, tempY + 90, -tempZ);
				tempTail = new Vector3( -tempX, (-tempY + 90), -tempZ);
				print("Tail Position: " + tempTail);
			}
			
			/*float extraX;
			float extraY;
			float extraZ;
			*/
			//float.TryParse(numberArray[7], out extraX);
			//float.TryParse(numberArray[9], out extraX);
			//float.TryParse(numberArray[8], out extraX);
			/* Commenting out code to calculate orientation as data is now provided
			//Generate Vector Between the head and tail and Normalize to use for orientation calculation
			tempRotation = tempTail-tempHead;
			//print("Direction Vector " + tempRotation);
			//tempRotation.Normalize();
			//print("Normalized Orientation" + tempRotation);
			
			//Calculate Transform X Rotation using (Y,Z) -> (X,Y)
			//tempA = new Vector2( tempHead.y, tempHead.z );
			tempB = new Vector2( tempTail.y, tempTail.z);
			
			rotX = Vector2.Angle(new Vector2(0,1) , tempB);
			
			//Calculate Transform Y Rotation using (X,Z) -> (X,Y)
			//tempA.x = tempHead.x;
			//tempA.y = tempHead.y;
			
			tempB.x = tempTail.x;
			tempB.y = tempTail.z;
			
			rotY = Vector2.Angle(new Vector2(0,1), tempB);
			
			tempRotation.x = rotX;
			tempRotation.y = rotY;
			tempRotation.z = 0; 					//removing normalization error
			
			*/
			
			//print("Position : " + tempPosition + " Rotation: " + tempRotation);
			//tempTransform.position = tempPosition;
			//tempTransform.rotation = tempRotation;
			tempData.Position = tempPosition;
			//tempData.Rotation = tempRotation;	//commenting out orientation until it's calculated correctly
			//print(tempTail);
			
			
			
			// TODO TURN BACK ON
			
    		
			tempData.Rotation = tempTail;
			mocapData.Add(tempData);
		}
		float endTime = Time.realtimeSinceStartup;
		Debug.Log("Reading the file took: " + (endTime - startTime) + " ms");
		
		return mocapData;
		//print(path);
		//sr0.Close();
		//sr0.Dispose();
	}
}
