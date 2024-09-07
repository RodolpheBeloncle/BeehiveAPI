// Models/Sensor.cs
using System;

namespace BeehiveAPI.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public int BeehiveId { get; set; }
        public string SensorType { get; set; }
        public float Value { get; set; }
        public DateTime RecordedAt { get; set; }

        public Beehive Beehive { get; set; }
    }
}