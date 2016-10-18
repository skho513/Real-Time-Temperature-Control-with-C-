using System;
using System.IO;

namespace std
{
	class Parameters
    {

        StreamReader ParametersFile;
        string DevChannel;
        int AveragerNumber = 1;
        double SteadyStateTemperature = 25;
        
        // Constructor initialises file directory.
        public Parameters(string FileDir)
        {
            ParametersFile = new StreamReader(FileDir);
            this.ReadFile();
        }

        private void ReadFile()
        {
            string line;
            string DevChannelTxt = "DevChannel";
            string NumberOfPointsTxt = "NumberOfPoints";

            int DevSize = DevChannelTxt.Length;
            int NumberOfPointsSize = NumberOfPointsTxt.Length;

            while ( (line = ParametersFile.ReadLine()) != null)
            {
                if ( String.Compare( line, 0, DevChannelTxt, 0, DevSize) == 0)
                {
                    DevChannel = line.Substring( DevSize + 1, line.Length - DevSize - 1);
                }
                else if ( String.Compare( line, 0, NumberOfPointsTxt, 0, NumberOfPointsSize) == 0)
                {
                    Int32.TryParse( line.Substring( NumberOfPointsSize + 1, line.Length - NumberOfPointsSize - 1), out AveragerNumber);
                }
            }

        }

        // Methods used for accessing the parameters:
        public string Get_DevChannel()
        {
            return DevChannel;
        }
        public int Get_AveragerNumber()
        {
            return AveragerNumber;
        }
        public double Get_SteadyStateTemperature()
        {
            return SteadyStateTemperature;
        }

    }
}
