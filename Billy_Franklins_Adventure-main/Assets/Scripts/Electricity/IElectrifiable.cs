//using System.Collections;
//using System.Collections.Generic;

using System.Collections.Generic;
using UnityEngine;

public interface IElectrifiable
{
    int GetGroupNum();
    void SetGroupNum(int num);

    bool GetElectrified();
    void SetElectrified(bool state);

    List<GameObject> GetConnectedObjects();
    void ElectrifyConnectedObjects();

}
