using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{

    public ParticleSystem p1;
    public ParticleSystem p2;
    public ParticleSystem p3;
    public ParticleSystem p4;
    public ParticleSystem p5;

    public List<ParticleSystem> pList = new List<ParticleSystem>();
    public List<Vector3> pPositionList = new List<Vector3>();

    private float counterTime;
    public List<float> counter;

    public bool on;
    public float randomInterval;
    public float spawnCount;

    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        pList.Add(p1);
        pList.Add(p2);
        pList.Add(p3);
        pList.Add(p4);
        pList.Add(p5);

        on = true;
        timer = randomInterval;

        foreach(ParticleSystem p in pList)
        {
            pPositionList.Add(p.transform.position);

        }

        for (int i = 0; i < pList.Count; i++)
        {
            counter.Add(counterTime);
        }
        counterTime = 9;
    }

    void timeParticleSystem()
    {
        int i = 0;
        foreach (ParticleSystem p in pList)
        {
            if (p.isPlaying)
            {
                counter[i] -= Time.deltaTime;
            }
            
            if (counter[i] <= 0)
            {
                var main = p.main;
                main.loop = false;
                //p.Stop();
                //p.Clear();
                counter[i] = counterTime;
            }

            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //timeParticleSystem();
        /*if (timer <= 0)
        {
            if (on)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    int rand = Random.Range(0, pList.Count - 1);
                    //pList[rand].Play();
                    var main = pList[rand].main;
                    main.loop = true;
                }
            }
            timer = randomInterval;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            on = !on;
        }*/

       
    }
}
