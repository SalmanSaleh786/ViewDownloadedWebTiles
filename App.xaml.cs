using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ViewDownloadedBuildingTiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Note: it is not best practice to store API keys in source code.
            // The API key is referenced here for the convenience of this tutorial.
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK58ed9396579749c69946dc809f82bbc1w3RpMpeX7jFP0GsOQauaBN9xMiMJ3hVH6s43MLtIC9V0_XSJ96kRFA7CP1NhErGZ";
        }
    }
}
