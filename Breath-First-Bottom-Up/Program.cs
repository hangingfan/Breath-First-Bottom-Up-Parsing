﻿using System;
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

        private static Dictionary<char, List<string>> getFileContent(string name)
        {
            var csvReader = new StreamReader(name);
            int index = 0; //the first line is for description ,not real data, skip it
            var content = new Dictionary<char, List<string>>();
            while (!csvReader.EndOfStream)
            {
                var line = csvReader.ReadLine();

                index++;
                if (index == 1)
                {
                    continue;
                }

                var values = line.Split(',');
                if (values[0] != "" && values[0] != " ")
                {
                    var List = new List<string>();
                    for (int i = 1; i <= csvMax; ++i)
                    {
                        if (values[i] != "" && values[i] != " ")
                        {
                            List.Add(values[i]);
                        }
                    }
                    content[values[0][0]] = List;
                }
            }

            return content;
        }


        static char canBePlacedByNonTerminal(string StackString)
        {
            foreach (var VARIABLE in content)
            {
                foreach (var subVariable in VARIABLE.Value)
                {
                    if (subVariable == StackString)
                    {
                        return VARIABLE.Key;
                    }
                }
            }
            return illegalChar;
        }

        private static Dictionary<char, List<string>> content;

        private static HashSet<char> nonTerminalHashset = new HashSet<char>()
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z'
        };

        private static Stack<char> restInputStack = new Stack<char>();
        private static List<Stack<char>> currentPredictionStack = new List<Stack<char>>();

        static void Main(string[] args)
        {
            content = getFileContent("rules.csv");
            if (content.ContainsKey(StartSymbol) == false)
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
                    Console.WriteLine("index: " + intToUnicodeCharShow(i));
                }
            }

            Console.ReadLine();
        }

        static void predictionAndReplace(char nowChar)
        {
            for (int i = 0; i < currentPredictionStack.Count; i++)
            {
                currentPredictionStack[i].Push(nowChar);
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
                    currentPredictionStack.Add(newBranck);
                }
            }

            showCurrent("after add-----");

            if (currentPredictionStack.Count > 1)
            {
                for (int i = 1; i < currentPredictionStack.Count; i++)
                {
                    replace(i);
                }
            }

            showCurrent("after replace -----");
        }

        static void replace(int index)
        {
            Stack<char> currentStack = currentPredictionStack[index];
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
                    currentPredictionStack[index] = newBranck; //replace the old stack
                    replace(index);
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
