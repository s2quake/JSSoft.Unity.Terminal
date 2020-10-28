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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal
{
    public static class GameObjectUtility
    {
        public static string[] GetCompletions(string find)
        {
            var parentPath = string.Empty;
            var index = find.LastIndexOf('/');
            if (index >= 0)
                parentPath = find.Remove(index);
            var parent = GameObjectUtility.Find(parentPath);
            if (parent != null)
            {
                if (index >= 0)
                    parentPath += "/";
                var query = from item in GetChilds(parent)
                            let path = parentPath + item.name
                            where path.StartsWith(find)
                            select path;
                return query.ToArray();
            }
            return null;
        }

        public static object Find(string path)
        {
            if (path == "/" || path == string.Empty)
            {
                return SceneManager.GetActiveScene();
            }
            else
            {
                return GameObject.Find(path);
            }
        }

        public static object Get(string path)
        {
            return Get(path, typeof(object));
        }

        public static GameObject GetGameObject(string path)
        {
            return Get(path, typeof(GameObject)) as GameObject;
        }

        public static Component[] GetComponents(string path)
        {
            var gameObject = GetGameObject(path);
            return gameObject.GetComponents(typeof(Component));
        }

        public static Component GetComponent(string path, int index)
        {
            var components = GetComponents(path);
            return components[index];
        }

        public static void SetParent(object obj, object parent)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is Scene)
                throw new ArgumentException("invalid obj", nameof(obj));
            var transform = GetTransform(obj);
            var parentTransform = GetTransform(parent);
            transform.SetParent(parentTransform);
        }

        public static string GetPath(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is Scene)
            {
                return "/";
            }
            else if (obj is GameObject gameObject)
            {
                var transform = gameObject.transform;
                var path = transform.name;
                var parent = transform.parent;
                while (parent != null)
                {
                    path += parent.name + "/" + path;
                    parent = transform.parent;
                }
                return "/" + path;
            }
            else if (obj is Transform transform)
            {
                var path = transform.name;
                var parent = transform.parent;
                while (parent != null)
                {
                    path += parent.name + "/" + path;
                    parent = transform.parent;
                }
                return "/" + path;
            }
            else
            {
                throw new ArgumentException("invalid obj", nameof(obj));
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

        private static object Get(string path, Type type)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (Find(path) is object obj && type.IsAssignableFrom(obj.GetType()))
                return obj;
            throw new ArgumentException($"invalid path: '{path}'");
        }

        private static Transform GetTransform(object obj)
        {
            if (obj is GameObject gameObject)
                return gameObject.transform;
            else if (obj is Transform transform)
                return transform;
            else if (obj is Scene || obj == null)
                return null;
            throw new ArgumentException("invalid obj", nameof(obj));
        }
    }
}
