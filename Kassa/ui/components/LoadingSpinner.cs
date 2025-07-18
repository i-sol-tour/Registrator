using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Registrator.ui.components
{
    public sealed class LoadingSpinner : Control
    {
        private readonly Timer _animTimer;
        private int _rotationAngle;
        private Color _color = Color.FromArgb(63, 81, 181);
        private const int DefaultDiameter = 60;
        private const int DefaultThickness = 5;

        public LoadingSpinner()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.ResizeRedraw |
                   ControlStyles.AllPaintingInWmPaint, true);

            _animTimer = new Timer { Interval = 30 };
            _animTimer.Tick += OnAnimationTick;

            Size = new Size(DefaultDiameter, DefaultDiameter);
            DoubleBuffered = true;
        }

        public Color SpinnerColor
        {
            get => _color;
            set { _color = value; Refresh(); }
        }

        public void Start() => _animTimer.Start();
        public void Stop() => _animTimer.Stop();

        private void OnAnimationTick(object sender, EventArgs e)
        {
            _rotationAngle = (_rotationAngle + 12) % 360;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            int offset = DefaultThickness;
            var rect = new Rectangle(
                offset,
                offset,
                Width - offset * 2,
                Height - offset * 2);

            using (var pen = new Pen(_color, DefaultThickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                e.Graphics.DrawArc(pen, rect, _rotationAngle, 270);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer?.Stop();
                _animTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}