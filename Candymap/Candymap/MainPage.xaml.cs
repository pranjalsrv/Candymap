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
            var widgetWidth = 350;                          //Width of buttons screen               250----350
            Color buttonColor = Color.Transparent;
            int buttonWidth = 60;
            int buttonHeight = 60;
            //int randomnessFactor = 50;
            var curlinessFactor = 60;                       //Change if changing widgetWidth          30----60


            int heightScale = widgetWidth / numButtonsInPattern;
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
            var scorePoints = new List<SKPoint>();
            var donePoints = new List<SKPoint[]>();
            SKPath streetPath = new SKPath();
            SKPath scorePath = new SKPath(); 
            SKPaint streetStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 2,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Miter
            };

            SKPaint scoreStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 10,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Miter
            };


            void buttonCreater(int iteration)
            {

                var xyPoint = new List<SKPoint>();


                for (var i = 0; i < numButtonsInPattern; i++)
                {

                    int indexForButtonText = buttons.Count - 1;
                    //if (indexForButtonText == 241) { color = Color.Green; }
                    //else { color = Color.Default; }

                    if (i % 3 != 0)
                    {
                        buttons.Add(new Button
                        {
                            //Text = (indexForButtonText + 1).ToString(),
                            WidthRequest = buttonWidth,
                            HeightRequest = buttonHeight,
                            CornerRadius = 30,
                            BackgroundColor = buttonColor
                        });
                    }
                    else
                    {
                        buttons.Add(new Button
                        {
                            Text = (indexForButtonText + 1).ToString(),
                            WidthRequest = buttonWidth,
                            HeightRequest = buttonHeight,
                            CornerRadius = 30,
                            BackgroundColor = Color.Green
                        });
                    }

                    int currentButtonIndex = buttons.Count - 1;
                    int placementIndex = currentButtonIndex;

                    while (placementIndex > numButtonsInPattern - 1)
                    {
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

                skLineList = skLineList.Distinct().ToList();
                bool firstDone = false;


                foreach (var elem in skLineList)
                {
                    if (!donePoints.Contains(elem))
                    {
                        ;
                        //skCanvas.DrawLine(elem[0].X, elem[0].Y, elem[1].X, elem[1].Y, strokePaint);

                        if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) > 0 && !firstDone)
                        {  //0 -> 1
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X - 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X + 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                            firstDone = true;
                        }
                        else if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) > 0 && firstDone)
                        {  //2 -> 3
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y + 5), new SKPoint(elem[1].X - 5, elem[1].Y) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y - 5), new SKPoint(elem[1].X + 5, elem[1].Y) };
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, secondLineElem[1]);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);

                            firstDone = false;
                        }
                        if ((elem[1].Y - elem[0].Y) < 0 && (elem[1].X - elem[0].X) > 0)
                        {  //1 -> 2
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y - 5), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y + 5), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            var midpoint1 = new SKPoint((firstLineElem[0].X + firstLineElem[1].X) / 2, ((firstLineElem[0].Y + firstLineElem[1].Y) / 2));
                            var midpoint2 = new SKPoint((secondLineElem[0].X + secondLineElem[1].X) / 2, ((secondLineElem[0].Y + secondLineElem[1].Y) / 2));
                            var midlinemidpoint = new SKPoint((elem[0].X + elem[1].X) / 2, (elem[0].Y + elem[1].Y) / 2);

                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, midpoint1);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, midpoint2);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, midlinemidpoint);

                            streetPath.MoveTo(midpoint1);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, firstLineElem[1]);
                            streetPath.MoveTo(midpoint2);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, secondLineElem[1]);

                            scorePath.MoveTo(midlinemidpoint);
                            scorePath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise,  elem[1]);
                        }

                        if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) < 0)
                        {  //3 -> 4
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X - 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X + 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, secondLineElem[1]);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);

                        }
                        if ((elem[1].Y - elem[0].Y) < 0 && (elem[1].X - elem[0].X) < 0)
                        {  //4 -> 5

                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y - 5), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y + 5), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            var midpoint1 = new SKPoint((firstLineElem[0].X + firstLineElem[1].X) / 2, ((firstLineElem[0].Y + firstLineElem[1].Y) / 2));
                            var midpoint2 = new SKPoint((secondLineElem[0].X + secondLineElem[1].X) / 2, ((secondLineElem[0].Y + secondLineElem[1].Y) / 2));
                            var midlinemidpoint = new SKPoint((elem[0].X+elem[1].X)/2, (elem[0].Y + elem[1].Y) / 2);

                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, midpoint1);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, midpoint2);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, midlinemidpoint);

                            streetPath.MoveTo(midpoint1);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                            streetPath.MoveTo(midpoint2);
                            streetPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                            scorePath.MoveTo(midlinemidpoint);
                            scorePath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                        }
                        donePoints.Add(elem);
                    }
                }

                try
                {
                    for (var i = 0; i < lastPoints.Count; i++)
                    {
                        if (!donePoints.Contains(new SKPoint[] { new SKPoint(lastPoints[i].X, lastPoints[i].Y), new SKPoint(firstPoints[i + 1].X, firstPoints[i + 1].Y) }))
                        {
                            /*skLineList.Add(new SKPoint[] {
                                lastPoints[i],firstPoints[i+1]
                            });*/
                            var firstLineElem = new SKPoint[] { new SKPoint(lastPoints[i].X, lastPoints[i].Y - 5), new SKPoint(firstPoints[i + 1].X - 5, firstPoints[i + 1].Y) };
                            var secondLineElem = new SKPoint[] { new SKPoint(lastPoints[i].X, lastPoints[i].Y + 5), new SKPoint(firstPoints[i + 1].X + 5, firstPoints[i + 1].Y) };
                            //5->6
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                            scorePath.MoveTo(lastPoints[i]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstPoints[i + 1]);

                            donePoints.Add(new SKPoint[] { new SKPoint(lastPoints[i].X, lastPoints[i].Y), new SKPoint(firstPoints[i + 1].X, firstPoints[i + 1].Y) });
                        }
                        else { Console.WriteLine("herehere"); }             ///TODO:  Control not reaching here, all 5->6 lines are redrawn 

                    }
                }
                catch { }

                /*foreach(var p in scorePath.GetPoints(max:20))
                    {
                        var scoreBox = new BoxView { WidthRequest=5,HeightRequest=5,Color=Color.Red };
                        var x = p.X;
                        var y = p.Y;
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor/2, +randomnessFactor);
                        AbsoluteLayout.SetLayoutBounds(scoreBox, new Rectangle(x, y, 5, 5));
                        AbsoluteLayout.SetLayoutFlags(scoreBox, AbsoluteLayoutFlags.None);
                        ScoreLayout.Children.Add(scoreBox);
                    }*/



                skCanvas.DrawPath(scorePath, scoreStroke);
                skCanvas.DrawPath(streetPath, streetStroke);
            }



            void SkCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
            {
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas skCanvas = surface.Canvas;

                skCanvas.Clear(SKColors.Transparent);
                var skCanvasWidth = info.Width;
                var skCanvasheight = info.Height;

                skCanvas.Scale(skCanvasWidth / (float)widgetWidth);
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

            MainScrollView.Scrolled += (object sender, ScrolledEventArgs e) =>
            {
                if (e.ScrollY > MainScrollView.ContentSize.Height - MainScrollView.Height - 30)
                {
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