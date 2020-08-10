namespace Groundhog
{
    using System;
    using System.Collections.Generic;

    internal class PlantFactory
    {
        public static Dictionary<string, string> ParseToDictionary(string headers, string values)
        {
            var dictionary = new Dictionary<string, string>();

            var splitKeys = headers.Split(',');
            var splitValues = values.Split(',');

            int i;
            for (i = 0; i < splitKeys.Length; i = i + 1)
                if (!string.IsNullOrEmpty(splitValues[i].Trim()))
                    dictionary.Add(splitKeys[i].Trim(), splitValues[i].Trim());

            return dictionary;
        }

        public static Tuple<PlantSpecies, string> ParseFromDictionary(Dictionary<string, string> speciesInstance)
        {
            var warnings = string.Empty;
            // Naming
            string speciesName, commonName, indigenousName;
            // Lifespan
            double timetoMaturity;
            // Placement
            double requiredSpacingRadius;
            // Form
            double initialCrownRadius, matureCrownRadius, varianceCrownRadius;
            double initialRootRadius, matureRootRadius, varianceRootRadius;
            double initialHeight, matureHeight, varianceHeight;
            double initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius;
            // Aesthetics
            int displayR, displayG, displayB;

            // Naming
            if (speciesInstance.ContainsKey("Species Name"))
            {
                speciesName = speciesInstance["Species Name"];
            }
            else
            {
                speciesName = "Unnamed";
                warnings += "no Species name; ";
            }

            if (speciesInstance.ContainsKey("Common Name"))
            {
                commonName = speciesInstance["Common Name"];
            }
            else
            {
                commonName = "Unnamed";
                warnings += "no Common name; ";
            }

            if (speciesInstance.ContainsKey("Indigenous Name"))
            {
                indigenousName = speciesInstance["Indigenous Name"];
            }
            else
            {
                indigenousName = "Unnamed";
                warnings += "no Indigenous name; ";
            }

            // Lifespan
            if (speciesInstance.ContainsKey("Time to Maturity"))
            {
                timetoMaturity = Convert.ToDouble(speciesInstance["Time to Maturity"]);
            }
            else
            {
                timetoMaturity = 100.0;
                warnings += "no Time to Maturity; ";
            }

            // Placement
            if (speciesInstance.ContainsKey("Spacing Radius"))
            {
                requiredSpacingRadius = Convert.ToDouble(speciesInstance["Spacing Radius"]);
            }
            else
            {
                requiredSpacingRadius = 1000.0;
                warnings += "no Spacing Radius; ";
            }

            // Form
            if (speciesInstance.ContainsKey("Initial Crown Radius"))
            {
                initialCrownRadius = Convert.ToDouble(speciesInstance["Initial Crown Radius"]);
            }
            else
            {
                initialCrownRadius = 10.0;
                warnings += "no Initial Crown Radius; ";
            }

            if (speciesInstance.ContainsKey("Mature Crown Radius"))
            {
                matureCrownRadius = Convert.ToDouble(speciesInstance["Mature Crown Radius"]);
            }
            else
            {
                matureCrownRadius = 10.0;
                warnings += "no Mature Crown Radiu; s";
            }

            if (speciesInstance.ContainsKey("Crown Variance"))
            {
                varianceCrownRadius = Convert.ToDouble(speciesInstance["Crown Variance"]);
            }
            else
            {
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

            if (speciesInstance.ContainsKey("Initial Root Radius"))
            {
                initialRootRadius = Convert.ToDouble(speciesInstance["Initial Root Radius"]);
            }
            else
            {
                initialRootRadius = 1000.0;
                warnings += "no Initial Root Radius; ";
            }

            if (speciesInstance.ContainsKey("Mature Root Radius"))
            {
                matureRootRadius = Convert.ToDouble(speciesInstance["Mature Root Radius"]);
            }
            else
            {
                matureRootRadius = 1000.0;
                warnings += "no Mature Root Radius; ";
            }

            if (speciesInstance.ContainsKey("Root Variance"))
            {
                varianceRootRadius = Convert.ToDouble(speciesInstance["Root Variance"]);
            }
            else
            {
                varianceRootRadius = 10.0;
                warnings += "no Root Variance; ";
            }

            // Aesthetics
            if (speciesInstance.ContainsKey("Display R"))
            {
                displayR = Convert.ToInt16(speciesInstance["Display R"]);
            }
            else
            {
                displayR = 100;
                warnings += "no Display R; ";
            }

            if (speciesInstance.ContainsKey("Display G"))
            {
                displayG = Convert.ToInt16(speciesInstance["Display G"]);
            }
            else
            {
                displayG = 255;
                warnings += "no Display G; ";
            }

            if (speciesInstance.ContainsKey("Display B"))
            {
                displayB = Convert.ToInt16(speciesInstance["Display B"]);
            }
            else
            {
                displayB = 100;
                warnings += "no Display B; ";
            }

            var initialisedSpecies = new PlantSpecies(
                speciesName, commonName, indigenousName,
                timetoMaturity,
                requiredSpacingRadius,
                initialCrownRadius, matureCrownRadius, varianceCrownRadius,
                initialRootRadius, matureRootRadius, varianceRootRadius,
                initialHeight, matureHeight, varianceHeight,
                initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius,
                displayR, displayG, displayB);

            return Tuple.Create(initialisedSpecies, warnings);
        }
    }
}