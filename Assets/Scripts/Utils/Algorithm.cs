
using System;
using System.Collections.Generic;

public class Algorithm {
    private static Random random = new Random();

    public static List<T> RandomSample<T>(List<T> dataList, int count)
    {
        if (dataList == null || count <= 0)
            return new List<T>();
        if (count >= dataList.Count)
            return new List<T>(dataList);
        if(count == 1)
        {
            int randomIndex = random.Next() % dataList.Count;
            return new List<T> { dataList[randomIndex] };
        }
        List<T> samples = new List<T>();
        List<int> orderedSequence = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            orderedSequence.Add(i);
        }
        while (samples.Count < count)
        {
            int randomIndex = random.Next() % orderedSequence.Count;
            int dataIndex = orderedSequence[randomIndex];
            samples.Add(dataList[dataIndex]);
            orderedSequence.RemoveAt(randomIndex);
        }
        return samples;
    }

    public static byte[] AppendLength(byte[] input)
    {
        if (input == null)
            return input;
        byte[] newArray = new byte[input.Length + 4];
        input.CopyTo(newArray, 4);
        System.BitConverter.GetBytes(input.Length).CopyTo(newArray, 0);
        return newArray;
    }

    public static int ArgMax<T>(T[] input) where T : IComparable
    {
        if (input == null || input.Length == 0)
            return -1;
        int maxIndex = 0;
        T maxValue = input[0];
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i].CompareTo(maxValue) > 0)
            {
                maxValue = input[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }
}
