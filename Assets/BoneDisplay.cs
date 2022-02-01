using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoneDisplay : MonoBehaviour
{

    public List<Image> boneDisplays;
    public GameManager gM;
    public Image b1;
    public Image b2;
    public Image b3;
    public Image b4;
    public Image b5;
    public Image b6;
    public Image b7;

    public Sprite empty;
    public Sprite full;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        boneDisplays.Add(b1);
        boneDisplays.Add(b2);
        boneDisplays.Add(b3);
        boneDisplays.Add(b4);
        boneDisplays.Add(b5);
        boneDisplays.Add(b6);
        boneDisplays.Add(b7);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < boneDisplays.Count; i++)
        {
            if (i < gM.bones.Count)
            {
                boneDisplays[i].sprite = full;
            }
        }
    }
}
