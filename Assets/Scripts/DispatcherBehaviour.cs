using UnityEngine;

namespace JSSoft.Communication.Shells
{
    public class DispatcherBehaviour : MonoBehaviour
    {
        void Update()
        {
            Ntreev.Library.Threading.DispatcherScheduler.Current.ProcessAll();
        }

        void OnDestroy()
        {
            Debug.Log("DispatcherBehaviour.OnDestroy");
        }
    }
}