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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(CanvasRenderer))]
    [DefaultExecutionOrder(-196)]
    class TerminalForegroundItem : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private TerminalForeground foreground = null;
        [SerializeField]
        private Texture2D texture;

        private readonly TerminalMesh terminalMesh = new TerminalMesh();

        public TerminalForegroundItem()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override Texture mainTexture => this.texture ?? Texture2D.whiteTexture;

        public TerminalFont Font => this.grid?.Font;

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set => this.grid = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Texture2D Texture
        {
            get => this.texture;
            internal set => this.texture = value;
        }

        public TerminalForeground Foreground
        {
            get => this.foreground;
            internal set => this.foreground = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if (this.foreground != null)
            {
                var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
                var visibleCells = this.foreground.GetCells(this.texture);
                this.terminalMesh.SetForegroundVertices(visibleCells, rect);
                this.terminalMesh.Fill(vh);
            }
        }
    }
}
