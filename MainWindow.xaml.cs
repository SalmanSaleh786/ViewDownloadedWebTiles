using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ViewDownloadedBuildingTiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string folderPath = "C:\\Users\\Salman\\Downloads\\MapTilesDownloader-master\\MapTilesDownloader-master\\src\\output\\1682233700777\\15";
        private byte zoomLevel = 15;  //As per my need, zoom level is fixed
        private GraphicsOverlay? graphicsOverlay = null;
        List<MapPoint> pointsToUnify = new List<MapPoint>();
        List<Graphic> allGraphics = new List<Graphic>();
        int pointsToSkip;
        private string folderPath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();

            SetupMap();
            CreateGraphicsOverlay();

        }
        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            MainMapView.Map = new Map(BasemapStyle.ArcGISNavigation);
        }
        private void CreateGraphicsOverlay()
        {
            // Create a new graphics overlay to contain a variety of graphics.
            graphicsOverlay = new GraphicsOverlay();
            graphicsOverlay.RenderingMode = GraphicsRenderingMode.Dynamic;

            // Create a simple marker symbol - red, cross, size 12.
            SimpleMarkerSymbol mySymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 0.001);

            SimpleRenderer myRenderer = new SimpleRenderer(mySymbol);
            graphicsOverlay.Renderer = myRenderer;

            // Add the overlay to a graphics overlay collection.
            GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
            {
                graphicsOverlay
            };

            // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
            MainMapView.GraphicsOverlays = overlays;

        }
        private void CreateGraphics(int pointsToSkip)
        {
            var directoriesX = System.IO.Directory.GetDirectories(folderPath);
            //= int.Parse(PointsSkip_Textbox.Text);
            for (int i = 0; i < directoriesX.Count(); i += pointsToSkip)
            {
                var directoryX = directoriesX[i];
                var nameStartLocation = directoryX.LastIndexOf('\\');
                var X = Convert.ToUInt32(directoryX.Substring(nameStartLocation + 1));
                var filesY = System.IO.Directory.GetFiles(directoryX);
                int point = 0;
                foreach (var fileY in filesY)
                {
                    if (++point % pointsToSkip != 0)
                        continue;
                    nameStartLocation = fileY.LastIndexOf('\\');
                    var nameEndLocation = fileY.LastIndexOf('.');
                    try
                    {
                        var Y = Convert.ToUInt32(fileY.Substring(nameStartLocation + 1, nameEndLocation - nameStartLocation - 1));
                        CreateGraphic(X, Y);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Exception occured: " + ex.Message.ToString());
                    }
                }
            }
            if (graphicsOverlay != null)
                graphicsOverlay.Graphics.AddRange(allGraphics);
        }
        private void CreateGraphic(uint X, uint Y)
        {
            var latLng = NumberToDegree(X, Y, zoomLevel);
            MapPoint buildingLocation = new MapPoint(latLng.Item2, latLng.Item1, SpatialReferences.Wgs84);
            Graphic buildingGraphic = new Graphic(buildingLocation);
            allGraphics.Add(buildingGraphic);
            //pointsToUnify.Add(buildingLocation);
        }

        private Tuple<double, double> NumberToDegree(uint xtile, uint ytile, byte zoom)
        {

            double n = Math.Pow(2.0, zoom);
            double lon_deg = xtile / n * 360.0 - 180.0;

            double lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * ytile / n)));

            double lat_deg = Degrees(lat_rad);

            return new Tuple<double, double>(lat_deg, lon_deg);
        }
        public static double Degrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        private void BrowseTilesFolder_Clicked(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            folderPath = folderBrowserDialog.SelectedPath;


        }

        private void StartProcessing_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                System.Windows.MessageBox.Show("Folder path is empty!");
                return;
            }
            if (string.IsNullOrEmpty(PointsSkip_Textbox.Text.ToString()))
            {
                System.Windows.MessageBox.Show("Points to skip is empty!");
                return;
            }
            int pointsToSkip;
            if (!int.TryParse(PointsSkip_Textbox.Text, out pointsToSkip))
                {
                System.Windows.MessageBox.Show("Points to skip should be integer only!");
                return;
            }
            int zoomLevel;
            if (!int.TryParse(ZoomLevel_Textbox.Text, out zoomLevel))
            {
                System.Windows.MessageBox.Show("Zoom Level should be integer!");
                return;
            }
            folderPath += "\\" + zoomLevel;
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show(string.Format("Invalid folder path {0}", folderPath));
                return;
            }
            if (MainMapView.GraphicsOverlays != null && MainMapView.GraphicsOverlays.Count > 0)
            {
                MainMapView.GraphicsOverlays[0].Graphics.Clear();

                CreateGraphics(pointsToSkip);
            }
        }
    }
}
