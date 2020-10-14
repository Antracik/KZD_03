using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KZD_03
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

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

            var encrypted = Encrypt(alphabetCodes, plainText, 3);
            foreach (var code in encrypted)
            {
                Console.Write($"{code[0]}{code[1]} ");
            }
            Console.WriteLine();
            Console.WriteLine("(K, plain text)");
            Console.WriteLine(Decrypt(encrypted, alphabetCodes));
            Console.WriteLine(Decrypt(encryptedText, alphabetCodes));

            Console.ReadKey(true);

        }

        public static (int kValue, string decryptedText) Decrypt(List<int[]> encryptedText, Dictionary<char, int[]> alphabetCodes)
        {
            int n = 1;
            BlockingCollection<List<(int k, char letter)>> tupleCollections = new BlockingCollection<List<(int k, char letter)>>();
            
            foreach (var code in encryptedText)
            {
                Parallel.ForEach(alphabetCodes, (codes) =>
                {
                    var kCodeTuples = GenerateKCodesForLetter(codes.Key, codes.Value, code, n, 20, 1);

                    if (kCodeTuples.Any())
                    {

                        tupleCollections.Add(kCodeTuples);
                    }
                });
                //foreach (var (letter, codes) in alphabetCodes)
                //{
                //    var kCodeTuples = GenerateKCodesForLetter(letter, codes, code, n, 20, 1);

                //    if (kCodeTuples.Any())
                //    {
                //        tupleCollections.Add(kCodeTuples);
                //    }
                //}
                n += 2;
            }

            var flattenedTuples = tupleCollections.SelectMany(tuples => tuples);

            int kValue = flattenedTuples
                .GroupBy(kGrouping => kGrouping.k)
                .OrderByDescending(orderedKGrouping => orderedKGrouping.Count())
                .Select(actualKValue => actualKValue.Key)
                .First();

            var decryptedLetters = flattenedTuples
                .Where(kLetters => kLetters.k == kValue)
                .Select(letters => letters.letter);


            var decryptedMessage = new StringBuilder();
            foreach (var letter in decryptedLetters)
            {
                decryptedMessage.Append(letter);
            }

            return (kValue, decryptedMessage.ToString());
        }
        private static List<(int k, char letter)> GenerateKCodesForLetter(char letter, int[] letterCode, int[] cryptCode, int startNFrom, int howManyK, int startKFrom)
        {
            int[] letterCodes = new int[2];

            List<(int k, char letter)> tuples = new List<(int, char)>();
            while (howManyK > 0)
            {
                int n = startNFrom;
                BigInteger code = (BigInteger.Pow((n + startKFrom), (n + startKFrom)) + letterCode[0]) % 10;

                letterCodes[0] = (int)code;
                n++;

                code = (BigInteger.Pow((n + startKFrom), (n + startKFrom)) + letterCode[1]) % 10;
                letterCodes[1] = (int)code;

                if (letterCodes[0] == cryptCode[0] && letterCodes[1] == cryptCode[1])
                {
                    tuples.Add((startKFrom, letter));
                }

                startKFrom++;
                howManyK--;
            }

            return tuples;
        }
        public static List<int[]> Encrypt(Dictionary<char, int[]> alphabetCodes, string plainText, int k)
        {
            List<int[]> encryptedText = new List<int[]>(plainText.Length);

            var n = 1;
            foreach (var letter in plainText)
            {
                var alphabetCode = alphabetCodes[char.ToUpperInvariant(letter)];
                var cryptoCode = new int[2];

                for (int i = 1; i < 3; i++)
                {
                    BigInteger tempBig = (BigInteger.Pow((n + k), (n + k)) + alphabetCode[i - 1]) % 10;
                    cryptoCode[i - 1] = (int)tempBig;
                    n++;
                }
                encryptedText.Add(cryptoCode);
            }

            return encryptedText;
        }

    }
}
