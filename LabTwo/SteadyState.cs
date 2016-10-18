namespace std
{
    class SteadyState
    {
        double eSS = 25,    // Default to room temp.
               sensor_degC = 0;

        int FanState = 0,
            HeaterState = 0,
            RunTime = 1022,
            MinRunTime = 0;

        public void Run(double readings)
        {
            sensor_degC = readings;
            RunTime += 1;

            if (RunTime > MinRunTime)
            {
                // The readings have to stay within +/- 0.2 degC of input stead state.
                if (readings > eSS /*+ 0.05*(eSS - 27)*/)         // If actual temp is higher than desired.
                {
                    FanState = 1;       // Cool the system down.
                    HeaterState = 0;
                    RunTime = 0;
                }
                else if (readings < eSS /*- 0.05*(30-eSS)*/)    // If actual temp is lower than desired.
                {
                    FanState = 0;       // Heat the system up.
                    HeaterState = 1;
                    RunTime = 0;
                }
                else
                {
                    FanState = 0;       // Otherwise there is no need to operate anything.
                    HeaterState = 0;
                }
            }
        }

        public void GetChangedState(ref int fan, ref int heater)
        {
            fan = FanState;
            heater = HeaterState;
        }
        public void Set_MinRunTime(int x)
        {
            MinRunTime = x;
        }
        public void Set_SteadyState(double num)
        {
            eSS = num;
        }

    }
}
