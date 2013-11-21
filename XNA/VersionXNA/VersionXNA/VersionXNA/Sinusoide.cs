using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2
{
    public class Sinusoid
    {
        private static Sinusoid instance;

        //Arrays which keep all data.
        private float[] counters;
        private float[] xOffsets;
        private float[] amplitudes;
        private float[] periods;
        private float[] adjustedPeriods;
        private float[] sinResults;

        //Size variables.
        private int maxSize;
        private int size;

        private Sinusoid()
        {
            counters = new float[1];
            xOffsets = new float[1];
            amplitudes = new float[1];
            periods = new float[1];
            adjustedPeriods = new float[1];
            sinResults = new float[1];

            maxSize = 1;
            size = 0;
        }

        public static Sinusoid GetInstance()
        {
            if (instance == null)
            {
                instance = new Sinusoid();
            }

            return instance;
        }

        /// <summary>
        /// Create a new graph to update.
        /// </summary>
        /// <param name="period">How long do you want it to be.</param>
        /// <param name="xOffset">How far do you want the x offset to be.</param>
        /// <param name="amplitude">How high and low can the graph go.</param>
        /// <returns>The graph's index.</returns>
        public int AddGraph(float period, float xOffset, float amplitude)
        {
            //Is there space for another graph?
            if (size == maxSize)
            {
                //Increase all sizes.
                counters = IncreaseSize(counters);
                xOffsets = IncreaseSize(xOffsets);
                amplitudes = IncreaseSize(amplitudes);
                periods = IncreaseSize(periods);
                adjustedPeriods = IncreaseSize(adjustedPeriods);
                sinResults = IncreaseSize(sinResults);

                maxSize++;
            }

            //Fill the arrays.
            counters[size] = 0;
            xOffsets[size] = xOffset;
            periods[size] = period;
            adjustedPeriods[size] = MathHelper.TwoPi / period;
            amplitudes[size] = amplitude;

            //Fill in the first results.
            sinResults[size] = amplitude * (float)Math.Sin(adjustedPeriods[size] * (0 - xOffset));

            //Return the last size and increment it.
            return size++;
        }

        /// <summary>
        /// Get a random index number for sinusoid results.
        /// </summary>
        public int GetRandomIndex()
        {
            Random randomizer = new Random();
            int ran = randomizer.Next(size - 1);
            return ran;
        }

        /// <summary>
        /// Get the result of one of the sinusoids.
        /// Unavailable index returns 0.
        /// </summary>
        /// <param name="index">The index of the sinusoid you want.</param>
        /// <returns>The index' sinusoid result. 0 if index is unavailable.</returns>
        public float GetSinusoid(int index)
        {
            if (index < size && index >= 0)
            {
                return sinResults[index];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Increases the size of an array by 1.
        /// </summary>
        /// <param name="target">The target array</param>
        /// <returns>The array with the same data but with 1 more spot left.</returns>
        private float[] IncreaseSize(float[] target)
        {
            float[] temp = new float[target.Length + 1];

            for (int i = 0; i < target.Length; i++)
            {
                temp[i] = target[i];
            }

            return temp;
        }

        public void Reset()
        {
            //We're only resetting the size in case we need the arrays later.
            size = 0;
        }

        public void Update(GameTime gameTime)
        {
            float dT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Add the elapsed time to all counters.
            for (int i = 0; i < size; i++)
            {
                counters[i] += dT;

                if (counters[i] >= periods[i])
                {
                    counters[i] -= periods[i];
                }
            }

            /**
             * Calculate the results using the following mathematical expression.
             * f(x) = a + b sin(c (x - d))
             * a = the sinusoid's center height. In this function set to 0.
             * b = amplitude.
             * c = 2Pi / period. (adjustedPeriods)
             * d = xOffset.
             * x = counter.
             */
            for (int i = 0; i < size; i++)
            {
                sinResults[i] = amplitudes[i] * (float)Math.Sin(adjustedPeriods[i] * 
                    (counters[i] - xOffsets[i]));
            }
        }
    }
}
