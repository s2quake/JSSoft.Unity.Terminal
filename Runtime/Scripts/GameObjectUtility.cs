////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace JSSoft.Unity.Terminal
{
    public static class GameObjectUtility
    {
        public static string[] SplitPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path.StartsWith("/") == false)
                throw new ArgumentException($"invalid path: '{path}'", nameof(path));

            var ss = Regex.Split(path, @"(?<!\\)/");
            var items = new string[ss.Length - 1];
            for (var i = 1; i < ss.Length; i++)
            {
                items[i - 1] = ss[i];
            }
            return items;
        }

        public static string[] GetPathCompletions(string find)
        {
            if (find == null)
                throw new ArgumentNullException(nameof(find));
            if (find.StartsWith("/") == false)
                throw new ArgumentException($"invalid path: '{find}'", nameof(find));
            var index = find.LastIndexOf('/');
            var parentPath = find.Substring(0, index + 1);
            var parent = GameObjectUtility.Find(parentPath);
            if (parent != null)
            {
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
            var items = SplitPath(path);
            return Find(SceneManager.GetActiveScene(), items, 0);
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

        private static object Find(object obj, string[] paths, int index)
        {
            var path = paths[index];
            if (path == string.Empty)
                return obj;
            foreach (var item in GetChilds(obj))
            {
                if (item.name == path)
                {
                    if (paths.Length <= index + 1)
                        return item;
                    return Find(item, paths, index + 1);
                }
            }
            return null;
        }
    }
}
