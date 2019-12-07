// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI.Editor
{
    public class TerminalStyleBehaviourEndNameEditAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText("Assets/JSSoft Terminal/Scripts/Editor/TerminalStyleBehaviour.txt");
            var name = Path.GetFileNameWithoutExtension(pathName);
            var code = text.Replace("public class TerminalStyleBehaviour", $"public class {name}");



            File.WriteAllText(pathName, code);
            AssetDatabase.ImportAsset(pathName);
            // Debug.Log(pathName);
            // Debug.Log(resourceFile);
            //             var mat = (Material)EditorUtility.InstanceIDToObject (instanceId);

            //         //강제적으로 Material을 빨간색으로 설정
            //         mat.color = Color.red;

            //         AssetDatabase.CreateAsset (mat, pathName);
            //         AssetDatabase.ImportAsset (pathName);
            //         ProjectWindowUtil.ShowCreatedAsset (mat);
            // [출처] [에디터 확장 입문] 번역 11장 ProjectWindowUtil|작성자 해머임팩트


        }
    }
}