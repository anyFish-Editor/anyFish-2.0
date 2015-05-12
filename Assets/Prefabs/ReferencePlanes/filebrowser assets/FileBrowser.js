// UniFileBrowser 1.3 © 2011 Starscene Software

import System.IO;
import UnityEngine.GUILayout;

var filterFiles = false;			// Filter file names by the items in the filterFileExtensions array
var filterFileExtensions : String[];// When filterFiles is true, show only the files with these extensions
var useFilterButton = false;		// Have a button which allows filtering to be turned on and off
var autoAddExtension = false;		// When saving, automatically add this extension to file names
var addedExtension : String;		// Extension to use if automatically adding when saving
var allowAppBundleBrowsing = false;	// Allow browsing .app bundles on OS X as folders (which is what they are)
var showHiddenOSXFiles = false;		// Shows files and folders starting with "." on OS X
var fileWindowID = 0;				// The window ID for the file requester window
var messageWindowID = 1;			// The window ID for the dialog message window

var defaultFileWindowRect = Rect(400, 150, 500, 600);
var minWindowWidth : int = 350;
var minWindowHeight : int = 300;
var messageWindowSize = Vector2(400, 150);	// Width and height of message window
var popupRect = Rect(25, 53, 300, 35);
var buttonPosition = Vector2(190, 58);	// Position of OK and Cancel buttons
var buttonSize = Vector2(72, 35);	// Size of OK and Cancel buttons
var guiDepth = -1;
var guiSkin : GUISkin;
var windowTab : Texture;
var folderIcon : Texture;
var fileIcon : Texture;

private var scrollPos : Vector2;
private var selectedFile : int = -1;
private var oldSelectedFile : int = -1;
private var dirList : Array;
private var fileList : Array;
private var fileDisplayList : GUIContent[];

// Number of pixels used that are NOT in the actual file list window (i.e. space for buttons & stuff).  Smaller number = taller file list window
private var windowControlsSpace : int = 205;

private var scrollViewStyle : GUIStyle;
private var popupListStyle : GUIStyle;
private var popupButtonStyle : GUIStyle;
private var popupBoxStyle : GUIStyle;
private var messageWindowStyle : GUIStyle;

private var filePath : String;
private var fileName = "";
private var frameDone = true;
private var pathList : GUIContent[];
private var showPopup = false;
private var selectedPath = -1;
private var pathChar = "/"[0];
private var windowsSystem = false;

@HideInInspector
var fileWindowOpen = false;
private var fileWindowRect : Rect;
enum FileType {Open, Save, Folder}
private var fileWindowTitle = ["Open file", "Save file", "Select folder"];
private var selectButtonText = ["Open", "Save", "Open"];
private var fileType = FileType.Open;
private var handleClicked = false;
private var clickedPosition : Vector3;
private var originalWindowRect : Rect;
private var cmdKey1 : int;
private var cmdKey2 : int;
private var mousePos : Vector3;
private var linePixelHeight : int;
private var selectFileInProgress = false;
private var arrowKeysDown = false;

function Awake () {
	if (!guiSkin) {
		Debug.LogError("GUI skin missing");
		enabled = false;
		return;
	}

	filePath = Application.dataPath;
	switch (Application.platform) {
		case RuntimePlatform.OSXEditor:
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = KeyCode.LeftApple; cmdKey2 = KeyCode.RightApple;
			break;
		case RuntimePlatform.WindowsEditor:
			pathChar = "\\"[0];
			filePath = filePath.Replace("/", "\\");
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = KeyCode.LeftControl; cmdKey2 = KeyCode.RightControl;
			windowsSystem = true;
			break;
		case RuntimePlatform.OSXPlayer:
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar));
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = KeyCode.LeftApple; cmdKey2 = KeyCode.RightApple;
			break;
		case RuntimePlatform.WindowsPlayer:
			pathChar = "\\"[0];
			filePath = filePath.Replace("/", "\\");
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = KeyCode.LeftControl; cmdKey2 = KeyCode.RightControl;
			windowsSystem = true;
			break;
		default:
			Debug.LogError("You are not using a supported platform");
			Application.Quit();
			break;
	}
	
	// Set up file window position
	fileWindowRect = defaultFileWindowRect;
	fileWindowRect.width = Mathf.Clamp(fileWindowRect.width, minWindowWidth, 1600);	// The file browser window really doesn't need to be huge
	fileWindowRect.height = Mathf.Clamp(fileWindowRect.height, minWindowHeight, 1200);
	fileWindowRect.x = Mathf.Min(fileWindowRect.x, Screen.width - fileWindowRect.width);	// In case small resolutions make it go partially off screen
	// Set up message window position to be in the middle of the screen
	messageWindowRect = Rect(Screen.width/2-messageWindowSize.x/2, Screen.height/2-messageWindowSize.y/2, messageWindowSize.x, messageWindowSize.y);
	
	// Styles are packaged in the GUI skin
	scrollViewStyle = guiSkin.GetStyle("listScrollview");
	popupListStyle = guiSkin.GetStyle("popupList");
	popupButtonStyle = guiSkin.GetStyle("popupButton");
	popupBoxStyle = guiSkin.GetStyle("popupBox");
	messageWindowStyle = guiSkin.GetStyle("messageWindow");
	
	// Add "." to file extensions if not already there
	for (extension in filterFileExtensions) {
		if (!extension.StartsWith(".")) {
			extension = "." + extension;
		}
	}
	if (autoAddExtension && !addedExtension.StartsWith(".")) {
		addedExtension = "." + addedExtension;
	}
	
	linePixelHeight = scrollViewStyle.CalcHeight(GUIContent(" ", folderIcon), 1.0);
	enabled = false;
}

