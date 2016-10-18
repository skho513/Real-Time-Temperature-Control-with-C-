using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace std
{
    public partial class Form1 : Form
    {
        // DAQ Channels
        DigitalO dOut = new DigitalO();
        AnalogI aIn0 = new AnalogI();
        AnalogI aIn1 = new AnalogI();
        AnalogI aIn2 = new AnalogI();
       
        // Deals with converting voltage value into temperature.
        Conversion convert = new Conversion();

        // Allows for an automatic mode.
        SteadyState eSS_Mode = new SteadyState();

        // Specify the location of the parameters file.
        Parameters parameters = new Parameters("D:\\parameters.txt");

        //
        StreamWriter writetoFile = new StreamWriter(@"\\uoa.auckland.ac.nz\engdfs\home\skho591\Desktop\Peng\Temperature Data.txt");

        // For ending task
        int end_task = 0,
            cooling_temp = 26;

        // Input from parameters file.
        int num_averager = 1;
        string Channel;

        //
        bool writeflag = false;

        // Status of buttons/checkboxes.
        int fanstatus = 0,
            heaterstatus = 0,
            autostatus = 0,
            c1 = 0,
            c1_TEMP = 0,
            c2 = 0,
            c2_TEMP = 0,
            c3 = 0,
            c3_TEMP = 0;

        // Readings from sensors (Updated every 0.1s).
        double channel0,
               channel1,
               channel2,
               channelAverage;

        // Temporary variable to store input temperature value.
        double TEMPVar = 0;
        int    i_SS_MinRunTime_TEMP;
               

        // Replaced with input from textbox
        string SS_Temp = "0",
               SS_MinRunTime = "0";

        int    i_SS_MinRunTime = 20;
        double d_SS_Temp = 0;

        double tempD;
        int tempI;

        public Form1()                                                                          
        {
            InitializeComponent();
            num_averager = parameters.Get_AveragerNumber();
            Channel = parameters.Get_DevChannel();

            // Initialises fan and heater to off.
            dOut.WriteData(0);

            // Opens the channel for writing/reading.
            dOut.OpenChannel(Channel + "/port0");                 // Dev#/port0;
            aIn0.OpenChannel(Channel + "/ai0", "Ainport0");       // Dev#/ai0
            aIn1.OpenChannel(Channel + "/ai1", "Ainport1");       // Dev#/ai1
            aIn2.OpenChannel(Channel + "/ai2", "Ainport2");       // Dev#/ai2

            // Sets number of points to average within the filter.
            aIn0.NumberOfPoints(num_averager);
            aIn1.NumberOfPoints(num_averager);
            aIn2.NumberOfPoints(num_averager);

            // Shows the Parameters
            label5.Text = "Filter: " + num_averager.ToString() + " Point Averager";
            label6.Text = "Channel: " + Channel;

            // Button defaults
            button1.Text = "Automatic Mode is OFF";
            button1.ForeColor = Color.Red;
            button_fan.Text = "Fan is OFF";
            button_fan.ForeColor = Color.Red;
            button_heater.Text = "Heater is OFF";
            button_heater.ForeColor = Color.Red;

            // Label Defaults
            label8.Text = "";
            checkBox1.Text = "Sensor 1";
            checkBox2.Text = "Sensor 2";
            checkBox3.Text = "Sensor 3";
            textBox5.Text = "0";
            textBox8.Text = "0";
            button3.Text = "Update Parameters";
    }

        // Timer 1 runs every 0.1seconds to read.
        private void timer1_Tick(object sender, EventArgs e)                                    
        {
            // If the task is suppose to end.
            if (end_task == 1)
            {

                if (channelAverage!=0) {
                    label8.Text = "Cooling down the system...";
                    button2.Text = "Press again\nto EXIT.";
                    eSS_Mode.Set_MinRunTime(0);
                    eSS_Mode.Set_SteadyState(cooling_temp - 1);
                    eSS_Mode.Run(channelAverage);
                    eSS_Mode.GetChangedState(ref fanstatus, ref heaterstatus);
                }

                if ((channelAverage!=0)&&(channelAverage < cooling_temp))
                {
                    dOut.WriteData(0);
                    writetoFile.Close();
                    Environment.Exit(0);
                }
            }

            // If there is a request to run in automatic mode...
            // ...do these checks:
            else if (autostatus == 1)
            {
                // Does not run when there are no sensors active.
                if ((c1 + c2 + c3) == 0)
                {
                    autostatus = 0;
                    fanstatus = 0;
                    heaterstatus = 0;
                }

                // Does not run when temperature input is not in range.
                else if ((d_SS_Temp < 27) || (d_SS_Temp > 30))
                {
                    autostatus = 0;
                    fanstatus = 0;
                    heaterstatus = 0;
                } // Note: Will be a redundant check if initial is set to a value within range.
                  //       Currently, initial is set to 0.

                // Otherwise run automatic mode:
                else
                {
                    eSS_Mode.Set_MinRunTime(i_SS_MinRunTime);
                    eSS_Mode.Set_SteadyState(d_SS_Temp);
                    eSS_Mode.Run(channelAverage);
                    eSS_Mode.GetChangedState(ref fanstatus, ref heaterstatus);
                }

            }

            // Turns the heater or fan on/off accordingly.
            if ((fanstatus == 1) && (heaterstatus == 1))
            {
                dOut.WriteData(3);
            }
            else if ((fanstatus == 0) && (heaterstatus == 1))
            {
                dOut.WriteData(2);
            }
            else if ((fanstatus == 1) && (heaterstatus == 0))
            {
                dOut.WriteData(1);
            }
            else
            {
                dOut.WriteData(0);
            }

            // Reads the data.
            ain0(); // Sensor 1
            ain1(); // Sensor 2
            ain2(); // Sensor 3

            // No average unless there is at least 1 sensor on.
            // *To avoid zero division.
            if ( (c1 + c2 + c3) == 0) {
                channelAverage = 0;
            }
            else{
                channelAverage = (channel0 + channel1 + channel2) / (c1 + c2 + c3);
            }

            if (writeflag == true)
            {
                writetoFile.WriteLine("{0:00.000} {1:00.000} {2:00.000} {3:00.000} {4:00.000} {5:00.000} {6:00.000};", channel0, channel1, channel2, channelAverage, convert.ConvertValue(1,aIn0.ReadData_Raw()), convert.ConvertValue(2,aIn1.ReadData_Raw()), convert.ConvertValue(3,aIn2.ReadData_Raw()));
            }

        }

        // Reading sensor 1 is put into thread1
        private void ain0()                                                                     
        {
            if (c1 == 1)
            {
                aIn0.ReadData();
                channel0 = convert.ConvertValue(1, aIn0.ReadData());
            }
            else {
                channel0 = 0;
            }
            
        }

        // Reading sensor 2 is put into thread2
        private void ain1()                                                                     
        {
            if (c2 == 1)
            {
                channel1 = convert.ConvertValue(2, aIn1.ReadData());
            }
            else {
                channel1 = 0;
            }
        }

        // Reading sensor 3 is put into thread3
        private void ain2()                                                                     
        {
            if (c3 == 1)
            {
                channel2 = convert.ConvertValue(3, aIn2.ReadData());
            }
            else {
                channel2 = 0;
            }
        }

        // Timer 2 runs every 0.5s.
        // Deals with displaying colours and text and reading from textbox.
        private void timer2_Tick(object sender, EventArgs e)                                    
        {
            // Outputs sensor readings to textbox.
            // Value of 0 is displayed if tickbox not ticked.
            textBox1.Text = channel0.ToString();
            textBox2.Text = channel1.ToString();
            textBox3.Text = channel2.ToString();
            textBox4.Text = channelAverage.ToString();
            textBox6.Text = d_SS_Temp.ToString();

            // Reads variables from textbox.
            if ((SS_Temp.Length != 0)&&(double.TryParse(SS_Temp, out tempD)))
            {
                TEMPVar = Convert.ToDouble(SS_Temp);
            }   // Stores a temporary variable.
            if ((SS_MinRunTime.Length != 0) && (int.TryParse(SS_MinRunTime, out tempI)))
            {            // Only updates the variables if there is a value.
                i_SS_MinRunTime_TEMP = Convert.ToInt16(SS_MinRunTime);
            }

            // Changes button colour and text based on status of heater.
            if (heaterstatus == 1)
            {
                button_heater.ForeColor = Color.Green;
                button_heater.Text = "Heater is ON";
            }
            else
            {
                button_heater.ForeColor = Color.Black;
                button_heater.Text = "Heater is OFF";
            }

            // Changes button colour and text based on status of fan.
            if (fanstatus == 1)
            {
                button_fan.ForeColor = Color.Green;
                button_fan.Text = "Fan is ON";
            }
            else
            {
                button_fan.ForeColor = Color.Black;
                button_fan.Text = "Fan is OFF";
            }

            // Changes button colour and text if Automatic mode is turned off/on.
            if (autostatus == 0)
            {
                button1.Text = "Automatic Mode is OFF";
                button1.ForeColor = Color.Black;
            }
            else
            {
                button1.Text = "Automatic Mode is ON";
                button1.ForeColor = Color.Green;
            }
            
            
        }

        private void button1_Click_1(object sender, EventArgs e)          // Heater             
        {
            if (autostatus == 0) // Button only operable when Automatic mode is off.
            {
                if (heaterstatus == 1)
                {
                    heaterstatus = 0;
                }
                else
                {
                    heaterstatus = 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)            // Fan                
        {
            if (autostatus == 0) // Button only operable when Automatic mode is off.
            {
                if (fanstatus == 1)
                {
                    fanstatus = 0;
                }
                else
                {
                    fanstatus = 1;
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)          // Automatic Mode     
        {
            fanstatus = 0;
            heaterstatus = 0;

            label8.Text = "";
            if (autostatus == 1)
            {
                autostatus = 0;
            }
            else
            {
                autostatus = 1;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)                           
        {
            SS_MinRunTime = textBox8.Text;
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (writeflag)
            {
                writeflag = false;
                writetoFile.Close();
                button4.Text = "Press to log data.";
            }

            else
            {
                writeflag = true;
                button4.Text = "Logging data...";
            }
        }

        private void label13_Click(object sender, EventArgs e)                                  
        {

        }

        private void button3_Click(object sender, EventArgs e)                                  
        {
            c1 = c1_TEMP;
            c2 = c2_TEMP;
            c3 = c3_TEMP;
            i_SS_MinRunTime = i_SS_MinRunTime_TEMP;

            // Warning in case there are no sensor inputs.
            if ((c1_TEMP + c2_TEMP + c3_TEMP) == 0)
            {
                label8.Text = "WARNING: No sensor input!";
            }
            else
            {
                if ((TEMPVar > 26) && (TEMPVar < 31))
                {
                    // Only updates the steady state temperature if input is in range.
                    d_SS_Temp = TEMPVar;
                    label8.Text = "";
                }
                else
                {
                    label8.Text = "WARNING: New value not in range.\nRange: 27degC - 30degC.";
                }
            }

        }
        
        private void button2_Click(object sender, EventArgs e)            // Thread testing     
        {
            fanstatus = 0;
            heaterstatus = 0;
            end_task += 1;
            button_heater.Enabled = false;
            button_fan.Enabled = false;
            button1.Enabled = false;

            if ((end_task == 1) && ((c1 + c2 + c3) == 0))
            {
                button2.Text = "Requires sensor\nreading to cool";
            }

            if (end_task > 1)
            {
                dOut.WriteData(0);
                writetoFile.Close();
                Environment.Exit(0);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)     // 'Average' textbox  
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)     // Thermistor 1       
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)     // Thermistor 2       
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)     // Thermistor 3       
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)                           
        {
            SS_Temp = textBox5.Text;
        }

        private void label5_Click(object sender, EventArgs e)             // N-th pt Averager   
        {

        }

        private void Form1_Load(object sender, EventArgs e)                                     
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) // Checkbox Therm1    
        {
            c1_TEMP = Convert.ToInt16(checkBox1.Checked);
        }
        
        private void checkBox2_CheckedChanged(object sender, EventArgs e) // Checkbox Therm3    
        {
            c2_TEMP = Convert.ToInt16(checkBox2.Checked);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) // Checkbox Therm2    
        {
            c3_TEMP = Convert.ToInt16(checkBox3.Checked);
        }
    }
}