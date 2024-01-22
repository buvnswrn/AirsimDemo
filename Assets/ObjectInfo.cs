using UnityEngine;

namespace RoboticWarehouse
{
    public class ObjectInfo
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int InstanceId { get; set; }
        
        public bool IsVisible { get; set; }
    }
}