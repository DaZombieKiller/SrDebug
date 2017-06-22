using System;
using UnityEngine;

/// <summary>The restored Debug Director</summary>
public class SrDebugDirector : MonoBehaviour
{
    // Directors
    // --------------------------------------------------------------------------------
    private TimeDirector _timeDir;
    private PediaDirector _pediaDir;
    private TutorialDirector _tutorialDir;
    private AchievementsDirector _achieveDir;
    private ProgressDirector _progressDir;
    private PlayerState _playerState;
    private AutoSaveDirector _autoSave;

    // Debug Settings
    // --------------------------------------------------------------------------------
    private static bool _debug = true;
    private static bool _showHelp = true;
    
    private bool _noclip;
    private bool _infiniteEnergy;
    private bool _infiniteHealth;
    
    // Object & Component References
    // --------------------------------------------------------------------------------
    private vp_FPController _fpController;
    private GameObject _player;
    private GameObject _camera;
    private GameObject _hudUi;
    
    // Miscellaneous
    // --------------------------------------------------------------------------------
    private Vector3 _noclipPos;
    private Font _helpFont;
    
    /// <summary>Initializes the debug menu</summary>
    public static void Init()
    {
        new GameObject()
            .AddComponent<SrDebugDirector>();
    }

    /// <summary>Retrieves various directors for debugging</summary>
    private void Awake()
    {
        // retrieve the directors
        _timeDir = FindObjectOfType<TimeDirector>();
        _pediaDir = FindObjectOfType<PediaDirector>();
        _tutorialDir = FindObjectOfType<TutorialDirector>();
        _achieveDir = FindObjectOfType<AchievementsDirector>();
        _progressDir = FindObjectOfType<ProgressDirector>();
        _playerState = FindObjectOfType<PlayerState>();
        _autoSave = FindObjectOfType<AutoSaveDirector>();
        
        // set the default help menu font
        _helpFont = Font.CreateDynamicFontFromOSFont("Consolas", 18);

        // prevent user from submitting bug reports.
        var inputDir = FindObjectOfType<InputDirector>();
        inputDir.bugReportPrefab = new GameObject();
        inputDir.bugReportPrefab.SetActive(false);
    }

    /// <summary>Handles debugging features</summary>
    private void Update()
    {
        // the debug menu shouldn't function if the game is frozen
        if (Time.timeScale <= 0f) return;
        
        // toggle the debug menu
        if (Input.GetKeyDown(KeyCode.F9))
        {
            _debug = !_debug;
            
            // if the debug menu was on previously,
            // turn off any cheats that may be on.
            if (!_debug)
            {
                _noclip = false;
                _infiniteEnergy = false;
                _infiniteHealth = false;
                
                // make hud visible
                _hudUi.SetActive(true);
            }
        }

        // debug mode is off, nothing more to do here
        if (!_debug)
            return;

        // retrieve object and component references
        if (_player == null) _player = GameObject.Find("SimplePlayer");
        if (_fpController == null) _fpController = _player?.GetComponent<vp_FPController>();
        if (_camera == null) _camera = GameObject.Find("FPSCamera");
        if (_hudUi == null) _hudUi = GameObject.Find("HudUI");

        // decrement time of day
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            _timeDir.AdjustTimeOfDay(-0.0416666679f);
            
            // check for negative time
            if (_timeDir.WorldTime() <= 0f)
                _timeDir.SetWorldTime(0f);
        }

        // increment time of day
        else if (Input.GetKeyDown(KeyCode.RightBracket))
            _timeDir.AdjustTimeOfDay(0.0416666679f);

        // toggle help menu
        if (Input.GetKeyDown(KeyCode.F10))
            _showHelp = !_showHelp;

        // add 1000 credits
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            _playerState.AddCurrency(1000);
        
        // subtract 1000 credits
        else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            _playerState.AddCurrency(-1000);

        // toggle heads-up display
        if (Input.GetKeyDown(KeyCode.F6))
            _hudUi.SetActive(!_hudUi.activeSelf);

