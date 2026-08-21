namespace IESLights
{
    public static class ParseIES
    {
        private const float SpotlightCutoff = 0.1f;

        public static global::IESLights.IESData Parse(string path, global::IESLights.NormalizationMode normalizationMode)
        {
            string[] array = global::System.IO.File.ReadAllLines(path);
            int lineNumber = 0;
            FindNumberOfAnglesLine(array, ref lineNumber);
            if (lineNumber == array.Length - 1)
            {
                throw new global::IESLights.IESParseException("No line containing number of angles found.");
            }
            ReadProperties(array, ref lineNumber, out var numberOfVerticalAngles, out var numberOfHorizontalAngles, out var photometricType);
            global::System.Collections.Generic.List<float> verticalAngles = ReadValues(array, numberOfVerticalAngles, ref lineNumber);
            global::System.Collections.Generic.List<float> horizontalAngles = ReadValues(array, numberOfHorizontalAngles, ref lineNumber);
            global::System.Collections.Generic.List<global::System.Collections.Generic.List<float>> list = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<float>>();
            for (int i = 0; i < numberOfHorizontalAngles; i++)
            {
                list.Add(ReadValues(array, numberOfVerticalAngles, ref lineNumber));
            }
            global::IESLights.IESData iESData = new global::IESLights.IESData
            {
                VerticalAngles = verticalAngles,
                HorizontalAngles = horizontalAngles,
                CandelaValues = list,
                PhotometricType = photometricType
            };
            NormalizeValues(iESData, normalizationMode == global::IESLights.NormalizationMode.Logarithmic);
            if (normalizationMode == global::IESLights.NormalizationMode.EqualizeHistogram)
            {
                EqualizeHistogram(iESData);
            }
            if (photometricType != global::IESLights.PhotometricType.TypeA)
            {
                DiscardUnusedVerticalHalf(iESData);
                SetVerticalAndHorizontalType(iESData);
                iESData.HalfSpotlightFov = CalculateHalfSpotFov(iESData);
            }
            else
            {
                PadToSquare(iESData);
            }
            return iESData;
        }

        private static void DiscardUnusedVerticalHalf(global::IESLights.IESData iesData)
        {
            if (iesData.VerticalAngles[0] != 0f || iesData.VerticalAngles[iesData.VerticalAngles.Count - 1] != 180f)
            {
                return;
            }
            int i;
            for (i = 0; i < iesData.VerticalAngles.Count && !global::System.Linq.Enumerable.Any(iesData.NormalizedValues, (global::System.Collections.Generic.List<float> slice) => slice[i] > 0.1f); i++)
            {
                if (iesData.VerticalAngles[i] == 90f)
                {
                    DiscardBottomHalf(iesData);
                    return;
                }
                if (iesData.VerticalAngles[i] > 90f)
                {
                    iesData.VerticalAngles[i] = 90f;
                    DiscardBottomHalf(iesData);
                    return;
                }
            }
            int i2;
            for (i2 = iesData.VerticalAngles.Count - 1; i2 >= 0 && !global::System.Linq.Enumerable.Any(iesData.NormalizedValues, (global::System.Collections.Generic.List<float> slice) => slice[i2] > 0.1f); i2--)
            {
                if (iesData.VerticalAngles[i2] == 90f)
                {
                    DiscardTopHalf(iesData);
                    break;
                }
                if (iesData.VerticalAngles[i2] < 90f)
                {
                    iesData.VerticalAngles[i2] = 90f;
                    DiscardTopHalf(iesData);
                    break;
                }
            }
        }

        private static void DiscardBottomHalf(global::IESLights.IESData iesData)
        {
            int num = 0;
            for (int i = 0; i < iesData.VerticalAngles.Count && iesData.VerticalAngles[i] != 90f; i++)
            {
                num++;
            }
            DiscardHalf(iesData, 0, num);
        }

        private static void DiscardTopHalf(global::IESLights.IESData iesData)
        {
            int num = 0;
            for (int i = 0; i < iesData.VerticalAngles.Count; i++)
            {
                if (iesData.VerticalAngles[i] == 90f)
                {
                    num = i + 1;
                    break;
                }
            }
            int range = iesData.VerticalAngles.Count - num;
            DiscardHalf(iesData, num, range);
        }

        private static void DiscardHalf(global::IESLights.IESData iesData, int start, int range)
        {
            iesData.VerticalAngles.RemoveRange(start, range);
            for (int i = 0; i < iesData.CandelaValues.Count; i++)
            {
                iesData.CandelaValues[i].RemoveRange(start, range);
                iesData.NormalizedValues[i].RemoveRange(start, range);
            }
        }

        private static void PadToSquare(global::IESLights.IESData iesData)
        {
            if (global::UnityEngine.Mathf.Abs(iesData.HorizontalAngles.Count - iesData.VerticalAngles.Count) > 1)
            {
                int num = global::UnityEngine.Mathf.Max(iesData.HorizontalAngles.Count, iesData.VerticalAngles.Count);
                if (iesData.HorizontalAngles.Count < num)
                {
                    PadHorizontal(iesData, num);
                }
                else
                {
                    PadVertical(iesData, num);
                }
            }
        }

        private static void PadHorizontal(global::IESLights.IESData iesData, int longestSide)
        {
            int num = longestSide - iesData.HorizontalAngles.Count;
            int num2 = num / 2;
            int padBeforeAmount = (iesData.PadAfterAmount = num2);
            iesData.PadBeforeAmount = padBeforeAmount;
            global::System.Collections.Generic.List<float> item = global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Repeat(0f, iesData.VerticalAngles.Count));
            for (int i = 0; i < num2; i++)
            {
                iesData.NormalizedValues.Insert(0, item);
            }
            for (int j = 0; j < num - num2; j++)
            {
                iesData.NormalizedValues.Add(item);
            }
        }

        private static void PadVertical(global::IESLights.IESData iesData, int longestSide)
        {
            int num = longestSide - iesData.VerticalAngles.Count;
            if (global::UnityEngine.Mathf.Sign(iesData.VerticalAngles[0]) == (float)global::System.Math.Sign(iesData.VerticalAngles[iesData.VerticalAngles.Count - 1]))
            {
                int num2 = (iesData.PadBeforeAmount = num / 2);
                iesData.PadAfterAmount = num - num2;
                {
                    foreach (global::System.Collections.Generic.List<float> normalizedValue in iesData.NormalizedValues)
                    {
                        normalizedValue.InsertRange(0, new global::System.Collections.Generic.List<float>(new float[num2]));
                        normalizedValue.AddRange(new global::System.Collections.Generic.List<float>(new float[num - num2]));
                    }
                    return;
                }
            }
            int num4 = longestSide / 2 - global::System.Linq.Enumerable.Count(iesData.VerticalAngles, (float v) => v >= 0f);
            if (iesData.VerticalAngles[0] < 0f)
            {
                iesData.PadBeforeAmount = num - num4;
                iesData.PadAfterAmount = num4;
                {
                    foreach (global::System.Collections.Generic.List<float> normalizedValue2 in iesData.NormalizedValues)
                    {
                        normalizedValue2.InsertRange(0, new global::System.Collections.Generic.List<float>(new float[num - num4]));
                        normalizedValue2.AddRange(new global::System.Collections.Generic.List<float>(new float[num4]));
                    }
                    return;
                }
            }
            iesData.PadBeforeAmount = num4;
            iesData.PadAfterAmount = num - num4;
            foreach (global::System.Collections.Generic.List<float> normalizedValue3 in iesData.NormalizedValues)
            {
                normalizedValue3.InsertRange(0, new global::System.Collections.Generic.List<float>(new float[num4]));
                normalizedValue3.AddRange(new global::System.Collections.Generic.List<float>(new float[num - num4]));
            }
        }

        private static void SetVerticalAndHorizontalType(global::IESLights.IESData iesData)
        {
            if ((iesData.VerticalAngles[0] == 0f && iesData.VerticalAngles[iesData.VerticalAngles.Count - 1] == 90f) || (iesData.VerticalAngles[0] == -90f && iesData.VerticalAngles[iesData.VerticalAngles.Count - 1] == 0f))
            {
                iesData.VerticalType = global::IESLights.VerticalType.Bottom;
            }
            else if (iesData.VerticalAngles[iesData.VerticalAngles.Count - 1] == 180f && iesData.VerticalAngles[0] == 90f)
            {
                iesData.VerticalType = global::IESLights.VerticalType.Top;
            }
            else
            {
                iesData.VerticalType = global::IESLights.VerticalType.Full;
            }
            if (iesData.HorizontalAngles.Count == 1)
            {
                iesData.HorizontalType = global::IESLights.HorizontalType.None;
                return;
            }
            if (iesData.HorizontalAngles[iesData.HorizontalAngles.Count - 1] - iesData.HorizontalAngles[0] == 90f)
            {
                iesData.HorizontalType = global::IESLights.HorizontalType.Quadrant;
                return;
            }
            if (iesData.HorizontalAngles[iesData.HorizontalAngles.Count - 1] - iesData.HorizontalAngles[0] == 180f)
            {
                iesData.HorizontalType = global::IESLights.HorizontalType.Half;
                return;
            }
            iesData.HorizontalType = global::IESLights.HorizontalType.Full;
            if (iesData.HorizontalAngles[iesData.HorizontalAngles.Count - 1] != 360f)
            {
                StitchHorizontalAssymetry(iesData);
            }
        }

        private static void StitchHorizontalAssymetry(global::IESLights.IESData iesData)
        {
            iesData.HorizontalAngles.Add(360f);
            iesData.CandelaValues.Add(iesData.CandelaValues[0]);
            iesData.NormalizedValues.Add(iesData.NormalizedValues[0]);
        }

        private static float CalculateHalfSpotFov(global::IESLights.IESData iesData)
        {
            if (iesData.VerticalType == global::IESLights.VerticalType.Bottom && iesData.VerticalAngles[0] == 0f)
            {
                return CalculateHalfSpotlightFovForBottomHalf(iesData);
            }
            if (iesData.VerticalType == global::IESLights.VerticalType.Top || (iesData.VerticalType == global::IESLights.VerticalType.Bottom && iesData.VerticalAngles[0] == -90f))
            {
                return CalculateHalfSpotlightFovForTopHalf(iesData);
            }
            return -1f;
        }

        private static float CalculateHalfSpotlightFovForBottomHalf(global::IESLights.IESData iesData)
        {
            for (int num = iesData.VerticalAngles.Count - 1; num >= 0; num--)
            {
                for (int i = 0; i < iesData.NormalizedValues.Count; i++)
                {
                    if (iesData.NormalizedValues[i][num] >= 0.1f)
                    {
                        if (num < iesData.VerticalAngles.Count - 1)
                        {
                            return iesData.VerticalAngles[num + 1];
                        }
                        return iesData.VerticalAngles[num];
                    }
                }
            }
            return 0f;
        }

        private static float CalculateHalfSpotlightFovForTopHalf(global::IESLights.IESData iesData)
        {
            for (int i = 0; i < iesData.VerticalAngles.Count; i++)
            {
                for (int j = 0; j < iesData.NormalizedValues.Count; j++)
                {
                    if (!(iesData.NormalizedValues[j][i] >= 0.1f))
                    {
                        continue;
                    }
                    if (iesData.VerticalType == global::IESLights.VerticalType.Top)
                    {
                        if (i > 0)
                        {
                            return 180f - iesData.VerticalAngles[i - 1];
                        }
                        return 180f - iesData.VerticalAngles[i];
                    }
                    if (i > 0)
                    {
                        return 0f - iesData.VerticalAngles[i - 1];
                    }
                    return 0f - iesData.VerticalAngles[i];
                }
            }
            return 0f;
        }

        private static void NormalizeValues(global::IESLights.IESData iesData, bool squashHistogram)
        {
            iesData.NormalizedValues = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<float>>();
            float num = global::System.Linq.Enumerable.Max(global::System.Linq.Enumerable.SelectMany(iesData.CandelaValues, (global::System.Collections.Generic.List<float> v) => v));
            if (squashHistogram)
            {
                num = global::UnityEngine.Mathf.Log(num);
            }
            foreach (global::System.Collections.Generic.List<float> candelaValue in iesData.CandelaValues)
            {
                global::System.Collections.Generic.List<float> list = new global::System.Collections.Generic.List<float>();
                if (squashHistogram)
                {
                    for (int num2 = 0; num2 < candelaValue.Count; num2++)
                    {
                        list.Add(global::UnityEngine.Mathf.Log(candelaValue[num2]));
                    }
                }
                else
                {
                    list.AddRange(candelaValue);
                }
                for (int num3 = 0; num3 < candelaValue.Count; num3++)
                {
                    list[num3] /= num;
                    list[num3] = global::UnityEngine.Mathf.Clamp01(list[num3]);
                }
                iesData.NormalizedValues.Add(list);
            }
        }

        private static void EqualizeHistogram(global::IESLights.IESData iesData)
        {
            int num = global::UnityEngine.Mathf.Min((int)global::System.Linq.Enumerable.Max(global::System.Linq.Enumerable.SelectMany(iesData.CandelaValues, (global::System.Collections.Generic.List<float> v) => v)), 10000);
            float[] array = new float[num];
            float[] array2 = new float[num];
            foreach (global::System.Collections.Generic.List<float> normalizedValue in iesData.NormalizedValues)
            {
                foreach (float item in normalizedValue)
                {
                    array[(int)(item * (float)(num - 1))] += 1f;
                }
            }
            float num2 = iesData.HorizontalAngles.Count * iesData.VerticalAngles.Count;
            for (int num3 = 0; num3 < array.Length; num3++)
            {
                array[num3] /= num2;
            }
            for (int num4 = 0; num4 < num; num4++)
            {
                array2[num4] = global::System.Linq.Enumerable.Sum(global::System.Linq.Enumerable.Take(array, num4 + 1));
            }
            foreach (global::System.Collections.Generic.List<float> normalizedValue2 in iesData.NormalizedValues)
            {
                for (int num5 = 0; num5 < normalizedValue2.Count; num5++)
                {
                    int num6 = (int)(normalizedValue2[num5] * (float)(num - 1));
                    normalizedValue2[num5] = array2[num6] * (float)(num - 1) / (float)num;
                }
            }
        }

        private static void FindNumberOfAnglesLine(string[] lines, ref int lineNumber)
        {
            int i;
            for (i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith("TILT"))
                {
                    try
                    {
                        i = ((!(lines[i].Split('=')[1].Trim() != "NONE")) ? (i + 1) : (i + 5));
                    }
                    catch (global::System.ArgumentOutOfRangeException)
                    {
                        throw new global::IESLights.IESParseException("No TILT line present.");
                    }
                    break;
                }
            }
            lineNumber = i;
        }

        private static void ReadProperties(string[] lines, ref int lineNumber, out int numberOfVerticalAngles, out int numberOfHorizontalAngles, out global::IESLights.PhotometricType photometricType)
        {
            global::System.Collections.Generic.List<float> list = ReadValues(lines, 13, ref lineNumber);
            numberOfVerticalAngles = (int)list[3];
            numberOfHorizontalAngles = (int)list[4];
            photometricType = (global::IESLights.PhotometricType)list[5];
        }

        private static global::System.Collections.Generic.List<float> ReadValues(string[] lines, int numberOfValuesToFind, ref int lineNumber)
        {
            global::System.Collections.Generic.List<float> list = new global::System.Collections.Generic.List<float>();
            while (list.Count < numberOfValuesToFind)
            {
                if (lineNumber >= lines.Length)
                {
                    throw new global::IESLights.IESParseException("Reached end of file before the given number of values was read.");
                }
                char[] separator = null;
                if (lines[lineNumber].Contains(","))
                {
                    separator = new char[1] { ',' };
                }
                string[] array = lines[lineNumber].Split(separator, global::System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in array)
                {
                    try
                    {
                        list.Add(float.Parse(s));
                    }
                    catch (global::System.Exception inner)
                    {
                        throw new global::IESLights.IESParseException("Invalid value declaration.", inner);
                    }
                }
                lineNumber++;
            }
            return list;
        }
    }
}
