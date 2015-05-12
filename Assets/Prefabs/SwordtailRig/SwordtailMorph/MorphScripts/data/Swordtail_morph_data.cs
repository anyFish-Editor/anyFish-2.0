using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Swordtail_morph_data: MonoBehaviour {
	
	public float ScaleConvert = 1;
	public Vector3[] tpsData;
	private float scale ;
	
	public void LoadFile (string filename){

		string[] lines = File.ReadAllLines(filename);
		bool doData= false;
		Vector3[] tmpTpsData = new Vector3[1];
		int tpsDataSize = 0;
		int count = 1;
		
		try{
		foreach (string line in lines)
		{
			//added Chengde two lines, 06062013
			if (line == "")	//sometimes tps file will contain nothing every the other line due to users' mis-operation of tps file generation.
					continue;
			
			if (line.StartsWith("LM="))
			{
				
			    tpsDataSize = int.Parse(line.Remove(0,3));
				doData = true;
				tmpTpsData = new Vector3[tpsDataSize+1];

				
			}
			else if (line.StartsWith("IMAGE="))
			{	
				doData = false;	
			}
			else if (line.StartsWith("SCALE="))
			{
				scale = float.Parse(line.Remove(0,6)) * ScaleConvert;
			}
					
			else if (doData)
			{
				if (count<=tpsDataSize)
				{
					string[] points = line.Split(' ');
					float x = 0;
					float z = float.Parse(points[0]);
					float y = float.Parse(points[1]);
					tmpTpsData[count] = new Vector3(x,y,z);
					
					count ++;
				}
				
			}
		}
		}catch (Exception e)
        {
			WarningSystem.addWarning(e.Message, "Error Parsing Swordtail Morph, only single spaces?", Code.Error);
            Console.WriteLine("{0} Exception caught.", e);
        }
		Vector3[] newTpsPoint =new Vector3[tpsDataSize+1];
		for (int i =1; i<=tpsDataSize; i++)
		{
				

			
			try{
			if(tmpTpsData[1].y < tmpTpsData[2].y)
				newTpsPoint[i].y = (tmpTpsData[i].y - tmpTpsData[1].y)*scale ;	
			else
				newTpsPoint[i].y = (tmpTpsData[1].y - tmpTpsData[i].y)*scale;		
			
			
			if(tmpTpsData[1].z > tmpTpsData[16].z)
				newTpsPoint[i].z = (tmpTpsData[1].z - tmpTpsData[i].z)* scale ;	
			else
				newTpsPoint[i].z = (tmpTpsData[i].z - tmpTpsData[1].z)*scale ;	
			}catch(Exception e){
				WarningSystem.addWarning("TPS File Length Error!", "Check TPS Length for correct fish type!", Code.Error);
			}
			
		}
		tpsData = newTpsPoint;
				
	}	
	
	
}
