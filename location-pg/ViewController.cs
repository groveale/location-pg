using System;
using CoreLocation;
using MapKit;
using UIKit;

namespace locationpg
{
    public partial class ViewController : UIViewController
    {

        MKMapView mapView;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            mapView = new MKMapView(View.Bounds);
            mapView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            View.AddSubview(mapView);

            CLLocationCoordinate2D coords = new CLLocationCoordinate2D(51.5001524, -0.1262362);
            MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(20), MilesToLongitudeDegrees(20, coords.Latitude));

            mapView.Region = new MKCoordinateRegion(coords, span);
        }




        public double MilesToLatitudeDegrees(double miles)
        {
            double earthRadius = 3960.0; // in miles
            double radiansToDegrees = 180.0 / Math.PI;
            return (miles / earthRadius) * radiansToDegrees;
        }

        public double MilesToLongitudeDegrees(double miles, double atLatitude)
        {
            double earthRadius = 3960.0; // in miles
            double degreesToRadians = Math.PI / 180.0;
            double radiansToDegrees = 180.0 / Math.PI;
            // derive the earth's radius at that point in latitude
            double radiusAtLatitude = earthRadius * Math.Cos(atLatitude * degreesToRadians);
            return (miles / radiusAtLatitude) * radiansToDegrees;
        }
    }
}