function OnGUI () {
	var defaultSkin = GUI.skin;
	GUI.skin = guiSkin;
	GUI.depth = guiDepth;
	
	// A bit of a hack to get around the fact that the text field eats OnGUI event keyboard input when saving, making arrow key navigation impossible
	// The boolean is used to set FocusControl when drawing the file browser window to something that doesn't eat the keyboard input
	if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)) {arrowKeysDown = true;}
	else {arrowKeysDown = false;}
	
	if (Event.current.type == EventType.KeyDown) {
		FileWindowKeys();
	}

	fileWindowRect = GUI.Window(fileWindowID, fileWindowRect, DrawFileWindow, fileWindowTitle[fileType]);
	if (!showMessageWindow) {
		GUI.FocusWindow(fileWindowID);
	}
	else {
		// Double-clicking can cause an error, probably related to window focus, if the message window is drawn in the same frame when the
		// mouse pointer is over the message window.  So we wait until the next OnGUI frame if this is the case: messageWindowDelay is set
		// true in DrawFileWindow if a double-click is detected.
		if (!messageWindowDelay || !messageWindowRect.Contains(Event.current.mousePosition)) {
			GUI.Window(messageWindowID, messageWindowRect, MessageWindow, messageWindowTitle, messageWindowStyle);
			GUI.BringWindowToFront(messageWindowID);
		}
	}
	messageWindowDelay = false;
	
	// Resize window by dragging corner...must be done outside window code, or else mouse drag events outside the window are unrecognized	
	if (Event.current.type == EventType.MouseDown && Rect(fileWindowRect.width-25, fileWindowRect.height-25, 25, 25).Contains(mousePos)) {
		handleClicked = true;
		clickedPosition = mousePos;
		originalWindowRect = fileWindowRect;
	}
	if (handleClicked) {
		if (Event.current.type == EventType.MouseDrag) {
			fileWindowRect.width = Mathf.Clamp(originalWindowRect.width + (mousePos.x - clickedPosition.x), minWindowWidth, 1600);
			fileWindowRect.height = Mathf.Clamp(originalWindowRect.height + (mousePos.y - clickedPosition.y), minWindowHeight, 1200);
		}
		else if (Event.current.type == EventType.MouseUp) {
			handleClicked = false;
		}
	}

	GUI.skin = defaultSkin;
}

