using UnityEngine;
using System.Collections;

namespace dassault
{
    public abstract class ConnectionObserver
    {
        public abstract void NotifyNewConnection(Connection cxn);
    }

    public class ConnectionObservable
    {
        protected ConnectionObserver m_observer;

        public void AttachObserver(ConnectionObserver observer)
        {
            m_observer = observer;
        }
    }
}
