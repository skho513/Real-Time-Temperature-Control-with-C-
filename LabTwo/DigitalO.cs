using System;
using NationalInstruments.DAQmx;

namespace std
{
    class DigitalO
    {
        //Create a new task for a digital channel
        Task digitalOut = new Task();
        //Create a new single channel writer
        DigitalSingleChannelWriter writer;

        public void OpenChannel(string channel)
        {
            //Create a new digital output Channel "called Doutport0"
            digitalOut.DOChannels.CreateChannel(channel,             //assigned line for the channel
             "DigitalChn0",                                          //assigned name for the channel
             ChannelLineGrouping.OneChannelForAllLines);             //grouping feature of the lines

            //Initialise the single channel writer and assign stream of channel to it
            writer = new DigitalSingleChannelWriter(digitalOut.Stream);
        }

        public void WriteData(int length)
        {
            //call WriteSingleSamplePort method to write the data to the channel
            if (writer != null)
                writer.WriteSingleSamplePort(true, (UInt32)length);
        }
    }
}
