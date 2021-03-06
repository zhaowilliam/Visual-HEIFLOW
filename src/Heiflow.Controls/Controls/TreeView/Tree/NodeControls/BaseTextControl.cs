//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Heiflow.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl : EditableControl
	{
		private TextFormatFlags _baseFormatFlags;
        private TextFormatFlags _formatFlags;
        private Pen _focusPen;
		private StringFormat _format;

		#region Properties

		private Font _font = null;
		public Font Font
		{
			get
			{
				if (_font == null)
					return Control.DefaultFont;
				else
					return _font;
			}
			set
			{
				if (value == Control.DefaultFont)
					_font = null;
				else
					_font = value;
			}
		}

		protected bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set 
			{ 
				_textAlign = value;
				SetFormatFlags();
			}
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming
		{
			get { return _trimming; }
			set 
			{ 
				_trimming = value;
				SetFormatFlags();
			}
		}

		private bool _displayHiddenContentInToolTip = true;
		[DefaultValue(true)]
		public bool DisplayHiddenContentInToolTip
		{
			get { return _displayHiddenContentInToolTip; }
			set { _displayHiddenContentInToolTip = value; }
		}

		private bool _useCompatibleTextRendering = false;
		[DefaultValue(false)]
		public bool UseCompatibleTextRendering
		{
			get { return _useCompatibleTextRendering; }
			set { _useCompatibleTextRendering = value; }
		}

		[DefaultValue(false)]
		public bool TrimMultiLine
		{
			get;
			set;
		}

		#endregion

		protected BaseTextControl()
		{
			IncrementalSearchEnabled = true;
			_focusPen = new Pen(Color.Black);
			_focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			_format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces);
			_baseFormatFlags = TextFormatFlags.PreserveGraphicsClipping |
						   TextFormatFlags.PreserveGraphicsTranslateTransform;
			SetFormatFlags();
			LeftMargin = 3;
		}

		private void SetFormatFlags()
		{
			_format.Alignment = TextHelper.TranslateAligment(TextAlign);
			_format.Trimming = Trimming;

			_formatFlags = _baseFormatFlags | TextHelper.TranslateAligmentToFlag(TextAlign)
				| TextHelper.TranslateTrimmingToFlag(Trimming);
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context);
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context, GetLabel(node));
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context, string label)
		{
			PerformanceAnalyzer.Start("GetLabelSize");
			CheckThread();
			Font font = GetDrawingFont(node, context, label);
			Size s = Size.Empty;
			if (!UseCompatibleTextRendering)
				s = TextRenderer.MeasureText(label, font);
			else
			{
				SizeF sf = context.Graphics.MeasureString(label, font);
				s = new Size((int)Math.Ceiling(sf.Width), (int)Math.Ceiling(sf.Height));
			}
			PerformanceAnalyzer.Finish("GetLabelSize");

			if (!s.IsEmpty)
				return s;
			else
				return new Size(10, Font.Height);
		}

		protected Font GetDrawingFont(TreeNodeAdv node, DrawContext context, string label)
		{
			Font font = context.Font;
			if (DrawTextMustBeFired(node))
			{
				DrawEventArgs args = new DrawEventArgs(node, this, context, label);
				args.Font = context.Font;
				OnDrawText(args);
				font = args.Font;
			}
			return font;
		}

		protected void SetEditControlProperties(Control control, TreeNodeAdv node)
		{
			string label = GetLabel(node);
			DrawContext context = new DrawContext();
			context.Font = control.Font;
			control.Font = GetDrawingFont(node, context, label);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (context.CurrentEditorOwner == this && node == Parent.CurrentNode)
				return;

			PerformanceAnalyzer.Start("BaseTextControl.Draw");

			Rectangle bounds = GetBounds(node, context);
			Rectangle focusRect = new Rectangle(bounds.X, context.Bounds.Y,	
				bounds.Width, context.Bounds.Height);
            
            DrawEventArgs args;
			CreateBrushes(node, context, out args);

			if (args.BackgroundBrush != null)
				context.Graphics.FillRectangle(args.BackgroundBrush, focusRect);
			if (context.DrawFocus)
			{
				focusRect.Width--;
				focusRect.Height--;
				if (context.DrawSelection == DrawSelectionMode.None)
					_focusPen.Color = SystemColors.ControlText;
				else
					_focusPen.Color = SystemColors.InactiveCaption;
				context.Graphics.DrawRectangle(_focusPen, focusRect);
			}
			
			PerformanceAnalyzer.Start("BaseTextControl.DrawText");
            
            DrawHighLight(context, bounds, args);
            
            if (!UseCompatibleTextRendering)
                TextRenderer.DrawText(context.Graphics, args.Text, args.Font, bounds, args.TextColor, _formatFlags);
            else
               context.Graphics.DrawString(args.Text, args.Font, GetFrush(args.TextColor), bounds, _format);


			PerformanceAnalyzer.Finish("BaseTextControl.DrawText");
			PerformanceAnalyzer.Finish("BaseTextControl.Draw");
		}

        private void DrawHighLight(DrawContext context, Rectangle bounds, DrawEventArgs args)
        {
            string label = args.Text;
            if (!String.IsNullOrEmpty(args.HighLightToken) && label.ToLower().Contains(args.HighLightToken.ToLower()))
            {
                Graphics g = context.Graphics;
                int numChars = label.Length;
                CharacterRange[] characterRanges = new CharacterRange[1];
                int idx = label.ToLower().IndexOf(args.HighLightToken.ToLower());
                //Get the max width.. for the complete length
                SizeF size = g.MeasureString(label, args.Font);

                //Assume the string is in a stratight line, just to work out the 
                //regions. We will adjust the containing rectangles later.
                RectangleF layoutRect =
                    new RectangleF(0.0f, 0.0f, size.Width, size.Height);

                //
                // Set up the array to accept the regions.
                Region[] stringRegions = new Region[numChars];
                for (int i = 0; i < 1; i++)
                    characterRanges[i] = new CharacterRange(idx, args.HighLightToken.Length);

                _format.SetMeasurableCharacterRanges(characterRanges);

                stringRegions = context.Graphics.MeasureCharacterRanges(label, args.Font, bounds, _format);
                RectangleF highLightedBounds = stringRegions[0].GetBounds(g);
                g.FillRectangle(GetFrush(args.HighLightColor), Rectangle.Round(highLightedBounds));
            }
        }

		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color,Brush>();
		private static Brush GetFrush(Color color)
		{
			Brush br;
			if (_brushes.ContainsKey(color))
				br = _brushes[color];
			else
			{
				br = new SolidBrush(color);
				_brushes.Add(color, br);
			}
			return br;
		}

		private void CreateBrushes(TreeNodeAdv node, DrawContext context, out DrawEventArgs args)
		{
            args = new DrawEventArgs(node, this, context, GetLabel(node));
			args.TextColor = SystemColors.ControlText;
            args.BackgroundBrush = null;
			args.Font = context.Font;
			if (context.DrawSelection == DrawSelectionMode.Active)
			{
                args.TextColor = SystemColors.HighlightText;
                args.BackgroundBrush = Parent._highlightColorActiveBrush;
			}
			else if (context.DrawSelection == DrawSelectionMode.Inactive)
			{
                args.TextColor = SystemColors.ControlText;
                args.BackgroundBrush = Parent._highlightColorInactiveBrush;
			}
			else if (context.DrawSelection == DrawSelectionMode.FullRowSelect)
                args.TextColor = SystemColors.HighlightText;

			if (!context.Enabled)
                args.TextColor = SystemColors.GrayText;

			if (DrawTextMustBeFired(node))
                OnDrawText(args);
		}

		public string GetLabel(TreeNodeAdv node)
		{
			if (node != null && node.Tag != null)
			{
				object obj = GetValue(node);
				if (obj != null)
					return FormatLabel(obj);
			}
			return string.Empty;
		}

		protected virtual string FormatLabel(object obj)
		{
			var res = obj.ToString();
			if (TrimMultiLine && res != null)
			{
				string[] parts = res.Split('\n');
				if (parts.Length > 1)
					return parts[0] + "...";
			}
			return res;
		}

		public void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_focusPen.Dispose();
				_format.Dispose();
			}
		}

		/// <summary>
		/// Fires when control is going to draw a text. Can be used to change text or back color
		/// </summary>
		public event EventHandler<DrawEventArgs> DrawText;
		protected virtual void OnDrawText(DrawEventArgs args)
		{
			TreeViewAdv tree = args.Node.Tree;
			if (tree != null)
				tree.FireDrawControl(args);
			if (DrawText != null)
				DrawText(this, args);
		}

		protected virtual bool DrawTextMustBeFired(TreeNodeAdv node)
		{
			return DrawText != null || (node.Tree != null && node.Tree.DrawControlMustBeFired());
		}
	}
}
