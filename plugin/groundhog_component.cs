﻿using System;
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
            var hogVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var hogNiceVersion = hogVersion.Major.ToString() + '.' + hogVersion.Minor.ToString() + '.' + hogVersion.Build.ToString();

            // Make it really obvious when running from develop
            #if DEBUG
            this.Message = hogNiceVersion.ToString() + " dev";
            #endif

            #if !DEBUG
            if (Globals.Logged == false)
            {

                // Setup RavenClient
                var ravenClient = new RavenClient(SentryKey);
                ravenClient.Release = hogVersion.ToString();
                ravenClient.Tags["Language"] = CultureInfo.InstalledUICulture.EnglishName;
                ravenClient.Tags["System"] = System.Environment.OSVersion.ToString();
                ravenClient.Tags["Time"] = TimeZoneInfo.Local.StandardName;
                ravenClient.Tags["Groundhog"] = hogNiceVersion
                ravenClient.Tags["Grasshopper"] = Grasshopper.Versioning.Version.ToString();

                // Rhinoceros (seems to fail on rhino for Mac?)
                try
                {
                    var rhVersion = Rhino.RhinoApp.Version;
                    ravenClient.Tags["Rhino"] = rhVersion.Major.ToString() + '.' + rhVersion.Minor.ToString();
                }
                catch
                {
                }

                // Basic logging of component type
                var logMessage = "USED: " + base.Name;
                var sentryEvent = new SharpRaven.Data.SentryEvent(logMessage)
                {
                    Level = SharpRaven.Data.ErrorLevel.Info
                };
                    ravenClient.Capture(sentryEvent);

                // Set logging global
                Globals.Logged = true;
            }
            #endif

            return base.Read(reader);
        }

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected GroundHog_Component(string name, string nickname, string description, string category, string subCategory)
            :base(name, nickname, description, category, subCategory) {
        }

    }

}
