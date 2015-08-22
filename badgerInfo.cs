using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace badger
{
    public class badgerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Badger";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
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

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
