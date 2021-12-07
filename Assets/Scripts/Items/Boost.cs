using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : Item
{
    private void Start()
    {
        this.UsedBy.Boost(this.effectTime);
        this.StartParticles(this.UsedBy.transform);
    }
}
