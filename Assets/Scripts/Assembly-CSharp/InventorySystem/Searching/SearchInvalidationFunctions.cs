using Mirror;

namespace InventorySystem.Searching
{
    public static class SearchInvalidationFunctions
    {
        public static void Serialize(this NetworkWriter writer, SearchInvalidation value)
        {
            value.Serialize(writer);
        }

        public static SearchInvalidation Deserialize(this NetworkReader reader)
        {
            SearchInvalidation result = default(SearchInvalidation);
            result.Deserialize(reader);
            return result;
        }
    }
}
