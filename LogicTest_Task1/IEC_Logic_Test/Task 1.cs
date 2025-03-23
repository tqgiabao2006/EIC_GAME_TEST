namespace IEC_Logic_Test;

public class Task_1
{
    public string Solution(string s)
    {
        //Rules:
        //Library:
        /*
         Case1: if len s1 < len s2 => s1 < s2 <=> not the case
         Case2:  ab < bb because a < b
         Case3:  aabc < aacc => if same len, same prefix => compare the different part
        */
        
        //ASCII compare string => ascII a -> z lower case 
        //Problem: "ab" = "ba" => if only += (int)char 
        
        
        //Solution: i: maxValue * 10 + a => 0*10 + a, a * 10 + b => ab => ab is different ba
        //Brute force
        // Time complexity: O(n^2) : Compare each case of delete 
        // Space complexity O(1):

        /*string result = "";
        int minValue = int.MaxValue;

        for (int i = 0; i < s.Length; i++)
        {
            int sum = 0;
            string str = "";
            for (int j = 0; j < s.Length; j++)
            {
                if (i != j) //Remove char in i index => do not take to account
                {
                    str += s[j];
                    sum  = sum*10 + s[j];
                }
            }
             if (sum < minValue)
            {
                minValue = sum;
                result = str;
            }
        }

        return result;
        */
        
        //Method #2: Greedy => eliminate where char > large 
        // Time complexity: O(n)
        // Space complexity O(1):

        for (int i = 0; i < s.Length -1; i++)
        {
            if (s[i] > s[i + 1])
            {
                return s.Remove(i, 1);
            }
            
        }
        return s.Remove(s.Length - 1, 1);
    }
}