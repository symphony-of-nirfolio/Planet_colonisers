using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameResourcesStorage resouces;
    private People people;
    [SerializeField]
    private bool isFirstInColony;
    private List<InBaseObject> objects = new List<InBaseObject>();
    public bool isEnabled = false;

    void Start()
    {
        resouces = new GameResourcesStorage();
        people = new People();
        if (isFirstInColony)
        {
            people.AddPeople(50);
            objects.Add(new InBaseMoxie(this));
            objects.Add(new InBaseVaporator(this));
        }
    }

    private void Update()
    {
        if (isEnabled)
        {
            foreach (InBaseObject obj in objects)
            {
                obj.Work(Time.deltaTime);
            }
        }
        
    }
}
