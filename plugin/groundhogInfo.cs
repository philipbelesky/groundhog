using System;
using System.Drawing;
using Grasshopper.Kernel;
using groundhog.Properties;

namespace groundhog
{
    public class groundhogInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Groundhog";
            }
        }

        public override Bitmap Icon
        {
            get
            {
                return Resources.icon_groundhog;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("7dc547b5-ca43-457d-a3e2-8286f0784ad0");
            }
        }

        public override GH_LibraryLicense AssemblyLicense
        {
            get
            {
                return GH_LibraryLicense.opensource;
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Philip Belesky";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "contact@philipbelesky.com";
            }
        }
    }
}
