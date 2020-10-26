// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal
{
    public static class GameObjectUtility
    {
        public static object Find(string path)
        {
            if (path == "/")
            {
                return SceneManager.GetActiveScene();
            }
            else
            {
                return GameObject.Find(path);
            }
        }

        public static IEnumerable<GameObject> GetChilds(object obj)
        {
            if (obj is Scene scene)
            {
                foreach (var item in scene.GetRootGameObjects())
                {
                    yield return item;
                }
            }
            else if (obj is GameObject gameObject)
            {
                var transform = gameObject.transform;
                for (var i = 0; i < transform.childCount; i++)
                {
                    yield return transform.GetChild(i).gameObject;
                }
            }
            else if (obj is Transform transform)
            {
                for (var i = 0; i < transform.childCount; i++)
                {
                    yield return transform.GetChild(i).gameObject;
                }
            }
            else
            {
                throw new ArgumentException("invalid obj", nameof(obj));
            }
        }
    }
}
