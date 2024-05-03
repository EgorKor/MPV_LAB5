// See https://aka.ms/new-console-template for more information
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Serialization;


/*********************************
 *АЛГОРИТМЫ БЕЗ РАСПАРАЛЛЕЛИВАНИЯ*
 *********************************/

/*СОРТИРОВКА ШЕЛЛА*/
void shellSort(int[] array)
{
    var d = array.Length / 2;
    while (d >= 1)
    {
        for (var i = d; i < array.Length; i++)
        {
            var j = i;
            while ((j >= d) && (array[j - d] > array[j]))
            {
                swap(ref array[j], ref array[j - d]);
                j = j - d;
            }
        }

        d = d / 2;
    }
}

/*МЕТОД ОБМЕНА ЗНАЧЕНИЯМИ*/
void swap(ref int a, ref int b)
{
    var t = a;
    a = b;
    b = t;
}

/*СОРТИРОВКА СЛИЯНИЕМ - ФАСАДНЫЙ МЕТОД*/
void mergeSort(int[] array)
{
    MergeSort(array, 0, array.Length - 1);
}

/*СОРТИРОВКА СЛИЯНИЕМ - РЕАЛИЗАЦИЯ*/
void MergeSort(int[] a, int l, int r)
{
    int q;
    if (l < r)
    {
        q = (l + r) / 2;
        MergeSort(a, l, q);
        MergeSort(a, q + 1, r);
        merge(a, l, q, r);
    }
}


/*МЕТОД СЛИЯНИЯ*/
void merge(int[] array, int l, int m, int r)
{
    int i, j, k;
    int n1 = m - l + 1;
    int n2 = r - m;
    int[] left = new int[n1 + 1];
    int[] right = new int[n2 + 1];
    for (i = 0; i < n1; i++)
        left[i] = array[l + i];
    for (j = 1; j <= n2; j++)
        right[j - 1] = array[m + j];
    left[n1] = int.MaxValue;
    right[n2] = int.MaxValue;
    i = 0;
    j = 0;
    for (k = l; k <= r; k++)
    {
        if (left[i] < right[j])
        {
            array[k] = left[i];
            i = i + 1;
        }
        else
        {
            array[k] = right[j];
            j = j + 1;
        }
    }
}

/*МЕТОД НАХОЖДЕНИЯ ОПОРНОГО ЭЛЕМЕНТА*/
int Partition(int[] array, int minIndex, int maxIndex)
{
    var pivot = minIndex - 1;
    for (var i = minIndex; i < maxIndex; i++)
    {
        if (array[i] < array[maxIndex])
        {
            pivot++;
            swap(ref array[pivot], ref array[i]);
        }
    }
    pivot++;
    swap(ref array[pivot], ref array[maxIndex]);
    return pivot;
}

/*БЫСТРАЯ СОРТИРОВКА - РЕАЛИЗАЦИЯ*/
void QuickSort(int[] array, int leftIndex, int rightIndex)
{
    var i = leftIndex;
    var j = rightIndex;
    var pivot = array[leftIndex];
    while (i <= j)
    {
        while (array[i] < pivot)
            i++;
        while (array[j] > pivot)
            j--;
        if (i <= j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
            i++;
            j--;
        }
    }
    if (leftIndex < j)
        QuickSort(array, leftIndex, j);
    if (i < rightIndex)
        QuickSort(array, i, rightIndex);
}




/*БЫСТРАЯ СОРТИРОВКА - ФАСАДНЫЙ МЕТОД*/
void quickSort(int[] array)
{
    QuickSort(array, 0, array.Length - 1);
}


/*БИНАРНЫЙ ПОИСК*/
int binarySearch(int[] array, int searchedValue, int left, int right)
{
    while (left <= right)
    {
        int middle = (left + right) / 2;
        if (searchedValue == array[middle])
            return middle;
        else if (searchedValue < array[middle])
            right = middle - 1;
        else
            left = middle + 1;
    }
    return -1;
}

/*ИНТЕРПОЛЯЦИОННЫЙ ПОИСК*/
int interpolatingSearch(int[] array, int target, int start, int end)
{
    while (start <= end && target >= array[start] && target <= array[end])
    {
        int pos = start + ((target - array[start]) * (end - start) / (array[end] - array[start]));

        if (array[pos] == target)
        {
            return pos;
        }
        else if (array[pos] < target)
        {
            start = pos + 1;
        }
        else
        {
            end = pos - 1;
        }
    }

    return -1;
}


/********************************
 *АЛГОРИТМЫ С РАСПАРАЛЛЕЛИВАНИЕМ*
 ********************************/

/*СОРТИРОВКА СЛИЯНИЕМ - ФАСАДНЫЙ МЕТОД*/
void mergeSortParallel(int[] array)
{
    MergeSortParallel(array, 0, array.Length - 1);
}

/*СОРТИРОВКА СЛИЯНИЕМ*/
void MergeSortParallel(int[] a, int l, int r)
{
    int q;
    if (l < r)
    {
        q = (l + r) / 2;
        Parallel.Invoke(
            () => MergeSort(a, l, q),
            () => MergeSort(a, q + 1, r)
        );
        merge(a, l, q, r);
    }
}

/*БЫСТРАЯ СОРТИРОВКА - ФАСАДНЫЙ МЕТОД*/
void quickSortParallel(int[] array)
{
    ParallelQuickSort(array, 0, array.Length - 1);
}


/*БЫСТРАЯ СОРТИРОВКА - РЕАЛИЗАЦИЯ*/
void ParallelQuickSort(int[] array, int left, int right)
{
    if (left < right)
    {
        int pivot = Partition(array, left, right);
        Parallel.Invoke(
            () => ParallelQuickSort(array, left, pivot - 1),
            () => ParallelQuickSort(array, pivot + 1, right)
        );
    }
}

