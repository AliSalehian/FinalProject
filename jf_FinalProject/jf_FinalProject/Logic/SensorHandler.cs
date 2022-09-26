using System;
using System.Collections.Generic;
using jf_FinalProject.Logger;
using System.Runtime.CompilerServices;

namespace jf
{
    /// <summary>
    /// class <c>SensorHandler</c> is a handler for all our sensors.
    /// </summary>
    class SensorHandler
    {
        #region Attributes Of Class

        /// <summary>
        /// <c>sensors</c> attribute is an array of <c>Sensor</c> objects
        /// </summary>
        private Sensor[] sensors;
        private Dictionary<string, Tuple<double, double>> calibrations;
        #endregion

        #region Constructors Of Class
        public SensorHandler()
        {
            //this.calibrations = new Tuple<double, double>[7];
            //for (int i=0; i< this.calibrations.Length; i++)           
            //    this.calibrations[i] = new Tuple<double, double>(1.0, 0.0);
            this.sensors = new Sensor[7];
            this.calibrations = new Dictionary<string, Tuple<double, double>>();

            this.sensors[0] = new Sensor("t1");
            this.calibrations["t1"] =  new Tuple<double, double>(1.0, 0.0);

            this.sensors[1] = new Sensor("t2");
            this.calibrations["t2"] = new Tuple<double, double>(1.0, 0.0);
            
            this.sensors[2] = new Sensor("t3");
            this.calibrations["t3"] = new Tuple<double, double>(1.0, 0.0);

            this.sensors[3] = new Sensor("p");
            this.calibrations["p"] = new Tuple<double, double>(1.0, 0.0);

            this.sensors[4] = new Sensor("mleft");
            this.calibrations["mleft"] = new Tuple<double, double>(1.0, 0.0);

            this.sensors[5] = new Sensor("mright");
            this.calibrations["mright"] = new Tuple<double, double>(1.0, 0.0);

            this.sensors[6] = new Sensor("n");// TODO: haji erafn should ask about it
            this.calibrations["n"] = new Tuple<double, double>(1.0, 0.0);
            // TODO: there is another sensor that haji erfan should ask about it
        }
        #endregion

        #region Methods Of Class

        /// <summary>
        /// <c>getSensor</c> method is costum getter of sensor. its get name
        /// of a sensor and return current value of that sensor.
        /// (<paramref name="sensorName"/>)
        /// </summary>
        /// <param name="sensorName">is a string and its name of sensor that we neet its value</param>
        /// <returns>a float number that is current value of sensor. return 0 if sensor name is not in
        /// our list of sensors.
        /// </returns>
        public float getSensor(string sensorName)
        {
            foreach(Sensor sensor in this.sensors)
            {
                if (sensorName.Equals(sensor.sensorName))
                {
                    return sensor.sensorValue;
                }
            }
            return 0;
        }

        /// <summary>
        /// <c>setSensor</c> method set a value for a sensor
        /// (<paramref name="sensorName"/>, <paramref name="sensorValue"/>)
        /// </summary>
        /// <param name="sensorName">is a string and is name of sensor</param>
        /// <param name="sensorValue">is a float and is new value that we wanna set for sensor</param>
        public void setSensor(string sensorName, float sensorValue)
        {
            sensorName = sensorName.ToLower();
            for (int i = 0; i < this.sensors.Length; i++)
            {
                if (this.sensors[i].sensorName.Equals(sensorName.ToLower()))
                {
                    Tuple<double, double> selectedSensorCalibration = this.calibrations[sensorName];
                    this.sensors[i].sensorValue = 
                        (float)((sensorValue * selectedSensorCalibration.Item1) + selectedSensorCalibration.Item2);
                    break;
                }
            }
        }

        /// <summary>
        /// <c>checkSensorExist</c> method check that a sensor is exist or not
        /// (<paramref name="sensorName"/>)
        /// </summary>
        /// <param name="sensorName">is a string and is name of sensor that we wanna check it</param>
        /// <returns>a boolean. true if sensor exist else false.</returns>
        public bool checkSensorExist(string sensorName)
        {
            foreach (Sensor sensor in this.sensors)
            {
                if (sensor.sensorName.Equals(sensorName.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public Tuple<double, double> getCalibration(string sensorName)
        {
            sensorName = sensorName.ToLower();
            try
            {
                Tuple<double, double> result = this.calibrations[sensorName];
                return result;
            }
            catch (Exception)
            {
                Logger.Log(Logger.GetCurrentLine(), "SensorHandler.cs", $"sensor '{sensorName}' is not in sensor list");
                return new Tuple<double, double>(0.0, 0.0);
            }
        }

        public void setCalibration(string sensorName, double gain, double arzAzMabda)
        {
            sensorName = sensorName.ToLower();
            try
            {
                this.calibrations[sensorName] = new Tuple<double, double>(gain, arzAzMabda);
            }
            catch (Exception)
            {
                Logger.Log(Logger.GetCurrentLine(), "SensorHandler.cs", $"sensor '{sensorName}' is not in sensor list");
            }
        }
        #endregion
    }
}
