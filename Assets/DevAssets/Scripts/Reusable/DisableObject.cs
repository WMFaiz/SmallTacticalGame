using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.activeInHierarchy) 
        {
            gameObject.SetActive(false);
        }
    }
}
