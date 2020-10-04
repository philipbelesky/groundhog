namespace Groundhog
{
    using System;
    using System.Drawing;
    using Grasshopper;
    using Grasshopper.Kernel;
    using Groundhog.Properties;

    public class GroundhogInfo : GH_AssemblyInfo
    {
        public override string Name => "Groundhog";

        public override Bitmap Icon => Resources.icon_groundhog;

        // Return a short string describing the purpose of this GHA library.
        public override string Description => "Groundhog is a Grasshopper plugin and wiki exploring the applications of computational design in landscape architecture.";

        public override Guid Id => new Guid("7dc547b5-ca43-457d-a3e2-8286f0784ad0");

        public override GH_LibraryLicense AssemblyLicense => GH_LibraryLicense.opensource;

        // Return a string identifying you or your company.
        public override string AuthorName => "Philip Belesky";

        // Return a string representing your preferred contact details.
        public override string AuthorContact => "contact@philipbelesky.com";
    }

    internal class GroundhogTab : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            // Icon used in the components tabs
            Instances.ComponentServer.AddCategoryIcon("Groundhog", Resources.icon_groundhog);
            return GH_LoadingInstruction.Proceed;
        }
    }
}