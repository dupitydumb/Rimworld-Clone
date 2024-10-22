using UnityEngine;

public class Deer : Animal
{
    public Deer(string name, int age) : base(name, age)
    {
    }

    public override void Speak()
    {
        Debug.Log("The deer makes a sound.");
    }

    //Call every frame
    public void Update()
    {
        MoveToDestination();
        if (destinations.Count == 0 )
        {
            CreateNewDestination();
        }
    }
}
