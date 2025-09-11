using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenShot : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static TakeScreenShot instance;

    // Flag to enable/disable screenshot functionality
    public bool takeScreenshots = false;

    // Default location to save screenshots

    //Windows//
    public string screenshotLocation = "./";

    //Mac//
    //public string screenshotLocation = "/Users/yourName/Desktop/screenshots";


    // Array of resolutions to cycle through when taking screenshots
    public string[] resolutions;//640x480 etc;

    // Private variables for tracking screen dimensions and screenshot process
    private int width;
    private int height;
    private int screenN;
    private bool scalingScreen = false;
    private bool takingScreens = false;
    private float waitF;

    // Singleton pattern to ensure only one instance exists
    void Awake()
    {
        if (instance)
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this object and persist across scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Set screen resolution based on the provided string
    public void setXY(string _res)//separated ints with x//
    {
        // Split the resolution string into width and height
        string[] splitNs = _res.Split('x');
        width = int.Parse(splitNs[0]);
        height = int.Parse(splitNs[1]);

        // Set the screen resolution
        Screen.SetResolution(width, height, false);
    }


    // Update is called once per frame
    void Update()
    {
        // Check if screenshot functionality is enabled
        if (takeScreenshots)
        {
            // Start taking screenshots when right mouse button is pressed
            if (!takingScreens)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    // Play a sound effect
                    //soundManager.sound.playSFX(26);

                    // Initialize variables for taking screenshots
                    screenN = 0;
                    scalingScreen = true;
                    takingScreens = true;
                    Time.timeScale = 0f;
                }
            }
            // Scale the screen resolution
            else if (scalingScreen)
            {
                if (Time.realtimeSinceStartup > waitF)
                {
                    // Set the screen resolution and update wait time
                    setXY(resolutions[screenN]);//resolution will not update until the next frame//
                    waitF = Time.realtimeSinceStartup + 2f;
                    scalingScreen = false;// Stop scaling
                }
            }
            //take screenshots//
            else
            {
                if (Time.realtimeSinceStartup > waitF)
                {
                    // Capture screenshot
                    takeScreenshot(screenN);

                    // Check if more screens to capture
                    if (screenN + 1 < resolutions.Length)
                    {
                        screenN++;
                        scalingScreen = true;// Start scaling for the next resolution

                        waitF = Time.realtimeSinceStartup + 2f;
                    }
                    //finished taking screenshots
                    else
                    {
                        takingScreens = false;
                        Time.timeScale = 1f;// Resume the game
                    }
                }
            }
        }
    }

    // Capture screenshot with a given index in resolutions array
    void takeScreenshot(int _num)
    {
        // Save the screenshot with a filename containing resolution and timestamp
        ScreenCapture.CaptureScreenshot(screenshotLocation + "\\haunt" + resolutions[_num] + "_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png");
    }

}