private function DrawFileWindow () {
	GUI.DragWindow (Rect(0,0, 10000, 50));
	mousePos = Event.current.mousePosition;
	
	if (showMessageWindow) {GUI.enabled = false;}
	
	// Folder use button if this is a FileType.Folder window
	if (fileType == FileType.Folder &&
			GUI.Button(Rect(fileWindowRect.width - 100, popupRect.y, 78, popupRect.height), "Select")) {
		SendMessage("OpenFolder", filePath);
		CloseFileWindow();
		return;
	}

	// Editable file name if saving
	if (fileType != FileType.Save) {GUI.enabled = false;}
	if (fileType != FileType.Folder) {fileName = GUI.TextField(Rect(115, 100, fileWindowRect.width-140, 30), fileName, 60);}
	if (!showMessageWindow) {GUI.enabled = true;}
	
	if (fileType == FileType.Open) {GUI.Label(Rect(25, 101, 90, 30), "Open file:");}
	else if (fileType == FileType.Save) {GUI.Label(Rect(25, 101, 90, 30), "Save as:");}

	// List of folders/files
	var selectionAreaRect = Rect(25, 135, fileWindowRect.width-50, fileWindowRect.height-windowControlsSpace);
	if (arrowKeysDown) {GUI.SetNextControlName ("Area");}
	BeginArea(selectionAreaRect, "", "box");
		scrollPos = BeginScrollView(scrollPos);
		selectedFile = SelectionGrid(selectedFile, fileDisplayList, 1, scrollViewStyle, MaxWidth(1600));
		// See if a different file name was chosen, so we don't overwrite any user input in the text box except when needed
		if (selectedFile != oldSelectedFile && frameDone) {
			oldSelectedFile = selectedFile;
			if (selectedFile >= dirList.length && selectedFile - dirList.length < fileList.length) {
				fileName = fileList[selectedFile - dirList.length];
			}
			else {	// No file name if directory is selected
				fileName = "";
			}
			if (fileType == FileType.Save && autoAddExtension && !fileName.EndsWith(addedExtension)) {
				fileName += addedExtension;
			}
		}
		EndScrollView();
	EndArea();	
	if (arrowKeysDown) {GUI.FocusControl ("Area");}
	
	// Double-click - only in file selection area
	if (Event.current.clickCount == 2 && selectionAreaRect.Contains(mousePos) && frameDone) {
		SelectFile();
		WaitForFrame();
		messageWindowDelay = true;
	}
	
	// Filter button
	if (useFilterButton) {
		GUI.Label(Rect(40, fileWindowRect.height-76, 120, 50), windowTab);
		if (GUI.Button(Rect(50, fileWindowRect.height-66, 80, 33), (filterFiles)? "Show all" : "Filter") ) {
			filterFiles = !filterFiles;
			GetCurrentFileInfo();
		}
	}
	
	// Cancel and Open/Save buttons
	if (GUI.Button(Rect(fileWindowRect.width-buttonPosition.x, fileWindowRect.height-buttonPosition.y, buttonSize.x, buttonSize.y), "Cancel") ) {
		CloseFileWindow();
	}
	if (fileName == "" || (autoAddExtension && fileName == addedExtension) || fileType == FileType.Folder) {
		GUI.enabled = false;
	}
	if (GUI.Button(Rect(fileWindowRect.width-buttonPosition.x/2, fileWindowRect.height-buttonPosition.y, buttonSize.x, buttonSize.y),
			selectButtonText[fileType])) {
		SelectFile();
	}
	
	if (!showMessageWindow) {GUI.enabled = true;}
	
	// Path list popup -- done last so it's drawn on top of other stuff
	if (pathList.Length > 0 && Popup.List (popupRect, showPopup, selectedPath, pathList[0], pathList,
						 				   popupButtonStyle, popupBoxStyle, popupListStyle)) {
		if (selectedPath > 0) {
			BuildPathList(selectedPath);
		}
	}
}

enum MessageWindowType {Error, Confirm}
private var messageWindowType : MessageWindowType;
private var button1Text : String;
private var button2Text : String;
private var message : String;
private var showMessageWindow = false;
private var messageWindowTitle : String;
private var messageWindowRect : Rect;
private var confirm = true;
private var messageWindowDelay = false;

private function ShowError (msg : String) {
	message = msg;
	messageWindowTitle = "Error";
	showMessageWindow = true;
	messageWindowType = MessageWindowType.Error;
	fileName = "";
}

private function ShowConfirm (title : String, msg : String, b1Text : String, b2Text : String) {
	message = msg;
	button1Text = b1Text;
	button2Text = b2Text;
	messageWindowTitle = title;
	showMessageWindow = true;
	messageWindowType = MessageWindowType.Confirm;
}

private function MessageWindow () {
	Space(32);
	Label(message);
	
	if (messageWindowType == MessageWindowType.Error) {
		if (GUI.Button(Rect(messageWindowSize.x/2-25, messageWindowSize.y-(buttonSize.y+15), 50, buttonSize.y), "OK") && frameDone) {
			CloseMessageWindow(false);
		}
	}
	else if (messageWindowType == MessageWindowType.Confirm) {
		if (GUI.Button(Rect(messageWindowSize.x/2-110, messageWindowSize.y-(buttonSize.y+15), 100, buttonSize.y), button1Text) && frameDone) {
			CloseMessageWindow(false);
		}
		if (GUI.Button(Rect(messageWindowSize.x/2+10, messageWindowSize.y-(buttonSize.y+15), 100, buttonSize.y), button2Text) && frameDone) {
			CloseMessageWindow(true);
		}
	}
}

