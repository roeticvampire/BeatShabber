using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Example : MonoBehaviour
{
	public static float songduration;
	bool ended = false;
	public static int totalSpawns = 0;
	public static int totalHits = 0;
	public Canvas PreGame;
	public Canvas scoreCard;
	public TextMeshProUGUI timeLeft;
	float timer;
	bool seen;
	Spawner sp;
	AudioSource ad; 
	IEnumerator Start()
	{
		timer = 5.5f;
		scoreCard.enabled = false;
		seen = true;

		ad = GetComponent<AudioSource>(); 
		sp = FindObjectOfType<Spawner>();
		if(Database.song!=null)
		ad.clip = Database.song;
		yield return new WaitForSeconds(5.5f);
		AudioProcessor processor = FindObjectOfType<AudioProcessor>();
		songduration = ad.clip.length; ad.Play();
		PreGame.enabled = false;
		seen = false;
		scoreCard.enabled = true;
		processor.onBeat.AddListener(onOnbeatDetected);
		MouseMovement.shouldRender = true;
	}

    private void Update()
    {
        if (seen&timer>0)
        {
			timer -= Time.deltaTime;
			timeLeft.text = ((int)timer).ToString();

        }
		if (Spawner.remSeconds <= 0 &&!seen&& !ended) { Endgame(); }
    }



    IEnumerator startTimer()
    {
		yield return null;
    }

	public  void Endgame() {
		MouseMovement.shouldRender = false;
		scoreCard.enabled = false;
		PreGame.enabled = true;
		timeLeft.fontSize = 24;
		timeLeft.text = "You Scored: " + (1f * totalHits / totalSpawns).ToString("0.0") + "%";
		StartCoroutine (backToHome());
		ended = true;
	}

	IEnumerator backToHome()
    {
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene(0);

    }





	//this event will be called every time a beat is detected.
	//Change the threshold parameter in the inspector
	//to adjust the sensitivity
	public void onOnbeatDetected()
	{
		sp.spawn();
		totalSpawns++;
	}

	
}