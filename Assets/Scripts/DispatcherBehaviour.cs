using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using JSSoft.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

namespace JSSoft.Communication.Shells
{
    public class DispatcherBehaviour : MonoBehaviour
    {
        void Update()
        {
            //Ntreev.Library.Threading.DispatcherScheduler.Current.ProcessAll();
        }

        void OnDestroy()
        {
            Debug.Log("DispatcherBehaviour.OnDestroy");
        }
    }
}