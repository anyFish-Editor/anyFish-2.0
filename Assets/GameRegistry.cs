using UnityEngine;
using System.Collections;

public class GameRegistry : MonoBehaviour {
	
	public static States currentState = States.ProjectSelector;
	protected BasicState activeState;
	
	public string activeProjectDirectory ="";
	public ProjectData activeProjectData;
	public WarningSystem warningSystem;
	private static bool created = false;
	// Use this for initialization
	void Start () {
		if(created)
			Destroy(gameObject);
		activeState = gameObject.AddComponent<ProjectSelectorState>();
		Application.runInBackground = true;
		created = true;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(Input.GetKeyDown(KeyCode.A))
		{
			if(warningSystem)
			WarningSystem.addWarning("Test", "Lorem ipsum dolor sit amet", Code.Success);
		}
		if(Input.GetKeyDown(KeyCode.S))
		{
			if(warningSystem)
			WarningSystem.addWarning("File Read Error", "File was unsuccessfully opened, check file integrety", Code.Error);
		}
		if(Input.GetKeyDown(KeyCode.D))
		{
			if(warningSystem)
			WarningSystem.addWarning("Elderberries", "Your father was a hamster, your mother smelled of elderberries.", Code.Warning);
		}
		*/
	}
	
	public void switchState(States newState)
	{
		
			activeState.SaveState();
			activeState.CloseState();
		
		
		switch(newState)
		{
			case States.ProjectSelector:
				activeState = gameObject.AddComponent<ProjectSelectorState>();
				break;
			case States.ProjectEditor:
				activeState = gameObject.AddComponent<ProjectEditorState>();
				break;
			case States.AnimationEditor:
				activeState = gameObject.AddComponent<AnimationEditorState>();
				break;
			case States.BatchRenderer:
				activeState = gameObject.AddComponent<BatchRendererState>();
				//TODO: Implement Animation Playback State
			break;
		}
	}
}