using System;
using Grasshopper.Kernel;

namespace groundhog
{

    public static class Globals // Global variable
    {
        public static bool Logged = false; // Keep track of whether a log has been issued
    }

    public abstract class GroundHog_Component : GH_Component
    {

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {

            Console.WriteLine("Parent Constructor.");
            if (Globals.Logged == false)
            {
                Globals.Logged = true;
                Console.WriteLine("Log things");
            }
            else
            {

                Console.Write("Already Log things");
            }

            return base.Read(reader);
        }

    }

}
