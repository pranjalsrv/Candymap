using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Reflection;

namespace Candymap
{

    public partial class MainPage : ContentPage
    {

        public MainPage()
        {

            InitializeComponent();

            int numButtonsInPattern = 6;
            var widgetWidth = 350;                          //Width of buttons screen
            int heightScale = 60;
            Color buttonColor = Color.Default;
            int buttonWidth = 60;
            int buttonHeight = 60;
            //int randomnessFactor = 50;
            var curlinessFactor = 60;


            int scrollThreshold = heightScale * numButtonsInPattern / 4;
            SkCanvasView.WidthRequest = widgetWidth;
            MainAbsoluteLayout.WidthRequest = widgetWidth;


            var buttons = new List<Button>();
            var skLineList = new List<SKPoint[]>();
            Random rnd = new Random();
            Size size = Device.Info.PixelScreenSize;
            double screenWidth = size.Width;
            double screenHeight = size.Height;
            int TotalIterations = 0;
            var lastPoints = new List<SKPoint>();
            var firstPoints = new List<SKPoint>();



            void buttonCreater(int iteration)
            {

                var xyPoint = new List<SKPoint>();


                for (var i = 0; i < numButtonsInPattern; i++)
                {

                    int indexForButtonText = buttons.Count - 1;
                    //if (indexForButtonText == 241) { color = Color.Green; }
                    //else { color = Color.Default; }
                    buttons.Add(new Button
                    {
                        Text = (indexForButtonText + 1).ToString(),
                        WidthRequest = buttonWidth,
                        HeightRequest = buttonHeight,
                        CornerRadius = 30,
                        BackgroundColor = buttonColor
                    });

                    int currentButtonIndex = buttons.Count - 1;
                    int placementIndex = currentButtonIndex;

                    while (placementIndex > numButtonsInPattern - 1) {
                        placementIndex = placementIndex - numButtonsInPattern;
                    }

                    if (i < numButtonsInPattern / 2.0f)
                    {
                        var x = 0 + placementIndex * (2.0 / (numButtonsInPattern));
                        var y = currentButtonIndex * heightScale + Math.Sin((i / (numButtonsInPattern / 8.0f)) * Math.PI / 2) * curlinessFactor;
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor, +randomnessFactor/2);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttonWidth, buttonHeight));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);

