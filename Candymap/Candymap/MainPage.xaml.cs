using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;


namespace Candymap
{
    
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            
            InitializeComponent();

            bool scaled = false;
            var buttons = new List<Button>();
            var skPointsListc = new List<SKPoint[]>();
            MainAbsoluteLayout.WidthRequest = 400;                          //Width of buttons screen
            double widgetWidth = MainAbsoluteLayout.WidthRequest;

            void buttonCreater(int iteration)
            {
                Size size = Device.Info.PixelScreenSize;
                double screenWidth = size.Width;

                var xyPoint = new List<SKPoint>();
                int numButtonsInPattern = 8;
                
                for (var i = 0; i < numButtonsInPattern; i++)
                {
                    Color color = Color.Default;

                    int indexForButtonText = buttons.Count - 1;
                    //if (indexForButtonText == 241) { color = Color.Green; }
                    //else { color = Color.Default; }
                    buttons.Add(new Button
                    {
                        Text = (indexForButtonText + 1).ToString(),
                        WidthRequest = 60,
                        HeightRequest = 60,
                        CornerRadius = 30,
                        BackgroundColor = color
                    });

                    int currentButtonIndex = buttons.Count - 1;
                    int placementIndex = currentButtonIndex;

                    while (placementIndex > 7) {
                        placementIndex = placementIndex - 8;
                    }

                    if (i < numButtonsInPattern / 2)
                    {
                        var x = 0 + placementIndex * (2.0 / (numButtonsInPattern));
                        var y = currentButtonIndex * 70;
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, 60, 60));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);
                        
                        xyPoint.Add(new SKPoint((float)(x * widgetWidth), y));
                        Console.WriteLine("x=" + ((float)(x * widgetWidth)).ToString() + "y=" + y.ToString());
                    }
                    else
                    {
                        var x = 2 - placementIndex * (2.0 / (numButtonsInPattern));
                        var y = currentButtonIndex * 70;
                        AbsoluteLayout.SetLayoutBounds(buttons[currentButtonIndex], new Rectangle(x, y, 60, 60));
                        AbsoluteLayout.SetLayoutFlags(buttons[currentButtonIndex], AbsoluteLayoutFlags.XProportional);

                        xyPoint.Add(new SKPoint((float)(x * widgetWidth), y));
                        
                        Console.WriteLine("x=" + ((float)(x * widgetWidth)).ToString() + "y=" + y.ToString());
                    }
                    
                    MainAbsoluteLayout.Children.Add(buttons[currentButtonIndex]);
                }

                for (var j = 0; j < numButtonsInPattern-1; j++)
                {
                    skPointsListc.Add(new SKPoint[]
                    {
                            xyPoint[j],xyPoint[j+1]
                    });
                }
            }

            void Draw_RandomShape(SKCanvas skCanvas)
            {
                // Draw any kind of Shape
                SKPaint strokePaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black,
                    StrokeWidth = 2,
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round
                };
                //Console.WriteLine("heyheyhey" + skPointsListc.Count);
                foreach(var elem in skPointsListc) { //Console.WriteLine("heyheyhey" + elem[0].ToString());
                    //Console.WriteLine("heyheyhey"+elem[0].X.ToString()+"ab"+ elem[0].Y.ToString() + "ab" + elem[1].X.ToString() + "ab" + elem[1].Y.ToString() );
                    skCanvas.DrawLine(elem[0].X, elem[0].Y, elem[1].X, elem[1].Y, strokePaint);
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

                skCanvas.Scale(skCanvasWidth/400f); 
                // set the pixel scale of the canvas
                Draw_RandomShape(skCanvas);
            }
            

            for (var i = 0; i < 3; i++) { buttonCreater(i); }
            int TotalIterations = 3;
            double currentYLocation = 0.0;

            SkCanvasView.PaintSurface += SkCanvasView_OnPaintSurface;

            MainScrollView.Scrolled+= (object sender, ScrolledEventArgs e) => {
                if (e.ScrollY - currentYLocation > 500) {
                    buttonCreater(TotalIterations);
                    TotalIterations++;
                    currentYLocation = e.ScrollY;
                    Device.BeginInvokeOnMainThread(() => {
                        SkCanvasView.InvalidateSurface();
                    });
                }
            };
        }
    }
}