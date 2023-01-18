using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace FenomPlus.Controls
{
    public class BreathGauge : SKCanvasView
    {
        public static float White1     = BreathGaugeValues.White1;
        public static float White1Top  = BreathGaugeValues.White1Top;
        public static float Red1       = BreathGaugeValues.Red1;
        public static float Red1Top    = BreathGaugeValues.Red1Top;
        public static float Red2       = BreathGaugeValues.Red2;
        public static float Red2Top    = BreathGaugeValues.Red2Top;
        public static float Yellow1    = BreathGaugeValues.Yellow1;
        public static float Yellow1Top = BreathGaugeValues.Yellow1Top;
        public static float Green1     = BreathGaugeValues.Green1;
        public static float Green1Top  = BreathGaugeValues.Green3Top;
        public static float Yellow2    = BreathGaugeValues.Yellow2;
        public static float Yellow2Top = BreathGaugeValues.Yellow2Top;
        public static float Red3       = BreathGaugeValues.Red3;
        public static float Red3Top    = BreathGaugeValues.Red3Top;
        public static float Red4       = BreathGaugeValues.Red4;
        public static float Red4Top    = BreathGaugeValues.Red4Top;
        public static float White2     = BreathGaugeValues.White2;
        public static float White2Top  = BreathGaugeValues.White2Top;


        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(float), typeof(BreathGauge));

        public static readonly BindableProperty GaugeSizeProperty =
            BindableProperty.Create(nameof(GaugeSize), typeof(float), typeof(BreathGauge));

        public static readonly BindableProperty GaugeDataProperty =
            BindableProperty.Create(nameof(GaugeData), typeof(float), typeof(BreathGauge));

        public static readonly BindableProperty CountDownProperty =
            BindableProperty.Create(nameof(CountDown), typeof(string), typeof(BreathGauge), "");

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(BreathGauge), "");

        public static readonly BindableProperty IsShowStepProperty =
            BindableProperty.Create(nameof(IsShowStep), typeof(bool), typeof(BreathGauge));

        public bool IsShowStep
        {
            get => (bool)GetValue(IsShowStepProperty);
            set => SetValue(IsShowStepProperty, value);
        }

        public float Size
        {
            get => (float)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public float GaugeSize
        {
            get => (float)GetValue(GaugeSizeProperty);
            set => SetValue(GaugeSizeProperty, value);
        }

        public float GaugeData
        {
            get => (float)GetValue(GaugeDataProperty);
            set => SetValue(GaugeDataProperty, value);
        }

        public string CountDown
        {
            get => (string)GetValue(CountDownProperty);
            set => SetValue(CountDownProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        readonly SKColor hardShadowColor = new SKColor(0, 0, 0, 50);

        readonly float gaugeStrokeThickness = 20;

        readonly SKPaint backgroundCirclePaint;
        readonly SKPaint gaugeStickPaint;
        readonly SKPaint gaugeStickCutPaint;
        readonly SKPaint dangerArcPaint;
        readonly SKPaint warningArcPaint;
        readonly SKPaint safeArcPaint;
        readonly SKPaint arrowPaint;
        private readonly SKPaint starPaintFill;
        private readonly SKPaint starPaintStroke;

        public BreathGauge()
        {

            backgroundCirclePaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#FFFFFF"),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDropShadow(0, 0, 8, 8, hardShadowColor)
            };

            dangerArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#F25C5C"),
                StrokeWidth = gaugeStrokeThickness,
                IsAntialias = true
            };

            warningArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#F2C744"),
                StrokeWidth = gaugeStrokeThickness,
                IsAntialias = true
            };


            // Green Zone
            safeArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#6CBF60"),
                StrokeWidth = gaugeStrokeThickness,
                IsAntialias = true
            };

            gaugeStickPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#606063"),
                IsAntialias = true
            };

            gaugeStickCutPaint = new SKPaint()
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

            starPaintFill = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#FFFFFF"),
                IsAntialias = true,
                StrokeWidth = 1
            };

            starPaintStroke = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#000000").WithAlpha(128),
                IsAntialias = true,
                StrokeWidth = 1
            };
        }

        private SKCanvas canvas;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if (GaugeSize > 0)
            {
                WidthRequest = HeightRequest = GaugeSize;
            }

            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            canvas = surface.Canvas;

            canvas.Clear();

            DrawBackground(canvas, e.Info.Width, e.Info.Height);

            DrawStar(canvas, GaugeData);

            DrawNeedle(canvas, GaugeData);

            DrawCountDown(canvas);
        }

        private void DrawBackground(SKCanvas canvas, float width, float height)
        {
            canvas.Translate(width / 2, height / 2);
            canvas.Scale(Math.Min(width / 105, height / 260f));
            SKPoint center = new SKPoint(0, 0);

            SKRect bounds = new SKRect(-100, -100, 100, 100);
            bounds.Inflate(-10, -10);

            canvas.DrawCircle(center, 105, backgroundCirclePaint);

            SKPath dangerPath = new SKPath();
            dangerPath.AddArc(bounds, 112.5f, 315);

            SKPath warningPath = new SKPath();
            warningPath.AddArc(bounds, 202.5f, 135);

            SKPath safePath = new SKPath();
            safePath.AddArc(bounds, 225, 90);

            canvas.DrawPath(dangerPath, dangerArcPaint);
            canvas.DrawPath(warningPath, warningArcPaint);
            canvas.DrawPath(safePath, safeArcPaint);

            var arrow = new SKPath();

            if (IsShowStep)
            {
                var arrowheadPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Parse("#FFFFFF").WithAlpha(100),
                    IsAntialias = true
                };

                arrow.AddArc(bounds, 112.5f, 153f);

                //Arrow head path
                SKPath arrowhead = new SKPath();
                arrowhead.RMoveTo(arrow.LastPoint);
                arrowhead.RLineTo(-8, -8);
                arrowhead.RLineTo(0, 16);
                arrowhead.Close();
                canvas.DrawPath(arrow, arrowPaint);
                canvas.DrawPath(arrowhead, arrowheadPaint);
            }
        }

        void DrawStar(SKCanvas canvas, float value)
        {

            canvas.Save();

            if (GaugeData is > 2.9f and < 3.1f)
            {
                canvas.Scale(0.9f);
                canvas.Translate(0, -100); // 0.7f scale
            }
            else
            {
                canvas.Scale(0.6f);
                canvas.Translate(0, -150); // 0.7f scale
            }

            SKPath starPath = SKPath.ParseSvgPathData("m-11,-1.49329l8.32289,0l2.57184,-7.90671l2.57184,7.90671l8.32289,0l-6.73335,4.88656l2.57197,7.90671l-6.73336,-4.8867l-6.73335,4.8867l2.57197,-7.90671l-6.73335,-4.88656l0,0z");

            canvas.DrawPath(starPath, starPaintFill);
            canvas.DrawPath(starPath, starPaintStroke);

            canvas.Restore();
        }

        void DrawNeedle(SKCanvas canvas, float value)
        {
            float startAngle = -180;
            float angle = 0f;

                 if (value >= 0.0        && value < White1Top)  angle = startAngle + 22.5f;
            else if (value >= White1Top  && value < Red1Top)    angle = startAngle + 22.5f  + ((value - White1Top)  * (67.5f - 0) / (Red1Top    - White1Top))  + 0;
            else if (value >= Red1Top    && value < Red2Top)    angle = startAngle + 90f    + ((value - Red1Top)    * (22.5f - 0) / (Red2Top    - Red1Top))    + 0;
            else if (value >= Red2Top    && value < Yellow1Top) angle = startAngle + 112.5f + ((value - Red2Top)    * (22.5f - 0) / (Yellow1Top - Red2Top))    + 0;
            else if (value >= Yellow1Top && value < Green1Top)  angle = startAngle + 135f   + ((value - Yellow1Top) * (90f - 0)   / (Green1Top  - Yellow1Top)) + 0;
            else if (value >= Green1Top  && value < Yellow2Top) angle = startAngle + 225f   + ((value - Green1Top)  * (22.5f - 0) / (Yellow2Top - Green1Top))  + 0;
            else if (value >= Yellow2Top && value < Red3Top)    angle = startAngle + 247.5f + ((value - Yellow2Top) * (22.5f - 0) / (Red3Top    - Yellow2Top)) + 0;
            else if (value >= Red3Top    && value < Red4Top)    angle = startAngle + 270f   + ((value - Red3Top)    * (67.5f - 0) / (Red4Top    - Red3Top))    + 0;
            else if (value >= Red4Top    && value <= White2Top) angle = startAngle + 337.5f;

            canvas.Save();
            canvas.RotateDegrees(angle);

            SKPaint paint = gaugeStickPaint;

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
            canvas.DrawCircle(0, 0, 3f, gaugeStickCutPaint);
            canvas.Restore();
        }

        private void DrawCountDown(SKCanvas canvas)
        {
            canvas.Save();

            SKPaint textPaint = gaugeStickPaint;

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
            var valueText = CountDown.ToString();
            float valueTextWidth = textPaint.MeasureText(valueText);
            textPaint.TextSize = ValueFontSize;

            textPaint.MeasureText(valueText, ref textBounds);

            xText = -1 * textBounds.MidX;
            yText = 75 - textBounds.Height;

            // And draw the text
            canvas.DrawText(valueText, xText, yText, textPaint);

            canvas.Restore();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == GaugeDataProperty.PropertyName
                || propertyName == TextProperty.PropertyName
                || propertyName == CountDownProperty.PropertyName)
            {
                InvalidateSurface();
            }
        }
    }
}