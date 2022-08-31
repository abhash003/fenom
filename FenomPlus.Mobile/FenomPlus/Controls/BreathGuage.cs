using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace FenomPlus.Controls
{

    // ------------------------------------------------------------------------------------------------------------------------
    // Breath Flow(lpm)     On the Gauge(degree)    Colour on the UI        Comments	
    // 0 to 1	            0 to 22.5	            White(Blank)
    // 1 to 2.6	            22.5 to 90	            Red zone                16 parts of 4.21875 degree each Far Below Range<2.4
    // 2.6 to 2.7	        90 to 112.5	            Red zone                1 part of 22.5 degree Below Range (<2.7)
    // 2.7 to 2.8	        112.5 to 135.0	        Yellow Zone             1 part of 22.5 degree within Range(low)
    // 2.8 to 3.2	        135.0 to 225.0	        Green Zone              4 parts of 22.5 degree each within range(Optimal)
    // 3.2 to 3.3	        225 to 247.5	        Yellow Zone             1 part of 22.5 degree within range(high)
    // 3.3 to 3.4	        247.5 to 270	        Red zone                1 part of 22.5 degree Above Range(>3.3)
    // 3.4 to 5	            270 to 337.5	        Red zone                16 parts of 4.21875 degree each Far Above Range(>3.6)
    // 5 to 6	            337.5 to 360	        White(Blank)
    // ------------------------------------------------------------------------------------------------------------------------

    public class BreathGuage : SKCanvasView
    {
        public static double LowWhite = 0.0d;
        public static double LowRed = 1.0d;
        public static double LowYellow = 2.7d;
        public static double LowGreen = 2.8d;
        public static double HighGreen = 3.2d;
        public static double HighYellow = 3.3d;
        public static double HighRed = 5.0d;
        public static double HighWhite = 6.00;


        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(float), typeof(BreathGuage));

        public static readonly BindableProperty GuageSizeProperty =
            BindableProperty.Create(nameof(GuageSize), typeof(float), typeof(BreathGuage));

        public static readonly BindableProperty GuageDataProperty =
            BindableProperty.Create(nameof(GuageData), typeof(float), typeof(BreathGuage));

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(BreathGuage), "");

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(BreathGuage), "");

        public static readonly BindableProperty IsStepfiveProperty =
            BindableProperty.Create(nameof(IsStepfive), typeof(bool), typeof(BreathGuage));

        public static readonly BindableProperty IsStepsixProperty =
            BindableProperty.Create(nameof(IsStepsix), typeof(bool), typeof(BreathGuage));

        public static readonly BindableProperty IsStepsevenProperty =
            BindableProperty.Create(nameof(IsStepseven), typeof(bool), typeof(BreathGuage));

        public bool IsStepfive
        {
            get => (bool)GetValue(IsStepfiveProperty);
            set => SetValue(IsStepfiveProperty, value);
        }
        public bool IsStepsix
        {
            get => (bool)GetValue(IsStepsixProperty);
            set => SetValue(IsStepsixProperty, value);
        }
        public bool IsStepseven
        {
            get => (bool)GetValue(IsStepsevenProperty);
            set => SetValue(IsStepsevenProperty, value);
        }

        public float Size
        {
            get => (float)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public float GuageSize
        {
            get => (float)GetValue(GuageSizeProperty);
            set => SetValue(GuageSizeProperty, value);
        }

        public float GuageData
        {
            get => (float)GetValue(GuageDataProperty);
            set => SetValue(GuageDataProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        SKColor hardShadowColor = new SKColor(0, 0, 0, 50);

        float guageStrokeThickness = 20;

        SKPaint backgroundCirclePaint;
        SKPaint guagestickPaint;
        SKPaint guagestickcutPaint;
        SKPaint dangerArcPaint;
        SKPaint warningArcPaint;
        SKPaint safeArcPaint;
        SKPaint arrowPaint;

        public BreathGuage()
        {

            backgroundCirclePaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("ffffff"),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDropShadow(0, 0, 8, 8, hardShadowColor)
            };

            dangerArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#F25C5C"),
                StrokeWidth = guageStrokeThickness,
                IsAntialias = true
            };

            warningArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#F2C744"),
                StrokeWidth = guageStrokeThickness,
                IsAntialias = true
            };

            safeArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#6CBF60"),
                StrokeWidth = guageStrokeThickness,
                IsAntialias = true
            };

            guagestickPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#606063"),
                IsAntialias = true
            };

            guagestickcutPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#ffffff"),
                IsAntialias = true
            };

            arrowPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#FFFFFF").WithAlpha(100),
                IsAntialias = true,
                StrokeWidth = 4
            };
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            if (GuageSize > 0)
            {
                WidthRequest = HeightRequest = GuageSize;
            }

            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            int width = e.Info.Width;
            int height = e.Info.Height;

            canvas.Translate(width / 2, height / 2);
            canvas.Scale(Math.Min(width / 105, height / 260f));
            SKPoint center = new SKPoint(0, 0);

            SKRect bounds = new SKRect(-100, -100, 100, 100);
            bounds.Inflate(-10, -10);

            canvas.DrawCircle(center, 105, backgroundCirclePaint);

            SKPath dangerpath = new SKPath();
            dangerpath.AddArc(bounds, 112.5f, 315);

            SKPath warningpath = new SKPath();
            warningpath.AddArc(bounds, 202.5f, 135);

            SKPath safepath = new SKPath();
            safepath.AddArc(bounds, 225, 90);

            canvas.DrawPath(dangerpath, dangerArcPaint);
            canvas.DrawPath(warningpath, warningArcPaint);
            canvas.DrawPath(safepath, safeArcPaint);

            var arrow = new SKPath();

            if (IsStepfive)
            {
                var arrowheadPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Parse("#FFFFFF").WithAlpha(100),
                    IsAntialias = true
                };
                arrow.AddArc(bounds, 112.5f, 50);
                //Arrow headpath
                SKPath arrowhead = new SKPath();
                arrowhead.RMoveTo(arrow.LastPoint + new SKPoint(3, 8));
                arrowhead.RLineTo(-3, 12);
                arrowhead.RLineTo(13, -6);
                arrowhead.Close();
                canvas.DrawPath(arrow, arrowPaint);
                canvas.DrawCircle(arrow.LastPoint, 8, backgroundCirclePaint);
                canvas.DrawPath(arrowhead, arrowheadPaint);
            }
            else if (IsStepsix)
            {
                var arrowheadPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Parse("#FFFFFF").WithAlpha(100),
                    IsAntialias = true
                };
                arrow.AddArc(bounds, 112.5f, 280);

                //Arrow headpath
                SKPath arrowhead = new SKPath();
                arrowhead.RMoveTo(arrow.LastPoint + new SKPoint(3, 8));
                arrowhead.RLineTo(-3, -12);
                arrowhead.RLineTo(13, -6);
                arrowhead.Close();

                canvas.DrawPath(arrow, arrowPaint);
                canvas.DrawCircle(arrow.LastPoint, 8, backgroundCirclePaint);
                canvas.DrawPath(arrowhead, arrowheadPaint);
            }
            else if (IsStepseven)
            {
                var arrowheadPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Parse("#FFFFFF").WithAlpha(100),
                    IsAntialias = true
                };
                arrow.AddArc(bounds, 112.5f, 153f);
                //Arrow headpath
                SKPath arrowhead = new SKPath();
                arrowhead.RMoveTo(arrow.LastPoint);
                arrowhead.RLineTo(-8, -8);
                arrowhead.RLineTo(0, 16);
                arrowhead.Close();
                canvas.DrawPath(arrow, arrowPaint);
                canvas.DrawPath(arrowhead, arrowheadPaint);
            }

            canvas.Save();
            canvas.Scale(0.5f);
            canvas.Translate(0, -180);
            if (!IsStepfive && !IsStepsix)
            {
                SKPath starPath = SKPath.ParseSvgPathData("m-11,-1.49329l8.32289,0l2.57184,-7.90671l2.57184,7.90671l8.32289,0l-6.73335,4.88656l2.57197,7.90671l-6.73336,-4.8867l-6.73335,4.8867l2.57197,-7.90671l-6.73335,-4.88656l0,0z");
                canvas.DrawPath(starPath, new SKPaint()
                {
                    Color = SKColors.White,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                });

            }
            canvas.Restore();

            DrawNeedle(canvas, GuageData);

            SKPaint textPaint = guagestickPaint;

            string UnitsText = Text;
            float ValueFontSize = 20;

            float textWidth = textPaint.MeasureText(UnitsText);
            textPaint.TextSize = 9f;

            SKRect textBounds = SKRect.Empty;
            textPaint.MeasureText(UnitsText, ref textBounds);

            float xText = -1 * textBounds.MidX;
            float yText = 60 - textBounds.Height;

            // And draw the text
            canvas.DrawText(UnitsText, xText, yText, textPaint);

            // Draw the Value on the display
            var valueText = Value.ToString();
            float valueTextWidth = textPaint.MeasureText(valueText);
            textPaint.TextSize = ValueFontSize;

            textPaint.MeasureText(valueText, ref textBounds);

            xText = -1 * textBounds.MidX;
            yText = 50 - textBounds.Height;

            // And draw the text
            canvas.DrawText(valueText, xText, yText, textPaint);
            canvas.Restore();
        }

        void DrawNeedle(SKCanvas canvas, float value)
        {

            float startangle = -180;
            float angle = 0f;

                 if (value >= 0.0 && value < 1.0) angle = startangle + 22.5f;
            else if (value >= 1.0 && value < 2.6) angle = startangle + 22.5f + ((value - 1) * (67.5f - 0) / (2.6f - 1)) + 0;
            else if (value >= 2.6 && value < 2.7) angle = startangle + 90 + ((value - 2.6f) * (22.5f - 0) / (2.7f - 2.6f)) + 0;
            else if (value >= 2.7 && value < 2.8) angle = startangle + 112.5f + ((value - 2.7f) * (22.5f - 0) / (2.8f - 2.7f)) + 0;
            else if (value >= 2.8 && value < 3.2) angle = startangle + 135 + ((value - 2.8f) * (90 - 0) / (3.2f - 2.8f)) + 0;
            else if (value >= 3.2 && value < 3.3) angle = startangle + 225 + ((value - 3.2f) * (22.5f - 0) / (3.3f - 3.2f)) + 0;
            else if (value >= 3.3 && value < 3.4) angle = startangle + 247.5f + ((value - 3.3f) * (22.5f - 0) / (3.4f - 3.3f)) + 0;
            else if (value >= 3.4 && value < 5.0) angle = startangle + 270 + ((value - 3.4f) * (67.5f - 0) / (5 - 3.4f)) + 0;
            else if (value >= 5.0 && value <= 6.0) angle = startangle + 337.5f;

            canvas.Save();
            canvas.RotateDegrees(angle);

            SKPaint paint = guagestickPaint;

            float needleWidth = 6f;
            float needleHeight = 90f;
            float x = 0f, y = 0f;

            SKPath needleRightPath = new SKPath();
            needleRightPath.MoveTo(x, y);
            needleRightPath.LineTo(x + needleWidth, y);
            needleRightPath.LineTo(x, y - needleHeight);
            needleRightPath.LineTo(x - needleWidth, y);
            needleRightPath.LineTo(x, y);

            canvas.DrawPath(needleRightPath, paint);

            canvas.DrawCircle(0, 0, 6f, paint);
            canvas.DrawCircle(0, 0, 3f, guagestickcutPaint);
            canvas.Restore();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == GuageDataProperty.PropertyName
                || propertyName == TextProperty.PropertyName
                || propertyName == ValueProperty.PropertyName)
            {
                InvalidateSurface();
            }
        }

    }
}