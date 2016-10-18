using System;

namespace std
{

    class Conversion
    {
        double Converted = 0;
        int roomtemp = 298; // room temp 25 celsius in kelvins
        double R_0;
        double R_1;
        double R_2;
        int R0 = 10;
        int R1 = 5;
        int R2 = 100;
        int B0 = 3380;
        int B1 = 3960;
        int B2 = 4380;
        int vin = 5;

        public double ConvertValue(int REF, double data)
        {

            if (REF == 1) {
                R_0 = (R0 * data / (vin - data));
                Converted = (roomtemp * B0) / ((roomtemp) * Math.Log(R_0 / R0) + B0);
            }
            else if (REF == 2) {
                R_1 = (R1 * data / (vin - data));
                Converted = (roomtemp * B1) / ((roomtemp) * Math.Log(R_1 / R1) + B1);
            }
            else {
                R_2 = (R2 * data / (vin - data));
                Converted = (roomtemp * B2) / ((roomtemp) * Math.Log(R_2 / R2) + B2);
            }

            return (Converted - 273);
        }

    }
}