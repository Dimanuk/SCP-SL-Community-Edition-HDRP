using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.DeferredDecals
{
    public class DecalsGroup : IEquatable<DecalsGroup>
    {
        public List<Decal> CameraInsideDecalList;
        public List<Decal> CameraOutsideDecalList;
        public Material Material;
        public bool Instancing;
        private int _instanceID;

        public DecalsGroup(Material material)
        {
            CameraInsideDecalList = new List<Decal>();
            CameraOutsideDecalList = new List<Decal>();
            Material = material;

            if (material != null)
            {
                Instancing = material.enableInstancing;
                _instanceID = material.GetInstanceID();
            }
        }

        public bool Equals(DecalsGroup other)
        {
            if (other == null)
                return false;
            return _instanceID == other._instanceID;
        }

        public override int GetHashCode()
        {
            return _instanceID;
        }
    }
}
