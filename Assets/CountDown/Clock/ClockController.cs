
using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ClockController : UdonSharpBehaviour
{
    private int clockState = 0;
    private float clockNormalizedTime = 0f;
    private float clockThemeTime = 0f;
    private float clockSpeed = 1f;

    [UdonSynced]
    private int globalClockState = 0;
    [UdonSynced]
    private float globalClockNormalizedTime = 0f;
    [UdonSynced]
    private float globalClockThemeTime = 0f;
    [UdonSynced]
    private float globalClockSpeed = 1f;

    [SerializeField] Animator ClockAnimationController;
    [SerializeField] AudioSource clockTheme;

    private float startTime = 0f;
    private float pausedTime = 0f;

    //public GameObject clockHand;

    [UdonSynced]
    private float globalPausedTime = 0f;

    VRCPlayerApi player;

    void Start()
    {
        Debug.Log("Clock has been summoned!");
        
    }

    private void Update()
    {
        if(Networking.IsOwner(this.gameObject) && clockState == 1 && !clockIsPaused())
        {
            if (ClockAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                clockNormalizedTime = ClockAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime;
                globalClockNormalizedTime = clockNormalizedTime;

                clockThemeTime = clockTheme.time;
                globalClockThemeTime = clockThemeTime;
            }
            //Debug.Log("GlobalClockNormalizedTime = " + globalClockNormalizedTime);
            if (Time.time - startTime >= 32f && !clockIsPaused())
            {
                clockState = 0;
                globalClockState = clockState;
                ClockAnimationController.SetInteger("ClockState", clockState);
                clockTheme.gameObject.SetActive(false);
                clockNormalizedTime = 0f;
                globalClockNormalizedTime = clockNormalizedTime;
                
                
            }
            RequestSerialization();

            //Debug.Log("globalClockThemeTime = " + globalClockThemeTime);
        }
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && clockState == 0)
        {
            startTime = Time.time;
            clockState = 1;
            globalClockState = clockState;

            clockNormalizedTime = 0f;
            globalClockNormalizedTime = clockNormalizedTime;

            clockThemeTime = 0f;
            globalClockThemeTime = clockThemeTime;
            clockTheme.time = clockThemeTime;
            //clockTheme.Play();

            ClockAnimationController.SetInteger("ClockState", clockState);

            clockTheme.gameObject.SetActive(true);


            RequestSerialization();
            return;
            //Debug.Log("Clock Interacted with.");
            //Debug.Log("GlobalClockState: " + globalClockState);
            //Debug.Log("ClockAnimationController ClockState: " + ClockAnimationController.GetInteger("ClockState"));
        }
        if ( clockIsPaused())
        {
            Debug.Log("Resuming Clock");
            clockTheme.gameObject.SetActive(true);
            clockTheme.time = clockThemeTime;
            clockSpeed = 1f;
            globalClockSpeed = clockSpeed;
            ClockAnimationController.speed = clockSpeed;

            startTime = Time.time - pausedTime;

            //clockState = 1;
            //globalClockState = clockState;
            RequestSerialization();
        }
    }

    public void stopClock()
    {
        Debug.Log("Entered StopClock()");
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && clockState == 1)
        {
            clockTheme.gameObject.SetActive(false);
            clockSpeed = 0f;
            globalClockSpeed = clockSpeed;
            ClockAnimationController.speed = clockSpeed;

            pausedTime = Time.time - startTime;
            globalPausedTime = pausedTime;
            /*clockState = 2;
            globalClockState = clockState;
            ClockAnimationController.SetInteger("ClockState", clockState);*/
            Debug.Log("Clock Stopped");
            RequestSerialization();
        }
    }

    /**
     * Checks if the clock is in the running animation state **/
    public bool clockIsRunning()
    {
        if(clockState == 1)
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

    /*
     Checks if the clock is currently paused due to a player buzzing in.
     */
    public bool clockIsPaused()
    {
        if(ClockAnimationController.speed == 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
     Forces the clock back to start before the animation cycle is completed. Used for
    both the conundrum and if the crew needs to restart the clco9 k
     */
    public void hardReset()
    {
        Debug.Log("Hard Resetting");
        ClockAnimationController.SetInteger("ClockState", clockState);
        //clockHand.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        clockNormalizedTime = 1f;
        globalClockNormalizedTime = clockNormalizedTime;
        //ClockAnimationController.Play("Running", 0, clockNormalizedTime);
        //ClockAnimationController.StopPlayback();

        //Animator anim = gameObject.GetComponentInChildren<Animator>();
        

        clockState = 0;
        globalClockState = clockState;

        

        

        

        clockThemeTime = 0f;
        globalClockThemeTime = clockThemeTime;
        clockTheme.time = clockThemeTime;
        //clockTheme.Play();

        clockSpeed = 1f;
        globalClockSpeed = clockSpeed;
        ClockAnimationController.speed = clockSpeed;




        clockTheme.gameObject.SetActive(false);
        RequestSerialization();

        //clockState = 0;
        //globalClockState = clockState;

        //ClockAnimationController.SetInteger("ClockState", clockState);
        //RequestSerialization();
    }

    public override void OnDeserialization()
    {
        //TODO: Add code for syncing a paused clock
        if (!Networking.IsOwner(this.gameObject))
        {
            clockState = globalClockState;
            clockNormalizedTime = globalClockNormalizedTime;
            clockThemeTime = globalClockThemeTime;
            clockSpeed = globalClockSpeed;
            ClockAnimationController.speed = clockSpeed;
            ClockAnimationController.SetInteger("ClockState", clockState);

            pausedTime = globalPausedTime;
            startTime = Time.time - pausedTime;
            if (clockState == 1 && !clockIsPaused())
            {
                clockTheme.gameObject.SetActive(true);
                clockTheme.time = clockThemeTime;
            }
            else
            {
                clockTheme.gameObject.SetActive(false);
            }
            //Debug.Log("Deserialization Data Recieved:");
            //Debug.Log("GlobalClockState: " + globalClockState);
            //Debug.Log("ClockAnimationController ClockState: " + ClockAnimationController.GetInteger("ClockState"));

        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        this.player = player;
        if (globalClockState == 1)
        {
            ClockAnimationController.Play("Running", 0, clockNormalizedTime);
            clockTheme.time = clockThemeTime;
            //clockTheme.Play();
        }
    }




}
