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

public class LargeScreenshot : MonoBehaviour {
    public int resWidth = 6000; 
    public int resHeight = 3375;

    private bool takeHiResShot = false;

    public static string ScreenShotName(int width, int height) {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
                             Application.dataPath, 
                             width, height, 
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot() {
        takeHiResShot = true;
    }

    void LateUpdate() {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot) {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }
}