private function CloseMessageWindow (isConfirmed : boolean) {
	showMessageWindow = false;
	confirm = isConfirmed;
}

private function FileWindowKeys () {
	var arrowKey = 0;
	switch (Event.current.keyCode) {
		case KeyCode.DownArrow: arrowKey = 1; break;
		case KeyCode.UpArrow: arrowKey = -1; break;
		case KeyCode.Return: ReturnHit(); break;
		case KeyCode.Escape: EscapeHit(); break;
	}
	if (arrowKey == 0) {return;}
	
	// Go to top or bottom of list if alt key is down
	if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
		if (arrowKey == -1) {selectedFile = 0;}
		else {selectedFile = fileDisplayList.Length-1;}
	}
	// Go up or down a folder hierarchy level if command key is down (command/apple on OS X, control on Windows)
	else if (Input.GetKey(cmdKey1) || Input.GetKey(cmdKey2)) {
		if (arrowKey == -1 && pathList.Length > 1) {
			BuildPathList(1);
			return;
		}
		else if (selectedFile >= 0 && selectedFile < dirList.length) {
			SelectFile();
			return;
		}
	}
	// Move file selection up or down
	else {
		selectedFile += arrowKey;
		if (selectedFile < -1) {
			selectedFile = fileDisplayList.Length-1;
		}
		selectedFile = Mathf.Clamp(selectedFile, 0, fileDisplayList.Length-1);
	}
	
	// Handle keyboard scrolling of the list view properly
	var wantedPos = linePixelHeight * selectedFile;
	if (wantedPos < scrollPos.y) {
		scrollPos.y = wantedPos;
	}
	else if (wantedPos > scrollPos.y + fileWindowRect.height - (windowControlsSpace + linePixelHeight + linePixelHeight/2)) {
		scrollPos.y = wantedPos - (fileWindowRect.height - (windowControlsSpace + linePixelHeight + linePixelHeight/2));
	}
}

private function ReturnHit () {
	if (showMessageWindow) {
		CloseMessageWindow(true);
	}
	else {
		SelectFile();
	}
}

private function EscapeHit () {
	if (showMessageWindow) {
		CloseMessageWindow(false);
	}
	else {
		fileWindowOpen = false;
	}
}

private function BuildPathList (pathEntry : int) {
	filePath = "";
	for (i = pathList.Length-1; i >= pathEntry; i--) {
		filePath += pathList[i].text;
		if (i < pathList.Length-1 || windowsSystem) {filePath += pathChar;}
	}
	selectedPath = -1;
	GetCurrentFileInfo();
}

// Work-around for behavior where double-clicking selects files it shouldn't
private function WaitForFrame () {
	frameDone = false;
	selectedFile = -1;
	yield;
	frameDone = true;
	selectedFile = -1;
}

