using System.Collections.Generic;
using UnityEngine;

public interface IElectrifiable
{
    int GetGroupNum();
    void SetGroupNum(int num);

    bool GetElectrified();
    void SetElectrified(bool state);
    bool IsOldElectrifiable();
    void SetIsOldElectrifiable(bool state);

    //bool GetStarted();
    //void SetStarted(bool state);

    List<GameObject> GetConnectedObjects();
    void ElectrifyConnectedObjects();

}
