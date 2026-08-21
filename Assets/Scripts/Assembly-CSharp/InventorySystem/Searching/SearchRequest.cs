using InventorySystem.Items.Pickups;
using Mirror;
using System;

namespace InventorySystem.Searching
{
    public struct SearchRequest : ISearchSession, NetworkMessage, IEquatable<SearchRequest>
    {
        private SearchSession _body;

        public byte Id { get; private set; }

        public SearchSession Body => _body;

        public ItemPickupBase Target
        {
            get => _body.Target;
            set => _body.Target = value;
        }

        public double InitialTime
        {
            get => _body.InitialTime;
            set => _body.InitialTime = value;
        }

        public double FinishTime
        {
            get => _body.FinishTime;
            set => _body.FinishTime = value;
        }

        public double Progress => _body.Progress;

        public SearchRequest(byte id, SearchSession body)
        {
            Id = id;
            _body = body;
        }

        public void Deserialize(NetworkReader reader)
        {
            Id = reader.ReadByte();
            _body.Deserialize(reader);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.WriteByte(Id);
            _body.Serialize(writer);
        }

        public bool Equals(SearchRequest other)
        {
            return Id == other.Id && _body.Equals(other._body);
        }

        public override bool Equals(object obj)
        {
            return obj is SearchRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_body.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }
    }
}