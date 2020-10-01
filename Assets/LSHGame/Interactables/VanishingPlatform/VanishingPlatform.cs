using LogicC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("LSHGame/2D/VanishingPlatform")]
public class VanishingPlatform : Connection
{
    public override string Title => "VanishingPlatform";



    protected override List<BasePort> GetPorts(List<BasePort> ports)
    {
        

        return ports;
    }

    protected override void InputPortUpdate()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
