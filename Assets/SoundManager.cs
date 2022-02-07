using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{


    public List<AudioClip> player1Sounds;
    public List<AudioClip> player2Sounds;
    public List<AudioClip> player3Sounds;

    public List<AudioClip> combiTiles;

    public List<AudioClip> objectSounds;

    public List<List<AudioClip>> sound = new List<List<AudioClip>>();
    // Start is called before the first frame update

    private void Awake()
    {
        sound.Add(player1Sounds);
        sound.Add(player2Sounds);
        sound.Add(player3Sounds);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
