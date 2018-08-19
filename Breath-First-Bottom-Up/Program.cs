using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Breath_First_Bottom_Up
{
    class Program
    {
        private static int csvMax = 5;
        private static char StartSymbol = 'S';
        private static int intToChar = 48;
        private static char illegalChar = '0';

        private static List<KeyValuePair<char, string>> getFileContent(string name)
        {
            var csvReader = new StreamReader(name);
            int index = 0; //the first line is for description ,not real data, skip it
            var content = new List<KeyValuePair<char, string>>();
            while (!csvReader.EndOfStream)
            {
                var line = csvReader.ReadLine();

                index++;
                if (index == 1)
                {
                    continue;
                }

                if (line == null)
                {
                    break;
                }

                var values = line.Split(',');
                if (values[0] != "" && values[0] != " ")
                {
                    content.Add(new KeyValuePair<char, string>(values[0][0], values[1]));
                }
            }

            return content;
        }

        static char canBePlacedByNonTerminal(string StackString)
        {
            for (int i = 0; i < content.Count; ++i)
            {
                if (content[i].Value == StackString)
                {
                    return content[i].Key;
                }
            }
            return illegalChar;
        }

        private static List<KeyValuePair<char, string>> content;

        private static Stack<char> restInputStack = new Stack<char>();
        private static List<Stack<char>> currentPredictionStack = new List<Stack<char>>();
        private static List<string> currentPredictionStackCache = new List<string>();

        static void Main(string[] args)
        {
            content = getFileContent("rules.csv");
            if (content.Count == 0)
            {
                return;
            }

            restInputStack.Clear();
            Console.WriteLine("input your input string:");
            var UserInput = Console.ReadLine();
            if (UserInput.Length == 0)
            {
                return;
            }
            for (int i = UserInput.Length - 1; i > -1; i--)
            {
                restInputStack.Push(UserInput[i]);
            }

            Stack<char> initPredictionStack = new Stack<char>();
            currentPredictionStack.Add(initPredictionStack);

            while (restInputStack.Count > 0)
            {
                predictionAndReplace(restInputStack.Pop());
            }

            Console.WriteLine("the satified index is -----");
            for (int i = 0; i < currentPredictionStack.Count; i++)
            {
                if (currentPredictionStack[i].Count == 1 && currentPredictionStack[i].Peek() == StartSymbol)
                {
                    Console.WriteLine("index: " + intToUnicodeCharShowForAllInteger(i));
                }
            }

            Console.ReadLine();
        }

        static void predictionAndReplace(char nowChar)
        {
            currentPredictionStackCache.Clear();
            for (int i = 0; i < currentPredictionStack.Count; i++)
            {
                currentPredictionStack[i].Push(nowChar);
                currentPredictionStackCache.Add(getReplaceRules(currentPredictionStack[i]));
            }

            Stack<char> initialStack = currentPredictionStack[0];
            //traverse all the string from the top, for example,  a  aa aaa aaaa aaaab
            Stack<char> tempStack = new Stack<char>(initialStack.Reverse());
            string content = "";
            while (tempStack.Count > 0)
            {
                content = content + tempStack.Pop();
                char ret = canBePlacedByNonTerminal(content);
                if (ret != illegalChar)
                {
                    Stack<char> newBranck = new Stack<char>(tempStack.Reverse());
                    newBranck.Push(ret);
                    if (false == currentPredictionStackCache.Contains(getReplaceRules(newBranck)))
                    {
                        currentPredictionStack.Add(newBranck);
                    }
                }
            }

            showCurrent("after add-----");

            if (currentPredictionStack.Count > 1)
            {
                for (int i = 1; i < currentPredictionStack.Count; i++)
                {
                    replace(currentPredictionStack[i]);
                }
            }

            showCurrent("after replace -----");
        }

        static void replace(Stack<char> currentStack)
        {
            Stack<char> tempStack = new Stack<char>(currentStack.Reverse());
            string content = "";
            while (tempStack.Count > 0)
            {
                content = tempStack.Pop() + content;
                char ret = canBePlacedByNonTerminal(content);
                if (ret != illegalChar)
                {
                    Stack<char> newBranck = new Stack<char>(tempStack.Reverse());
                    newBranck.Push(ret);
                    if (currentPredictionStackCache.Contains(getReplaceRules(newBranck)) == false)
                    {
                        currentPredictionStack.Add(newBranck); //replace the old stack
                        replace(newBranck);
                        currentPredictionStackCache.Add(getReplaceRules(newBranck));
                    }
                }
            }
        }

        private static void showCurrent(string title)
        {
            Console.WriteLine(title);
            for (int i = 0; i < currentPredictionStack.Count; ++i)
            {
                Console.WriteLine(getReplaceRules(currentPredictionStack[i]));
            }
        }

        private static string getReplaceRules(Stack<char> currentStack)
        {
            Stack<char> tempStack = new Stack<char>(currentStack.Reverse());
            string content = "";
            while (tempStack.Count != 0)
            {
                int pop = tempStack.Pop();
                if (pop < 10 && pop > -1)//  0-9(int) change to 0-9(unicode)
                {
                    content = intToUnicodeCharShow(tempStack.Pop()) + intToUnicodeCharShow(pop) + content;
                }
                else
                {
                    content = intToUnicodeCharShow(pop) + content;
                }
            }
            return content;
        }

        private static char intToUnicodeCharShow(int pop)
        {
            if (pop < 10 && pop > -1)//  0-9(int) change to 0-9(unicode)
            {
                return Convert.ToChar(pop + 1 + intToChar);
            }
            else
            {
                return Convert.ToChar(pop);
            }
        }

        private static string intToUnicodeCharShowForAllInteger(int pop)
        {
            if (pop < 10 && pop > -1)//  0-9(int) change to 0-9(unicode)
            {
                return Convert.ToChar(pop + 1 + intToChar).ToString();
            }
            else
            {
                string ret = "";
                while (pop > 9)
                {
                    ret = pop % 10 + ret;
                    pop = pop / 10;
                }
                ret = pop + ret;
                return ret;
            }
        }
    }
    

    public static class staticClass
    {
        public static string StackString(this Stack<char> charStack)
        {
            Stack<char> tempStack = new Stack<char>(charStack);
            string content = "";
            while (tempStack.Count > 0)
            {
                content = content + tempStack.Pop();
            }
            return content;
        }
    }
}
