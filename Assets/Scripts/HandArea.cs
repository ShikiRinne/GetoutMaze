using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandArea : MonoBehaviour
{
    [SerializeField]
    private float HandAreaSize = 0;
    [SerializeField]
    private float HandAreaLength = 0;

    void Start()
    {
        transform.localScale = new Vector3(HandAreaSize, HandAreaSize, HandAreaLength);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, HandAreaLength / 2f);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
