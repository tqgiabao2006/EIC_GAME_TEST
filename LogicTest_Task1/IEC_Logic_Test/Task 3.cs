namespace IEC_Logic_Test;

public class Task_3
{
     public int Solution(int[] A)
    {
        //Method #1: Greedy
        // Sort first O(nlogn), then compare to expected number
        // Because number restrict in range [1,N], which mean if if ar.len = 6 => then ideal array should be
        // {1,2,3,4,5,6}
        // Calculate difference between current value to the expected value then add to move
        
        // Time complexity: O(n) + O(nlogn) = O(nlogn)
        // Space complexity: O(1)
       
        /*Array.Sort(ar);
        int expected = 1;
        int cnt = 0;

        for (int i = 0; i < ar.Length; i++)
        {
            if (ar[i] != expected)
            {
                cnt += Math.Abs(ar[i] - expected);
            }
            expected++;
        }

        if (cnt > 1 * Math.Pow(10,9))
        {
            return -1;
        }
        return cnt;
        */
        
        
        //Method #2: Hashstable && Greedy
        // Time complexity: O(3n) = O(n)
        // Space complexity: O(n^2) worst case, average case O(n)
        
        Dictionary<int, int> ocurrences = new Dictionary<int, int>();

        for (int i = 0; i < A.Length; i++)
        {
            if (!ocurrences.ContainsKey(i+1))
            {
                ocurrences.Add(i+ 1, 0);
            }
            
            if (ocurrences.ContainsKey(A[i]))
            {
                ocurrences[A[i]]++;
            }
            else
            {
                ocurrences.Add(A[i], 1);
            }
        }

        int cnt = 0;
        
        //Because small value should be remapped to small value => minimum steps
        Queue<int> st = new Queue<int>();
        for (int i = 0; i < A.Length; i++)
        {
            if (ocurrences[i + 1] == 0)
            {
                st.Enqueue(i+1);
            }
        }

        for (int i = 0; i < A.Length; i++)
        {
            if(ocurrences[i + 1] > 1 && st.Count > 0)
            {
                ocurrences[i + 1]--;
                cnt += Math.Abs(i+1 - st.Dequeue());
            }
        }
        return cnt;
    }
}