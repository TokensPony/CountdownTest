using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LettersBoardController : UdonSharpBehaviour
{

	public TextMeshProUGUI lettersText;
	public string drawnLetters = "";

	[UdonSynced]
	private string globalDrawnLetters = "";

    private char[] vowel = new char[67];
    private char[] consonant = new char[74];
    private int vPos;
    private int cPos;
    void Start()
    {
		populateVowels();
		populateConsonants();
		Debug.Log(new string(vowel));
		Debug.Log(new string(consonant));
		vowel = shuffle(vowel);
		consonant = shuffle(consonant);
		Debug.Log(new string(vowel));
		Debug.Log(new string(consonant));
		lettersText.text = "COUNTDOWN";

		vPos = 0;
		cPos = 0;

		Debug.Log("Letters Board has been summoned!");
    }

	private void populateConsonants()
	{
		//char[] cL = { 'R', 'S', 'T', 'N', 'D', 'L', 'M', 'P', 'C', 'G', 'B', 'F', 'H', 'J', 'K', 'Q', 'V', 'W', 'X', 'Y', 'Z' };
		string cLetters = "RSTNDLMPCGBFHJKQVWXYZ";
		char[] cL = cLetters.ToCharArray();
		//int[] cT = new int[] { 9, 9, 9, 9, 5, 5, 4, 4, 3, 3, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1 };
		string cTotal = "999955443322211111111";
		char[] cT = cTotal.ToCharArray();
		for (int x = 0; x < cL.Length; x++)
		{
			for (int y = 0; y < (int)char.GetNumericValue(cT[x]); y++)
			{
				consonant[cPos] = cL[x];
				cPos++;
			}
		}
	}

	public void populateVowels()
	{
		string vLetters = "EAIOU";
		char[] vL = vLetters.ToCharArray(); //{ 'E', 'A', 'I', 'O', 'U' };//
		int[] vT = new int[] { 21, 15, 13, 13, 5 };
		for (int x = 0; x < vL.Length; x++)
		{
			for (int y = 0; y < vT[x]; y++)
			{
				vowel[vPos] = vL[x];
				vPos++;
			}
		}
	}

	public void drawVowel()
    {
		if (!Networking.IsOwner(this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}
		if (Networking.IsOwner(this.gameObject))
		{
			if (drawnLetters.Length < 9)
			{
				char nextVowel = ' ';
				if (vPos < vowel.Length)
				{
					nextVowel = vowel[vPos];
					vPos++;
					drawnLetters += nextVowel;
					globalDrawnLetters = drawnLetters;
					lettersText.text = drawnLetters;
					RequestSerialization();
				}
				else
				{
					Debug.Log("Reshuffling Vowels");
					shuffle(vowel);
					vPos = 0;
					drawVowel();
				}
			}
		}
	}

	public void drawConsonant()
    {
		if (!Networking.IsOwner(this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}
		if (Networking.IsOwner(this.gameObject))
		{
			if (drawnLetters.Length < 9)
			{
				char nextConsonant = ' ';
				if (cPos < consonant.Length)
				{
					nextConsonant = consonant[cPos];
					cPos++;
					drawnLetters += nextConsonant;
					globalDrawnLetters = drawnLetters;
					lettersText.text = drawnLetters;
					RequestSerialization();
				}
				else
				{
					Debug.Log("Reshuffling Consonant");
					shuffle(consonant);
					cPos = 0;
					drawConsonant();
				}
			}
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
			drawnLetters = "";
			globalDrawnLetters = drawnLetters;
			lettersText.text = drawnLetters;
			RequestSerialization();
		}
    }

    public char[] shuffle(char[] a)
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
		return a;
	}

	public override void OnDeserialization()
	{
		if (!Networking.IsOwner(this.gameObject))
		{
			drawnLetters = globalDrawnLetters;
			lettersText.text = drawnLetters;
		}
	}
}
