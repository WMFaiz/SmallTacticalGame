using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShellPool : MonoBehaviour
{
    public List<GameObject> Pools = new List<GameObject>();
    public List<GameObject> _Pools = new List<GameObject>();

    private void Awake()
    {
        foreach (Transform child in gameObject.transform) 
        {
            _Pools.Add(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Pools.Count <= 0)
        {
            foreach (GameObject go in _Pools) 
            {
                go.transform.parent = gameObject.transform;
                go.transform.position = Vector3.zero;
            }
        }
    }
}
