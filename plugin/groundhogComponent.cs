namespace Groundhog
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using GH_IO.Serialization;
    using Grasshopper.Kernel;
    using Rhino;
    using Sentry;
    using Sentry.Protocol;

    public abstract class GroundHogComponent : GH_Component
    {
        protected readonly double docAngularTolerance = RhinoDoc.ActiveDoc.ModelAngleToleranceRadians;
        protected readonly double docUnitTolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected GroundHogComponent(string name, string nickname, string description, string category,
            string subCategory)
            : base(name, nickname, description, category, subCategory)
        {
        }

        // Provides the message with current version string while debugging 
#if DEBUG
        public override bool Read(GH_IReader reader) // Triggered on definition load
        {
            this.Message = "Gh" + this.GetNiceGroundHogVersion();
            return base.Read(reader);
        }

        public override void AddedToDocument(GH_Document document)
        {
            this.Message = "Gh v" + this.GetNiceGroundHogVersion();
            base.AddedToDocument(document); // Triggered on component placement
        }
#endif

        // Components must implement the method
        protected abstract void GroundHogSolveInstance(IGH_DataAccess DA);

        // Override the main solve instance method to wrap it in a try/catch for error reporting purposes
        protected override void SolveInstance(IGH_DataAccess DA)
        {
#if DEBUG
            this.GroundHogSolveInstance(DA);
#else
            try
            {
                GroundHogSolveInstance(DA);
            }
            catch (Exception componentException)
            {
                Console.WriteLine("Exception caught: {0}", componentException);
                // Log exception to Sentry
                using (SentrySdk.Init(o => {
                    o.Dsn = new Dsn("https://2677778a4e2147f0b2e2aa2c39c403b0@o99429.ingest.sentry.io/218018");
                    o.Release = this.GetGroundHogVersion().ToString();
                }))
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.SetTag("Language", CultureInfo.InstalledUICulture.EnglishName);
                        scope.SetTag("System", System.Environment.OSVersion.ToString());
                        scope.SetTag("Time", TimeZoneInfo.Local.StandardName);
                        scope.SetTag("Groundhog", this.GetNiceGroundHogVersion());
                        scope.SetTag("Grasshopper", Grasshopper.Versioning.Version.ToString());
                        scope.SetTag("Component", this.Name);

                        // Rhinoceros (seems to fail on rhino for Mac?)
                        try
                        {
                            var rhVersion = Rhino.RhinoApp.Version;
                            scope.SetTag("Rhino", rhVersion.Major.ToString() + '.' + rhVersion.Minor.ToString());
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            scope.SetTag("Rhino", "Unknown macOS");
                        }
                    });

                    SentrySdk.AddBreadcrumb(
                        message: null,
                        category: this.Name.ToString(), // Component Name
                        level: BreadcrumbLevel.Error
                    );

                    SentrySdk.CaptureException(componentException);
                }
                // Throw the error anyway so it bubbles up
                throw;
            }
#endif
        }

        private Version GetGroundHogVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        private string GetNiceGroundHogVersion()
        {
            var v = this.GetGroundHogVersion();
            return v.Major.ToString() + '.' + v.Minor + '.' + v.Build;
        }
    }
}