namespace IEC_Logic_Test;

public class Task_2
{
    public int Solution(int[][] A)
    {
        //Method #1: Brute force: 2 Loops select first number, 2 loops select another number
        //Time complexity: O(n^4) 
        //Space complexity: O(1) 
        
        /*int maxVal = int.MinValue;
        int n = ar.Length; //Row
        int m = ar[0].Length; //Col

        for (int i = 0; i < n; i++) 
        {
            for (int j = 0; j < m; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    for (int q = 0; q < m; q++)
                    {
                        if (i != k && j != q)
                        {
                            if(ar[i][j] + ar[k][q] > maxVal)
                            {
                                maxVal = ar[i][j] + ar[k][q]; 
                            }
                        }
                    }
                }
            }
        }
        
        return maxVal;
        */
        
        //Method #2: Greedy
        //(value, col index) largest value of each row: {1,4}, {2,3} pick {4, 3}
        // => IF PICK 4 => 2 
        // => If pick 3 => 1 
        // => Pick 4 + 2 = 6 => largest
        // If 4,3 != col index => pick 4,3 
        // if 4 and 3 has same col index => Greedy => pick second largest
        
        //Time Complexity: O(n^2 + n^2) = O(2n^2) = O(n^2)
        //Space Complexity: O(3n) = O(n)

        int n = A.Length;
        int m = A[0].Length;
        
        (int, int, int)[] rows = new (int, int, int)[n]; // (Max Row, Second Row, Col index of max row)

        for (int i = 0; i < n; i++)
        {
            int maxRow = int.MinValue;
            int secondRow = int.MinValue;
            
            for (int j = 0; j < m; j++)
            {
                if (A[i][j] > maxRow)
                {
                    secondRow = maxRow;
                    maxRow = A[i][j];
                    
                    rows[i].Item1 = A[i][j];
                    rows[i].Item3 = j;

                }else if (A[i][j] > secondRow)
                {
                    secondRow = A[i][j];
                }
            }
            rows[i].Item2 = secondRow;
        }
        
        int result = int.MinValue;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (rows[i].Item3 != rows[j].Item3) //Diffeern col => save to choose
                {
                    result = Math.Max(result, rows[i].Item1 + rows[j].Item1);
                }
                else //If same choose second largest in that row
                {
                    result = Math.Max(result, rows[i].Item1 + rows[j].Item2);
                }
            }
        }
        return result;
    }
}