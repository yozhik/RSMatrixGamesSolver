using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplex
{
    public class Simplex
    {
        #region Variables
        private int _row, _col; //рядок, стрічка
        private int _vars; //кількість змінних
        private double rozvElement; //розв'язуючий елемент
        private int inBasis, outBasis; //змінна, яку вводимо і виводимо з базису
        private double optPlan; //результуючий план\
        private double BIG_Value; //змінна М - дуже велике, або маленьке число
        private int delta; //змінна, для визначення к-сті введених штучних змінних

        private double[] Cb; //Сб
        private double[] Plan; //План
        private double[] X; //вектор змінних
        private double[] Marks; //вектор оцінок
        private double[] Teta; //вектор тета
        private double[,] Table; //головна таблиця 
        private int[] Basis; //вектор базису
        #endregion

        public int Variables
        {
            get { return _vars; }
        }

        public Simplex()
        {
            Initialize();
        }

        public double FindSolution(int N, int M, double[] ZKoef, double[,] Arr, double[] B, Sign[] Signs, Task tsk, out double[] vectorX)
        {
            Transform(N, M, ZKoef, Arr, B, Signs, tsk);
            //InitBasis();
            bool flag = false;
            int iteration = 0;
            do
            {                  
                if (flag)
                {
                    InBasisIndex(tsk);
                    OutBasisIndex(tsk);
                    SetRozvElement();
                    ChangeBasis();
                    ChangeCb();
                    JordanGauss();
                }
                CalcOptimalPlan();
                flag = true;
                CalcMarks();
                iteration++;
                Report(iteration);
            } while (!IsTaskFinished(tsk));
            Console.WriteLine();

            vectorX = new double[M];


            Console.Write("Optimal plan: ");
            for (int i = 0; i < N; i++)
            {
                if (Basis[i] < M)
                    vectorX[Basis[i]] = Plan[i];
                //Console.Write("X" + (++Basis[i]) + "="+Plan[i]+", ");

            }
            return optPlan;
        }

        private void Report(int num)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.Write(num.ToString() + " iteration");
            Console.WriteLine();

            Console.Write("Marks:");
            Print(_col, Marks);

            Console.WriteLine();
            Console.Write("Cb:");
            Print(_row, Cb);

            Console.WriteLine();
            Console.Write("Plan:");
            Print(_row, Plan);

            Console.WriteLine();
            Console.Write("Teta:");
            Print(_row, Teta);

            Console.WriteLine();
            Console.Write("Optimal value = "+ optPlan.ToString());
        }

        private void SetRozvElement()
        {
            rozvElement = Table[outBasis, inBasis];
        }   

        private void Initialize()
        {
            _row = 0;
            _col = 0;
            _vars = 0;
            optPlan = 0;
            rozvElement = 1;
            inBasis = 0;
            outBasis = 0;
            BIG_Value = 10000;
            delta = 0;
        }

        private void CalcOptimalPlan()
        {
            optPlan = 0;
            for (int i = 0; i < _row; i++)
            {
                optPlan += (Cb[i] * Plan[i]);
            }
        }

        private void InBasisIndex(Task task)
        {
            int i = 0;
            double temp = 0;
            switch (task)
            { 
                case Task.min:
                    for (i = 0; i < _col; i++)
                    {
                        if (Marks[i] > temp)  //!!!!!!!!
                        {
                            temp = Marks[i];
                            inBasis = i;
                        }
                    }
                    break;
                case Task.max:
                    for (i = 0; i < _col; i++)
                    {
                        if (Marks[i] < temp)
                        {
                            temp = Marks[i];
                            inBasis = i;
                        }
                    }
                    break;
            }
        }

        private void OutBasisIndex(Task task)
        {
            CalcTeta();
            int i = 0;
            double temp = 0;
                for (i = 0; i < _row; i++)
                {
                    if (Teta[i] >= 0)
                    {
                        temp = Teta[i];
                        outBasis = i; // додав
                        break;
                    }
                }
                for (i = 0; i < _row; i++)
                {
                    if ((Teta[i] < temp) && (Teta[i] >= 0)) //забрав знак =
                    {
                        temp = Teta[i];
                        outBasis = i;
                    }
                }
        }

        private void JordanGauss()
        {
            //додаткова таблиця для перерахунку головної симплекс-таблиці
            double[,] Temp = new double[_row, _col+1];
            int i = 0, j=0;
            Temp[outBasis, j] = Plan[outBasis] / rozvElement;
            //записуємо стрічку, де знах. розв. елемент, поділивши її на нього
            for (j = 1; j < (_col+1); j++) 
            {
                Temp[outBasis, j] = Table[outBasis, j - 1] / rozvElement;  
            }
            //заповнуємо ячейки масива значеннями методом Жордана-Гаусса
            for (i = 0; i < _row; i++)
            {
                for (j = 1; j < (_col+1); j++)
                {
                    if ((Temp[i, j] == 0) && (i != outBasis)/* && (j != inBasis)*/) ////(i != inBasis) && (j != outBasis)
                    {
                        Temp[i, j] = ((rozvElement * Table[i, j-1]) - (Table[outBasis, j-1] * Table[i, inBasis])) / rozvElement;
                    }
                }
            }
            //заповнюємо ствовпчик плана
            for (i = 0; i < _row; i++)
            {
                if ((Temp[i, 0] == 0) && (i != outBasis))
                {
                    Temp[i, 0] = ((rozvElement * Plan[i]) - (Plan[outBasis] * Table[i, inBasis])) / rozvElement;
                }
            }
            //Переносимо в план значення проміжних обчислень
            for (i = 0; i < _row; i++)
                Plan[i] = Temp[i, 0];
            //Переносимо в головну симплекс таблицю, значення перерахунків
            for (i = 0; i < _row; i++)
            {
                for (j = 1; j < (_col+1); j++)
                {
                    Table[i, j-1] = Temp[i, j];
                }
            }
        }

        private void CalcTeta()
        {
            int i = 0;
            for (i = 0; i < _row; i++)
            {
                Teta[i] = Plan[i] / Table[i, inBasis];
            }
        }

        private void CalcMarks()
        {
            int i = 0, j = 0;
            for (i = 0; i < _col; i++)
            {
                Marks[i] = 0;
            }
                for (j = 0; j < _col; j++)
                {
                    for (i = 0; i < _row; i++)
                    {
                        Marks[j] += (Cb[i] * Table[i, j]);
                    }
                    Marks[j] -= X[j];
                }
        }

        private bool IsTaskFinished(Task task)
        {
            int i = 0;
            
            switch (task)
            { 
                case Task.min:
                    for (i = 0; i < _col; i++)
                    {
                        if (Marks[i] > 0)
                            return false;
                    }
                    break;
                case Task.max:
                    for (i = 0; i < _col; i++)
                    {
                        if (Marks[i] < 0)
                            return false;
                    }
                    break;
            }
            return true;
        }

        private void ChangeBasis()
        {
            Basis[outBasis] = inBasis;
        }

        private void ChangeCb()
        {
            Cb[outBasis] = X[inBasis];
        }

        private void InitBasis()
        {
            int i = 0, j = 0, k = 0;
            bool flag = false;
            for (i = 0; i < _row; i++)
            {
                for (j = 0; j < _col; j++)
                {
                    flag = false;
                    if (Table[i, j] == 1)
                    {
                        for (k = 0; k < _row; k++)
                        {
                            if ((Table[k, j] > 0 || Table[k, j] < 0) && (k!=i))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            Basis[i] = j;
                            break;
                        }
                    }
                }
            }
        }

        private void ChangeSign(int N, int M, double[,] Arr, double[] B, Sign[] Signs)
        { 
            int i=0, j=0;
            /*якщо права частина від'ємна, то домножаємо все рівняння на -1, і 
             змінюємо знак нерівності на протилежний*/
            for (i = 0; i < N; i++)
            {
                if (B[i] < 0)
                {
                    for (j = 0; j < M; j++)
                    {
                        Arr[i, j] = Arr[i, j] * (-1);
                    }
                    B[i] *= (-1);
                    switch (Signs[i])
                    {
                        case Sign.Less: Signs[i] = Sign.More; break;
                        case Sign.More: Signs[i] = Sign.Less; break;
                        default: break;
                    }
                }
            }
        }

        private void AllocMemory(int N)
        {
            Cb = new double[N];
            Plan = new double[N];
            X = new double[Variables];
            Marks = new double[Variables];
            Teta = new double[N];
            Table = new double[N, Variables];
            Basis = new int[N];
            _row = N;
            _col = Variables;
            for (int i = 0; i < _col; i++)
                Marks[i] = 0;
        }

        private void Fill(int N, int M, double[] ZKoef, double[,] Arr, double[] B, Sign[] Signs)
        {
            int i = 0, j = 0, k = 0;
            bool flag = false;
            /*заповнуємо першу симплекс таблицю данними з рівнянь*/
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    Table[i, j] = Arr[i, j];
                    if (!flag)
                    {
                        X[j] = ZKoef[j]; //заповнюємо вектор Х коеф.цільової функції
                    }
                }
                flag = true;
                Plan[i] = B[i]; //заповнюємо вектор плану
            }
            for (i = (Variables - 1); i > (Variables - 1 - delta); i--)
                X[i] = BIG_Value; //копіюємо коеф. М для штучних змінних

                //аналізуємо, з якими коефіцієнтами входять додані програмно змінні
            for (i = (N - 1), j = (Variables - 1 - delta); j >= M; i--, j--) //j >= M
                {
                    switch (Signs[i])
                    {
                        case Sign.Less: Table[i, j]  =  1;  break;
                        case Sign.Equal: Table[i, j] = 0; j++;  break;
                        case Sign.More: Table[i, j]  = -1;  break;
                    }
                }
            //додаємо коеф. при штучних змінних до тих рівнянь, де нерівність '>=' OR '='
                k = 0;
                for (j = (Variables - delta); j < Variables; j++ ) 
                {
                    for (i = k; i < N; i++)
                    {
                        if (Signs[i] == Sign.More || Signs[i] == Sign.Equal)
                        {
                            Table[i, j] = 1;
                            k = ++i;
                            break;
                        }
                        else Table[i, j] = 0;
                    }
                }
                InitBasis();
                    //ініціалізуємо вектор Сб коефіцієнтами при базисних змінних
                    for (i = 0; i < N; i++)
                    {
                        Cb[i] = X[ Basis[i] ];
                    }
        }

        private void Print(int Size, double[] A)
        {
            for (int i = 0; i < Size; i++)
            {
                Console.Write("{0:F}  ", A[i]);
            }
        }
        private int GetBasisVarPos(int N, int M, double[,] Arr, double[] ZKoef, int[] tempBasis, double[] tempCb)
        {
            int i = 0, j = 0, b = 0;
            int c = 0, temp_N = 0;

            for (j = 0, b = 0; j < M && b < N; j++)
            {
                c = 0;
                if (Arr[0, j] == 0)
                {
                    for (i = 1; i < N; i++)
                    {
                        if (Arr[i, j] > 1 || Arr[i, j] < 0) break;
                        if (Arr[i, j] == 1) c++;
                        if (c > 1) break;
                    }
                    if (c == 1)
                    {
                        temp_N++;
                        tempBasis[b] = j;
                        tempCb[b] = ZKoef[j];
                        b++;
                    }
                }
            }

            for (j = 0; j < M && b < N; j++)
            {
                c = 0;
                if (Arr[0, j] == 1)
                {
                    for (i = 1; i < N; i++)
                    {
                        if (Arr[i, j] > 0 || Arr[i, j] < 0) break;
                        if (Arr[i, j] == 0) c++;
                    }
                    if (c == (N - 1))
                    {
                        temp_N++;
                        tempBasis[b] = j;
                        tempCb[b] = ZKoef[j];
                        b++;
                    }
                }
            }
            //повертаємо к-сть незалежних змінних, які можна виділити з матриці обмежень Arr
            return temp_N; 
        }
        private void Transform(int N, int M, double[] ZKoef, double[,] Arr, double[] B, Sign[] Signs, Task tsk)
        {
            if (tsk == Task.max)
            {
                BIG_Value *= (-1);
            }
            int i = 0, j = 0, temp_N = 0;
            _vars = M;
            int[] tempBasis = new int[N];
            double[] tempCb = new double[N];

            ChangeSign(N, M, Arr, B, Signs); //позбуваємося від'ємних елементів в масиві В

            Console.WriteLine("Before Variables = " + Variables.ToString());
            //визначаємо, скільки незалежних базисних змінних можна взяти з матриці обмежень
            temp_N = GetBasisVarPos(N, M, Arr, ZKoef, tempBasis, tempCb);
            
            int lessCount = 0, moreCount = 0;
            for (i = 0; i < N; i++)
            {
                if (Signs[i] == Sign.Less) lessCount++;
                if (Signs[i] == Sign.More) moreCount++;
            }
            

            if (temp_N < N) //якщо змінних не вистачає
            {
                temp_N += lessCount; //то беремо ті, що з коеф. +1
                if (temp_N < N) //якщо всеодно недостача
                {
                    delta = N - temp_N; //визначаємо, скільки треба додати штучних змінних
                }
            }

            _vars = _vars + lessCount + moreCount + delta; //загальна к-сть змінних

            Console.WriteLine("After Variables = " + Variables.ToString());

            AllocMemory(N); //виділяємо пам'ять під симплекс-таблицю

            Fill(N, M, ZKoef, Arr, B, Signs); //заповнюємо симплексщ-таблицю данними

            Console.WriteLine();
            Print(Variables, X);
            Console.WriteLine();
            Print(N, Plan);
            Console.WriteLine();
            Print(N, Teta);
            Console.WriteLine();
            Print(N, Cb);
            Console.WriteLine();

            for (i = 0; i < N; i++)
            {
                for (j = 0; j < Variables; j++)
                {
                    Console.Write(Table[i, j].ToString()+" ");
                }
                Console.WriteLine();
            }

        }
    }
}
