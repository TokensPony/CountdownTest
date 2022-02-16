using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ScoreboardController : UdonSharpBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score = 0;

    [UdonSynced]
    private int globalScore = 0;

    void Start()
    {
        scoreText.text = score.ToString();
    }

    private void Update()
    {
        
    }

    //public override void Interact()
    public void increaseScore()
    {
        Debug.Log("Added 1 Point");
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            if (score < 1000)
            {
                score++;
            }
            globalScore = score;
            scoreText.text = score.ToString();
            RequestSerialization();
        }
    }

    public void decreaseScore()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject))
        {
            if (score > 0)
            {
                score--;
            }
            globalScore = score;
            scoreText.text = score.ToString();
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            score = globalScore;
            scoreText.text = score.ToString();
        }
    }

    public void buttonTest()
    {
        Debug.Log("You Pressed the button!");
    }
}
