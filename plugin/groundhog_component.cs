using System;
using Grasshopper.Kernel;
using SharpRaven;
using System.Globalization;


namespace groundhog
{

    public static class Globals // Global variable
    {
        public static bool Logged = false; // Keep track of whether a log has been issued
    }

    public abstract class GroundHog_Component : GH_Component
    {
        const string SentryKey = "https://2677778a4e2147f0b2e2aa2c39c403b0:119205cb453641ad9da791b5a83af67b@sentry.io/218018";

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {

            if (Globals.Logged == false)
            {
                // Setup RavenClient
                var ravenClient = new RavenClient(SentryKey);
                ravenClient.Tags["Language"] = CultureInfo.InstalledUICulture.EnglishName;
                ravenClient.Tags["System"] = System.Environment.OSVersion.ToString();

                // Groundhog
                var hogVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                ravenClient.Tags["Groundhog"] = hogVersion.Major.ToString() + '.' + hogVersion.Minor.ToString() + '.' + hogVersion.Build.ToString();

                // Rhinoceros (seems to fail on rhino for Mac?)
                try
                {
                    var rhVersion = Rhino.RhinoApp.Version;
                    ravenClient.Tags["Rhino"] = rhVersion.Major.ToString() + '.' + rhVersion.Minor.ToString();
                }
                catch
                {
                }

                // Grasshopper
                ravenClient.Tags["Grasshopper"] = Grasshopper.Versioning.Version.ToString();

                // Basic logging of component type
                var logMessage = "USED: " + base.Name;
                var sentryEvent = new SharpRaven.Data.SentryEvent(logMessage)
                {
                    Level = SharpRaven.Data.ErrorLevel.Info
                };
                #if !DEBUG
                    ravenClient.Capture(sentryEvent);
                #endif

                // Set logging global
                Console.Write("Logged " + logMessage);
                Globals.Logged = true;
            }
            else
            {
                Console.Write("Already logged");
            }

            return base.Read(reader);
        }

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected GroundHog_Component(string name, string nickname, string description, string category, string subCategory)
            :base(name, nickname, description, category, subCategory) {
        }

    }

}
