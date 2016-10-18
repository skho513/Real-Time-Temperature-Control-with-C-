using NationalInstruments.DAQmx;
using System.Threading;

namespace std
{
    class AnalogI
    {
        //create a new task for an analog input
        Task analogIn = new Task();
        
        AveragingFilter filter = new AveragingFilter();

        //creating a new single analog channel reader
        AnalogSingleChannelReader reader;

        public void OpenChannel(string channel, string channelName)
        {

            //create a new analog inputchannel called "Ainport#"
            analogIn.AIChannels.CreateVoltageChannel(channel,            // the physical name of the channel 
                channelName,                                             // the given name to the channel 
                AITerminalConfiguration.Rse,                             // input type (Differential, RSE, NRSE)
                -10.0, 10.0,                                             // Input voltage range
                AIVoltageUnits.Volts);                                   // input unit 
            // initiliase the single analog input channel reader 
            reader = new AnalogSingleChannelReader(analogIn.Stream);

        }
        
        // Method sets the number of points to average in the filter.
        public void NumberOfPoints(int num)
        {
            filter.Set_NumPoint(num);
        }

        // Method produces a reading from a thermistor.
        // The values have been filtered by an n-th point averager.
        public double ReadData()
        {
            filter.AddToList(reader.ReadSingleSample());  // Adds new data value to list.
            return filter.Get_Data();
        }


        // Method produces readings from a thermistor.
        // The values have not been filtered by a filter.
        public double ReadData_Raw()
        {
            return reader.ReadSingleSample();
        }

    }

}
