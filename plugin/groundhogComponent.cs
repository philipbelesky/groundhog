using System;
using Grasshopper.Kernel;
using SharpRaven;
using SharpRaven.Data;
using System.Globalization;


namespace groundhog
{

    public static class Globals // Global variable
    {
        public static bool Logged = false; // Keep track of whether a log has been issued
    }

    public abstract class GroundHogComponent : GH_Component
    {
        const string SentryKey = "https://2677778a4e2147f0b2e2aa2c39c403b0:119205cb453641ad9da791b5a83af67b@sentry.io/218018";

        private Version getGroundHogVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        private string getNiceGroundHogVersion()
        {
            var v = this.getGroundHogVersion();
            return v.Major.ToString() + '.' + v.Minor.ToString() + '.' + v.Build.ToString();
        }

        private RavenClient constructRavenClient()
        {
            // Setup RavenClient
            var ravenClient = new RavenClient(SentryKey);
            ravenClient.Release = getGroundHogVersion().ToString();
            ravenClient.Tags["Language"] = CultureInfo.InstalledUICulture.EnglishName;
            ravenClient.Tags["System"] = System.Environment.OSVersion.ToString();
            ravenClient.Tags["Time"] = TimeZoneInfo.Local.StandardName;
            ravenClient.Tags["Groundhog"] = getNiceGroundHogVersion();
            ravenClient.Tags["Grasshopper"] = Grasshopper.Versioning.Version.ToString();

            // Rhinoceros (seems to fail on rhino for Mac?)
            try
            {
                var rhVersion = Rhino.RhinoApp.Version;
                ravenClient.Tags["Rhino"] = rhVersion.Major.ToString() + '.' + rhVersion.Minor.ToString();
            }
            catch (System.IO.FileNotFoundException)
            {
                ravenClient.Tags["Rhino"] = "Unknown macOS";
            }

            return ravenClient;            
        }


        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {

            // Make it really obvious when running from develop by adding a tag to the component itself
            #if DEBUG
            this.Message = getNiceGroundHogVersion().ToString() + " dev";
            #endif

            #if !DEBUG
            if (Globals.Logged == false)
            {
                // Basic logging of component type
                var logMessage = "USED: " + base.Name;
                var sentryEvent = new SharpRaven.Data.SentryEvent(logMessage)
                {
                    Level = SharpRaven.Data.ErrorLevel.Info
                };
                constructRavenClient().Capture(sentryEvent);

                // Set logging global
                Globals.Logged = true;
            }
            #endif

            return base.Read(reader);
        }

        // Components must implement the method
        protected abstract void GroundHogSolveInstance(IGH_DataAccess DA);

        #if !DEBUG
        // Override the main solve instance method to wrap it in a try/catch for error re"ortin purposes
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            try {
                GroundHogSolveInstance(DA);
            }
            catch (Exception componentException)
            {
                Console.WriteLine("Exception caught: {0}", componentException);
                // Log exception to Sentry
                constructRavenClient().Capture(new SentryEvent(componentException)); 
            // Throw the error anyway so it bubbles up
            throw; 
            }
        }
        #else
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GroundHogSolveInstance(DA);
        }
        #endif

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected GroundHogComponent(string name, string nickname, string description, string category, string subCategory)
            :base(name, nickname, description, category, subCategory) {
        }

    }

}
