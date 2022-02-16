
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BuzzerController : UdonSharpBehaviour
{
    public GameObject clock;
    public int playerNum;

    public GameObject opponentBuzzer;

    public bool buzzedIn;
    public bool lockedout;

    [UdonSynced]
    public bool globalBuzzedIn;
    [UdonSynced]
    public bool globalLockedout;

    [SerializeField] AudioSource buzzerSound;

    void Start()
    {
        clock = this.transform.parent.gameObject;
        Debug.Log("Player " + playerNum + " buzzer has spawned");
    }

    private void Update()
    {
        
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && !buzzedIn && !lockedout
            && clock.GetComponent<ClockController>().clockIsRunning()  && !opponentBuzzer.GetComponent<BuzzerController>().isBuzzedIn())
        {
            Debug.Log("Player " + playerNum + " buzzed in");
            buzzedIn = true;
            globalBuzzedIn = buzzedIn;
            buzzerSound.gameObject.SetActive(true);
            clock.GetComponent<ClockController>().stopClock();
            RequestSerialization();
        }
    }

    public void setLockOut(bool lockoutState)
    {
        this.lockedout = lockoutState;
        this.globalLockedout = this.lockedout;
        RequestSerialization();
    }

    public void reset()
    {
        buzzedIn = false;
        globalBuzzedIn = buzzedIn;
        lockedout = false;
        globalLockedout = lockedout;
        buzzerSound.gameObject.SetActive(false);
        clock.GetComponent<ClockController>().hardReset();
        RequestSerialization();
    }

    public void setBuzzedIn(bool buzzIn)
    {
        this.buzzedIn = buzzIn;
        this.globalBuzzedIn = this.buzzedIn;
        RequestSerialization();
    }

    public bool isBuzzedIn()
    {
        return buzzedIn;
    }

    public override void OnDeserialization()
    {
        //Add buzzer sound deserialization stuff
        if (!Networking.IsOwner(this.gameObject))
        {
            buzzedIn = globalBuzzedIn;
            lockedout = globalLockedout;
            Debug.Log("[TOKENS] Entered Deserialization. Player " + playerNum + " buzzedIn: "  + buzzedIn.ToString());


            if (buzzedIn)
            {
                buzzerSound.gameObject.SetActive(true);
            }
            else
            {
                buzzerSound.gameObject.SetActive(false);
            }
        }
    }
}
