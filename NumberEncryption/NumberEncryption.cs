using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NumberEncryption
{
    public class NumberEncryption
    {
        public (int kValue, string decryptedText) Decrypt(List<int[]> encryptedText, Dictionary<char, int[]> alphabetCodes)
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
        private List<(int k, char letter)> GenerateKCodesForLetter(char letter, int[] letterCode, int[] cryptCode, int startNFrom, int howManyK, int startKFrom)
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
        public List<int[]> Encrypt(Dictionary<char, int[]> alphabetCodes, string plainText, int k)
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
