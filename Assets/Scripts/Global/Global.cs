using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{

    // Global variables here

    // Movement keys
    public static KeyCode kcJump = KeyCode.C;
    public static KeyCode kcAbility = KeyCode.X;
    public static KeyCode kcRetry = KeyCode.R;

    public static KeyCode kcUp = KeyCode.UpArrow;

    // Game state
    private static GameObject current_checkpoint = null;

    // LOADS FROM SAVE FILE ===================================================
    // load current_checkpoint
    private static string current_checkpoint_object_name = "Start"; // = current_checkpoint.name
    private static Vector3 level_entry_coords = new Vector3(0f, -2f, 10f); // You get to these coords when you quit a level // initial value = game starting coords

    // getters and setters =========================================
    public static string CurrentCheckpointObjectName { get { return current_checkpoint_object_name; } }
    public static GameObject CurrentCheckpoint
    { 
        get { return current_checkpoint; }
        set { current_checkpoint = value;
            current_checkpoint_object_name = value.name; } 
    }
    public static Vector3 LevelEntryCoords { get { return level_entry_coords; } set { level_entry_coords = value; } }
  

    // DOES NOT LOAD FROM SAVE FILE ===========================================
    public static bool PlayingCutscene { get; set; } = true;


    // Global static functions
    public static void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart current game scene
    }

    public static void SetCheckpointToOverworld()
    {
        current_checkpoint = null;
        current_checkpoint_object_name = "Start";
    }

    // Awake
    public static Global Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Debug
    /*private void Update()
    {
        Debug.Log(current_checkpoint_object_name);
        Debug.Log(current_checkpoint);
    }*/
}
