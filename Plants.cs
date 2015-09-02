using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace badger
{

    class PlantFactory
    {

        public static Dictionary<string, string> parseToDictionary(string headers, string values)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string[] splitKeys = headers.Split(',');
            string[] splitValues = values.Split(',');

            int i;
            for (i = 0; i < splitKeys.Length; i = i + 1)
            {
                if (splitValues[i].Trim() != "")
                {
                    dictionary.Add(splitKeys[i].Trim(), splitValues[i].Trim());
                }
            }

            return dictionary;
        }

        public static Tuple<PlantSpecies, string> parseFromDictionary(Dictionary<string, string> speciesInstance)
        {
            string warnings = "";
            // Naming
            string speciesName, commonName, indigenousName;
            // Lifespan
            double timetoMaturity, deathRate;
            // Placement
            double relativeDistribution, requiredSpacingRadius, cost;
            // Form
            double initialCrownRadius, matureCrownRadius, varianceCrownRadius;
            double initialRootRadius, matureRootRadius, varianceRootRadius;
            double initialHeight, matureHeight, varianceHeight;
            double initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius;
            // Aesthetics
            int displayR, displayG, displayB;

            if (speciesInstance.ContainsKey("Species Name")) {
                speciesName = speciesInstance["Species Name"];
            } else {
                speciesName = "Unnamed";
                warnings += "no Species name; ";
            }
            if (speciesInstance.ContainsKey("Common Name")) {
                commonName = speciesInstance["Common Name"];
            } else {
                commonName = "Unnamed";
                warnings += "no Common name; ";
            }
            if (speciesInstance.ContainsKey("Indigenous Name")) {
                indigenousName = speciesInstance["Indigenous Name"];
            } else {
                indigenousName = "Unnamed";
                warnings += "no Indigenous name; ";
            }
            
            if (speciesInstance.ContainsKey("Time to Maturity")) {
                timetoMaturity = Convert.ToDouble(speciesInstance["Time to Maturity"]);
            } else {
                timetoMaturity = 100.0;
                warnings += "no Time to Maturity; ";
            }
            if (speciesInstance.ContainsKey("Death Rate")) {
                deathRate = Convert.ToDouble(speciesInstance["Death Rate"]);
            } else {
                deathRate = 10.0;
                warnings += "no Death Rate; ";
            }
            
            if (speciesInstance.ContainsKey("Relative Distribution")) {
                relativeDistribution = Convert.ToDouble(speciesInstance["Relative Distribution"]);
            } else {
                relativeDistribution = 1.0;
                warnings += "no Relative Distribution; ";
            }
            if (speciesInstance.ContainsKey("Spacing Radius")) {
                requiredSpacingRadius = Convert.ToDouble(speciesInstance["Spacing Radius"]);
            } else {
                requiredSpacingRadius = 1000.0;
                warnings += "no Spacing Radius; ";
            }
            if (speciesInstance.ContainsKey("Cost")) {
                cost = Convert.ToDouble(speciesInstance["Cost"]);
            } else {
                cost = 10.0;
                warnings += "no Cost; ";
            }
            
            if (speciesInstance.ContainsKey("Initial Crown Radius")) {
                initialCrownRadius = Convert.ToDouble(speciesInstance["Initial Crown Radius"]);
            } else {
                initialCrownRadius = 10.0;
                warnings += "no Initial Crown Radius; ";
            }            
            if (speciesInstance.ContainsKey("Mature Crown Radius")) {
                matureCrownRadius = Convert.ToDouble(speciesInstance["Mature Crown Radius"]);
            } else {
                matureCrownRadius = 10.0;
                warnings += "no Mature Crown Radiu; s";
            } 
            if (speciesInstance.ContainsKey("Crown Variance")) {
                varianceCrownRadius = Convert.ToDouble(speciesInstance["Crown Variance"]);
            } else {
                varianceCrownRadius = 10.0;
                warnings += "no Crown Variance; ";
            }


            if (speciesInstance.ContainsKey("Initial Trunk Radius"))
            {
                initialTrunkRadius = Convert.ToDouble(speciesInstance["Initial Trunk Radius"]);
            }
            else
            {
                initialTrunkRadius = 10.0;
                warnings += "no Initial Trunk Radius; ";
            }
            if (speciesInstance.ContainsKey("Mature Height"))
            {
                matureTrunkRadius = Convert.ToDouble(speciesInstance["Mature Trunk Radius"]);
            }
            else
            {
                matureTrunkRadius = 100.0;
                warnings += "no Mature Trunk Radius; ";
            }
            if (speciesInstance.ContainsKey("Trunk Variance"))
            {
                varianceTrunkRadius = Convert.ToDouble(speciesInstance["Trunk Variance"]);
            }
            else
            {
                varianceTrunkRadius = 10.0;
                warnings += "no Trunk Variance; ";
            }

            if (speciesInstance.ContainsKey("Initial Height"))
            {
                initialHeight = Convert.ToDouble(speciesInstance["Initial Height"]);
            }
            else
            {
                initialHeight = 1000.0;
                warnings += "no Initial Height; ";
            }
            if (speciesInstance.ContainsKey("Mature Height"))
            {
                matureHeight = Convert.ToDouble(speciesInstance["Mature Height"]);
            }
            else
            {
                matureHeight = 1000.0;
                warnings += "no Mature Height; ";
            }
            if (speciesInstance.ContainsKey("Height Variance"))
            {
                varianceHeight = Convert.ToDouble(speciesInstance["Height Variance"]);
            }
            else
            {
                varianceHeight = 10.0;
                warnings += "no Height Variance; ";
            }

            if (speciesInstance.ContainsKey("Initial Root Radius")) {
                initialRootRadius = Convert.ToDouble(speciesInstance["Initial Root Radius"]);
            } else {
                initialRootRadius = 1000.0;
                warnings += "no Initial Root Radius; ";
            }
            if (speciesInstance.ContainsKey("Mature Root Radius")) {
                matureRootRadius = Convert.ToDouble(speciesInstance["Mature Root Radius"]);
            } else {
                matureRootRadius = 1000.0;
                warnings += "no Mature Root Radius; ";
            }
            if (speciesInstance.ContainsKey("Root Variance")) {
                varianceRootRadius = Convert.ToDouble(speciesInstance["Root Variance"]);
            } else {
                varianceRootRadius = 10.0;
                warnings += "no Root Variance; ";
            }
            
            
            if (speciesInstance.ContainsKey("Display R")) {
                displayR = Convert.ToInt16(speciesInstance["Display R"]);
            } else {
                displayR = 100;
                warnings += "no Display R; ";
            }
            if (speciesInstance.ContainsKey("Display G")) {
                displayG = Convert.ToInt16(speciesInstance["Display G"]);
            } else {
                displayG = 255;
                warnings += "no Display G; ";
            }
            if (speciesInstance.ContainsKey("Display B")) {
                displayB = Convert.ToInt16(speciesInstance["Display B"]);
            } else {
                displayB = 100;
                warnings += "no Display B; ";
            }

            PlantSpecies initialisedSpecies = new PlantSpecies(
                speciesName: speciesName, commonName: commonName, indigenousName: indigenousName,                        
                timetoMaturity: timetoMaturity, deathRate: deathRate,                     
                relativeDistribution: relativeDistribution, requiredSpacingRadius: requiredSpacingRadius, cost: cost,         
                initialCrownRadius: initialCrownRadius, matureCrownRadius: matureCrownRadius, varianceCrownRadius: varianceCrownRadius,
                initialRootRadius: initialRootRadius, matureRootRadius: matureRootRadius, varianceRootRadius: varianceRootRadius,                    
                initialHeight: initialHeight, matureHeight: matureHeight, varianceHeight: varianceHeight,                        
                initialTrunkRadius: initialTrunkRadius, matureTrunkRadius: matureTrunkRadius, varianceTrunkRadius: varianceTrunkRadius,
                displayR: displayR, displayG: displayG, displayB: displayB                
            );

            return Tuple.Create(initialisedSpecies, warnings);
        }


    }

        
    class PlantSpecies : GH_Param<IGH_Goo>
    {
        // Naming
        public readonly string speciesName, commonName, indigenousName;
        // Lifespan
        private readonly double timetoMaturity, deathRate;
        // Placement
        private readonly double relativeDistribution, requiredSpacingRadius, cost;
        // Form
        private readonly double initialCrownRadius, matureCrownRadius, varianceCrownRadius;
        private readonly double initialRootRadius, matureRootRadius, varianceRootRadius;
        private readonly double initialHeight, matureHeight, varianceHeight;
        private readonly double initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius;
        // Aesthetics
        private readonly int displayR, displayG, displayB;

        // Get current state
        private double getGrowth(double initial, double eventual, double time)
        {
            double annualRate = (eventual - initial) / this.timetoMaturity;
            double grownTime = Math.Min(time, this.timetoMaturity);
            double grownState = (grownTime * annualRate) + initial;
            return grownState;
        }

        // Get geometry
        public Circle getCrown(Point3d location, double time)
        {
            double height = getGrowth(this.initialHeight, this.matureHeight, time);
            double radius = getGrowth(this.initialCrownRadius, this.matureCrownRadius, time);
            Point3d canopyLocation = new Point3d(location.X, location.Y, (location.Z + height));
            return new Circle(canopyLocation, radius);
        }
        public Circle getRoot(Point3d location, double time)
        {
            double radius = getGrowth(this.initialRootRadius, this.matureRootRadius, time);
            return new Circle(location, radius);
        }
        public Circle getTrunk(Point3d location, double time)
        {
            double radius = getGrowth(this.initialTrunkRadius, this.matureTrunkRadius, time);
            return new Circle(location, radius);
        }
        public Circle getSpacing(Point3d location)
        {
            return new Circle(location, this.requiredSpacingRadius);
        }

        public System.Drawing.Color getColour()
        {
            System.Drawing.Color colour = System.Drawing.Color.FromArgb(this.displayR, this.displayG, this.displayB);
            return colour;
        }
        public GH_String getLabel()
        {
            return new GH_String(this.speciesName);
        }


        #region properties
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("2d268bdc-ecaa-4cf7-815a-c8111d1798d7"); }
        }
        public override string ToString()
        {
            return "Badger Plant Species (" + speciesName + ")";
        }
        #endregion

        #region casting
        protected override IGH_Goo InstantiateT()
        {
            return new GH_ObjectWrapper();
        }
        #endregion

        // Init
        public PlantSpecies(
            string speciesName, string commonName, string indigenousName,
            double timetoMaturity, double deathRate, 
            double relativeDistribution, double requiredSpacingRadius, double cost,
            double initialCrownRadius, double matureCrownRadius, double varianceCrownRadius,
            double initialRootRadius, double matureRootRadius, double varianceRootRadius,
            double initialHeight, double matureHeight, double varianceHeight,
            double initialTrunkRadius, double matureTrunkRadius, double varianceTrunkRadius,
            int displayR, int displayG, int displayB
        )
            : base(new GH_InstanceDescription("Plant param", "P", "TODO:", "Params"))
        {
            // Naming
            this.speciesName = speciesName;
            this.commonName = commonName;
            this.indigenousName = indigenousName;
            // Lifespan
            this.timetoMaturity = timetoMaturity;
            this.deathRate = deathRate;
            // Placement
            this.relativeDistribution = relativeDistribution;
            this.requiredSpacingRadius = requiredSpacingRadius;
            this.cost = cost;
            // Form
            this.initialCrownRadius = initialCrownRadius;
            this.matureCrownRadius = matureCrownRadius;
            this.varianceCrownRadius = varianceCrownRadius;
            this.initialRootRadius = initialRootRadius;
            this.matureRootRadius = matureRootRadius;
            this.varianceRootRadius = varianceRootRadius;
            this.initialHeight = initialHeight;
            this.matureHeight = matureHeight;
            this.varianceHeight = varianceHeight;
            this.initialTrunkRadius = initialTrunkRadius;
            this.matureTrunkRadius = matureTrunkRadius;
            this.varianceTrunkRadius = varianceTrunkRadius;
            // Display
            this.displayR = displayR;
            this.displayG = displayG;
            this.displayB = displayB;
        }

    }

}
