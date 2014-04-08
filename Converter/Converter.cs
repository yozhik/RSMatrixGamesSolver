using System;
using System.Collections.Generic;
using System.Text;

using Simplex;

namespace Converter
{
    public class Converter
    {
        public Converter()
        { 
        
        }

        public void Convert(int N, int M, double[] ZKoef, double[,] Arr, double[] B, Sign[] Signs, Task task1, out double[] FKoef, out double[,] Arr2, out double[] C, out Sign[] Signs2, out Task task2)
        {
            int i=0, j=0;
            switch (task1)
            { 
                case Task.min:
                    for (i = 0; i < N; i++)
                    {
                        switch (Signs[i])
                        { 
                            case Sign.Less:
                                for (j = 0; j < M; j++)
                                {
                                    Arr[i, j] *= (-1);
                                }
                                B[i] *= (-1);
                                Signs[i] = Sign.More;
                                break;
                            case Sign.Equal: break;
                            case Sign.More: break;
                        }
                    }
                    break;
                case Task.max:
                    for (i = 0; i < N; i++)
                    {
                        switch (Signs[i])
                        {
                            case Sign.More:
                                for (j = 0; j < M; j++)
                                {
                                    Arr[i, j] *= (-1);
                                }
                                B[i] *= (-1);
                                Signs[i] = Sign.Less;
                                break;
                            case Sign.Equal: break;
                            case Sign.Less: break;
                        }
                    }
                    break;
            }
            //*********
            FKoef = new double[N];
            C = new double[M];
            Signs2 = new Sign[M];
            Arr2 = new double[M,N];
            for (i = 0; i < M; i++)
            {
                if (task1 == Task.max) Signs2[i] = Sign.More;
                else Signs2[i] = Sign.Less;
                C[i] = ZKoef[i];
            }
            for (i = 0; i < N; i++)
                FKoef[i] = B[i];
            for (j = 0; j < N; j++)
            {
                for (i = 0; i < M; i++)
                {
                    Arr2[i, j] = Arr[j, i];
                }
            }
            if (task1 == Task.max) task2 = Task.min;
            else task2 = Task.max;
        }
    }
}
