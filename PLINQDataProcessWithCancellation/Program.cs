using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PLINQDataProcessWithCancellation
{
    class Program
    {
        static CancellationTokenSource cancelToken = new CancellationTokenSource();
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Press any key to start processing");
                Console.ReadKey();
                Console.WriteLine("Processing...");
                Task.Factory.StartNew(() => ProcessIntData());
                Console.Write("Enter Q to quit: ");
                string answer = Console.ReadLine();
                if (answer.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    cancelToken.Cancel();
                    break;
                }
            } while (true);

            Console.ReadLine();
        }

        private static void ProcessIntData()
        {
            //Получить очень большой массив целых чисел
            var source = Enumerable.Range(1, 10_000_000).ToArray();
            //Найти числа для которых истинной условие num % 3 == 0
            //и возвратить их в убывающем порядке
            int[] modThreeIsZero = null;
            try
            {
                modThreeIsZero = (from num in source.AsParallel().WithCancellation(cancelToken.Token)
                    where num % 3 == 0 orderby num descending select num).ToArray();
                Console.WriteLine();
                Console.WriteLine($"Found {modThreeIsZero.Length} numbers that match query!");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
