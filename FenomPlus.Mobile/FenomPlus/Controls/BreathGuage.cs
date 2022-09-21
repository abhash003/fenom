using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public class BreathGuage : SKCanvasView
    {
        public static float White1     = BreathGuageValues.White1;
        public static float White1Top  = BreathGuageValues.White1Top;
        public static float Red1       = BreathGuageValues.Red1;
        public static float Red1Top    = BreathGuageValues.Red1Top;
        public static float Red2       = BreathGuageValues.Red2;
        public static float Red2Top    = BreathGuageValues.Red2Top;
        public static float Yellow1    = BreathGuageValues.Yellow1;
        public static float Yellow1Top = BreathGuageValues.Yellow1Top;
        public static float Green1     = BreathGuageValues.Green1;
        public static float Green1Top  = BreathGuageValues.Green3Top;
        public static float Yellow2    = BreathGuageValues.Yellow2;
        public static float Yellow2Top = BreathGuageValues.Yellow2Top;
        public static float Red3       = BreathGuageValues.Red3;
        public static float Red3Top    = BreathGuageValues.Red3Top;
        public static float Red4       = BreathGuageValues.Red4;
        public static float Red4Top    = BreathGuageValues.Red4Top;
        public static float White2     = BreathGuageValues.White2;
        public static float White2Top  = BreathGuageValues.White2Top;


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
            yText = 75 - textBounds.Height;

            // And draw the text
            canvas.DrawText(valueText, xText, yText, textPaint);
            canvas.Restore();
        }

        void DrawNeedle(SKCanvas canvas, float value)
        {

            float startangle = -180;
            float angle = 0f;

                 if (value >= 0.0        && value < White1Top)  angle = startangle + 22.5f;
            else if (value >= White1Top  && value < Red1Top)    angle = startangle + 22.5f  + ((value - White1Top)  * (67.5f - 0) / (Red1Top    - White1Top))  + 0;
            else if (value >= Red1Top    && value < Red2Top)    angle = startangle + 90f    + ((value - Red1Top)    * (22.5f - 0) / (Red2Top    - Red1Top))    + 0;
            else if (value >= Red2Top    && value < Yellow1Top) angle = startangle + 112.5f + ((value - Red2Top)    * (22.5f - 0) / (Yellow1Top - Red2Top))    + 0;
            else if (value >= Yellow1Top && value < Green1Top)  angle = startangle + 135f   + ((value - Yellow1Top) * (90f - 0)   / (Green1Top  - Yellow1Top)) + 0;
            else if (value >= Green1Top  && value < Yellow2Top) angle = startangle + 225f   + ((value - Green1Top)  * (22.5f - 0) / (Yellow2Top - Green1Top))  + 0;
            else if (value >= Yellow2Top && value < Red3Top)    angle = startangle + 247.5f + ((value - Yellow2Top) * (22.5f - 0) / (Red3Top    - Yellow2Top)) + 0;
            else if (value >= Red3Top    && value < Red4Top)    angle = startangle + 270f   + ((value - Red3Top)    * (67.5f - 0) / (Red4Top    - Red3Top))    + 0;
            else if (value >= Red4Top    && value <= White2Top) angle = startangle + 337.5f;

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