                        if (i == 0) { xyPoint.Add(new SKPoint((float)(x * widgetWidth) + 30, (float)y + 30)); }
                        else { xyPoint.Add(new SKPoint((float)(x * widgetWidth), (float)y + 30)); }
                    }
                    else if (i == numButtonsInPattern / 2.0f)
                    {
                        var x = 2 - placementIndex * (2.0 / (numButtonsInPattern));
                        var y = currentButtonIndex * heightScale;
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor/2, +randomnessFactor);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttonWidth, buttonHeight));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);
                        xyPoint.Add(new SKPoint(((float)x * widgetWidth) - 30, y + 30));
                    }

                    else
                    {
                        var x = 2 - placementIndex * (2.0 / (numButtonsInPattern));
                        var y = currentButtonIndex * heightScale + Math.Sin((i / (numButtonsInPattern / 8.0f)) * Math.PI / 2) * curlinessFactor;
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor/2, +randomnessFactor);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttonWidth, buttonHeight));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);

                        xyPoint.Add(new SKPoint((float)(x * widgetWidth), (float)y + 30));
                    }
                    MainAbsoluteLayout.Children.Add(buttons[currentButtonIndex]);
                }

                firstPoints.Add(xyPoint[0]);
                lastPoints.Add(xyPoint[numButtonsInPattern - 1]);

                for (var j = 0; j < numButtonsInPattern - 1; j++)
                {
                    skLineList.Add(new SKPoint[]
                    {
                            xyPoint[j],xyPoint[j+1]
                    });
                }
            }


            void Draw_RandomShape(SKCanvas skCanvas, int curveNumber)
            {
                SKPaint strokePaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black,
                    StrokeWidth = 2,
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round,
                };
                try
                {
                    for (var i = 0; i < lastPoints.Count; i++)
                    {
                        /*skLineList.Add(new SKPoint[] {
                            lastPoints[i],firstPoints[i+1]
                        });*/

                        using (SKPath path = new SKPath())
                        {
                            path.MoveTo(lastPoints[i]);
                            path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstPoints[i + 1]);
                            path.MoveTo(lastPoints[i]);
                            path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstPoints[i + 1]);
                            skCanvas.DrawPath(path, strokePaint);
                        }
                        
                    }
                }
                catch { }

                skLineList = skLineList.Distinct().ToList();
                bool firstDone = false;
                bool secondDone = false;
                var donePoints = new List<SKPoint[]>();
                bool nextis3 = false;

                foreach (var elem in skLineList) {
                    if (!donePoints.Contains(elem))
                    {
                        //skCanvas.DrawLine(elem[0].X, elem[0].Y, elem[1].X, elem[1].Y, strokePaint);
                        using (SKPath path = new SKPath())
                        {
                            if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) > 0 && !firstDone)
                            {  //0 -> 1
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);

                                firstDone = true;
                            }
                            else if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) > 0 && firstDone)
                            {  //2 -> 3
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);

                                firstDone = false;
                            }
                            if ((elem[1].Y - elem[0].Y) < 0 && (elem[1].X - elem[0].X) > 0)
                            {  //1 -> 2
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                            }
                             
                             if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) < 0)
                            {  //3 -> 4
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                
                            }
                            if ((elem[1].Y - elem[0].Y) < 0 && (elem[1].X - elem[0].X) < 0)
                            {  //4 -> 5
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                path.MoveTo(elem[0]);
                                path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                            }
                             


                            /*if (!donePoints.Contains(elem))
                            {
                                if (curveNumber == 0)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                    curveNumber++;
                                }
                                else if (curveNumber == 1)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                    curveNumber++;
                                }
                                else if (curveNumber == 2)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    curveNumber++;
                                }
                                else if (curveNumber == 3)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    curveNumber++;
                                }
                                else if (curveNumber == 4)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    curveNumber++;
                                }
                                else if (curveNumber == 5)
                                {
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    path.MoveTo(elem[0]);
                                    path.ArcTo(new SKPoint(180, 180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                    curveNumber = 0;
                                }*/

                            donePoints.Add(elem);
                            //SKPoint center = new SKPoint(10+(elem[0].X + elem[1].X)/2.0f,10+(elem[0].Y + elem[1].Y) /2.0f);

                            //ArcTo (Single rx, Single ry, Single xAxisRotate, SKPathArcSize largeArc, SKPathDirection sweep, Single x, Single y)
                            /*path.ArcTo(new SKPoint(180,180), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                            path.MoveTo(elem[0]);
                            path.ArcTo(new SKPoint(180,180), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);*/
                            skCanvas.DrawPath(path, strokePaint);
                        }
                }
            }
        }
    

            void SkCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
            {
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas skCanvas = surface.Canvas;

                skCanvas.Clear(SKColors.Transparent);
                var skCanvasWidth = info.Width;
                var skCanvasheight = info.Height;
                
                skCanvas.Scale(skCanvasWidth/(float)widgetWidth);
                int curveNumber = 0;
                Draw_RandomShape(skCanvas, curveNumber);
            }
            
            for (var i = 0; i < 4; i++) { buttonCreater(i); TotalIterations++; }
            //while(MainScrollView.ContentSize.Height < screenHeight) { buttonCreater(TotalIterations); TotalIterations++; }
            
            SkCanvasView.PaintSurface += SkCanvasView_OnPaintSurface;

            /*if(MainScrollView.ScrollY >= (MainScrollView.ContentSize.Height - MainScrollView.Height) + 1) {
                buttonCreater(TotalIterations);
                TotalIterations++;
                //currentYLocation = e.ScrollY;
                Device.BeginInvokeOnMainThread(() => {
                    SkCanvasView.InvalidateSurface();
                });
            }*/

            MainScrollView.Scrolled+= (object sender, ScrolledEventArgs e) => {
                if (e.ScrollY > MainScrollView.ContentSize.Height - MainScrollView.Height -30) {
                    buttonCreater(TotalIterations);
                    TotalIterations++;
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                        SkCanvasView.InvalidateSurface();
                    //});
                }
            };
        }
    }
}