        // reset player progress
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Console.WriteLine("DEBUG: Tutorials/Pedia/Achievements/Progress Reset");
            _pediaDir.DebugClearUnlocked();
            _tutorialDir.DebugClearCompleted();
            _achieveDir.DebugClearAwarded();
            _progressDir.DebugClearProgress();
        }
        
        // unlock all progress
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            Console.WriteLine("DEBUG: Tutorials/Pedia/Achievements/Progress Unlocked");
            _pediaDir.DebugAllUnlocked();
            _tutorialDir.DebugAllCompleted();
            _achieveDir.DebugAllAwarded();
            _progressDir.DebugUnlockProgress();
        }

        // unlock all upgrades
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Console.WriteLine("DEBUG: All Personal Upgrades");
            _playerState.DebugGiveAllUpgrades();
            _playerState.SetHealth(_playerState.GetMaxHealth());
            _playerState.SetEnergy(_playerState.GetMaxEnergy());
            
            // add 5 keys
            for (var i = 0; i < 5; i++)
                _playerState.AddKey();
        }

        // fill inventory with random items and ammo
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Console.WriteLine("DEBUG: Fill Ammo");
            _playerState.Ammo.DebugFillRandomAmmo(_playerState.GetMaxAmmo());
        }

        // toggle infinite energy
        if (Input.GetKeyDown(KeyCode.Alpha6))
            _infiniteEnergy = !_infiniteEnergy;
        
        // toggle infinite health
        if (Input.GetKeyDown(KeyCode.Alpha7))
            _infiniteHealth = !_infiniteHealth;
        
        // clear inventory
        if (Input.GetKeyDown(KeyCode.M))
            _playerState.Ammo.Clear();
        
        // toggle noclip
        if (Input.GetKeyDown(KeyCode.N) && _player)
        {
            _noclip = !_noclip;
            _noclipPos = _player.transform.position;
        }

        // force the game to save
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Console.WriteLine("DEBUG: Forcing save now");
            _autoSave.SaveAllNow();
        }

        // handle noclip, if it's enabled
        if (_noclip && _camera && _fpController)
        {
            // calculate speed, will be multiplied by two if run is held
            var speed = 20f * (SRInput.Actions.run.State ? 2f : 1f);
            
            // add movement
            _noclipPos += _camera.transform.forward * SRInput.Actions.vertical.RawValue * speed * Time.deltaTime;
            _noclipPos += _camera.transform.right * SRInput.Actions.horizontal.RawValue * speed * Time.deltaTime;

            // stop all movement on the controller and reposition it
            _fpController.Stop();
            _fpController.SetPosition(_noclipPos);
        }

        if (_infiniteEnergy) // infinite energy's on, set our energy to max
            _playerState.SetEnergy(_playerState.GetMaxEnergy());
        
        if (_infiniteHealth) // god mode's on, set our health to max
            _playerState.SetHealth(_playerState.GetMaxHealth());
    }
    
    /// <summary>Displays the help menu</summary>
    private void OnGUI()
    {
        if (!_showHelp)
            return;

        GUI.skin.label.font = _helpFont;
        GUI.skin.label.alignment = TextAnchor.LowerRight;

        // construct the help menu text
        var helpMessage =
           $"<b>DEBUG MODE IS {(_debug ? "ENABLED" : "DISABLED")} \n\n" +
           
            "AWARD ALL PERSONAL UPGRADES     0 \n" +
            "TOGGLE INFINITE ENERGY     6 \n" +
            "TOGGLE INFINITE HEALTH     7 \n" +
            "FORCE SAVE     8 \n" +
            "RANDOM ITEMS/AMMO     9 \n\n" +
           
            "CLEAR INVENTORY     M \n" +
            "TOGGLE NOCLIP     N \n\n" +
           
            "ADD 1000 CREDITS     + \n" +
            "REMOVE 1000 CREDITS     - \n" +
            "DECREMENT TIME OF DAY     [ \n" +
            "INCREMENT TIME OF DAY     ] \n\n" +
           
            "TOGGLE HUD    F6 \n" +
            "TOGGLE DEBUG MODE    F9 \n" +
            "TOGGLE DEBUG HELP DISPLAY   F10 \n" +
            "UNLOCK ALL PROGRESS   F11 \n" +
            "RESET PROGRESS   F12 \n</b>";

        // display outline for text
        for (var x = -2; x <= 2; x += 2)
        {
            for (var y = -2; y <= 2; y += 2)
            {
                GUI.skin.label.normal.textColor = Color.black;
                GUI.Label(new Rect(x, y, Screen.width, Screen.height), helpMessage);
            }
        }

        GUI.skin.label.normal.textColor = Color.white;
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), helpMessage);
    }
}
