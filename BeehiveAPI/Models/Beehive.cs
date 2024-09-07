// Models/Beehive.cs
using System;
using System.Collections.Generic;

namespace BeehiveAPI.Models
{
    public class Beehive
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Sensor> Sensors { get; set; }
    }
}
