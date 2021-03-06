﻿//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Atmosphere
{
    /// <summary>
    /// Caculate Water net Long-Wave Radiation using Stefan-Boltzmann law
    /// </summary>
    public class WaterNetLongWaveRadiation:LongWaveRadiation
    {
        public WaterNetLongWaveRadiation()
        {
            //Name = "S-B Water LongWave Radiation Model";
            //Descriptions = "Stefan-Boltzmann law based Water LongWave Radiation Model";
            //ID = "AC112";
            EmissivityContant = 0.96;
            ReflectionRate = 0.03;
            
            AirLongWaveRadiationModel = new AirLongWaveRadiation();
            ReflectionRateForAirLongWave = 0.03;
        }

        public ILongWaveRadiation AirLongWaveRadiationModel { get; set; }

        public double ReflectionRateForAirLongWave { get; set; }

        /// <summary>
        /// Calculate water emissivity
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <returns></returns>
        public override double Emissivity(double temperature)
        {
            return EmissivityContant;
        }

        /// <summary>
        ///  Calculate net Long-Wave Radiation Flux
        /// </summary>
        /// <param name="airTemperature">not used</param>
        /// <param name="waterTemperature">in the unit of Celsius degree</param>
        /// <param name="cloudCover">not used</param>
        /// <returns></returns>
        public override double LongWaveRadiationFlux(double airTemperature, double waterTemperature, double cloudCover)
        {
            double essimited = EmissivityContant * base.LongWaveRadiationFlux(waterTemperature, waterTemperature, cloudCover);
            double absorbed = AirLongWaveRadiationModel.LongWaveRadiationFlux(airTemperature, waterTemperature, cloudCover);

            return absorbed - essimited;
        }
    }

    /// <summary>
    /// Caculate  Water net Long-Wave Radiation using Berliand formula
    /// </summary>
    public class BerliandWaterNetLongWaveRadiation : LongWaveRadiation
    {
        public BerliandWaterNetLongWaveRadiation()
        {
            //Name = "Berliand Water LongWave Radiation Model";
            //Descriptions = "Berliand formula based Water LongWave Radiation Model";
            //ID = "AC113";
            EmissivityContant = 0.975;
            VaporPressureModel = new BuckVaporPressure();
        }

        public IVaporPressure VaporPressureModel { get; set; }

        /// <summary>
        /// Calculate water emissivity
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <returns></returns>
        public override double Emissivity(double temperature)
        {
            return EmissivityContant;
        }

        /// <summary>
        /// Calculate net Long-Wave Radiation Flux
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <param name="cloudCover">not used</param>
        /// <returns></returns>
        public override double LongWaveRadiationFlux(double airTemperature, double waterTemperature, double cloudCover)
        {
            double es = VaporPressureModel.VaporPressure(airTemperature);
            double air = EmissivityContant * StefanBoltzmannContant * Math.Pow(airTemperature, 4) * (0.05 * Math.Sqrt(es) - 0.39) * (1 - 0.72 * cloudCover);
            double water=4 * EmissivityContant * StefanBoltzmannContant * Math.Pow(waterTemperature, 3) * (waterTemperature - airTemperature);
            return air - water;
        }
    }
}
