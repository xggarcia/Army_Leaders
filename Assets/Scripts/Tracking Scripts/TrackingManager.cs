using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;


public class TrackingManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CalibrationUI calibrationUI;     // Handles UI interactions

    [Header("Interface")]
    [SerializeField] private GameObject trackingInterface;
    private bool isInterfaceActive = false;

    // Enable or disable tracking plugin.
    [Header("On / Off")]
    [SerializeField] private bool enableTracking;

    // Options for configuration
    [Header("Tracking Configuration")]
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int numberOfBaseStations;
    [SerializeField] private bool enableRotation;
    [SerializeField] private bool enableYAxis;
    //[SerializeField] private bool swapXZ;
    //[SerializeField] private bool invertX;
    //[SerializeField] private bool invertZ;
    [Tooltip("Provided virtual world space is the size of the plane or surface that is seen as for height, as mush as one meter in the real world should match to")]
    [SerializeField] private Vector3 virtualWorldSpace;

    //Players objects reference
    [Header("Player Objects")]
    [SerializeField] private List<GameObject> players;

    //Calibration path information
    [Header("Calibration File Path")]
    [Tooltip("Provided path must be absolute <C:/usr/...> . If no path provided, file will be saved at default location")]
    private string fullCalibrationSaveFilePath;
    [SerializeField] private string calibrationSaveFilePath;
    private string calibrationSaveFileName = "trackingCalibration";

    //Calibration information
    [Header("Calibration")]
    private Calibration calibration;
    private bool calibrated = false;

    private int playersPosAndRotDatatSize;

    // Attributes for non-tracking input
    [Header("Non-tracking")]
    [SerializeField] private int playerSelected = 1;
    [SerializeField] private int trackingDisabledPlayerSpeed = 5;

    private int playerRotDatatSize = 7;
    private float positionUpdateInterval = 0.01f;
    private bool isTrackingInitialized;

    /// <summary>
    /// Initialize the system.
    /// </summary>
    private void Awake()
    {

        // Validate dependencies
        if (calibrationUI == null)
        {
            Debug.LogError("Missing one or more dependencies. Assign required scripts in the Inspector.");
            return;
        }

        trackingInterface.SetActive(isInterfaceActive);

        // Start tracking if enabled
        if (enableTracking)
        {
            PluginConnector.StartTracking(numberOfPlayers, numberOfBaseStations);
            isTrackingInitialized = true;

            // Set interface text for base station number and checks consistency
            int detectedBaseStations = PluginConnector.GetNumberOfBaseStations();
            if (detectedBaseStations == numberOfBaseStations)
            {
                calibrationUI.SetNumberOfBaseStations(detectedBaseStations);
            }
            else
            {
                calibrationUI.SetNumberOfBaseStations("Discrepancy");
            }
            // Set interface text for player number and checks consistency
            int detectedPlayers = PluginConnector.GetNumberOfTrackers();
            if (detectedPlayers == numberOfPlayers)
            {
                calibrationUI.SetNumberOfPlayers(detectedPlayers);
                playersPosAndRotDatatSize = numberOfPlayers * playerRotDatatSize;
            }
            else
            {
                Debug.Log("No tracker detcted");
                calibrationUI.SetNumberOfPlayers("Discrepancy");
            }

            calibration.Initialize();
        }
    }

    /// <summary>
    /// Loads the data from the json file if it exists
    /// </summary>
    private void Start()
    {
        //assign calibration save File
        if (string.IsNullOrEmpty(calibrationSaveFilePath))
        {
            calibrationSaveFilePath = Application.persistentDataPath;
        }
        fullCalibrationSaveFilePath = calibrationSaveFilePath + "/" + calibrationSaveFileName + ".json";

        //load calibration if saved
        LoadCalibrationJson();

        //set visibility for number of playres
        for (int i = 0; i < players.Count; i++)
        {
            if (i >= numberOfPlayers)
            {
                players[i].SetActive(false);
            }
        }

        //start getNewPositions loop when tracking enabled
        if (enableTracking)
        {
            StartCoroutine(GetPositions());
        }
    }


    /// <summary>
    /// Load the calibration saved in the file and give feedback.
    /// </summary>
    public void LoadCalibrationJson()
    {
        Debug.Log("Fetching file at: " + fullCalibrationSaveFilePath);

        try
        {
            calibration = CalibrationUtils.LoadCalibrationJson(fullCalibrationSaveFilePath);

            UpdateCalibrationUICalibrationData();

            calibrated = true;
            calibrationUI.SetCalibrationFileStatus("Loaded Calibration!");
        }
        catch (Exception)
        {
            Debug.Log("Calibration not found. If you want to Start with a preloaded calibration, please generate a file with 'Save Current Calibration' button");
            calibrationUI.SetCalibrationFileStatus("Calibration Failed!");
        }
    }

    /// <summary>
    /// Updates the position and rotation of the players
    /// </summary>
    private IEnumerator GetPositions()
    {
        for (; ; )
        {
            float[] openVrOutputArr = new float[playersPosAndRotDatatSize];
            PluginConnector.UpdatePositions(openVrOutputArr, false, true, false);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                int playerIndex = i * playerRotDatatSize;

                // Get position from openvr array
                Vector3 playersRawPosition = new Vector3(openVrOutputArr[0 + playerIndex], openVrOutputArr[1 + playerIndex], openVrOutputArr[2 + playerIndex]);

                Quaternion playerRotation = new Quaternion(openVrOutputArr[3 + playerIndex], openVrOutputArr[4 + playerIndex], openVrOutputArr[5 + playerIndex], openVrOutputArr[6 + playerIndex]);

                if (calibrated)
                {
                    //Calculates the calibrated position using the Calibration data
                    Vector3 calibratedPos = CalibrationUtils.CalibrateRawPos(playersRawPosition, enableYAxis, calibration, virtualWorldSpace);
                    players[i].GetComponent<PlayerMovement>().SetPosition(calibratedPos);
                    calibrationUI.SetPlayerXPos(i, calibratedPos);

                    if (enableRotation)
                    {
                        //Calculates the calibrated rotation using the Calibration data
                        Quaternion calibratedPlayerRotation = CalibrationUtils.CalibratedRawRot(playerRotation, calibration);
                        players[i].GetComponent<PlayerMovement>().SetRotation(calibratedPlayerRotation);
                        calibrationUI.SetPlayerXRot(i, calibratedPlayerRotation);
                    }
                }
                else
                {
                    players[i].GetComponent<PlayerMovement>().SetPosition(playersRawPosition);
                    calibrationUI.SetPlayerXPos(i, playersRawPosition);
                    if (enableRotation)
                    {
                        players[i].GetComponent<PlayerMovement>().SetRotation(playerRotation);
                        calibrationUI.SetPlayerXRot(i, playerRotation);
                    }
                }

            }

            yield return new WaitForSeconds(positionUpdateInterval);
        }
    }

    /// <summary>
    /// Handles user input during tracking and movement of the players when tracking not enabled
    /// </summary>
    private void Update()
    {
        ListenToControls();

        //if tracking is not enabled move players with keyboard
        if (!enableTracking)
        {
            DisabledTrackingPlayerSelector();

            DisabledTrackingPlayerMovement();
        }
    }

    /// <summary>
    /// Handles input for starting calibration, saving data, toggling the UI, or quit the application.
    /// </summary>
    private void ListenToControls()
    {
        //shows and hides tracking interface
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInterfaceActive = !isInterfaceActive;
            trackingInterface.SetActive(isInterfaceActive);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //select the player that will move when trackingDisabled (default player 1)
    private void DisabledTrackingPlayerSelector()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerSelected = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerSelected = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerSelected = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                playerSelected = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                playerSelected = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                playerSelected = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                playerSelected = 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                playerSelected = 8;
            }
        }
    }

    //read inputs form keyboard and move player selected when tracking is diabled
    private void DisabledTrackingPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            players[playerSelected - 1].transform.Translate(Vector3.forward * Time.deltaTime * trackingDisabledPlayerSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            players[playerSelected - 1].transform.Translate(Vector3.left * Time.deltaTime * trackingDisabledPlayerSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            players[playerSelected - 1].transform.Translate(Vector3.back * Time.deltaTime * trackingDisabledPlayerSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            players[playerSelected - 1].transform.Translate(Vector3.right * Time.deltaTime * trackingDisabledPlayerSpeed);
        }
    }

    /// <summary>
    /// Ensures tracking is properly disabled during script shutdown.
    /// </summary>
    private void OnDisable()
    {
        if (enableTracking)
        {
            if (isTrackingInitialized)
                PluginConnector.StopTracking();
        }
    }

    /// <summary>
    /// Update the calibration center, physicalWorldSize and rotation offset showed in the UI with the calibration values.
    /// </summary>
    private void UpdateCalibrationUICalibrationData()
    {

        calibrationUI.SetCenter(calibration.GetCalibrationCenter());
        calibrationUI.SetPhysicalWorldSize(calibration.GetCalibrationRealWorldSize());
        calibrationUI.SetRotationOffset(calibration.GetCalibrationRotation());
    }
}
