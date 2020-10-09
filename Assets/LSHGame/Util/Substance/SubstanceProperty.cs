using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public abstract class SubstanceProperty : MonoBehaviour
    {
        internal protected abstract void RecieveData(IDataReciever reciever);

    }

    public interface IDataReciever { }
}
