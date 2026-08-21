using System;
using UnityEngine;


    public struct CreditsList : IEquatable<CreditsList>, IJsonSerializable
    {

        public CreditsListCategory[] credits;
        public CreditsListCategory[] Members => credits;

        public CreditsList(CreditsListCategory[] credits)
        {
            this.credits = credits;
        }

            public bool Equals(CreditsList other)
        {
            return credits == other.credits;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CreditsList))
                return false;
            return Equals((CreditsList)obj);
        }

         public override int GetHashCode()
        {
            return credits != null ? credits.GetHashCode() : 0;
        }

        public static bool operator ==(CreditsList left, CreditsList right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CreditsList left, CreditsList right)
        {
            return !left.Equals(right);
        }

        
        public object FromJson(object jsonObject)
        {
            return this;
        }

        public object ToJson(object jsonWriter)
        {
            throw new NotImplementedException();
        }

        public object ToDictionary()
        {
            throw new NotImplementedException();
        }
    }
