using System.Collections;
using System.Collections.Generic;

public static class MathUtil
{
    public static int Pow2Reverse(int number){
		if(number == 0) return 0;
		if(number == 1) return 1;
		if(number == 2) return 2;
		int i = 1;
		int div = number;
		while(div > 0){
			div /= 2;
			if(div % 2 == 1)
				break;
			++i;
		}
		return i;
	}
}
