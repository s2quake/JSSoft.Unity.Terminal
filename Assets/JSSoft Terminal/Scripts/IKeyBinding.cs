using System;
using UnityEngine;

namespace JSSoft.UI
{
    public interface IKeyBinding
    {
        bool Verify(object obj);

        bool Action(object obj);

        EventModifiers Modifiers { get; }

        KeyCode KeyCode { get; }

        Type Type { get; }
    }
}
