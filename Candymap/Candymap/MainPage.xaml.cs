using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Candymap
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {

            InitializeComponent();

            int numButtonsInPattern = 12;
            var widgetWidth = 350;                          //Width of buttons screen                250----350
            int buttonWidth = 60;
            int buttonHeight = 60;
            int smallButtonHeight = 30;
            int smallButtonWidth = 30;
            //int randomnessFactor = 50;
            var curlinessFactor = 60;                       //Change if changing widgetWidth          30----60
            float currentScore = 10000;
            float segmentScore = 1000;


            int heightScale = widgetWidth / numButtonsInPattern;
            int scrollThreshold = heightScale * numButtonsInPattern / 4;
            SkCanvasView.WidthRequest = widgetWidth;
            SkCanvasView2.WidthRequest = widgetWidth;
            MainAbsoluteLayout.WidthRequest = widgetWidth;

            int cbiforcurve = 0;
            var buttons = new List<Button>();
            var skLineList = new List<SKPoint[]>();
            Random rnd = new Random();
            Size size = Device.Info.PixelScreenSize;
            double screenWidth = size.Width;
            double screenHeight = size.Height;
            float traversedScore = 0.0f;
            int TotalIterations = 0;
            bool clipLock = false;
            var lastPoints = new List<SKPoint>();
            var firstPoints = new List<SKPoint>();
            var scorePoints = new List<SKPoint>();
            
            SKPath streetPath = new SKPath();
            SKPath scorePath = new SKPath();
            SKPath collectedPath = new SKPath();
            SKPath clipperPath = new SKPath();
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
                StrokeWidth = 12,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Miter
            };

            SKPaint collectedStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Blue,
                StrokeWidth = 5,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Miter
            };

            SKPaint clipStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 9,
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

                    if (i % 6 != 0)
                    {
                        buttons.Add(new Button
                        {
                            Text = (indexForButtonText + 1).ToString(),
                            WidthRequest = smallButtonWidth,
                            HeightRequest = smallButtonHeight,
                            CornerRadius = smallButtonWidth/2,
                            BackgroundColor = Color.Yellow,
                            HorizontalOptions= LayoutOptions.Center,
                            VerticalOptions= LayoutOptions.Center
                        });
                    }
                    else
                    {
                        buttons.Add(new Button
                        {
                            Text = (indexForButtonText + 1).ToString(),
                            WidthRequest = buttonWidth,
                            HeightRequest = buttonHeight,
                            CornerRadius = buttonWidth/2,
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
                        var y = 600-(currentButtonIndex * heightScale + Math.Sin((i / (numButtonsInPattern / 8.0f)) * Math.PI / 2) * curlinessFactor +10);
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor, +randomnessFactor/2);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttons[currentButtonIndex].Width, buttons[currentButtonIndex].Height));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);

                        //if (i == 0) { xyPoint.Add(new SKPoint((float)(x * widgetWidth) + 30, (float)y + 30)); }
                        //else { xyPoint.Add(new SKPoint((float)(x * widgetWidth), (float)y + 30)); }
                    }
                    else if (i == numButtonsInPattern / 2.0f)
                    {
                        var x = 2 - placementIndex * (2.0 / (numButtonsInPattern));
                        var y = 600-(currentButtonIndex * heightScale + 10);
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor/2, +randomnessFactor);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttons[currentButtonIndex].Width, buttons[currentButtonIndex].Height));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);
                        
                        //xyPoint.Add(new SKPoint(((float)x * widgetWidth) - 30, y + 30));
                    }

                    else
                    {
                        var x = 2 - placementIndex * (2.0 / (numButtonsInPattern));
                        var y = 600-(currentButtonIndex * heightScale + Math.Sin((i / (numButtonsInPattern / 8.0f)) * Math.PI / 2) * curlinessFactor + 10);
                        //var y = currentButtonIndex * heightScale + rnd.Next(-randomnessFactor/2, +randomnessFactor);
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, buttons[currentButtonIndex].Width, buttons[currentButtonIndex].Height));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);
                        
                        //xyPoint.Add(new SKPoint((float)(x * widgetWidth), (float)y + 30));
                    }
                    MainAbsoluteLayout.Children.Add(buttons[currentButtonIndex]);
                }

                int curveHeightScale = widgetWidth / 6;
                for (var i = 0; i < 6; i++)
                {
                    int placementIndex = cbiforcurve;
                    while (placementIndex > 5)
                    {
                        placementIndex = placementIndex - 6;
                    }

                    if (i < 3.0f)
                    {
                        var xforcurve = 0 + placementIndex * (2.0 / (6));
                        var yforcurve = cbiforcurve * curveHeightScale + Math.Sin((i / (6 / 8.0f)) * Math.PI / 2) * curlinessFactor;

                        if (i == 0) { xyPoint.Add(new SKPoint((float)(xforcurve * widgetWidth) + 30, (float)yforcurve + 30)); }
                        else { xyPoint.Add(new SKPoint((float)(xforcurve * widgetWidth), (float)yforcurve + 30)); }
                    }
                    else if (i == 3.0f)
                    {
                        var xforcurve = 2 - placementIndex * (2.0 / (6));
                        var yforcurve = cbiforcurve * curveHeightScale;

                        xyPoint.Add(new SKPoint(((float)xforcurve * widgetWidth) - 30, (float)yforcurve + 30));
                    }
                    else
                    {
                        var xforcurve = 2 - placementIndex * (2.0 / (6));
                        var yforcurve = cbiforcurve * curveHeightScale + Math.Sin((i / (6 / 8.0f)) * Math.PI / 2) * curlinessFactor;

                        xyPoint.Add(new SKPoint((float)(xforcurve * widgetWidth), (float)yforcurve + 30));
                    }
                    cbiforcurve++;
                }

                firstPoints.Add(xyPoint[0]);
                lastPoints.Add(xyPoint[5]);

                for (var j = 0; j < 5; j++)
                {
                    skLineList.Add(new SKPoint[]
                    {
                            xyPoint[j],xyPoint[j+1]
                    });
                }
            }


            void Draw_RandomShape(SKCanvas skCanvas, object sender)
            {
                var donePoints = new List<SKPoint[]>();
                skLineList = skLineList.Distinct().ToList();
                bool firstDone = false;
                //bool secondDone = false;
                int senderIndex = 1;

                /*for (var i = 1; i < lastPoints.Count; i++)
                {
                    var thisLine = new SKPoint[] { new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y), new SKPoint(firstPoints[i].X, firstPoints[i].Y) };
                    //Adding 5->6 to skLineList
                    if (!donePoints.Contains(thisLine))
                    {
                        skLineList.Add(thisLine);
                    }
                }*/

                skLineList = skLineList.Distinct().ToList();
                foreach (var elem in skLineList)
                {
                    if (!donePoints.Contains(elem))
                    {
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

                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                Console.WriteLine(traversedScore+"0-1");
                                traversedScore += segmentScore;
                                if (clipLock)
                                {
                                    pathClipper(elem[0], elem[1], new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, (currentScore - traversedScore));
                                }
                                if (traversedScore + segmentScore >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                            
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

                            if (traversedScore <= currentScore )
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                Console.WriteLine(traversedScore + "2-3");
                                traversedScore += segmentScore;
                                if (clipLock)
                                {
                                    pathClipper(elem[0], elem[1], new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, currentScore - traversedScore);
                                }
                                if (traversedScore + segmentScore >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
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
                            scorePath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);

                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, midlinemidpoint);
                                Console.WriteLine(traversedScore + "1-2 1");
                                traversedScore += segmentScore/2;
                                if (clipLock)
                                {
                                    pathClipper(elem[0], midlinemidpoint, new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, currentScore - traversedScore);                          
                                }
                                if (traversedScore + segmentScore/2 >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(midlinemidpoint);
                                collectedPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                Console.WriteLine(traversedScore + "1-2 2");
                                traversedScore += segmentScore/2;
                                if (clipLock)
                                {
                                    pathClipper(midlinemidpoint, elem[1], new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, currentScore - traversedScore);
                                }
                                if (traversedScore + segmentScore/2 >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                                
                        }
                        
                        
                        if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) < 0 )
                        {  //3 -> 4
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X - 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X + 5, elem[0].Y), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, secondLineElem[1]);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);

                            //secondDone = true;

                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, elem[1]);
                                Console.WriteLine(traversedScore + "3-4");
                                traversedScore += segmentScore;

                                if (clipLock)
                                {
                                    pathClipper(elem[0], elem[1], new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.Clockwise, currentScore - traversedScore);
                                }

                                if (traversedScore + segmentScore >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                        }

                        /*if ((elem[1].Y - elem[0].Y) > 0 && (elem[1].X - elem[0].X) < 0)
                        {
                            Console.WriteLine("reached");
                            //5->6
                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y - 5), new SKPoint(elem[1].X - 5, elem[1].Y) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y + 5), new SKPoint(elem[1].X + 5, elem[1].Y) };
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                            scorePath.MoveTo(elem[0]);
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise,elem[1]);
                            if (traversedScore < currentScore)
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise,elem[1]);
                                traversedScore += 1000;
                            }
                            secondDone = false;
                        }*/

                        if ((elem[1].Y - elem[0].Y) < 0 && (elem[1].X - elem[0].X) < 0)
                        {  //4 -> 5

                            var firstLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y - 5), new SKPoint(elem[1].X, elem[1].Y - 5) };
                            var secondLineElem = new SKPoint[] { new SKPoint(elem[0].X, elem[0].Y + 5), new SKPoint(elem[1].X, elem[1].Y + 5) };
                            var midpoint1 = new SKPoint((firstLineElem[0].X + firstLineElem[1].X) / 2, ((firstLineElem[0].Y + firstLineElem[1].Y) / 2));
                            var midpoint2 = new SKPoint((secondLineElem[0].X + secondLineElem[1].X) / 2, ((secondLineElem[0].Y + secondLineElem[1].Y) / 2));
                            var midlinemidpoint = new SKPoint((elem[0].X + elem[1].X) / 2, (elem[0].Y + elem[1].Y) / 2);

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
                  
                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(elem[0]);
                                collectedPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, midlinemidpoint);
                                Console.WriteLine(traversedScore + "4-5 1");
                                traversedScore += segmentScore/2;
                                if (clipLock)
                                {
                                    pathClipper(elem[0], midlinemidpoint, new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.Clockwise, currentScore - traversedScore);
                                }
                                if (traversedScore + segmentScore/2 >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                            if (traversedScore <= currentScore)
                            {
                                collectedPath.MoveTo(midlinemidpoint);
                                collectedPath.ArcTo(new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, elem[1]);
                                Console.WriteLine(traversedScore + "4-5 2");
                                traversedScore += segmentScore/2;
                                if (clipLock)
                                {
                                    pathClipper(midlinemidpoint, elem[1], new SKPoint(70, 70), 55, SKPathArcSize.Small, SKPathDirection.CounterClockwise, currentScore - traversedScore);
                                }
                                if (traversedScore + segmentScore/2 >= currentScore && !clipLock)
                                {
                                    clipLock = true;
                                }
                            }
                            try
                            {
                                fivetosix(senderIndex);
                                senderIndex++;
                            }
                            catch { }
                        }
                        donePoints.Add(elem);
                    }
                    donePoints.Add(elem);
                }

                void fivetosix(int index) {
                    var thisLine = new SKPoint[] { new SKPoint(lastPoints[index - 1].X, lastPoints[index - 1].Y), new SKPoint(firstPoints[index].X, firstPoints[index].Y) };
                    if (!donePoints.Contains(thisLine))
                    {
                        var firstLineElem = new SKPoint[] { new SKPoint(lastPoints[index - 1].X, lastPoints[index - 1].Y - 5), new SKPoint(firstPoints[index].X - 5, firstPoints[index].Y) };
                        var secondLineElem = new SKPoint[] { new SKPoint(lastPoints[index - 1].X, lastPoints[index - 1].Y + 5), new SKPoint(firstPoints[index].X + 5, firstPoints[index].Y) };
                        //5->6
                        streetPath.MoveTo(firstLineElem[0]);
                        streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                        streetPath.MoveTo(secondLineElem[0]);
                        streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                        scorePath.MoveTo(thisLine[0]);
                        scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise,thisLine[1]);
                        if (traversedScore <= currentScore)
                        {
                            collectedPath.MoveTo(thisLine[0]);
                            collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, thisLine[1]);
                            Console.WriteLine(traversedScore + "5-6");
                            traversedScore += segmentScore;
                            if (clipLock)
                            {
                                pathClipper(thisLine[0], thisLine[1], new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, currentScore - traversedScore);
                            }
                            if (traversedScore + segmentScore >= currentScore && !clipLock)
                            {
                                clipLock = true;
                            }
                        }
                        donePoints.Add(thisLine);
                    }
                }

                void pathClipper(SKPoint point1, SKPoint point2, SKPoint radii, int angle, SKPathArcSize arcSize, SKPathDirection pathDirection, float clipLength) {

                    float onlyLength = segmentScore + clipLength;

                    if (onlyLength == segmentScore)
                    {
                        clipperPath.MoveTo(point1);
                        clipperPath.ArcTo(radii, angle, arcSize, pathDirection, point2);
                    }
                    else
                    {
                        clipperPath.MoveTo(point1);
                        clipperPath.ArcTo(radii, angle, arcSize, pathDirection, point2);
                        var clipPathPoints = clipperPath.GetPoints(max: 4);
                        clipperPath.Reset();

                        if (onlyLength < segmentScore / 2) {

                            clipperPath.MoveTo(clipPathPoints[2]);
                            clipperPath.LineTo(clipPathPoints[1]);   //full line 2 plus some of line 1 from reverse

                            clipperPath.MoveTo(clipPathPoints[1]);
                            clipperPath.LineTo(clipPathPoints[0]);

                            /*var toPoint = distPoint(clipPathPoints[1], clipPathPoints[0], segmentScore/2-onlyLength);
                            clipperPath.LineTo(toPoint);

                            clipperPath.MoveTo(clipPathPoints[2]);*/

                            //clipperPath.ArcTo(radii,angle, arcSize, pathDirection,toPoint);
                        }
                        else if(onlyLength == segmentScore/2)
                        {
                            clipperPath.MoveTo(clipPathPoints[2]);
                            clipperPath.LineTo(clipPathPoints[1]);   //full line 2 

                            //clipperPath.MoveTo(clipPathPoints[2]);
                            //clipperPath.ArcTo(radii, angle, arcSize, pathDirection, clipPathPoints[1]);
                        }
                        else
                        {
                            clipperPath.MoveTo(clipPathPoints[2]);
                            clipperPath.LineTo(clipPathPoints[1]);
                            //clipperPath.MoveTo(clipPathPoints[2]);
                            /*double remLength = segmentScore - onlyLength;

                            var toPoint = distPoint(clipPathPoints[2], clipPathPoints[1], remLength);
                            clipperPath.LineTo(toPoint);            //some line 2 from reverse

                            clipperPath.MoveTo(clipPathPoints[2]);*/
                            //clipperPath.ArcTo(radii, angle, arcSize, pathDirection, toPoint);
                            //totalLength += Math.Sqrt(Math.Pow((clipPathPoints[i].X - clipPathPoints[i + 1].X), 2) + Math.Pow((clipPathPoints[i].Y - clipPathPoints[i + 1].Y), 2));
                            //clipperPath.MoveTo(clipPathPoints[i]);
                            //clipperPath.LineTo(clipPathPoints[i + 1]);

                        }
                        Console.WriteLine("heyheyhey" + onlyLength.ToString());
                    }
                    //skCanvas.ClipPath(clipperPath, SKClipOperation.Difference);
                }

                SKPoint distPoint(SKPoint point1, SKPoint point2, double distance)
                {
                    var t = distance / segmentScore;
                    var x = (1 - t) * point1.X + t * point2.X;
                    var y = (1 - t) * point1.Y + t * point2.Y;
                    return new SKPoint((float)x,(float)y);
                }
                /*try
                index
                    for (var i = 1; i < lastPoints.Count; i++)
                    {
                        //var thisLine = new SKPoint[] { new SKPoint(lastPoints[i].X, lastPoints[i].Y), new SKPoint(firstPoints[i + 1].X, firstPoints[i + 1].Y) };
                        if (!donePoints.Contains(new SKPoint[] { new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y), new SKPoint(firstPoints[i].X, firstPoints[i].Y) }))
                        {
                            /*skLineList.Add(new SKPoint[] {
                                lastPoints[i],firstPoints[i+1]
                            });
                            var firstLineElem = new SKPoint[] { new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y - 5), new SKPoint(firstPoints[i].X - 5, firstPoints[i].Y) };
                            var secondLineElem = new SKPoint[] { new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y + 5), new SKPoint(firstPoints[i].X + 5, firstPoints[i].Y) };
                            //5->6
                            streetPath.MoveTo(firstLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, firstLineElem[1]);
                            streetPath.MoveTo(secondLineElem[0]);
                            streetPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, secondLineElem[1]);

                            scorePath.MoveTo(new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y));
                            scorePath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, new SKPoint(firstPoints[i].X, firstPoints[i].Y));
                            if (traversedScore < currentScore && !collectionLock)
                            {
                                collectedPath.MoveTo(new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y));
                                collectedPath.ArcTo(new SKPoint(100, 100), 45, SKPathArcSize.Small, SKPathDirection.CounterClockwise, new SKPoint(firstPoints[i].X, firstPoints[i].Y));
                                traversedScore += 1000;
                                collectionLock = true;
                            }

                            donePoints.Add(new SKPoint[] { new SKPoint(lastPoints[i - 1].X, lastPoints[i - 1].Y), new SKPoint(firstPoints[i].X, firstPoints[i].Y) });
                        }
                        else { Console.WriteLine("herehere"); }             ///TODO:  Control not reaching here, all 5->6 lines are redrawn 

                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }*/



                /*using (SKPath.RawIterator iterator = scorePath.CreateRawIterator())
                {
                    SKPoint[] points = new SKPoint[8];
                    SKPoint last = new SKPoint(30,30);
                    while (iterator.Next(points) != SKPathVerb.Done)
                    {
                        
                        //scorePath.MoveTo(points[0]);
                            //SKPoint[] linePoints = new SKPoint[] { points[0], points[3] };
                        Console.WriteLine("heyheyhey" + points[0].ToString());
                        skCanvas.DrawLine(points[0], last, streetStroke);
                        last = points[0];
                    }
                }*/

                if(sender == SkCanvasView)
                {
                    skCanvas.DrawPath(collectedPath, collectedStroke);
                    skCanvas.DrawPath(clipperPath, clipStroke);
                }
                else
                {
                    skCanvas.DrawPath(scorePath, scoreStroke);
                    skCanvas.DrawPath(streetPath, streetStroke);
                }
                //skCanvas.ClipPath(clipperPath, SKClipOperation.Intersect);

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
                Draw_RandomShape(skCanvas,sender);
            }

            for (var i = 0; i < 4; i++) { buttonCreater(i); TotalIterations++; }
            //while(MainScrollView.ContentSize.Height < screenHeight) { buttonCreater(TotalIterations); TotalIterations++; }

            SkCanvasView.PaintSurface += SkCanvasView_OnPaintSurface;
            SkCanvasView2.PaintSurface += SkCanvasView_OnPaintSurface;
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
                Console.WriteLine("reached here b");
                if (e.ScrollY > MainScrollView.ContentSize.Height - MainScrollView.Height - 30)
                {
                    Console.WriteLine(e.ScrollY.ToString());
                    buttonCreater(TotalIterations);
                    TotalIterations++;
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    SkCanvasView.InvalidateSurface();
                    SkCanvasView2.InvalidateSurface();
                    //});
                }
            };
        }
    }
}