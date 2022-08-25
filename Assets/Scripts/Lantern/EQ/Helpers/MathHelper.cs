using System.Collections.Generic;

public static class MathHelper
{
    public static int GetPowerOfTwo(int power)
    {
        return 1 << power;
    }

    private static Dictionary<int, int> _powerOfTwo = new Dictionary<int, int>
    {
        {1, 0},
        {2, 1},
        {4, 2},
        {8, 3},
        {16, 4},
        {32, 5},
        {64, 6},
        {128, 7},
        {256, 8},
        {512, 9},
        {1024, 10},
        {2048, 11},
        {4096, 12},
        {8192, 13},
        {16384, 14},
        {32768, 15},
        {65536, 16},
        {131072, 17},
        {262144, 18},
        {524288, 19},
        {1048576, 20},
    };
    
    public static int GetInversePowerOfTwo(int value)
    {
        if (!_powerOfTwo.ContainsKey(value))
        {
            return -1;
        }

        return _powerOfTwo[value];
    }
}
