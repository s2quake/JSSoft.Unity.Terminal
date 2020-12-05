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

using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public static class ITerminalGridExtensions
    {
        public static Vector2 GetPosition(this ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            return rectTransform.position;
        }

        public static void SetPosition(this ITerminalGrid grid, Vector2 position)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.position = new Vector2(position.x, position.y);
        }

        public static Rect GetRect(this ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var canvas = gameObject.GetComponentInParent<Canvas>();
            var pixelRect = canvas.pixelRect;
            var worldCorners = GetWorldCorners(grid);
            var width = worldCorners[2].x - worldCorners[0].x;
            var height = worldCorners[2].y - worldCorners[0].y;
            var x = worldCorners[0].x;
            var y = pixelRect.height - worldCorners[2].y;
            return new Rect(x, y, width, height);
        }

        private static Vector3[] GetWorldCorners(ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var items = new Vector3[4];
            rectTransform.GetWorldCorners(items);
            return items;
        }
    }
}
