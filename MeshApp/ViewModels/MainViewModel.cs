using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinUI;
using MeshApp.Generators;
using MeshApp.Geometries;
using MeshApp.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace MeshApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private ObservableCollection<ISeries> series;

        [ObservableProperty]
        private LabelVisual title;

        [ObservableProperty]
        private string numberOfPoints = "100";

        [ObservableProperty]
        private string minNodeSpacing = "0.5";

        [ObservableProperty]
        private BoundaryDomain innerBoundaryDomain;

        [ObservableProperty]
        private BoundaryDomain outerBoundaryDomain;

        [ObservableProperty]
        private List<Node> innerBoundaryNodes;

        [ObservableProperty]
        private List<Node> outerBoundaryNodes;

        [ObservableProperty]
        private Zone zone;

        partial void OnNumberOfPointsChanged(string value)
        {
            
        }

        public RelayCommand GenerateClickedCommand => new RelayCommand(OnGenerateClicked);

        public MainViewModel()
        {       
            SetLabel();
        }
        
        private void SetLabel()
        {
            Title = new LabelVisual
            {
                Text = "Mesh",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.White)
            };
        }

        #region Inner Boundary
        private async Task SetInnerBoundaryNodes(int numberOfPoints)
        {
            await Task.Run(() =>
            {
                InnerBoundaryNodes = GeometryGenerator.Circle(2.0, numberOfPoints + 1);
            });
        }
        private async Task SetInnerBoundaryDomain(double minNodeSpacing)
        {
            await Task.Run(() =>
            {
                InnerBoundaryDomain = new BoundaryDomain(InnerBoundaryNodes, minNodeSpacing);
                InnerBoundaryDomain.ReverseDirection();
            });
        }
        private void PlotInnerBoundary()
        {
            var point1 = InnerBoundaryDomain.BoundaryElements.Select(x => new ObservablePoint(x.P.X, x.P.Y)).ToList();
            var point2 = InnerBoundaryDomain.BoundaryElements.Select(x => new ObservablePoint(x.Q.X, x.Q.Y)).ToList();

            for (int i = 0; i < point1.Count; i++)
            {

                Series.Add(new LineSeries<ObservablePoint>
                {
                    Values = new List<ObservablePoint> { point1[i], point2[i] },
                    LineSmoothness = 0,

                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 },
                    Fill = null,
                    GeometrySize = 10,
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometryStroke = new SolidColorPaint(SKColors.Blue)
                });
            }
        }
        private async Task SetInnerBoundary(int numberOfPoints, double minNodeSpacing)
        {
            await SetInnerBoundaryNodes(numberOfPoints);
            await SetInnerBoundaryDomain(minNodeSpacing);
            PlotInnerBoundary();
        }
       

        #endregion


        #region Outer Boundary
        private async Task SetOuterBoundaryNodes(int numberOfPoints)
        {
            await Task.Run(() =>
            {
                OuterBoundaryNodes = GeometryGenerator.Rectangle(20.0, numberOfPoints);
            });
        }

        private async Task SetOuterBoundaryDomain(double minNodeSpacing)
        {
            await Task.Run(() =>
            {
                OuterBoundaryDomain = new BoundaryDomain(OuterBoundaryNodes, minNodeSpacing);
            });
        }

        private void PlotOuterBoundary()
        {
            var point1 = OuterBoundaryDomain.BoundaryElements.Select(x => new ObservablePoint(x.P.X, x.P.Y)).ToList();
            var point2 = OuterBoundaryDomain.BoundaryElements.Select(x => new ObservablePoint(x.Q.X, x.Q.Y)).ToList();

            for (int i = 0; i < point1.Count; i++)
            {

                Series.Add(new LineSeries<ObservablePoint>
                {
                    Values = new List<ObservablePoint> { point1[i], point2[i] },
                    LineSmoothness = 0,
                    
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4},                    
                    Fill = null,
                    GeometrySize = 15,
                    GeometryFill = null,
                    GeometryStroke = new SolidColorPaint(SKColors.Red)
                });
            }
        }

        private void PlotBackgroundGrid()
        {
            var outerBoundaryDomain = Zone.BackgroundNodes.Select(node => new ObservablePoint(node.X, node.Y)).ToList();

            Series.Add(new ScatterSeries<ObservablePoint>
            {
                Values = outerBoundaryDomain,
                Stroke = new SolidColorPaint(SKColors.White),
                GeometrySize = 5
            });
        }

        private async Task SetOuterBoundary(int numberOfPoints, double minNodeSpacing)
        {
            await SetOuterBoundaryNodes(numberOfPoints);
            await SetOuterBoundaryDomain(minNodeSpacing);

            PlotOuterBoundary();
        }
        

        #endregion

        private async void OnGenerateClicked()
        {
            var numberOfPointsParsed = int.TryParse(NumberOfPoints, out var numberOfPoints);
            var minNodeSpacingParsed = double.TryParse(MinNodeSpacing, out var minNodeSpacing);

            if (!numberOfPointsParsed || !minNodeSpacingParsed) return;

            Series = new ObservableCollection<ISeries>();

            await SetInnerBoundary(numberOfPoints, minNodeSpacing);
            await SetOuterBoundary(numberOfPoints, minNodeSpacing);

            Zone = new Zone(OuterBoundaryDomain, InnerBoundaryDomain);

            var backgroundGridGenerator = new BackgroundGridGenerator(Zone);

            backgroundGridGenerator.Generate();

            PlotBackgroundGrid();
        }
    }
}
;
