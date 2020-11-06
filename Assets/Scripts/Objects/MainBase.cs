using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField]
    private GameResouces resouces;
    private People people;
    [SerializeField]
    private bool isFirstInColony;
    private List<InBaseObject> objects = new List<InBaseObject>();

    // Start is called before the first frame update
    void Start()
    {
        if(isFirstInColony)
        {
            people.AddPeople(50);
            objects.Add(new InBaseMoxie());
            objects.Add(new InBaseVaporator());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
