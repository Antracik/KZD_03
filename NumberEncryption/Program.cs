using System;
using System.Collections.Generic;
using System.Text;

namespace NumberEncryption
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            NumberEncryption encryptor = new NumberEncryption();

            Dictionary<char, int[]> alphabetCodes = new Dictionary<char, int[]>()
            {
                ['А'] = new int[] { 0, 1 },
                ['Б'] = new int[] { 0, 2 },
                ['В'] = new int[] { 0, 3 },
                ['Г'] = new int[] { 0, 4 },
                ['Д'] = new int[] { 0, 5 },
                ['Е'] = new int[] { 0, 6 },
                ['Ж'] = new int[] { 0, 7 },
                ['З'] = new int[] { 0, 8 },
                ['И'] = new int[] { 0, 9 },
                ['Й'] = new int[] { 1, 0 },
                ['К'] = new int[] { 1, 1 },
                ['Л'] = new int[] { 1, 2 },
                ['М'] = new int[] { 1, 3 },
                ['Н'] = new int[] { 1, 4 },
                ['О'] = new int[] { 1, 5 },
                ['П'] = new int[] { 1, 6 },
                ['Р'] = new int[] { 1, 7 },
                ['С'] = new int[] { 1, 8 },
                ['Т'] = new int[] { 1, 9 },
                ['У'] = new int[] { 2, 0 },
                ['Ф'] = new int[] { 2, 1 },
                ['Х'] = new int[] { 2, 2 },
                ['Ц'] = new int[] { 2, 3 },
                ['Ч'] = new int[] { 2, 4 },
                ['Ш'] = new int[] { 2, 5 },
                ['Щ'] = new int[] { 2, 6 },
                ['Ъ'] = new int[] { 2, 7 },
                ['ь'] = new int[] { 2, 8 },
                ['Ю'] = new int[] { 2, 9 },
                ['Я'] = new int[] { 3, 0 },
                [' '] = new int[] { 3, 1 },

            };

            List<int[]> encryptedText = new List<int[]>
            {
                new[] { 1, 1 },
                new[] { 3, 7 },
                new[] { 8, 7 },
                new[] { 7, 8 },
                new[] { 0, 2 },
                new[] { 1, 0 },
                new[] { 7, 1 },
                new[] { 5, 7 },
                new[] { 4, 9 },
                new[] { 9, 6 },
                new[] { 4, 7 },
                new[] { 3, 1 },
                new[] { 5, 3 },
                new[] { 9, 4 },
                new[] { 0, 3 },
                new[] { 1, 5 },
                new[] { 8, 0 },
                new[] { 5, 1 },
                new[] { 3, 3 },
                new[] { 9, 9 },
            };

            var plainText = "Идеи за довечера".ToLower();

            var encrypted = encryptor.Encrypt(alphabetCodes, plainText, 3);
            foreach (var code in encrypted)
            {
                Console.Write($"{code[0]}{code[1]} ");
            }
            Console.WriteLine();
            Console.WriteLine("(K, plain text)");
            Console.WriteLine(encryptor.Decrypt(encrypted, alphabetCodes));
            Console.WriteLine(encryptor.Decrypt(encryptedText, alphabetCodes));

            Console.ReadKey(true);

        }

    }
}
