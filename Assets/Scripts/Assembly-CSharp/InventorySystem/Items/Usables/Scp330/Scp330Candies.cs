using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp330
{
    public static class Scp330Candies
    {
        public static readonly ICandy[] AllCandies;
        private static bool _dictionarySet;
		public static bool Dictionarized
		{
			get
			{
				return _dictionarySet;
			}
			set
			{
				_dictionarySet = value;
			}
		}
        public static readonly Dictionary<CandyKindID, ICandy> DictionarizedCandies;

        public static Dictionary<CandyKindID, ICandy> CandiesById
        {
            get
            {
                if (!_dictionarySet)
                {
                    foreach (ICandy candy in AllCandies)
                    {
                        if (candy != null)
                            DictionarizedCandies[candy.Kind] = candy;
                    }
                    _dictionarySet = true;
                }

                return DictionarizedCandies;
            }
        }

        public static CandyKindID GetRandom()
        {
            float totalWeight = 0f;

            foreach (ICandy candy in AllCandies)
            {
                if (candy != null)
                    totalWeight += candy.SpawnChanceWeight;
            }

            float randomValue = Random.Range(0f, totalWeight);

            foreach (ICandy candy in AllCandies)
            {
                if (candy == null)
                    continue;

                randomValue -= candy.SpawnChanceWeight;

                if (randomValue <= 0f)
                    return candy.Kind;
            }

            return CandyKindID.None;
        }

        static Scp330Candies()
        {
            AllCandies = new ICandy[7];

            AllCandies[0] = new CandyGreen();
            AllCandies[1] = new CandyPurple();
            AllCandies[2] = new CandyRainbow();
            AllCandies[3] = new CandyRed();
            AllCandies[4] = new CandyYellow();
            AllCandies[5] = new CandyBlue();
            AllCandies[6] = new CandyPink();

            _dictionarySet = false;
            DictionarizedCandies = new Dictionary<CandyKindID, ICandy>();
        }
    }
}
