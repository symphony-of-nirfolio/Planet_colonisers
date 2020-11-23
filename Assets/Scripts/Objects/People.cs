using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class People
{
    public int PeopleCount { get; private set; } = 0;

    public People(int peopleCount = 0)
    {
        PeopleCount = peopleCount;
    }

    public void AddPeople(int people)
    {
        Debug.Assert(people >= 0);
        PeopleCount += people;
    }

    public void RemovePeople(int people)
    {
        Debug.Assert(people >= 0);
        Debug.Assert(PeopleCount >= people);
        PeopleCount -= people;
    }
}
