using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour {

    // Public attributes
    public int playerID;
    public int colourIndex = -1;
    public int playerWins = 0;

    // Ready mechanics
    public bool ready = false;
    private bool readyReleased = true;

    // Controls
    private PlayerInput input;
    private InputAction lr;
    private InputAction jump;
    private InputAction start;
    private InputAction triggers;
    private InputAction sabotage;


    // Parameters
    float horizontal_accel_speed = 0.75f;
    float maxvel_x = 6f;

    float maxvel_y = 6f;
    float fly_accel = 1.25f;
    float jumpstrength = 8f;

    float last_jump = 0f;
    float fly_delay = 0.2f;
    float jump_cd = 0.1f;

    public float defaultGravity = 2;

    //Variables for Bouncy Sabotage (4)
    public PhysicsMaterial2D mat_normal;
    public PhysicsMaterial2D mat_bouncy;
    //Variables for Pullback Sabotage (5)
    public float sab_vel_percent = 1;


    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;
    public SpriteRenderer sprite;
    public SpriteRenderer outline;
    public SpriteRenderer sabDisplay;
    private bool grounded;
    public int sabSelected = -1;

    public int sabToUse = -1;

    // HUD
    public GameObject textPrefab;
    public GameObject sab_txt;
    public GameObject icon_prefab;
    public GameObject sab_icon;

    //Sabotage Countdown
    float sabApplyTime;
    private bool sabCalled = false;

    public Vector3 spawnPoint = new Vector3(0, 0, 0);

    private bool triggerDown = false;
    public List<float> playerSabotageDurs = new List<float>();
    private const int GENERAL_SABOTAGE_CD_DUR = 3;
    public float playerGeneralSabCD = 0;

    public int directionScale = 1;

    void Start() {

        // Physics Instantiation
        rigbod = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();
        rigbod.gravityScale = defaultGravity;

        // Input Instantiation
        input = GetComponent<PlayerInput>();
        lr = input.actions["LR"];
        jump = input.actions["Jump"];
        start = input.actions["Start"];
        triggers = input.actions["Triggers"];
        sabotage = input.actions["Sabotage"];

        // Sprite Instantiation
        sprite = GetComponent<SpriteRenderer>();
        SpriteRenderer[] sprites = FindObjectOfType<SpriteRenderer>().GetComponents<SpriteRenderer>();
        foreach (SpriteRenderer spr in sprites) {
            if (spr.CompareTag("Outline")) outline = spr;
            if (spr.CompareTag("SabDisplay")) sabDisplay = spr;
        }
        updatePlayerColour(1);

        // HUD Instantiation
        sab_txt = Instantiate(textPrefab, transform.position, Quaternion.identity);
        sab_txt.GetComponent<TextMeshPro>().text = "";
        sab_icon = Instantiate(icon_prefab, transform.position, Quaternion.identity);

        // Sabotage instantiation
        for (int i = 0; i < Sabotages.sabVars.Count; i++) {
            playerSabotageDurs.Add(-1);
        }

        // Game state control
        GameState.AddPlayer(this);
    }

    private void Update() {
        updateHUD();
        parseTriggers();
        checkReady();
        WaitForCountdown();
    }

    void FixedUpdate() {
        if (GameState.alivePlayers.Contains(this)) {
            // Physics updates
            grounded = IsGrounded();
            MoveAnywhere();

            // Sabotage usage
            tickSabotageTimers();
            CheckForSabotageUse();
        }
    }
    void parseTriggers() {
        if (!GameState.gameStarted && !ready) {
            // Colour Selection
            if (triggers.ReadValue<float>() > 0 && !triggerDown) {
                triggerDown = true;
                updatePlayerColour(1);
            }
            else if (triggers.ReadValue<float>() < 0 && !triggerDown) {
                triggerDown = true;
                updatePlayerColour(-1);
            }
            else if (triggers.ReadValue<float>() == 0) {
                triggerDown = false;
            }
        }

    }

    void checkReady() {
        if (!GameState.gameStarted) {
            if (sabotage.ReadValue<float>() > 0 && readyReleased) {
                ready = !ready;
                readyReleased = false;
                spawnPoint = rigbod.transform.position;
            }
            else if (sabotage.ReadValue<float>() == 0) {
                readyReleased = true;
            }
        }
    }

    bool IsGrounded() {
        // Grounding check for jumps

        float tolerance = 0.025f;
        Vector3 raycast_origin = boxcollider.bounds.center + (Vector3)Vector2.down * boxcollider.bounds.extents.y;
        RaycastHit2D ground_raycast = Physics2D.Raycast(raycast_origin, Math.Sign(rigbod.gravityScale) * Vector2.down, tolerance);
        bool on_ground = false;
        if (ground_raycast.collider != null && rigbod.velocity.y <= 0.05) {
            on_ground = true;
        }
        return (on_ground);
    }

    void MoveAnywhere() {
        // Horizontal acceleration control
        float accel_x = lr.ReadValue<float>();

        accel_x = accel_x * horizontal_accel_speed * directionScale * GameState.gameSpeed;
        if (Math.Abs(rigbod.velocity.x + accel_x) > maxvel_x * sab_vel_percent * GameState.gameSpeed && Math.Sign(accel_x) == Math.Sign(rigbod.velocity.x)) {
            accel_x = 0; // Clipping acceleration vs velocity allows fun side sliding on slanted surfaces
        }

        // Flying and jump control
        float accel_y = 0;

        if (jump.ReadValue<float>() > 0) {
            if (grounded & Time.time > last_jump + (1 / GameState.gameSpeed) * jump_cd) {
                accel_y = jumpstrength * Math.Sign(rigbod.gravityScale) * GameState.gameSpeed;
                last_jump = Time.time;
            }
            else if ((Time.time > last_jump + (1 / GameState.gameSpeed) * fly_delay)) {
                accel_y = fly_accel * Math.Sign(rigbod.gravityScale) * GameState.gameSpeed;
                // Cap only on fly
                if (Math.Abs(rigbod.velocity.y + accel_y) > maxvel_y * GameState.gameSpeed && Math.Sign(accel_y) == Math.Sign(rigbod.velocity.y))
                {
                    accel_y = 0;
                }
            } else
            {
                accel_y = 0;
            }
        }

        // Acceleration Application
        Vector2 acceleration = new Vector2(accel_x, accel_y);
        rigbod.velocity = rigbod.velocity + acceleration;


        // Fly Capping
        if (rigbod.velocity.y >= maxvel_y * GameState.gameSpeed && rigbod.gravityScale > 0) {
            rigbod.velocity = new Vector2(rigbod.velocity.x,
                                            maxvel_y * GameState.gameSpeed);
        }
        else if (rigbod.velocity.y <= -maxvel_y * GameState.gameSpeed && rigbod.gravityScale < 0) {
            rigbod.velocity = new Vector2(rigbod.velocity.x,
                                            -maxvel_y * GameState.gameSpeed);
        }

        // Frictional Slowing
        if (grounded && accel_x == 0) {
            rigbod.velocity = new Vector2(rigbod.velocity.x * 0.95f, rigbod.velocity.y);
        }
    }

    public void updateGravity() {
        int cur_grav_sign = Math.Sign(rigbod.gravityScale);
        rigbod.gravityScale = cur_grav_sign * defaultGravity * GameState.gameSpeed;
    }

    void CheckForSabotageUse() {
        if (GameState.gameStarted && sabotage.ReadValue<float>() > 0) {
            if (!GameState.counting_down) {
                AttemptSabotageUse();
            }
        }
    }

    void WaitForCountdown() {
        if (GameState.counting_down && Time.time - sabApplyTime >= 3 && sabCalled) {
            // Wait for countdown

            //Actually apply the sabotage
            if (GameState.alivePlayers.Contains(this)) {
                Sabotages.ApplySabotage(sabToUse, this);
            }

            // Update CDs
            playerSabotageDurs[sabToUse] = Sabotages.sabVars[sabToUse].dur;
            playerGeneralSabCD = GENERAL_SABOTAGE_CD_DUR;

            // Reset Countdown
            GameState.counting_down = false;
            sabCalled = false;
            GameState.DestroyCountdown();
        }
    }


    void AttemptSabotageUse() {
        if (playerGeneralSabCD == 0 && sabSelected != -1 && !GameState.counting_down) {
            // Start countdown
            GameState.DisplayCountdown(this);
            sabApplyTime = Time.time;
            sabCalled = true;

            //Sabotage to be used
            sabToUse = sabSelected;

            //Reset selected sabotage (the one displayed in HUD)
            sabSelected = -1;
        }
    }

    private void OnBecameInvisible() {
        if (GameState.gameStarted) {
            GameState.RemovePlayer(this);
        }
    }
    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Coin") {

            if (sabSelected == -1) {
                Destroy(col.gameObject);
                sabSelected = Sabotages.GrantSabotage();
            }
            else {
                Destroy(col.gameObject);
            }

        }
    }

    void updateHUD() {

        sab_icon.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        sab_icon.GetComponent<SpriteRenderer>().color = GameState.possibleColours[colourIndex];
        if (GameState.gameStarted) {
            // Display updates while running
            sab_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);

            // Display held sabotage if you have one
            if (sabSelected != -1) {
                sab_icon.GetComponent<SabSprites>().currentSprite = sabSelected;
            }
            else {
                sab_txt.GetComponent<TextMeshPro>().text = "";
                sab_icon.GetComponent<SabSprites>().currentSprite = -1;
            }
        }
        else {
            // Display updates for lobby
            sab_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            if (sab_icon.GetComponent<SabSprites>().currentSprite == -2) {
                sab_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.75f);
            }
            if (ready) {
                sab_txt.GetComponent<TextMeshPro>().text = "Ready!";
            }
            else sab_txt.GetComponent<TextMeshPro>().text = "Wins:" + playerWins;
        }
    }

    void updatePlayerColour(int direction) {
        GameState.GetNextColour(this, direction);
        sprite.color = GameState.possibleColours[colourIndex];
    }

    public void tickSabotageTimers() {

        // Update CDs

        // Update Individual Timers
        for (int i = 0; i < playerSabotageDurs.Count; i++) {
            // Tick timers
            playerSabotageDurs[i] -= Time.fixedDeltaTime;

            // Update durations and reset
            if (playerSabotageDurs[i] < 0 && playerSabotageDurs[i] > -1) {
                Sabotages.ResetSabotage(i, this);
                playerSabotageDurs[i] = -1;
            }
            else if (playerSabotageDurs[i] < -1) {
                playerSabotageDurs[i] = -1;
            }
        }

        // Update general timer
        playerGeneralSabCD -= Time.fixedDeltaTime;
        if (playerGeneralSabCD < 0) playerGeneralSabCD = 0;

    }
}