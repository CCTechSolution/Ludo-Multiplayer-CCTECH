using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyString
{
	public enum FuzzyStringComparisonOptions
	{
		UseHammingDistance,

		UseJaccardDistance,

		UseJaroDistance,

		UseJaroWinklerDistance,

		UseLevenshteinDistance,

		UseLongestCommonSubSequence,

		UseLongestCommonSubstring,

		UseNormalizedLevenshteinDistance,

		UseOverlapCoefficient,

		UseRatcliffObershelpSimilarity,

		UseSorensenDiceDistance,

		UseTanimotoCoefficient,

		CaseSensitive
	}
}
