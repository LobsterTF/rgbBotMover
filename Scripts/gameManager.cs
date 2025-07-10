// Eyrie inspired puzzle game november 2024
// general mechanism manager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    static bool colorblindMode = true;
    public levelScriptable[] allLevels;
    public levelScriptable currentLevel;
    [SerializeField] static int mapSizeX=10,mapSizeY=10;
    [SerializeField] private tileData[,] map = new tileData[mapSizeX, mapSizeY];
    [SerializeField] private tileData[,] undoState = new tileData[mapSizeX, mapSizeY]; [SerializeField] private tileData[,] undoState2 = new tileData[mapSizeX, mapSizeY];
    [SerializeField] private pawnSpawner pawnSpawner;
    [SerializeField] private tutorialHandler tutorialHandler;
    public Tilemap coloredTiles;
    [SerializeField] private GameObject[] tilesVis;
    [SerializeField] private List<tileColor> moveCommands = new List<tileColor>();
    [SerializeField] private int currentCommandDepth = 0;
    [SerializeField] private bool[] hasBeenFulfilled = new bool[10];
    public commandBoxManager commandBoxManager;
    public musicHandler musicMan;
    public mouseFollower mouseFollower;
    private int level, yellowCommandsHeld, yellowCommandsToBeAdded, yellowCommandsUsed;
    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("CurrentLevel");
        tutorialHandler.activateLevelTutorial(level);
        currentLevel = allLevels[level];
        int colorBlind = PlayerPrefs.GetInt("colorblind");
        if (colorBlind == 0) { colorblindMode = false; } else { colorblindMode = true; }
        commandBoxManager.setColorBlind(colorblindMode);
        musicMan.playSound(1);
        musicMan.startStop(true);
        // Assign game tiles as specified by current level scriptable object
        int y = 0;
        for (int i = 0; i < currentLevel.tileData.Length; i++)
        {
            if (i % currentLevel.dimensions.x == 0 && i != 0)
            {
                y++;
            }
            tileData emptyTileData = new tileData();
            map[i % currentLevel.dimensions.x, y] = emptyTileData;
            map[i % currentLevel.dimensions.x, y].hasFactory = currentLevel.tileData[i].hasFactory;
            map[i % currentLevel.dimensions.x, y].depositAmount = currentLevel.tileData[i].depositAmount; 
            map[i % currentLevel.dimensions.x, y].color = currentLevel.tileData[i].color; 
            map[i % currentLevel.dimensions.x, y].exists = currentLevel.tileData[i].exists;
            map[i % currentLevel.dimensions.x, y].pawnCount = 0;
            map[i % currentLevel.dimensions.x, y].hasWall = currentLevel.tileData[i].hasWall;
            map[i % currentLevel.dimensions.x, y].autoMover = currentLevel.tileData[i].autoMover;
            map[i % currentLevel.dimensions.x, y].hasYellowCommand = currentLevel.tileData[i].hasYellowCommand;


        }
        
        for (int i = 0; i < currentLevel.commands.Length; i++)
        {
            moveCommands.Add(currentLevel.commands[i]);

        }
        makeFirstUndoState();
        initialSetup();

    }
    // currently, yellow does not get recorded in undos
    void makeFirstUndoState()
    {
        int y = 0;
        for (int i = 0; i < currentLevel.tileData.Length; i++)
        {
            if (i % currentLevel.dimensions.x == 0 && i != 0)
            {
                y++;
            }
            tileData emptyTileData = new tileData();
            tileData emptyTileData2 = new tileData();
            undoState[i % currentLevel.dimensions.x, y] = emptyTileData;
            undoState2[i % currentLevel.dimensions.x, y] = emptyTileData2;
            undoState[i % currentLevel.dimensions.x, y].hasFactory = map[i % currentLevel.dimensions.x, y].hasFactory;
            undoState[i % currentLevel.dimensions.x, y].depositAmount = map[i % currentLevel.dimensions.x, y].depositAmount;
            undoState[i % currentLevel.dimensions.x, y].color = map[i % currentLevel.dimensions.x, y].color;
            undoState[i % currentLevel.dimensions.x, y].exists = map[i % currentLevel.dimensions.x, y].exists;
            undoState[i % currentLevel.dimensions.x, y].pawnCount = 0;
            undoState2[i % currentLevel.dimensions.x, y].pawnCount = 0;
            undoState[i % currentLevel.dimensions.x, y].hasWall = map[i % currentLevel.dimensions.x, y].hasWall;
            undoState[i % currentLevel.dimensions.x, y].autoMover = map[i % currentLevel.dimensions.x, y].autoMover;
            undoState[i % currentLevel.dimensions.x, y].hasYellowCommand = map[i % currentLevel.dimensions.x, y].hasYellowCommand;
            undoState2[i % currentLevel.dimensions.x, y].hasYellowCommand = map[i % currentLevel.dimensions.x, y].hasYellowCommand;
        }
        for (int i = 0; i < moveCommands.Count; i++)
        {
            uStateMoveCommands[i] = moveCommands[i];
            uStateMoveCommands2[i] = moveCommands[i];
        }
    }
    private int uStateCommandDepth, uStatePawnsSpawned, uStateYellowCommandsToBeAdded, uStateCommandDepth2, uStatePawnsSpawned2, uStateYellowCommandsToBeAdded2;
    private bool[] uStatehasBeenFulfilled = new bool[10]; private bool[] uStatehasBeenFulfilled2 = new bool[10];
    private tileColor[] uStateMoveCommands = new tileColor[10]; private tileColor[] uStateMoveCommands2 = new tileColor[10];
    [SerializeField] private GameObject undoSoundObj, resetSoundObj;
    void saveUndoState() // called when picked up before logic
    {
        int y = 0;
        for (int i = 0; i < currentLevel.tileData.Length; i++)
        {
            if (i % currentLevel.dimensions.x == 0 && i != 0)
            {
                y++;
            }
            undoState[i % currentLevel.dimensions.x, y].pawnCount = map[i % currentLevel.dimensions.x, y].pawnCount;
            undoState[i % currentLevel.dimensions.x, y].hasYellowCommand = map[i % currentLevel.dimensions.x, y].hasYellowCommand;
        }
        uStateCommandDepth = currentCommandDepth;
        uStatePawnsSpawned = pawnsSpawned;
        uStateYellowCommandsToBeAdded = yellowCommandsToBeAdded;
        for (int i = 0;i < uStatehasBeenFulfilled.Length; i++)
        {
            uStatehasBeenFulfilled[i] = hasBeenFulfilled[i];
        }
        for (int i = 0; i < moveCommands.Count; i++)
        {
            uStateMoveCommands[i] = moveCommands[i];
        }
    }
    void saveUndoState2() // called when placed before logic
    {
        int y = 0;
        for (int i = 0; i < currentLevel.tileData.Length; i++)
        {
            if (i % currentLevel.dimensions.x == 0 && i != 0)
            {
                y++;
            }
            undoState2[i % currentLevel.dimensions.x, y].pawnCount = undoState[i % currentLevel.dimensions.x, y].pawnCount;
            undoState2[i % currentLevel.dimensions.x, y].hasYellowCommand = undoState[i % currentLevel.dimensions.x, y].hasYellowCommand;
        }
        uStateCommandDepth2 = uStateCommandDepth;
        uStatePawnsSpawned2 = uStatePawnsSpawned;
        uStateYellowCommandsToBeAdded2 = uStateYellowCommandsToBeAdded;
        for (int i = 0; i < uStatehasBeenFulfilled.Length; i++)
        {
            uStatehasBeenFulfilled2[i] = uStatehasBeenFulfilled[i];
        }
        for (int i = 0; i < moveCommands.Count; i++)
        {
            uStateMoveCommands2[i] = uStateMoveCommands[i];
        }
    }
    void revertUndoState()
    {
        int y = 0;
        for (int i = 0; i < currentLevel.tileData.Length; i++)
        {
            if (i % currentLevel.dimensions.x == 0 && i != 0)
            {
                y++;
            }
            map[i % currentLevel.dimensions.x, y].pawnCount = undoState2[i % currentLevel.dimensions.x, y].pawnCount;
            map[i % currentLevel.dimensions.x, y].hasYellowCommand = undoState2[i % currentLevel.dimensions.x, y].hasYellowCommand;

        }
        currentCommandDepth = uStateCommandDepth2;
        pawnsSpawned = uStatePawnsSpawned2;
        yellowCommandsToBeAdded = uStateYellowCommandsToBeAdded2;
        heldPawns = 0;
        for (int i = 0; i < hasBeenFulfilled.Length; i++)
        {
            hasBeenFulfilled[i] = uStatehasBeenFulfilled2[i];
        }
        int yellowCommandsinMove = 0;
        for (int i = 0; i < moveCommands.Count; i++)
        {
            if (uStateMoveCommands2[i] == tileColor.Yellow)
            {
                yellowCommandsinMove++;
            }


        }
        while (moveCommands.Count > currentLevel.commands.Length + yellowCommandsinMove)
        {
            for (int i = moveCommands.Count - 1; i >= 0; i--)
            {
                if (moveCommands[i] == tileColor.Yellow)
                {
                    moveCommands.RemoveAt(i);
                    break;
                }
               

            }
        }

        for (int i = 0; i < moveCommands.Count; i++)
        {
            moveCommands[i] = uStateMoveCommands2[i];
        }
        
        if (gameOver)
        {
            gameOver = false;
            listeningForInputs = true;
            loseText.SetActive(false);
        }
        if (pawnsSpawned >= currentLevel.pawnMax)
        {
            finalRoundText.SetActive(true);
        }
        else
        {
            finalRoundText.SetActive(false);
        }
        mouseFollower.setCarryNum(heldPawns);
        updateCommandBox();
        updateMap();
    }
    void initialSetup()
    {
        
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                int color = (int)(map[x, y].color);
                bool exists = map[x, y].exists;
                drawWorld(x, y, color, exists);
                if (exists)
                {
                    pawnSpawner.spawn(x, y);
                }
               
            }
        }
        tilesVis = GameObject.FindGameObjectsWithTag("tileHolder");
        updateCommandBox();
        setPawnVisuals();
        turnStart();
    }
    private bool listeningForInputs = false, firstTurn = true;
    private int pawnsSpawned;

    void turnStart()
    {
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                bool hasFactory = map[x, y].hasFactory;
                if (hasFactory)
                {
                    map[x, y].pawnCount++;
                    if (!firstTurn)
                    {
                        pawnsSpawned++;

                    }else { firstTurn = false; }
                }

            }
        }
        if (pawnsSpawned >= currentLevel.pawnMax)
        {
            finalRoundText.SetActive(true);
        }
        updateMap();
        listeningForInputs = true;
    }
    bool checkForNewTurn()
    {
        for (int i = 0; i <= currentCommandDepth; i++)
        {
            if (!hasBeenFulfilled[i])
            {
                return false;
            }
        }
        if (yellowCommandsUsed < yellowCommandsHeld)
        {
            return false;
        }
        return true;
    }
    [SerializeField] private GameObject finalRoundSound, pickUpSound, winSound, conveyorSound;

    void newTurn()
    {
        listeningForInputs = false;
        
        if (pawnsSpawned == currentLevel.pawnMax - 1 && level != 0)
        {
            GameObject snd = Instantiate(finalRoundSound, transform.position, Quaternion.identity);
            soundObject sndObjScrpt = snd.GetComponent<soundObject>();
            sndObjScrpt.playSound(1f);
        }
        for (int i = 0; i <= currentCommandDepth; i++)
        {
            hasBeenFulfilled[i] = false;
        }
        
        if (pawnsSpawned < currentLevel.pawnMax)
        {
            currentCommandDepth++;
            turnStart();
        }
        updateCommandBox();
    }

    void updateCommandBox()
    {
        
        for (int i = 0; i < 8; i++)
        {
            if (!hasBeenFulfilled[i] && i <= currentCommandDepth)
            {
                commandBoxManager.setColor(i, (int)moveCommands[i]);
            }
            else if (i <= currentCommandDepth)
            {
                commandBoxManager.setExhausted(i);
            }
            else
            {
                commandBoxManager.setInactive(i);
            }
        }
        for (int i = 0;i <= currentLevel.pawnMax; i++)
        {
            
            if (i < currentLevel.pawnMax - pawnsSpawned)
            {
                commandBoxManager.enableDisablePawn(i, true);
            }
            else
            {
                commandBoxManager.enableDisablePawn(i, false);

            }

        }
        int forecastBump = 0;
        for (int i = 0; i < currentCommandDepth; i++)
        {
            if (moveCommands[i] == tileColor.Yellow)
            {
                forecastBump++;
            }
        }
        for (int i = 0 ; i < 3; i++)
        {
            if (i + currentCommandDepth + 1 < moveCommands.Count)
            {
                commandBoxManager.setForecastColor(i, (int)moveCommands[i + currentCommandDepth + 1]);
            }
            else
            {
                commandBoxManager.setForecastColor(i, 4);

            }
        }
    }
    void updateMap()
    {
        setPawnVisuals();
    }
    public TileBase[] colorTiles, colorBlindColorTiles; // 0R 1G 2B
    void drawWorld(int x, int y, int color, bool exists)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        if (!exists)
        {
            return;
        }
        for (int i = 0; i < colorTiles.Length; i++) 
        { 
            if (color == i)
            {
                if (colorblindMode)
                {
                    coloredTiles.SetTile(pos, colorBlindColorTiles[color]);
                }
                else
                {
                    coloredTiles.SetTile(pos, colorTiles[color]);

                }
            }
        }
        
        
    }
    void setPawnVisuals()
    {
        bool finalTurn = false;
        if (pawnsSpawned >= currentLevel.pawnMax)    
        {
            finalTurn = true;
        }
        foreach (GameObject visual in tilesVis)
        {
            pawnIndividualVisual pawnVis = visual.GetComponent<pawnIndividualVisual>();
            Vector3Int coords = pawnVis.coordinates;
            
            pawnVis.UpdateVisuals(map[coords.x, coords.y].pawnCount, map[coords.x, coords.y].hasFactory, map[coords.x, coords.y].depositAmount, finalTurn, (int)map[coords.x, coords.y].autoMover, map[coords.x, coords.y].hasYellowCommand);
            for (int i = 0; i < 4; i++)
            {
                if (map[coords.x, coords.y].hasWall[i])
                {
                    pawnVis.setWalls(i);
                }
            }
        }
    }
    [SerializeField] private Vector3Int selectedTile, originTile;
    [SerializeField] private Camera cam;
    [SerializeField] private int heldPawns;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        selectedTile = coloredTiles.WorldToCell(mousePos);
        if (listeningForInputs && !gameOver)
        {
            interactLogic();

        }
        if (Input.GetButtonDown("Restart"))
        {
            restart();
        }
        if (Input.GetButtonDown("Undo") && !gameWin && heldPawns == 0 && currentCommandDepth > 0)
        {
            GameObject snd = Instantiate(undoSoundObj, transform.position, Quaternion.identity);
            soundObject sndObjScrpt = snd.GetComponent<soundObject>();
            sndObjScrpt.playSound(1f);
            revertUndoState();
        }
        if (gameWin)
        {
            if (Input.GetButtonDown("Jump"))
            {
                openNewLevel();
            }
        }
    }
    public void restart()
    {
        GameObject snd = Instantiate(resetSoundObj, transform.position, Quaternion.identity);
        DontDestroyOnLoad(snd);
        soundObject sndObjScrpt = snd.GetComponent<soundObject>();
        sndObjScrpt.playSound(1.4f);
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    [SerializeField] private GameObject endText;
    public void openNewLevel()
    {
        int levelNew = PlayerPrefs.GetInt("CurrentLevel");
        levelNew++;
        if (levelNew < allLevels.Length)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelNew);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        else
        {
            endText.SetActive(true);
        }
        
    }
    void FixedUpdate()
    {
        if (pickupPitchUpDuration > 0)
        {
            pickupPitchUpDuration -= .1f;
        }
        else
        {
            speedPickupPitchModifier = 0;
        }
    }
    private float speedPickupPitchModifier = 0f, pickupPitchUpDuration;
    private bool gameOver = false, gameWin = false;
    void interactLogic() // remove entering conveyor belts pointing towards origin.
    {
        
        bool moved = false;
        if (heldPawns > 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                
                if (selectedTile.x < 0 || selectedTile.y < 0 || selectedTile.x > currentLevel.dimensions.x - 1 || selectedTile.y > currentLevel.dimensions.y - 1)
                {
                    return;
                }
                if (!map[selectedTile.x, selectedTile.y].exists) { return; }
                if ((int)map[originTile.x, originTile.y].color == 3 && yellowCommandsUsed < yellowCommandsHeld) // yellow check
                {
                    if (originTile != selectedTile) // if not same tile as origin
                    {
                        if ((Mathf.Abs(originTile.x - selectedTile.x) == 1 && originTile.y - selectedTile.y == 0) || (Mathf.Abs(originTile.y - selectedTile.y) == 1 && originTile.x - selectedTile.x == 0)) // if orthagonally adjacent
                        {
                            if (hitsWall(selectedTile, originTile))
                            {
                                return;
                            }
                            if (hitsReturningConveyor(selectedTile, originTile))
                            {
                                return;
                            }
                            yellowCommandsUsed++;
                            if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                            {
                                yellowCommandsToBeAdded++;
                                map[selectedTile.x, selectedTile.y].hasYellowCommand = false;
                            }
                            saveUndoState2();
                            moved = true;
                            
                            map[selectedTile.x, selectedTile.y].pawnCount += heldPawns;
                            heldPawns = 0;
                            pickupPitchUpDuration = 6f;
                            speedPickupPitchModifier++;
                        }

                    }
                    else
                    {
                        if (map[originTile.x, originTile.y].pawnCount > 0)
                        {

                            map[originTile.x, originTile.y].pawnCount--;
                            heldPawns++;
                            pickupPitchUpDuration = 6f;
                            speedPickupPitchModifier++;
                            GameObject snd = Instantiate(pickUpSound, transform.position, Quaternion.identity);
                            soundObject sndObjScrpt = snd.GetComponent<soundObject>();
                            sndObjScrpt.playSound(1f + (speedPickupPitchModifier / 19));
                        }
                    }

                }
                else
                {
                    for (int i = 0; i <= currentCommandDepth; i++)
                    {
                        if (map[originTile.x, originTile.y].color == moveCommands[i] && hasBeenFulfilled[i] == false)
                        {

                            if (originTile != selectedTile) // if not same tile as origin
                            {
                                if ((Mathf.Abs(originTile.x - selectedTile.x) == 1 && originTile.y - selectedTile.y == 0) || (Mathf.Abs(originTile.y - selectedTile.y) == 1 && originTile.x - selectedTile.x == 0)) // if orthagonally adjacent
                                {
                                    if (hitsWall(selectedTile, originTile))
                                    {
                                        return;
                                    }
                                    if (hitsReturningConveyor(selectedTile, originTile))
                                    {
                                        return;
                                    }
                                    if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                                    {
                                        yellowCommandsToBeAdded++;
                                        map[selectedTile.x, selectedTile.y].hasYellowCommand = false;

                                    }
                                    saveUndoState2();
                                    moved = true;
                                    hasBeenFulfilled[i] = true;
                                    map[selectedTile.x, selectedTile.y].pawnCount += heldPawns;
                                    heldPawns = 0;
                                    pickupPitchUpDuration = 6f;
                                    speedPickupPitchModifier++;
                                }

                            }
                            else
                            {
                                if (map[originTile.x, originTile.y].pawnCount > 0)
                                {

                                    map[originTile.x, originTile.y].pawnCount--;
                                    heldPawns++;
                                    pickupPitchUpDuration = 6f;
                                    speedPickupPitchModifier++;
                                    GameObject snd = Instantiate(pickUpSound, transform.position, Quaternion.identity);
                                    soundObject sndObjScrpt = snd.GetComponent<soundObject>();
                                    sndObjScrpt.playSound(1f + (speedPickupPitchModifier / 19));
                                }
                            }
                            break;
                        }
                    }
                }
                
                mouseFollower.setCarryNum(heldPawns);
                updateCommandBox();
                updateMap();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                map[originTile.x, originTile.y].pawnCount += heldPawns;
                heldPawns = 0;
                mouseFollower.setCarryNum(heldPawns);
                updateMap();
            }
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            if (verticalInput > 0)
            {
                if (originTile.y + 1 > currentLevel.dimensions.y - 1)
                {
                    return;
                }
                if (!map[originTile.x, originTile.y + 1].exists) { return; }
                
                for (int i = 0; i <= currentCommandDepth; i++)
                {
                    if (map[originTile.x, originTile.y].color == moveCommands[i] && hasBeenFulfilled[i] == false)
                    {
                        if (hitsWall(originTile + new Vector3Int(0,1,0), originTile))
                        {
                            return;
                        }
                        if (hitsReturningConveyor(selectedTile, originTile))
                        {
                            return;
                        }
                        if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                        {
                            yellowCommandsToBeAdded++;
                            map[selectedTile.x, selectedTile.y].hasYellowCommand = false;
                        }
                        saveUndoState2();
                        moved = true;
                        hasBeenFulfilled[i] = true;
                        map[originTile.x, originTile.y + 1].pawnCount += heldPawns;
                        heldPawns = 0;
                        pickupPitchUpDuration = 6f;
                        speedPickupPitchModifier++;
                        break;
                    }
                }
                mouseFollower.setCarryNum(heldPawns);
                updateCommandBox();
                updateMap();
            }else if (verticalInput < 0)
            {
                if (originTile.y - 1 < 0)
                {
                    return;
                }
                if (!map[originTile.x, originTile.y - 1].exists) { return; }
                for (int i = 0; i <= currentCommandDepth; i++)
                {
                    if (map[originTile.x, originTile.y].color == moveCommands[i] && hasBeenFulfilled[i] == false)
                    {
                        if (hitsWall(originTile - new Vector3Int(0, 1, 0), originTile))
                        {
                            return;
                        }
                        if (hitsReturningConveyor(selectedTile, originTile))
                        {
                            return;
                        }
                        if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                        {
                            yellowCommandsToBeAdded++;
                            map[selectedTile.x, selectedTile.y].hasYellowCommand = false;
                        }
                        saveUndoState2();
                        moved = true;
                        hasBeenFulfilled[i] = true;
                        map[originTile.x, originTile.y - 1].pawnCount += heldPawns;
                        heldPawns = 0;
                        pickupPitchUpDuration = 6f;
                        speedPickupPitchModifier++;


                        break;
                    }
                }
                mouseFollower.setCarryNum(heldPawns);
                updateCommandBox();
                updateMap();
            }
            else if (horizontalInput > 0)
            {
                if (originTile.x + 1 > currentLevel.dimensions.y - 1)
                {
                    return;
                }
                if (!map[originTile.x + 1, originTile.y].exists) { return; }
                for (int i = 0; i <= currentCommandDepth; i++)
                {
                    if (map[originTile.x, originTile.y].color == moveCommands[i] && hasBeenFulfilled[i] == false)
                    {
                        if (hitsWall(originTile + new Vector3Int(1, 0, 0), originTile))
                        {
                            return;
                        }
                        if (hitsReturningConveyor(selectedTile, originTile))
                        {
                            return;
                        }
                        if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                        {
                            yellowCommandsToBeAdded++;
                            map[selectedTile.x, selectedTile.y].hasYellowCommand = false;
                        }
                        saveUndoState2();
                        moved = true;
                        hasBeenFulfilled[i] = true;
                        map[originTile.x + 1, originTile.y].pawnCount += heldPawns;
                        heldPawns = 0;
                        pickupPitchUpDuration = 6f;
                        speedPickupPitchModifier++;


                        break;
                    }
                }
                mouseFollower.setCarryNum(heldPawns);
                updateCommandBox();
                updateMap();
            }
            else if (horizontalInput < 0)
            {
                if (originTile.x - 1 < 0)
                {
                    return;
                }
                if (!map[originTile.x - 1, originTile.y].exists) { return; }
                for (int i = 0; i <= currentCommandDepth; i++)
                {
                    if (map[originTile.x, originTile.y].color == moveCommands[i] && hasBeenFulfilled[i] == false)
                    {
                        if (hitsWall(originTile - new Vector3Int(1, 0, 0), originTile))
                        {
                            return;
                        }
                        if (hitsReturningConveyor(selectedTile, originTile))
                        {
                            return;
                        }
                        if (map[selectedTile.x, selectedTile.y].hasYellowCommand)
                        {
                            yellowCommandsToBeAdded++;
                            map[selectedTile.x, selectedTile.y].hasYellowCommand = false;
                        }
                        saveUndoState2();
                        moved = true;
                        hasBeenFulfilled[i] = true;
                        map[originTile.x - 1, originTile.y].pawnCount += heldPawns;
                        heldPawns = 0;
                        pickupPitchUpDuration = 6f;
                        speedPickupPitchModifier++;


                        break;
                    }
                }
                mouseFollower.setCarryNum(heldPawns);
                updateCommandBox();
                updateMap();
            }
        }
        else
        if (Input.GetButtonDown("Fire1"))
        {
            
            if (selectedTile.x < 0 || selectedTile.y < 0 || selectedTile.x > currentLevel.dimensions.x - 1 || selectedTile.y > currentLevel.dimensions.y - 1)
            {
                return;
            }

            if (!map[selectedTile.x, selectedTile.y].exists) { return; }
            
            originTile = selectedTile;
            if (map[originTile.x, originTile.y].pawnCount > 0)
            {
                saveUndoState();
                heldPawns++;
                map[originTile.x, originTile.y].pawnCount--;
                pickupPitchUpDuration = 6f;

                speedPickupPitchModifier++;

                GameObject snd = Instantiate(pickUpSound, transform.position, Quaternion.identity);
                soundObject sndObjScrpt = snd.GetComponent<soundObject>();
                sndObjScrpt.playSound(1f + (speedPickupPitchModifier / 19));
                
            }
            mouseFollower.setCarryNum(heldPawns);
            updateMap();
        }
        if (moved) // turn into courontine and split final part of function into seperate function
        {
            
            StartCoroutine(conveyorBeltProcedure());
            updateMap();
            
        }
       
    }
    [SerializeField] private GameObject yellowSoundObj;
    void finishInteractLogic()
    {
        listeningForInputs = true;
        if (checkForNewTurn()) // all moves have been done
        {
            if (yellowCommandsToBeAdded > 0)
            {
                GameObject snd = Instantiate(yellowSoundObj, transform.position, Quaternion.identity);
                soundObject sndObjScrpt = snd.GetComponent<soundObject>();
                sndObjScrpt.playSound(1f);
                tileColor yellow = tileColor.Yellow;
                for (int i = 0; i < yellowCommandsToBeAdded; i++)
                {
                    moveCommands.Insert(currentCommandDepth + 1, yellow);
                    currentCommandDepth++;
                }
                
                yellowCommandsToBeAdded = 0;
            }
            yellowCommandsUsed = 0;
            if (checkLose())
            {

                loseText.SetActive(true);
                listeningForInputs = false;
                gameOver = true;
                return;
            }
            if (pawnsSpawned >= currentLevel.pawnMax)
            {
                if (checkWin())
                {
                    gameWin = true;
                    winText.SetActive(true);
                    musicMan.startStop(false);
                    Invoke("winSoundPlay", .16f);
                    
                }
            }
            tutorialHandler.checkForUpdateGraphic(level);
            newTurn();
        }
    }
    void winSoundPlay()
    {
        GameObject snd = Instantiate(winSound, transform.position, Quaternion.identity);
        soundObject sndObjScrpt = snd.GetComponent<soundObject>();
        sndObjScrpt.playSound(1f);
        Invoke("restartMusic", 5f);
    }
    void restartMusic()
    {
        musicMan.swapTrack();
        musicMan.startStop(true);
    }
    IEnumerator conveyorBeltProcedure()
    {
        listeningForInputs = false;
        yield return new WaitForSeconds(.25f);
        bool hasAutoMover = false;
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                if ((int)map[x, y].autoMover != 0 && map[x, y].pawnCount > 0)
                {
                    StartCoroutine(autoMoverMove(new Vector3Int(x,y,0)));
                }
            }
            
        }
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                if ((int)map[x, y].autoMover != 0 && map[x, y].pawnCount > 0)
                {
                    hasAutoMover = true;
                    break;
                }
                if (hasAutoMover) { break; }
            }

        }
        updateMap();
        if (hasAutoMover) // if still has pieces on an automover, repeat step
        {
            StartCoroutine(conveyorBeltProcedure());
        }
        else
        {
            finishInteractLogic();
        }
    }
    IEnumerator autoMoverMove(Vector3Int origin)
    {
        yield return new WaitForSeconds(.24f);
        GameObject snd = Instantiate(conveyorSound, transform.position, Quaternion.identity);
        soundObject sndObjScrpt = snd.GetComponent<soundObject>();
        sndObjScrpt.playSound(1f);
        int direction = (int)map[origin.x, origin.y].autoMover;
        switch (direction) 
        {
            case 1: // down
                {
                    map[origin.x, origin.y - 1].pawnCount += map[origin.x, origin.y].pawnCount;
                    map[origin.x, origin.y].pawnCount = 0;
                    if (map[origin.x, origin.y - 1].hasYellowCommand)
                    {
                        yellowCommandsToBeAdded++;
                        map[origin.x, origin.y - 1].hasYellowCommand = false;
                    }
                    break;
                }
            case 2: // right
                {
                    map[origin.x + 1, origin.y].pawnCount += map[origin.x, origin.y].pawnCount;
                    map[origin.x, origin.y].pawnCount = 0;
                    if (map[origin.x + 1, origin.y].hasYellowCommand)
                    {
                        yellowCommandsToBeAdded++;
                        map[origin.x + 1, origin.y].hasYellowCommand = false;
                    }
                    break;
                }
            case 3: // up
                {
                    map[origin.x, origin.y + 1].pawnCount += map[origin.x, origin.y].pawnCount;
                    map[origin.x, origin.y].pawnCount = 0;
                    if (map[origin.x, origin.y + 1].hasYellowCommand)
                    {
                        yellowCommandsToBeAdded++;
                        map[origin.x, origin.y + 1].hasYellowCommand = false;
                    }
                    break;
                }
            case 4: // left
                {
                    map[origin.x - 1, origin.y].pawnCount += map[origin.x, origin.y].pawnCount;
                    map[origin.x, origin.y].pawnCount = 0;
                    if (map[origin.x - 1, origin.y].hasYellowCommand)
                    {
                        yellowCommandsToBeAdded++;
                        map[origin.x - 1, origin.y].hasYellowCommand = false;
                    }
                    break;
                }
            default:
                break;
        }
    }
    bool hitsWall(Vector3Int destination, Vector3Int origin)
    {
        Vector3Int direction = (destination - origin);
        int directionInt = 0;
        if (direction.y < 0 && direction.x == 0)
        {
            directionInt = 0;
        }
        else if (direction.x > 0 && direction.y == 0) 
        {
            directionInt = 1;
        }else if (direction.y > 0 && direction.x == 0)
        {
            directionInt = 2;
        }if (direction.x < 0 && direction.y == 0)
        {
            directionInt= 3;
        }
        if (map[origin.x, origin.y].hasWall[directionInt])
        {
            return true;
        }
        if (directionInt == 0) { directionInt = 2; } else if (directionInt == 1) { directionInt = 3; } else if (directionInt == 2) { directionInt = 0; } else if (directionInt == 3) { directionInt = 1; }
        if (map[destination.x, destination.y].hasWall[directionInt])
        {
            return true;
        }
        return false;
        
    }
    bool hitsReturningConveyor(Vector3Int destination, Vector3Int origin)
    {
        Vector3Int direction = (destination - origin);
        int directionInt = 0;

        if (direction.y < 0 && direction.x == 0) // invert direction int
        {
            directionInt = 3;
        }
        else if (direction.x > 0 && direction.y == 0)
        {
            directionInt = 4;
        }
        else if (direction.y > 0 && direction.x == 0)
        {
            directionInt = 1;
        }
        if (direction.x < 0 && direction.y == 0)
        {
            directionInt = 2;
        }
        if (directionInt == (int)map[destination.x, destination.y].autoMover)
        {
            return true;
        }
        return false;
    }
    [SerializeField] private GameObject winText, loseText, finalRoundText;
    [SerializeField] private TMP_Text loseReasonText;
    [SerializeField] private string[] reasons; 
    bool checkWin()
    {
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                int depAmount = map[x, y].depositAmount;
                int pawnCount = map[x, y].pawnCount;
                if (depAmount > 0 && depAmount != pawnCount)
                {
                    return false;
                }else if (depAmount > 0 && depAmount == pawnCount)
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool checkLose()
    {
        for (int x = 0; x < currentLevel.dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.dimensions.y; y++)
            {
                int depAmount = map[x, y].depositAmount;
                int pawnCount = map[x, y].pawnCount;
                if (depAmount > 0 && pawnCount > 0 && pawnsSpawned < currentLevel.pawnMax)
                {
                    loseReasonText.text = reasons[1];
                    return true;
                }
                if (depAmount > 0 && pawnCount != depAmount && pawnCount > 0)
                {
                    loseReasonText.text = reasons[0];
                    return true;
                }
                
                if (depAmount > 0 && pawnCount != depAmount && pawnsSpawned >= currentLevel.pawnMax)
                {
                    loseReasonText.text = reasons[0];
                    return true;
                }

            }
        }
        return false;
    }
}
