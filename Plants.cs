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
                dictionary.Add(splitKeys[i], splitValues[i]);
            }

            return dictionary;
        }

        public static PlantSpecies parseFromDictionary(Dictionary<string, string> speciesInstance)
        {
            PlantSpecies initialisedSpecies = new PlantSpecies(

                speciesName:            speciesInstance.ContainsKey("Species Name Name") ? speciesInstance["Species Name"] : "Unnamed",
                commonName:             speciesInstance.ContainsKey("Common Name") ? speciesInstance["Common Name"] : "Unnamed",                             
                indigenousName:         speciesInstance.ContainsKey("Indigenous Name") ? speciesInstance["Indigenous Name"] : "Unnamed",                        
                
                timetoMaturity:         speciesInstance.ContainsKey("Time to Maturity") ? Convert.ToDouble(speciesInstance["Time to Maturity"]) : 100.0,                        
                deathRate:              speciesInstance.ContainsKey("Death Rate") ? Convert.ToDouble(speciesInstance["Death Rate"]) : 100.0,                     
               
                relativeDistribution:   speciesInstance.ContainsKey("Relative Distribution") ? Convert.ToDouble(speciesInstance["Relative Distribution"]) : 100.0,                  
                requiredSpacingRadius:  speciesInstance.ContainsKey("Spacing Radius") ? Convert.ToDouble(speciesInstance["Spacing Radius"]) : 2000.0,                
                cost:                   speciesInstance.ContainsKey("Cost") ? Convert.ToDouble(speciesInstance["Cost"]) : 100.0,         
                
                initialCrownRadius:     speciesInstance.ContainsKey("Initial Crown Radius") ? Convert.ToDouble(speciesInstance["Initial Crown Radius"]) : 100.0,                
                matureCrownRadius:      speciesInstance.ContainsKey("Mature Crown Radius") ? Convert.ToDouble(speciesInstance["Mature Crown Radius"]) : 1000.0,                    
                varianceCrownRadius:    speciesInstance.ContainsKey("Crown Variance") ? Convert.ToDouble(speciesInstance["Crown Variance"]) : 10.0,                  
                
                initialRootRadius:      speciesInstance.ContainsKey("Initial Root Radius") ? Convert.ToDouble(speciesInstance["Initial Root Radius"]) : 100.0,                  
                matureRootRadius:       speciesInstance.ContainsKey("Mature Root Radius") ? Convert.ToDouble(speciesInstance["Mature Root Radius"]) : 1000.0,                      
                varianceRootRadius:     speciesInstance.ContainsKey("Root Variance") ? Convert.ToDouble(speciesInstance["Root Variance"]) : 10.0,                    
                
                initialHeight:          speciesInstance.ContainsKey("Initial Height") ? Convert.ToDouble(speciesInstance["Initial Height"]) : 100.0,               
                matureHeight:           speciesInstance.ContainsKey("Mature Height") ? Convert.ToDouble(speciesInstance["Mature Height"]) : 1000.0,                  
                varianceHeight:         speciesInstance.ContainsKey("Height Variance") ? Convert.ToDouble(speciesInstance["Height Variance"]) : 10.0,                        
                
                initialTrunkRadius:     speciesInstance.ContainsKey("Initial Trunk Radius") ? Convert.ToDouble(speciesInstance["Initial Trunk Radius"]) : 100.0,                    
                matureTrunkRadius:      speciesInstance.ContainsKey("Mature Trunk Radius") ? Convert.ToDouble(speciesInstance["Mature Trunk Radius"]) : 1000.0,                     
                varianceTrunkRadius:    speciesInstance.ContainsKey("Trunk Variance") ? Convert.ToDouble(speciesInstance["Trunk Variance"]) : 10.0,                  
                
                displayR:               speciesInstance.ContainsKey("Display R") ? Convert.ToInt16(speciesInstance["Display R"]) : 100,                      
                displayG:               speciesInstance.ContainsKey("Display G") ? Convert.ToInt16(speciesInstance["Display G"]) : 255,                      
                displayB:               speciesInstance.ContainsKey("Display B") ? Convert.ToInt16(speciesInstance["Display B"]) : 100                

            );
            return initialisedSpecies;
        }


    }

        
    class PlantSpecies : GH_Param<IGH_Goo>
    {
        // Naming
        public readonly String speciesName, commonName, indigenousName;
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
            double radius = getGrowth(this.initialCrownRadius, this.matureCrownRadius, time);
            return new Circle(location, radius);
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
