using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BTChayBoPage : Page
    {
        Geopoint currentPoint;
        Geopoint startPoint;
        Geoposition position;
        Geolocator locator;
        MapIcon mapIcon;
        public double Distance { get; set; }
        public double Duration { get; set; }
        public BTChayBoPage()
        {
            this.InitializeComponent();
            locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                position = await locator.GetGeopositionAsync();
            }
            catch (Exception)
            {
                // Handle errors like unauthorized access to location services or no Internet access.
                throw;
            }
            startPoint = position.Coordinate.Point;
            currentPoint = position.Coordinate.Point;
            await myMap.TrySetViewAsync(currentPoint, 15);
            locator.MovementThreshold = 10;
            //tao icon pin vao dia diem cho Map
            mapIcon = new MapIcon();
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1);
            mapIcon.Location = position.Coordinate.Point;
            mapIcon.Title = "You are here";
            myMap.MapElements.Add(mapIcon);
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //position = await locator.GetGeopositionAsync();
            //currentPoint = position.Coordinate.Point;
            //MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(startPoint, currentPoint);
            //if (routeResult.Route == null) return;
            //if(routeResult.Status == MapRouteFinderStatus.Success)
            //{
            //    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        //Use the route to initialize a MapRouteView.
            //        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
            //        viewOfRoute.RouteColor = Colors.Blue;
            //        viewOfRoute.OutlineColor = Colors.Blue;
            //        // Add the new MapRouteView to the Routes collection
            //        // of the MapControl.
            //        myMap.Routes.Add(viewOfRoute);
            //        // Fit the MapControl to the route.
                    
            //    });
            //    await myMap.TrySetViewBoundsAsync(
            //          routeResult.Route.BoundingBox,
            //          null,
            //          Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
            //}
            //Distance += routeResult.Route.LengthInMeters;
            ////Duration += Convert.ToDouble(routeResult.Route.EstimatedDuration);
            //startPoint = currentPoint;
        }

        private void tiepButton_Click(object sender, RoutedEventArgs e)
        {
            ////locator.PositionChanged += geolocator_PositionChanged;
            //// Start at Grand Central Station.
            //BasicGeoposition startLocation = new BasicGeoposition();
            //startLocation.Latitude = 40.7517;
            //startLocation.Longitude = -073.9766;
            //Geopoint startPoint = new Geopoint(startLocation);
            //// End at Central Park.
            //BasicGeoposition endLocation = new BasicGeoposition();
            //endLocation.Latitude = 40.7669;
            //endLocation.Longitude = -073.9790;
            //Geopoint endPoint = new Geopoint(endLocation);
            //// Get the route between the points.
            //MapRouteFinderResult routeResult =
            //await MapRouteFinder.GetDrivingRouteAsync(
            //  startPoint,
            //  endPoint,
            //  MapRouteOptimization.Time,
            //  MapRouteRestrictions.None,
            //  290);
            //if (routeResult.Status == MapRouteFinderStatus.Success)
            //{
            //    // Use the route to initialize a MapRouteView.
            //    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
            //    viewOfRoute.RouteColor = Colors.Blue;
            //    viewOfRoute.OutlineColor = Colors.Blue;
            //    // Add the new MapRouteView to the Routes collection
            //    // of the MapControl.
            //    myMap.Routes.Add(viewOfRoute);
            //    // Fit the MapControl to the route.
            //    await myMap.TrySetViewBoundsAsync(
            //      routeResult.Route.BoundingBox,
            //      null,
            //      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
            //}
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            myMap.MapElements.Remove(mapIcon);
            locator.PositionChanged += geolocator_PositionChanged;
        }
    }
}
