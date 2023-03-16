using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;

public class GameState : MonoBehaviour
{
    //Debug Variables
    public int playersNeededToStart = 2;
    public static float scrollSpeed = 0.05f;

    // State Variables
    public static float gameSpeed = 1;
    public static bool gameStarted = false;
    public static Color lastWinnerColor = Color.white;
    public static Player overallWinner = null;

    // Player Tracking
    public static List<Player> alivePlayers = new List<Player>();
    public static List<Player> deadPlayers = new List<Player>();
    private static int nextPlayerID = 0;
    public static List<Color> possibleColours = new List<Color>();
    private static List<int> coloursInUse = new List<int>();

    //Sabotage Countdown
    public static bool counting_down = false;
    public static GameObject countdownIcon;
    public static float startTime;


    // Camera
    private static Camera mainCamera;
    private Vector3 cameraStartingPos;
    public float countdownDistanceFromCamera = 10f;

    // Generator
    [SerializeField] private Generator generator;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        cameraStartingPos = mainCamera.transform.position;

        // Add possible colours

        List<Tuple<float, float>> sv = new List<Tuple<float, float>>();
        sv.Add(Tuple.Create(0.5f, 1f));
        sv.Add(Tuple.Create(1f, 1f));
        sv.Add(Tuple.Create(1f, 0.5f));

        foreach (Tuple<float, float> sv_pair in sv)
        {
            for (float i = 0; i < 1; i += 0.1f)
            {
                possibleColours.Add(Color.HSVToRGB(i, sv_pair.Item1, sv_pair.Item2));
            }
        }

    }

    private void Update()
    {
        if (counting_down)
        {
            UpdateCountdown();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameStarted)
        {
            checkForReset();
        } else
        {
            checkForGameStart();
        }

    }

    static public void AddPlayer(Player player)
    {
        // Player ID Handling
        player.playerID = nextPlayerID;
        nextPlayerID++;

        alivePlayers.Add(player);

    }

    static public void RemovePlayer(Player player)
    {
        // Reset player sabotages
        ResetPlayerSabotages(player);


        // Remove from alive
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            if (alivePlayers[i] == player)
            {
                alivePlayers.RemoveAt(i);
            }
        }

        deadPlayers.Add(player);
        resetPlayerToWaitingRoom(player);
    }

    static public void GetNextColour(Player player, int direction)
    {
        // Get next index
        int nextIndex = (player.colourIndex + direction) % possibleColours.Count;
        if (nextIndex < 0) nextIndex = possibleColours.Count - 1;

        for (int i = 0; i < coloursInUse.Count; i++)
        {
            if (coloursInUse[i] == player.colourIndex)
            {
                coloursInUse.RemoveAt(i);
                break;
            }
        }

        while (coloursInUse.Contains(nextIndex))
        {
            nextIndex = (nextIndex + direction) % possibleColours.Count;
            if (nextIndex < 0) nextIndex = possibleColours.Count - 1;
        }
        coloursInUse.Add(nextIndex);
        player.colourIndex = nextIndex;
    }

    static public void DisplayCountdown(Player p)
    {
        counting_down = true;
        startTime = Time.time;
        GameObject icon = p.icon_prefab;
        icon.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        var pos = mainCamera.transform.position + mainCamera.transform.forward * 10f + new Vector3(0f, 6f, 0f);
        countdownIcon = Instantiate(icon, pos, Quaternion.identity);
        countdownIcon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        countdownIcon.GetComponent<SabSprites>().currentSprite = p.sabSelected;
        countdownIcon.GetComponent<SpriteRenderer>().sortingOrder = 1;
        countdownIcon.GetComponent<SpriteRenderer>().color = possibleColours[p.colourIndex];
    }

    void UpdateCountdown()
    {
        // Update position
        if (counting_down)
        {
            Vector3 pos = mainCamera.transform.position + mainCamera.transform.forward * 10f + new Vector3(0f, 2f, 0f);
            countdownIcon.transform.position = pos;
        }
        // Make it flash
        MakeFlash();
    }

    void MakeFlash()
    {
        float curTime = Time.time - startTime;
        if ((curTime >= 0.5 && curTime <= 1) || (curTime >= 1.5 && curTime <= 2) || (curTime >= 2.5 && curTime <= 3))
        {
            countdownIcon.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            countdownIcon.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    static public void DestroyCountdown()
    {

        counting_down = false;
        Destroy(countdownIcon);
    }

    void checkForGameStart()
    {
        // Game Start Behaviour
        bool readyToStart = true;
        foreach (Player p in alivePlayers)
        {
            if (!p.ready)
            {
                readyToStart = false;
            }
        }
        if (readyToStart == true && (alivePlayers.Count >= playersNeededToStart))
        {
            gameStarted = true;
        }
    }
    

    void checkForReset()
    {
        if (alivePlayers.Count <= 1 && gameStarted)
        {
            Reset();
        }
    }

    private void Reset()
    {
        resetGameState();
        creditWinningPlayer();
        ResetCamera();
        ResetLevel();
        ResetPlayers();
        ResetSabotages();
        GiveCrown();
        DestroyCountdown();
    }
    void resetGameState()
    {
        gameStarted = false;
        gameSpeed = 1;
    }
    void creditWinningPlayer()
    {

        // Credit winning player (foreach in case of 0)
        foreach (Player p in alivePlayers)
        {
            p.playerWins += 1;
            deadPlayers.Add(p);
            lastWinnerColor = possibleColours[p.colourIndex];
        }
        alivePlayers.Clear();
    }
    void ResetCamera() {
        mainCamera.transform.position = cameraStartingPos;
    }
    void ResetPlayers()
    {
        // Reset players
        gameStarted = false;
        foreach (Player p in deadPlayers)
        {
            p.ready = false;
            p.sabSelected = -1;
            p.rigbod.gravityScale = p.defaultGravity;
            alivePlayers.Add(p);
        }
        foreach (Player p in FindObjectsOfType<Player>()) {
            resetPlayerToLobby(p);
        }

        deadPlayers.Clear();
    }

    static void resetPlayerToLobby(Player player) {
        player.rigbod.position = player.spawnPoint;
    }
    static void resetPlayerToWaitingRoom(Player player) {
        player.rigbod.position = new Vector3(-3,10,0);
    }
    void ResetLevel()
    {
        // Reset level
        foreach (Transform tf in generator.renderedSections)
        {
            Destroy(tf.GameObject());
        }

        generator.renderedSections.Clear();

        // Update for new section
        generator.latestSectionEndPos = generator.Lobby.Find("SectionEnd").position;
        generator.SpawnLevelSection();
    }

    void ResetSabotages() {
        
        foreach (Player p in alivePlayers)
        {
            ResetPlayerSabotages(p);
        }

    }

    static void ResetPlayerSabotages(Player p)
    {
        // Tick reset durations
        for (int i = 0; i < p.playerSabotageDurs.Count; i++)
        {
            p.playerSabotageDurs[i] = -1;
            Sabotages.ResetSabotage(i, p);
        }

        p.playerGeneralSabCD = 0;
    }


    public void GiveCrown() {
        int highscore = 0;
        foreach (Player p in FindObjectsOfType<Player>()) {
            p.sab_icon.GetComponent<SabSprites>().currentSprite = -1;
            if (p.playerWins > highscore) highscore = p.playerWins;
        }
        foreach (Player p in FindObjectsOfType<Player>()) {
            if (p.playerWins == highscore) p.sab_icon.GetComponent<SabSprites>().currentSprite = -2;
        }
    }
}
