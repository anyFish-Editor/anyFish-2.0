using UnityEngine;
using System.Collections;

public class DEBUG_swordtail_TPS : MonoBehaviour {
	public float scale = 0.5f;
	public void setTransforms(Vector3[] tpsData)
	{
		Transform[] boxes = new Transform[57];
		boxes[0] = transform.FindChild("tpsPoints");

		for (int i=1; i<tpsData.Length; i++)
		{

			string jointName = "tpsPoint" + i ;
			boxes[i] = boxes[0].FindChild(jointName);
			boxes[i].position = tpsData[i];
			boxes[i].localScale = new Vector3(scale, scale, scale);
		}		
	}
}
