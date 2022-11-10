using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGroupTester : MonoBehaviour
{
    public List<AudioGroupSegment> audioGroupSegments;

    void Reset()
    {
        audioGroupSegments = new List<AudioGroupSegment>(){
             new AudioGroupSegment()
         };
    }
}
