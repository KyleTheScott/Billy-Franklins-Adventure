//using System.Collections;
//using System.Collections.Generic;

using System.Collections.Generic;
using UnityEngine;

public interface IElectrifiable
{
    bool GetElectrified();
    void SetElectrified(bool state);

    List<GameObject> GetConnectedObjects();
}
