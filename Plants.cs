using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace badger
{
    class PlantFactory
    {

        static Dictionary<string, string> parseToDictionary(string headers, string values)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string[] splitKeys = headers.Split(',');
            string[] splitValues = values.Split(',');

            int i;
            for (i = 0; i < splitKeys.Length(); i = i + 1)
            {
                dictionary.Add(splitKeys[i], splitValues[i]);
            }

            return dictionary;
        }

        static PlantSpecies parseFromDictionary(Dictionary<string, string> speciesInstance)
        {
            PlantSpecies initialisedSpecies = new PlantSpecies(
                speciesName: speciesInstance["speciesName"],
                commonName: speciesInstance["commonName"],
                indigenousName: speciesInstance["indigenousName"],

                timetoMaturity: Convert.ToDouble(speciesInstance["timetoMaturity"]),
                deathRate: Convert.ToDouble(speciesInstance["deathRate"]),

                relativeDistribution: Convert.ToDouble(speciesInstance["relativeDistribution"]),
                requiredSpacingRadius: Convert.ToDouble(speciesInstance["requiredSpacingRadius"]),
                cost: Convert.ToDouble(speciesInstance["cost"]),

                initialCrownRadius: Convert.ToDouble(speciesInstance["initialCrownRadius"]),
                matureCrownRadius: Convert.ToDouble(speciesInstance["matureCrownRadius"]),
                varianceCrownRadius: Convert.ToDouble(speciesInstance["varianceCrownRadius"]),
                initialRootRadius: Convert.ToDouble(speciesInstance["initialRootRadius"]),
                matureRootRadius: Convert.ToDouble(speciesInstance["matureRootRadius"]),
                varianceRootRadius: Convert.ToDouble(speciesInstance["varianceRootRadius"]),
                initialHeight: Convert.ToDouble(speciesInstance["initialHeight"]),
                matureHeight: Convert.ToDouble(speciesInstance["matureHeight"]),
                varianceHeightRadius: Convert.ToDouble(speciesInstance["varianceHeightRadius"]),
                initialTrunkRadius: Convert.ToDouble(speciesInstance["initialTrunkRadius"]),
                matureTrunkRadius: Convert.ToDouble(speciesInstance["matureTrunkRadius"]),
                varianceTrunkRadius: Convert.ToDouble(speciesInstance["varianceTrunkRadius"]),
                
                displayR: Convert.ToInt16(speciesInstance["displayR"]),
                displayG: Convert.ToInt16(speciesInstance["displayG"]),
                displayB: Convert.ToInt16(speciesInstance["displayB"])

            );
            return initialisedSpecies;
        }


    }

    class PlantSpecies
    {
        // Naming
        private readonly String speciesName, commonName, indigenousName;
        // Lifespan
        private readonly double timetoMaturity, deathRate;
        // Placement
        private readonly double relativeDistribution, requiredSpacingRadius, cost;
        // Form
        private readonly double initialCrownRadius, matureCrownRadius, varianceCrownRadius;
        private readonly double initialRootRadius, matureRootRadius, varianceRootRadius;
        private readonly double initialHeight, matureHeight, varianceHeightRadius;
        private readonly double initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius;
        // Aesthetics
        private readonly int displayR, displayG, displayB;

        // Init
        public PlantSpecies(
            string speciesName, string commonName, string indigenousName,
            double timetoMaturity, double deathRate, 
            double relativeDistribution, double requiredSpacingRadius, double cost,
            double initialCrownRadius, double matureCrownRadius, double varianceCrownRadius,
            double initialRootRadius, double matureRootRadius, double varianceRootRadius,
            double initialHeight, double matureHeight, double varianceHeightRadius,
            double initialTrunkRadius, double matureTrunkRadius, double varianceTrunkRadius,
            int displayR, int displayG, int displayB
        )
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
            this.varianceHeightRadius = varianceHeightRadius;
            this.initialTrunkRadius = initialTrunkRadius;
            this.matureTrunkRadius = matureTrunkRadius;
            this.varianceTrunkRadius = varianceTrunkRadius;
            this.displayR = displayR;
            this.displayG = displayG;
            this.displayB = displayB;
        }

    }

}
