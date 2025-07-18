using System;
using System.Drawing;
using System.Windows.Forms;

namespace Registrator
{
    public class VerticalProgressBar : Control
    {
        private int _value;
        private int _minimum = 0;
        private int _maximum = 100;
        private Color _progressColor = Color.FromArgb(63, 81, 181); // Indigo500

        public int Value
        {
            get { return _value; }
            set
            {
                if (value < Minimum || value > Maximum) return; // Проверка на диапазон
                _value = value;
                Invalidate();
            }
        }

        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value >= Maximum) throw new ArgumentOutOfRangeException("Minimum must be less than Maximum.");
                _minimum = value;
                if (_value < Minimum) Value = Minimum;
                Invalidate();
            }
        }

        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value <= Minimum) throw new ArgumentOutOfRangeException("Maximum must be greater than Minimum.");
                _maximum = value;
                if (_value > Maximum) Value = Maximum;
                Invalidate();
            }
        }

        public Color ProgressColor
        {
            get { return _progressColor; }
            set { _progressColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем фон
            e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, Width, Height);

            // Рисуем прогресс
            float percentage = (float)(Value - Minimum) / (Maximum - Minimum);
            e.Graphics.FillRectangle(new SolidBrush(ProgressColor), 0, 0, Width, Height * percentage);
        }

        protected override Size DefaultSize => new Size(10, 100); // Задаем размер по умолчанию
    }
}