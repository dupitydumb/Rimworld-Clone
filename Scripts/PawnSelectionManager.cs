using System.Collections.Generic;
using UnityEngine;

public class PawnSelectionManager : MonoBehaviour
{

    public List<GameObject> pawns = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pawns == null)
        {
            return;
        }

        //Set pawn destination
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            foreach (GameObject pawn in pawns)
            {
                pawn.GetComponent<Pawns>().SetDestination(mousePos);
            }
        }

        //Deselect pawns
        if (Input.GetMouseButtonDown(1))
        {
            DeselectPawns();
        }
    }

    void DeselectPawns()
    {
        //Delete all pawns from the list
        for (int i = 0; i < pawns.Count; i++)
        {
            pawns[i].GetComponent<Pawns>().Deselect();
        }

        
    }


    public void HighLightPawns()
    {
        foreach (GameObject pawn in pawns)
        {
            pawn.GetComponent<Pawns>().HighLight();
        }
    }
}
