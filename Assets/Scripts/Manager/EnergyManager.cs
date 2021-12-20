using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{

    public List<Vector3Int> litFields;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Vector3Int f in litFields)
        {

        }
    }

    public bool isInSunLight(Vector3Int cell)
    {
        return litFields.Contains(cell);
    }
}
