using System.Collections.Generic;
using System.Linq;

namespace std
{

    class AveragingFilter
    {
        // Default set to no averaging incase not specified.
        int num_point = 1;
        List<double> Samples = new List<double>();

        // Method will add new values to the list.
        // The list will either expand if there is not enough values.
        // Or it will shift values along if there are enough values.
        public void AddToList(double data)
        {

            if (Samples.Count() < num_point)                // If the list is not up to size...
            {
                Samples.Add(data);                          // ...add data.
            }
            else
            {                                               // Shifts data in the list if list is full.
                for (int i = num_point - 1; i > 0; i--)
                {
                    Samples[i] = Samples[i - 1];
                }
                Samples[0] = data;                          // Reads in new data value into list.
            }
        }

        // Sets the averager to a n-point averager.
        public void Set_NumPoint(int number_of_points)
        {
            num_point = number_of_points;
        }

        // Gets the averaged value.
        public double Get_Data()
        {
            double AverageValue = 0;

            for (int i = 0; i < Samples.Count(); i++)
            {
                AverageValue += Samples[i];
            }

            return AverageValue / (Samples.Count());
        }
    }
}