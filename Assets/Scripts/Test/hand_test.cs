using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand_test : MonoBehaviour
{
    private List<GameObject> touchitemlist = new List<GameObject>();

    private GameObject notesobj;
    private bool istouchnotes = false;

    void Start()
    {
        
    }

    void Update()
    {
        TouchNotes(notesobj);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Notes"))
        //{
        //    istouchnotes = true;
        //}
        touchitemlist.Add(other.gameObject);
        notesobj = other.gameObject;

        Debug.Log(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.CompareTag("Notes"))
        //{
        //    istouchnotes = false;
        //}
        touchitemlist.Remove(other.gameObject);

        Debug.Log(other.name);
    }

    private void TouchNotes(GameObject touch)
    {
        if (Input.GetMouseButtonDown(0)/* && istouchnotes*/)
        {
            foreach (GameObject notes in touchitemlist)
            {
                if (notes.CompareTag("Notes"))
                {
                    touchitemlist.Remove(notes);
                    Destroy(touch);
                }
            }
        }
    }
}
