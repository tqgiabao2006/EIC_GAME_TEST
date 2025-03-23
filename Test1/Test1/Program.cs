namespace Test1;

class Program
{
    static void Main(string[] args)
    {
        //Aphatically order => order in dictionar
        // => "ab" > "ba" => start with "a", "b"
        // => "ab" > "abc" _ not in this case because everything reduce 1 alphabet
        // => abc < acb because b <

        /*
         "abc" =>  "ab"

         brute force O(n^2)

        "bc" == "cb" => "bc" => (int)b*10 + c => bc < cb


        //Greedy



         */
        // Console.WriteLine(LargestSum(new int[][] { new[] { 1, 2, 14 }, new[] { 8, 3, 15 } }, 1));
        Console.WriteLine(MinimumMove(new [] {6,2,3,5,6,3}));
    }


    public static int MinimumMove(int[] A)
    {
        int N = A.Length;
        Array.Sort(A);  // Step 1: Sort the array
        int move = 0;
        int expected = 1; // Numbers should be in range [1, N]

        for (int i = 0; i < N; i++)
        {
            move += Math.Abs(A[i] - expected);
            expected++; 
        }

        return move;
    }

    public static int LargestSum(int[][] arr, int a)
    {
        int n = arr.Length;
        int m = arr[0].Length;

        // Store two largest values per row (value, column index)
        (int max1, int max2, int idx1)[] rowMax = new (int, int, int)[n];

        // Compute row-wise max and second max
        for (int i = 0; i < n; i++)
        {
            int max1 = int.MinValue, max2 = int.MinValue;
            int idx1 = -1;

            for (int j = 0; j < m; j++)
            {
                int val = arr[i][j];
                if (val > max1)
                {
                    max2 = max1;
                    max1 = val;
                    idx1 = j;
                }
                else if (val > max2)
                {
                    max2 = val;
                }
            }

            rowMax[i] = (max1, max2, idx1);
        }

        int result = int.MinValue;

        // Compare every row pair
        for (int i = 0; i < n; i++)
        {
            for (int k = i + 1; k < n; k++)
            {
                // Try picking max1 from row i and max1 from row k
                if (rowMax[i].idx1 != rowMax[k].idx1)
                {
                    result = Math.Max(result, rowMax[i].max1 + rowMax[k].max1);
                }
                else // If they have the same column, take the second max from either row
                {
                    result = Math.Max(result, Math.Max(rowMax[i].max1 + rowMax[k].max2, rowMax[i].max2 + rowMax[k].max1));
                }
            }
        }

        return result;
    }

    public static int LargestSum(int[][] ar)
    {
        int result = int.MinValue;

        for (int i = 0; i < ar.Length; i++)
        {
            for (int j = 0; j < ar[0].Length; j++)
            {
                for (int k = 0; k < ar.Length; k++)
                {
                    for (int g = 0; g < ar[0].Length; g++)
                    {
                        
                        if (ar[i][j] + ar[k][g] > result && j != g && i != k)
                        {
                            result = ar[i][j] + ar[k][g];
                        }
                        
                    }
                }

            }
        }
        return result;
    }

    public static string SmallestSubStr(string s, int a)
    {
        for (int i = 0; i < s.Length - 1; i++)
        {
            if (s[i] > s[i + 1])
            {
                return s.Remove(i, 1);
            }
        }

        return s.Remove(s.Length - 1, 1);
    }
    public static string SmallestSubStr(string s)
    {
        string result =  ""; 
        int minVal = int.MaxValue;
        for (int i = 0; i < s.Length; i++)
        {
            string str = "";
            int val = 0;
                 for (int j = 0; j < s.Length; j++)
               {
                  if (j != i)
                {
                         str += s[j];
                    val = val * 10 + s[j];
                }
            }

            if (val < minVal)
            {
                minVal = val;
                result = str;
            }
        }
           return result;

        }
    }

array[1] = "aaa
array[1] = "bbb
array[1] = "ccc
array[1] = "ddd