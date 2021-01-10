using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GameLexer.TokenU;
using GameLexer.DFAU;

namespace GameLexer
{
    class Lexer
    {
        private string filePath;
        private StringBuilder buffer;
        private TokenBox wordBox;
        private List<Token> tokens;
        int line = 1;
        int column = 0;
        public Lexer(string filePath)
        {
            this.filePath = filePath;
            buffer = new StringBuilder(128);
            wordBox = new TokenBox();
            tokens = new List<Token>();
            wordBox.AddDFA(new ConstantDFA(), TokenType.Constant);
        }

        public List<Token> Analysis()
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string inputString = streamReader.ReadToEnd();
                int length = inputString.Length;
                inputString += "$";
                
                Token lastWord = null;//上一次有可能是单词

                for (int i = 0; i < length; i++)
                {
                    //if (!IsIgnore(inputString[i]))
                    //{
                        buffer.Append(inputString[i]);
                    //}
                    wordBox.Next(inputString[i]);

                    if (inputString[i] == '\n')
                    {
                        line++;
                        column = -1;
                    }
                    column++;

                    int notErrorCount;
                    int endCount;
                    Token token;
                    bool success = wordBox.Analysis(buffer.ToString(), inputString[i + 1], out token, out notErrorCount, out endCount);
                    if(endCount == 1)
                    {
                        lastWord = token;
                    }

                    if (notErrorCount == 1)
                    {
                        if (success)
                        {
                            AnalysisSucceed(token);
                            buffer.Clear();
                            wordBox.Reset();
                            lastWord = null;
                        }
                    }
                    else if(notErrorCount == 0)
                    {
                        if (lastWord != null)
                        {
                            AnalysisSucceed(lastWord);
                            i--;
                            lastWord = null;                            
                        }
                        else
                        {
                            if(!IsIgnore(inputString[i]))
                            {
                                if(endCount == 0)
                                {
                                    Console.WriteLine($"Error at Line {line} Column {column}");
                                    return null;
                                }
                            }
                        }
                        buffer.Clear();
                        wordBox.Reset();
                    }
                }
            }
            return tokens;
        }

        private void AnalysisSucceed(Token token)
        {           
            token.line = line;
            token.column = column;
            tokens.Add(token);
        }

        public void AddNotConstantWord(string token, TokenType wordType)
        {
            wordBox.AddDFA(new NotConstantDFA(token), wordType);
        }

        private bool IsIgnore(char c)
        {
            char[] ignore = new char[] { ' ', '\n', '\r', '\t' };
            foreach(char ch in ignore)
            {
                if(c == ch)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
