using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NumbersBoardController : UdonSharpBehaviour
{
    //Can I have UdonSynced Arrays?
    //Can I restrict owner of object to one person once it starts, then make it public?
    public TextMeshProUGUI numbersText;
    public TextMeshProUGUI targetNumText;
    public string drawnNumbers = "";
    public string targetNumber = "";

    public int totalDrawn = 0;

    [UdonSynced]
    public int globalTotalDrawn = 0;

    [UdonSynced]
    public string globalDrawnNumbers = "";
    [UdonSynced]
    public string globalTargetNumber = "";


    //If these are not serialized, then different people will have different
    //position and card stack values.
    public int[] bigNum = new int[4];
    public int[] smallNum = new int[20];
    private int bPos;
    private int sPos;


    void Start()
    {
        populateBig();
        populateSmall();
        bigNum = shuffle(bigNum);
        smallNum = shuffle(smallNum);
        targetNumText.text = "---";
        numbersText.text = "";
        bPos = 0;
        sPos = 0;
        totalDrawn = 0;
        Debug.Log("Numbers board ready!");
    }

    public void populateBig()
    {
        bigNum[0] = 25;
        bigNum[1] = 50;
        bigNum[2] = 75;
        bigNum[3] = 100;
    }

    public void populateSmall()
    {
        int smol = 1;
        for (int x = 0; x < smallNum.Length; x=x+2)
        {
            smallNum[x] = smol;
            smallNum[x+1] = smol;
            smol++;
        }
    }

    public void drawBigNum()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && totalDrawn < 6)
        {
            int output = 0;
            if (bPos < bigNum.Length)
            {
                output = bigNum[bPos];
                bPos++;
                drawnNumbers += output + " ";
                globalDrawnNumbers = drawnNumbers;
                numbersText.text = drawnNumbers;
                totalDrawn++;
                globalTotalDrawn = totalDrawn;
                RequestSerialization();
            }
        }
    }

    public void drawSmallNum()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && totalDrawn < 6)
        {
            int output = 0;
            output = smallNum[sPos];
            sPos++;
            drawnNumbers += output + " ";
            globalDrawnNumbers = drawnNumbers;
            numbersText.text = drawnNumbers;
            totalDrawn++;
            globalTotalDrawn = totalDrawn;
            RequestSerialization();
        }
    }

    public void generateTarget()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            int target = Random.Range(101, 1000);
            targetNumber = target.ToString();
            globalTargetNumber = targetNumber;
            targetNumText.text = targetNumber;
            RequestSerialization();
        }
    }

    public void reset()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            bPos = 0;
            sPos = 0;
            bigNum = shuffle(bigNum);
            smallNum = shuffle(smallNum);
            drawnNumbers = "";
            globalDrawnNumbers = drawnNumbers;
            numbersText.text = drawnNumbers;
            targetNumber = "---";
            globalTargetNumber = targetNumber;
            targetNumText.text = targetNumber;
            totalDrawn = 0;
            globalTotalDrawn = totalDrawn;
            RequestSerialization();
        }
    }

    public int[] shuffle(int[] a)
    {
        int n = a.Length;
        for (int i = 0; i < n; i++)
        {
            // between i and n-1
            int r = Random.Range(0, a.Length);
            int tmp = a[i];    // swap
            a[i] = a[r];
            a[r] = tmp;
        }
        return a;
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            drawnNumbers = globalDrawnNumbers;
            numbersText.text = drawnNumbers;
            targetNumber = globalTargetNumber;
            targetNumText.text = targetNumber;
            totalDrawn = globalTotalDrawn;
        }
    }
}
