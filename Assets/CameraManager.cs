using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public Camera[] CameraList;
	public Camera activeCamera;
	public static int cameraIndex = 0;
	// Use this for initialization
	void Awake () {
		activeCamera = CameraList[0];
		GameRegistry activeRegistry = GameObject.Find("EditorApplication").GetComponent<GameRegistry>();
		Color bg = new Color((float)activeRegistry.activeProjectData.backgroundR/255, (float)activeRegistry.activeProjectData.backgroundG/255, (float)activeRegistry.activeProjectData.backgroundB/255);
		RenderSettings.fogColor = bg;
		Debug.Log("Fog r: " + bg.r + ", g: " + bg.g + ", b: " + bg.b);
		CameraList[0].backgroundColor = bg;
		CameraList[1].backgroundColor = bg;
		CameraList[2].backgroundColor = bg;
		
		Messenger<int>.AddListener("SwitchCamera", switchCamera);
		Messenger<int>.AddListener("SwitchCamera", switchCamera);
		Messenger.AddListener("TurnOffGui", onTurnOffGui);
		Messenger.AddListener("TurnOnGui", onTurnOnGui);
	}
	
	public Vector3 getActiveCameraPosition()
	{
		if(CameraList == null){
			WarningSystem.addWarning("Null camera list", "Attempt to get active camera POSITION failed", Code.Error);
			return Vector3.zero;
		}
		return CameraList[1].gameObject.transform.position;	
	}
	public Quaternion getActiveCameraRotation()
	{
		if(CameraList == null){
			WarningSystem.addWarning("Null camera list", "Attempt to get active camera ROTATION failed", Code.Error);
			return Quaternion.identity;
		}
		
		return CameraList[1].gameObject.transform.rotation;	
	}
	public void setActiveCameraPosition(Vector3 newPosition)
	{
		if(CameraList == null){
			WarningSystem.addWarning("Null camera list", "Restoring Camera Position Failed", Code.Error);
			return;
		}
		
		CameraList[1].gameObject.transform.position = newPosition;	
	}
	public void setActiveCameraRotation(Quaternion newRotation)
	{
		if(CameraList == null){
			WarningSystem.addWarning("Null camera list", "Restoring Camera Rotation Failed", Code.Error);
			return;
		}
		
		CameraList[1].gameObject.transform.rotation = newRotation;	
	}
	
	void onTurnOnGui()
	{
		CameraList[2].enabled = true;
	}
	
	void onTurnOffGui()
	{
		CameraList[2].enabled = false;	
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space))
		{
			activeCamera.transform.rotation = Quaternion.identity;
		}
		if(Input.GetKey("1"))
		{			
			cameraIndex = 0;
			CameraList[0].gameObject.GetComponent<MoveCamera>().enabled = true;
			CameraList[1].gameObject.GetComponent<MoveCamera>().enabled = false;
			CameraList[1].enabled = false;
			CameraList[0].enabled = true;
			//gameObject.GetComponent<Camera>().orthographicSize = startSize;
			//gameObject.transform.position = startPosition;
		}
		if(Input.GetKey("2"))
		{
			cameraIndex = 1;
			CameraList[1].gameObject.GetComponent<MoveCamera>().enabled = true;
			CameraList[0].gameObject.GetComponent<MoveCamera>().enabled = false;
			CameraList[1].enabled = true;
			CameraList[0].enabled = false;
			//if(followObject != null)
			//{
			//	Vector3 newPosition = new Vector3(followObject.transform.position.x, followObject.transform.position.y, gameObject.transform.position.z);
			//	gameObject.transform.position = newPosition;
			//	gameObject.GetComponent<Camera>().orthographicSize = 80;
			//}
		}
	}
	
	void switchCamera(int index)
	{
		cameraIndex = index;
		switch(index)
		{
		case 0:
			CameraList[0].gameObject.GetComponent<MoveCamera>().enabled = true;
			CameraList[1].gameObject.GetComponent<MoveCamera>().enabled = false;
			CameraList[1].enabled = false;
			CameraList[0].enabled = true;
			break;
		case 1:
			CameraList[0].gameObject.GetComponent<MoveCamera>().enabled = false;
			CameraList[1].gameObject.GetComponent<MoveCamera>().enabled = true;
			CameraList[1].enabled = true;
			CameraList[0].enabled = false;
			break;
		}
	}
	
	
}
