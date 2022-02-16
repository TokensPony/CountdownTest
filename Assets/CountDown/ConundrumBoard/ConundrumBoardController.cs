using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ConundrumBoardController : UdonSharpBehaviour
{
    public TextMeshProUGUI anagramText;
    public TextMeshProUGUI solutionText;
    private string anagram = "";
    private string solution = "";

    public GameObject p1Buzzer;
    public GameObject p2Buzzer;

    public string[] words;

    [UdonSynced]
    private string globalAnagram;
    [UdonSynced]
    private string globalSolution;

    void Start()
    {
        setAnagram("COUNTDOWN");
        setSolution("CONUNDRUM");
        revealAnagram();
        revealSolution();
        RequestSerialization();

        Debug.Log("Conundrum Board ready!");
    }

    public void startConundrum()
    {
        string tempSolution = words[Random.Range(0, words.Length)];
        setSolution("CONUNDRUM");
        setAnagram(shuffle(tempSolution.ToCharArray()));
        revealAnagram();
        RequestSerialization();
        setSolution(tempSolution);
    }

    public void setAnagram(string anagram)
    {
        this.anagram = anagram;
        globalAnagram = anagram;
        RequestSerialization();
    }

    public void revealAnagram()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            anagramText.text = globalAnagram;
            //RequestSerialization();
        }
    }

    public void setSolution(string solution)
    {
        this.solution = solution;
        globalSolution = solution;
        //RequestSerialization();
    }

    public void revealSolution()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            solutionText.text = globalSolution;
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
            setAnagram("COUNTDOWN");
            setSolution("CONUNDRUM");
            revealAnagram();
            revealSolution();
            p1Buzzer.GetComponent<BuzzerController>().reset();
            p2Buzzer.GetComponent<BuzzerController>().reset();
            RequestSerialization();
        }
    }
    //
    public void lockoutPlayer()
    {
        if (p1Buzzer.GetComponent<BuzzerController>().isBuzzedIn())
        {
            p1Buzzer.GetComponent<BuzzerController>().setBuzzedIn(false);
            p1Buzzer.GetComponent<BuzzerController>().setLockOut(true);
            return;
        }

        if (p2Buzzer.GetComponent<BuzzerController>().isBuzzedIn())
        {
            p2Buzzer.GetComponent<BuzzerController>().setBuzzedIn(false);
            p2Buzzer.GetComponent<BuzzerController>().setLockOut(true);
            return;
        }
    }

    public string shuffle(char[] a)
    {
        int n = a.Length;
        for (int i = 0; i < n; i++)
        {
            // between i and n-1
            int r = Random.Range(0, a.Length);
            char tmp = a[i];    // swap
            a[i] = a[r];
            a[r] = tmp;
        }
        return new string(a);
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            anagram = globalAnagram;
            anagramText.text = anagram;
            solution = globalSolution;
            solutionText.text = solution;
        }
    }
}
