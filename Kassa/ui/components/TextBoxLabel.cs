using System.Windows.Forms;
using System.Drawing;
using MaterialSkin.Controls;
using System;

namespace Registrator.ui.components
{
    public class CopyableLabel : MaterialLabel
    {
        private int _selectionStart;
        private int _selectionLength;
        private bool _isSelecting;
        private Point _mouseDownPos;

        // Цвета для выделения (в стиле Material)
        private readonly Color _selectionBackColor = Color.FromArgb(63, 81, 181); // Material Blue 700
        private readonly Color _selectionTextColor = Color.White;

        public CopyableLabel()
        {
            this.Cursor = Cursors.IBeam;
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.Paint += OnPaint;
            this.DoubleClick += OnDoubleClick;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedText();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDownPos = e.Location;
                _selectionStart = GetCharIndexFromPosition(e.Location);
                _selectionLength = 0;
                _isSelecting = true;
                this.Invalidate();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                int currentPos = GetCharIndexFromPosition(e.Location);
                _selectionLength = currentPos - _selectionStart;
                this.Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isSelecting = false;
            if (_selectionLength == 0 && e.Button == MouseButtons.Right)
            {
                ShowContextMenu(e.Location);
            }
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            SelectAll();
            this.Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (_selectionLength != 0)
            {
                // Рисуем выделение
                Rectangle[] selectionRects = GetSelectionRects();
                using (var brush = new SolidBrush(_selectionBackColor))
                {
                    foreach (var rect in selectionRects)
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }

                // Рисуем текст с выделением
                string text = this.Text;
                using (var format = new StringFormat())
                {
                    format.Alignment = GetStringAlignment(this.TextAlign);
                    format.LineAlignment = GetStringAlignment(this.TextAlign);

                    // Рисуем невыделенный текст
                    using (var brush = new SolidBrush(this.ForeColor))
                    {
                        e.Graphics.DrawString(text, this.Font, brush, this.ClientRectangle, format);
                    }

                    // Рисуем выделенный текст
                    if (_selectionLength != 0)
                    {
                        string selectedText = text.Substring(
                            Math.Max(0, _selectionStart),
                            Math.Min(Math.Abs(_selectionLength), text.Length - _selectionStart));

                        RectangleF selectionRect = GetSelectionRect(selectionRects);
                        using (var brush = new SolidBrush(_selectionTextColor))
                        {
                            e.Graphics.DrawString(selectedText, this.Font, brush, selectionRect, format);
                        }
                    }
                }
            }
        }

        private void ShowContextMenu(Point location)
        {
            var menu = new MaterialContextMenuStrip();

            var copyItem = new ToolStripMenuItem("Копировать");
            copyItem.Click += (s, e) => CopySelectedText();
            copyItem.ShortcutKeyDisplayString = "Ctrl+C";
            copyItem.Enabled = _selectionLength != 0;

            var selectAllItem = new ToolStripMenuItem("Выделить всё");
            selectAllItem.Click += (s, e) => SelectAll();

            menu.Items.Add(copyItem);
            menu.Items.Add(selectAllItem);
            menu.Show(this, location);
        }

        public void SelectAll()
        {
            _selectionStart = 0;
            _selectionLength = this.Text.Length;
            this.Invalidate();
        }

        public void CopySelectedText()
        {
            if (_selectionLength != 0 && !string.IsNullOrEmpty(this.Text))
            {
                string selectedText = this.Text.Substring(
                    Math.Max(0, _selectionStart),
                    Math.Min(Math.Abs(_selectionLength), this.Text.Length - _selectionStart));

                Clipboard.SetText(selectedText);

                // Анимация копирования (опционально)
                var originalColor = this.BackColor;
                this.BackColor = Color.FromArgb(240, 240, 240);
                Timer t = new Timer { Interval = 200 };
                t.Tick += (s, e) => { this.BackColor = originalColor; t.Stop(); };
                t.Start();
            }
        }

        private StringAlignment GetStringAlignment(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return StringAlignment.Near;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
                default:
                    return StringAlignment.Center;
            }
        }

        private Rectangle[] GetSelectionRects()
        {
            // Упрощенная реализация - возвращаем весь прямоугольник
            return new[] { new Rectangle(0, 0, this.Width, this.Height) };
        }

        private RectangleF GetSelectionRect(Rectangle[] selectionRects)
        {
            // Упрощенная реализация
            return new RectangleF(0, 0, this.Width, this.Height);
        }

        private int GetCharIndexFromPosition(Point pt)
        {
            // Упрощенная реализация - возвращаем 0
            return 0;
        }
    }
}