private function GetCurrentFileInfo () {
	dirList = new Array();
	fileList = new Array();

	var info = new DirectoryInfo(filePath);
	if (!info.Exists) {
		ShowError("The directory \"" + filePath + "\" does not exist");
		// Make arrays not be null so the file window can still at least be drawn without errors
		fileDisplayList = new GUIContent[0];
		pathList = new GUIContent[0];
		return;
	}

	try {
		var fileInfo = info.GetFiles();
		var dirInfo = info.GetDirectories();
	}
	catch (err) {
		ShowError(err.Message);
		fileDisplayList = new GUIContent[0];
		pathList = new GUIContent[0];
		return;
	}
	
	// Put folder names into a sorted array
	if (dirInfo.Length > 0) {
		for (i = 0; i < dirInfo.Length; i++) {
			// Don't include ".app" folders or hidden folders, if set
			if (dirInfo[i].Name.EndsWith(".app") && !allowAppBundleBrowsing) {continue;}
			if (dirInfo[i].Name.StartsWith(".") && !showHiddenOSXFiles) {continue;}
			dirList.Push(dirInfo[i].Name);
		}
		dirList.Sort();
	}
	
	// Put file names into a sorted array
	if (fileInfo.Length > 0) {
		for (i = 0; i < fileInfo.Length; i++) {
			// Don't include hidden files, if set
			if (fileInfo[i].Name.StartsWith(".") && !showHiddenOSXFiles) {continue;}
			if (filterFiles && filterFileExtensions.Length > 0) {
				// Go through all extensions for this file type
				var dontAddFile = true;
				for (j = 0; j < filterFileExtensions.Length; j++) {
					if (fileInfo[i].Name.EndsWith(filterFileExtensions[j])) {
						dontAddFile = false;
						break;
					}
				}
				if (dontAddFile) {continue;}
			}
			fileList.Push(fileInfo[i].Name);
		}
		fileList.Sort();
	}

	// Create the combined folder + file list that's actually displayed
	fileDisplayList = new GUIContent[dirList.length + fileList.length];
	for (i = 0; i < dirList.length; i++) {
		fileDisplayList[i] = new GUIContent(dirList[i], folderIcon);
	}
	for (i = 0; i < fileList.length; i++) {
		fileDisplayList[i + dirList.length] = new GUIContent(fileList[i], fileIcon);
	}
	
	// Get path list
	var currentPathList = filePath.Split(pathChar);
	var pathListArray = new Array();
	for (i = 0; i < currentPathList.length-1; i++) {
		if (currentPathList[i] == "") {pathListArray.Push(pathChar.ToString());}
		else {pathListArray.Push(currentPathList[i]);}
	}
	pathListArray.Reverse();
	pathList = new GUIContent[pathListArray.length];
	for (i = 0; i < pathList.Length; i++) {
		pathList[i] = new GUIContent(pathListArray[i], folderIcon);
	}
	
	// Reset stuff so no filenames are selected and the scrollbar is always at the top
	selectedFile = oldSelectedFile = -1;
	scrollPos = Vector2.zero;
	if (autoAddExtension && fileType == FileType.Save) {
		fileName = addedExtension;
	}
	else {
		fileName = "";
	}
}

public function SetPath (thisPath : String) {
	filePath = thisPath;
	if (!filePath.EndsWith(pathChar.ToString())) {
		filePath += pathChar;
	}
	if (windowsSystem) {
		filePath = filePath.Replace("/", "\\");
	}
}

public function OpenFileWindow () {
	if (fileWindowOpen) return;

	fileType = FileType.Open;
	ShowFileWindow();
}

public function OpenFolderWindow () {
	if (fileWindowOpen) return;

	fileType = FileType.Folder;
	ShowFileWindow();
}

public function SaveFileWindow () {
	if (fileWindowOpen) return;

	fileType = FileType.Save;
	ShowFileWindow();
}

private function ShowFileWindow () {
	GetCurrentFileInfo();
	fileWindowOpen = true;
	enabled = true;
}

public function CloseFileWindow () {
	if (showMessageWindow) {return;}	// Don't let window close if error/confirm window is open

	fileWindowOpen = false;
	selectedFile = oldSelectedFile = -1;
	fileName = "";
	// For maximum efficiency, the OnGUI function in this script doesn't run at all when the file browser window isn't open,
	// but is enabled in ShowFileWindow when necessary
	enabled = false;
}

private function SelectFile () {
	if (showMessageWindow || selectFileInProgress) {return;}

	// If user opened a folder, change directories
	if (selectedFile >= 0 && selectedFile < dirList.length) {
		filePath += (dirList[selectedFile] as String) + pathChar;
		GetCurrentFileInfo();
		return;
	}
	
	// Do nothing if there's no file name, or if saving and no real filename has been supplied
	if (fileName == "" || (fileType == FileType.Save && autoAddExtension && fileName == addedExtension)) {return;}
	
	selectFileInProgress = true;
	var thisFileName = fileName;	// Make sure to keep the file name as it was when selected, since it can change later
	// Check for duplicate file name when saving
	if (fileType == FileType.Save) {
		if (autoAddExtension && !thisFileName.EndsWith(addedExtension)) {
			thisFileName += addedExtension;
		}
		for (file in fileList) {
			if (file == thisFileName) {
				ShowConfirm("Warning", "A file with that name already exists. Are you sure you want to replace it?", "Cancel", "Replace");
				while (showMessageWindow) {
					fileName = thisFileName;
					yield;
				}
				if (!confirm) {
					selectFileInProgress = false;
					return;
				}
			}
		}
	}

	// If user selected a name, load/save that file
	if (fileType == FileType.Open) {
		SendMessage("OpenFile", filePath + thisFileName);
		CloseFileWindow();
	}
	else if (fileType == FileType.Save) {
		SendMessage("SaveFile", filePath + thisFileName);
		GetCurrentFileInfo();	// Refresh with new file
		CloseFileWindow();
	}
	
	selectFileInProgress = false;
}