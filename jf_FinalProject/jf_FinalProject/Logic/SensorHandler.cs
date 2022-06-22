using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        #region Constructors Of Class
        public SensorHandler()
        {
            this.sensors = new Sensor[8];
            this.sensors[0] = new Sensor("t1");
            this.sensors[1] = new Sensor("t2");
            this.sensors[2] = new Sensor("t3");
            this.sensors[3] = new Sensor("p");
            this.sensors[4] = new Sensor("mleft");
            this.sensors[5] = new Sensor("mright");
            this.sensors[6] = new Sensor("handbrake");
            this.sensors[7] = new Sensor("n");// TODO: haji erafn should ask about it
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
            foreach (Sensor sensor in this.sensors)
            {
                if (sensorName.Equals(sensor.sensorName.ToLower()))
                {
                    sensor.sensorValue = sensorValue;
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
        #endregion
    }
}
