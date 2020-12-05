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
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    internal class TerminalCell : ITerminalCell
    {
        private int volume;

        public TerminalCell(TerminalRow row, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.Row = row ?? throw new ArgumentNullException(nameof(row));
            this.Index = index;
            this.Reset();
        }

        public override string ToString()
        {
            return $"{{{this.Index},{this.Row.Index}}}: '{this.Character}'";
        }

        public static Color32 GetBackgroundColor(ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var grid = cell.Grid;
            var isCursor = grid.CursorPoint == cell.Point;
            if (cell.IsCursor == true)
                return TerminalGridUtility.GetCursorColor(grid);
            if (cell.IsSelecting == true || cell.IsSelected == true)
                return TerminalGridUtility.GetSelectionColor(grid);
            return cell.BackgroundColor ?? TerminalRow.GetBackgroundColor(cell.Row);
        }

        public static Color32 GetForegroundColor(ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var grid = cell.Grid;
            var isCursor = grid.IsFocused == true && grid.CursorStyle == TerminalCursorStyle.Block && cell.IsCursor == true;
            if (isCursor == true)
                return TerminalGridUtility.GetCursorTextColor(grid);
            if (cell.IsSelecting == true || cell.IsSelected == true)
                return TerminalGridUtility.GetSelectionTextColor(grid);
            return cell.ForegroundColor ?? TerminalRow.GetForegroundColor(cell.Row);
        }

        public bool Intersect(Vector2 position)
        {
            return this.BackgroundRect.Intersect(position);
        }

        public void SetCharacter(TerminalCharacterInfo characterInfo)
        {
            var rect = GetCellRect(this.Grid, this);
            var character = characterInfo.Character;
            var volume = characterInfo.Volume;
            var texture = characterInfo.Texture;
            var indexOfText = characterInfo.TextIndex;
            var backgroundColor = characterInfo.BackgroundColor;
            var foregroundColor = characterInfo.ForegroundColor;
            this.Character = character;
            this.Volume = volume;
            this.Texture = texture;
            this.TextIndex = indexOfText;
            this.BackgroundColor = backgroundColor;
            this.BackgroundRect = new Rect(rect.x, rect.y, rect.width * volume, rect.height);
            this.BackgroundUV = (Vector2.zero, Vector2.zero);
            this.ForegroundColor = foregroundColor;
            this.ForegroundRect = FontUtility.GetForegroundRect(this.Font, character, (int)rect.x, (int)rect.y);
            this.ForegroundUV = FontUtility.GetUV(this.Font, character);
        }

        public void Reset()
        {
            var rect = GetCellRect(this.Grid, this);
            var uv = (Vector2.zero, Vector2.zero);
            this.Character = char.MinValue;
            this.Volume = 1;
            this.TextIndex = -1;
            this.BackgroundColor = null;
            this.ForegroundColor = null;
            this.BackgroundRect = rect;
            this.ForegroundRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            this.BackgroundUV = uv;
            this.ForegroundUV = uv;
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TerminalGrid Grid => this.Row.Grid;

        public TerminalFont Font => this.Grid.Font;

        public Texture2D Texture { get; private set; }

        public char Character { get; private set; }

        public int Volume
        {
            get => this.volume;
            private set
            {
                var row = this.Row;
                var cells = row.Cells;
                var index = this.Index;
                for (var i = 1; i < value; i++)
                {
                    var cell = cells[index + i];
                    cell.Reset();
                    cell.volume = -i;
                }
                this.volume = value;
            }
        }

        public bool IsSelected => TerminalGridUtility.IsSelected(this.Grid, this.Point);

        public bool IsCursor => this.Grid.CursorPoint == this.Point;

        public bool IsSelecting => TerminalGridUtility.IsSelecting(this.Grid, this.Point);

        public Rect BackgroundRect { get; private set; }

        public Rect ForegroundRect { get; private set; }

        public (Vector2, Vector2) BackgroundUV { get; private set; }

        public (Vector2, Vector2) ForegroundUV { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public int TextIndex { get; private set; }

        public TerminalPoint Point => new TerminalPoint(this.Index, this.Row.Index);

        private static Rect GetCellRect(TerminalGrid grid, ITerminalCell cell)
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(grid);
            var x = cell.Index * itemWidth + grid.Padding.Left;
            var y = cell.Row.Index * itemHeight + grid.Padding.Top;
            return new Rect(x, y, itemWidth, itemHeight);
        }

        #region ITerminalCell

        ITerminalRow ITerminalCell.Row => this.Row;

        ITerminalGrid ITerminalCell.Grid => this.Grid;

        #endregion
    }
}
