using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] cubes;
    public Transform[] positions;
    public float timer;
    public float beat;
    int n = 0;
    public int step = 5;
    public TextMeshProUGUI tmp;
    public static int remSeconds;
    public void spawn()
    {
        n++;
        if (n % step != 0) return;
        GameObject cube = Instantiate(cubes[Random.Range(0, 2)], positions[Random.Range(0, positions.Length)]);
        cube.transform.Rotate(new Vector3(0, 0, 1), 90 * Random.Range(0, 4));
        Object.Destroy(cube, 20f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        remSeconds = (int)(Example.songduration - timer);
        string resp = "TIME LEFT\n" + (remSeconds / 60).ToString("00") + ":" + (remSeconds % 60).ToString("00");
        tmp.SetText(resp);

    }
}
