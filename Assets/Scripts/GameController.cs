using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Networking;
using System;

public class GameController : MonoBehaviour
{
    public UnityEvent Beat;
    public float startBpm;
    public bool beatAction;

    private float wait;

    private float lastTick;

    private Coroutine beatCoroutine;
    [SerializeField] private float leniency;

    [SerializeField] private int WIDTH;
    [SerializeField] private int HEIGHT;
    [SerializeField] private Sprite tileSprite;
    private Tile[,] tiles;

    [SerializeField] string musicPath;

    int currentIndex = 0;

    [SerializeField] AudioSource audioSource;

    private void Start()
    {
        StartCoroutine(LoadAudio(Path.GetFileName(musicPath)));
        tiles = new Tile[WIDTH,HEIGHT];
        //initializePositions
        GameObject tileParent = new GameObject("TileParent");
        //tileParent.transform.position = new Vector3(0f, 0f, .1f);
        float xScale = 88f / WIDTH;
        float yScale = 50f / HEIGHT;
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int y = 0; y < HEIGHT; ++y)
            {
                GameObject newTile = new GameObject("Tile" + x + "," + y);
                tiles[x, y] = newTile.AddComponent<Tile>();
                newTile.transform.parent = tileParent.transform;
                newTile.transform.localPosition = new Vector3(-21.7f + ((2f * x) / (WIDTH * 2f)) * 44f, -12.2f + ((2f * y) / (HEIGHT * 2f)) * 25f);
                newTile.transform.localScale = new Vector3(xScale, yScale);
                newTile.AddComponent<SpriteRenderer>().color = ((x + y) % 2 == 0) ? new Color(.8f, .7f, .7f) : new Color(.6f, .3f, 0f);
                newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
                newTile.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Tiles");
            }
        }
        foreach (Entity entity in Resources.FindObjectsOfTypeAll(typeof(Entity)))
        {
            entity.SetPositionInit(tiles[entity.xPosition, entity.yPosition]);
        }
        foreach (Item item in Resources.FindObjectsOfTypeAll(typeof(Item))) {
            item.SetPositionInit(tiles[item.xPosition, item.yPosition]);
        }
    }

    IEnumerator LoadAudio(string path)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        // Load the MP3 file using UnityWebRequest
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.MPEG);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Failed to load MP3 file: " + www.error);
        }
        else
        {
            // Create AudioClip from downloaded MP3 data
            AudioClip mp3Clip = DownloadHandlerAudioClip.GetContent(www);

            // Play the audio clip
            PlayAudio(mp3Clip, path);
        }
    }

    public void PlayAudio(AudioClip mp3Clip, string fileName)
    {
        audioSource.clip = mp3Clip;
        BeatData data = JsonUtility.FromJson<BeatData>(File.ReadAllText(Application.streamingAssetsPath + "/" + Path.GetFileNameWithoutExtension(fileName) + ".json"));
        StartCoroutine(BeatCoroutine(data));
        audioSource.Play();
    }

    public IEnumerator BeatCoroutine(BeatData data)
    {
        List<double> times = data.beats;
        while (true)
        {
            if (currentIndex == 0)
            {
                yield return new WaitForSeconds((float)times[currentIndex]);
            }
            else
            {
                yield return new WaitForSeconds((float)times[currentIndex] - (float)times[currentIndex - 1]);
            }
            lastTick = Time.time;
            Beat.Invoke();
            beatAction = false;
            if (times.Count == ++currentIndex)
            {
                break;
            }
        }
    }

    public bool CheckInput()
    {
        if (!beatAction && (Mathf.Abs(lastTick - Time.time) < leniency || Mathf.Abs(lastTick + wait - Time.time) < leniency))
        {
            beatAction = true;
            return true;
        }

        return false;
    }

    public List<Tile> CheckCircle(int startX, int startY, int max)
    {
        List<Tile> returnList = new List<Tile>();
        CheckQuarterRecursive(startX, startY, 1, 1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, -1, 1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, 1, -1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, -1, -1, 0, max, returnList);
        return returnList;
    }
    
    void CheckQuarterRecursive(int startX, int startY, int x, int y, int t, int m, List<Tile> r)
    {
        if (t > m || startX >= WIDTH || startY >= HEIGHT || startX < 0 || startY < 0)
        {
            return;
        }
        if (!r.Contains(tiles[startX,startY]))
        {
            r.Add(tiles[startX, startY]);
        }
        CheckQuarterRecursive(startX + x, startY, x, y, t + 1, m, r);
        CheckQuarterRecursive(startX, startY + y, x, y, t + 1, m, r);
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= WIDTH || y >= HEIGHT || x < 0 || y < 0)
        {
            return null;
        }
        return tiles[x,y];
    }
}
