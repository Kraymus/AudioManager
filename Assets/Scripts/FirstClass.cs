using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstClass : MonoBehaviour
{
    public string Name;
    [SerializeField] private int Health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PrintInfoFromClass()
    {
        Debug.Log($"[Class] Name: {Name} Health: {Health}");
    }
}