/*БИНАРНЫЙ ПОИСК*/
int binarySearchParallel(int[] array, int target)
{
    int result1 = -1, result2 = -1;

    Task task1 = Task.Run(() =>
    {
        result1 = binarySearch(array, target, 0, array.Length / 2 - 1);
    });

    Task task2 = Task.Run(() =>
    {
        result2 = binarySearch(array, target, array.Length / 2, array.Length - 1);
    });

    Task.WaitAll(task1, task2);

    return result1 != -1 ? result1 : result2;
}

/*ИНТЕРПОЛЯЦИОННЫЙ ПОИСК*/
int interpolatingSearchParallel(int[] array, int target)
{
    int result1 = -1, result2 = -1;

    Task task1 = Task.Run(() =>
    {
        result1 = interpolatingSearch(array, target, 0, array.Length / 2 - 1);
    });

    Task task2 = Task.Run(() =>
    {
        result2 = interpolatingSearch(array, target, array.Length / 2, array.Length - 1);
    });

    Task.WaitAll(task1, task2);

    return result1 != -1 ? result1 : result2;
}



int[] generateData(int length, int left, int right)
{
    if (length < 1)
        throw new ArgumentException($"Длина должна быть не меньше 1, значение длины {length}");
    int[] data = new int[length];
    Random random = new Random();
    for(int i = 0; i < length; i++)
    {
        data[i] = random.Next(right * 2 + 1) - left;
    }
    return data;
}

/**********************
 *РУБРИКА ЭКСПЕРИМЕНТЫ*
 **********************/
void experimentQuickSortTime(int[] data)
{
    Console.WriteLine("********************************************************************");
    Console.WriteLine("Проверка производительности быстрой сортировки без распараллеливания");
    Console.WriteLine($"Размер выборки {data.Length}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    quickSort(data);    
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;

    string elapsedTime = string.Format("{0:00}.{1:00} сек",
        ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("Время работы " + elapsedTime);
}


void experimentShellSortTime(int[] data)
{
    Console.WriteLine("******************************************************************");
    Console.WriteLine("Проверка производительности сортировки Шелла без распараллеливания");
    Console.WriteLine($"Размер выборки {data.Length}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    shellSort(data);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = string.Format("{0:00}.{1:00} сек",
        ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("Время работы " + elapsedTime);
}


void experimentMergeSortTime(int[] data)
{
    Console.WriteLine("*********************************************************************");
    Console.WriteLine("Проверка производительности сортировки слиянием без распараллеливания");
    Console.WriteLine($"Размер выборки {data.Length}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    mergeSort(data);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = string.Format("{0:00}.{1:00} сек",
        ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("Время работы " + elapsedTime);
}

void experimentBinarySearchTime(int[] data, int searchedValue)
{
    Console.WriteLine("******************************************************************");
    Console.WriteLine("Проверка производительности бинарного поиска без распараллеливания");
    Console.WriteLine($"Размер выборки {data.Length}");
    Console.WriteLine($"Поиск элемента - {searchedValue}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    binarySearch(data, searchedValue, 0, data.Length - 1);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = string.Format("{0:00}.{1:000}{2:000} сек",
        ts.Seconds,
        ts.Milliseconds,
        ts.Nanoseconds);
    Console.WriteLine("Время работы " + elapsedTime);
}

void experimentInterpolatingSearchTime(int[] data, int searchedValue)
{
    Console.WriteLine("*************************************************************************");
    Console.WriteLine("Проверка производительности интеполяционного поиска без распараллеливания");
    Console.WriteLine($"Размер выборки {data.Length}");
    Console.WriteLine($"Поиск элемента - {searchedValue}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    interpolatingSearch(data,searchedValue,0,data.Length - 1);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = string.Format("{0:00}.{1:000}{2:000} сек",
        ts.Seconds,
        ts.Milliseconds,
        ts.Nanoseconds);
    Console.WriteLine("Время работы " + elapsedTime);
}

void experimentQuickSortTimeParallel(int[] data)
{
    Console.WriteLine("********************************************************************");
    Console.WriteLine("Проверка производительности быстрой сортировки с распараллеливанием");
    Console.WriteLine($"Размер выборки {data.Length}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    quickSortParallel(data);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;

    string elapsedTime = string.Format("{0:00}.{1:00} сек",
        ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("Время работы " + elapsedTime);
}

void experimentMergeSortTimeParallel(int[] data)
{
    Console.WriteLine("********************************************************************");
    Console.WriteLine("Проверка производительности сортировки слиянием с распараллеливанием");
    Console.WriteLine($"Размер выборки {data.Length}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    mergeSortParallel(data);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;

    string elapsedTime = string.Format("{0:00}.{1:00} сек",
        ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("Время работы " + elapsedTime);
}

void experimentBinarySearchTimeParallel(int[] data, int searchedValue)
{
    Console.WriteLine("******************************************************************");
    Console.WriteLine("Проверка производительности бинарного поиска c распараллеливанием");
    Console.WriteLine($"Размер выборки {data.Length}");
    Console.WriteLine($"Поиск элемента - {searchedValue}");
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    binarySearchParallel(data, searchedValue);
    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = string.Format("{0:00}.{1:000}{2:000} сек",
        ts.Seconds,
        ts.Milliseconds,
        ts.Nanoseconds);
    Console.WriteLine("Время работы " + elapsedTime);
}

Random random = new Random();
int[] data1 = generateData(10000000, -1000000, 1000000);
int[] data2 = generateData(10000000, -1000000, 1000000);
int[] data3 = generateData(10000000, -1000000, 1000000);
experimentQuickSortTime(data1);
experimentQuickSortTimeParallel(data2);
