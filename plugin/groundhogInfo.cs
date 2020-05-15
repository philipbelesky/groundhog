using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using groundhog.Properties;

namespace groundhog
{
    public class groundhogInfo : GH_AssemblyInfo
    {
        public override string Name => "Groundhog";

        public override Bitmap Icon => Resources.icon_groundhog;

        public override string Description =>
            //Return a short string describing the purpose of this GHA library.
            "";

        public override Guid Id => new Guid("7dc547b5-ca43-457d-a3e2-8286f0784ad0");

        public override GH_LibraryLicense AssemblyLicense => GH_LibraryLicense.opensource;

        public override string AuthorName =>
            //Return a string identifying you or your company.
            "Philip Belesky";

        public override string AuthorContact =>
            //Return a string representing your preferred contact details.
            "contact@philipbelesky.com";
    }

    internal class GroundHogTab : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            // Icon used in the components tabs
            Instances.ComponentServer.AddCategoryIcon("Groundhog", Resources.icon_groundhog);
            return GH_LoadingInstruction.Proceed;
        }
    }
}