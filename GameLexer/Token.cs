using System;
using System.Collections.Generic;
using System.Text;
using GameLexer.DFAU;

namespace GameLexer.TokenU
{
    public enum TokenType
    {
        Identifier,
        ReservedWord,
        Constant,
        Operator,
        Delimiter
    }
    class Token
    {      
        public string value;
        public TokenType type;
        public string parsingType;
        public int line;
        public int column;

        public Token(string value, TokenType type, string parsingType)
        {
            this.value = value;
            this.type = type;
            this.parsingType = parsingType;
        }

        public override string ToString()
        {
            return $"({value} {type} {parsingType})";
        }
    }

    class TokenBox
    {
        private List<TokenDFA> tokenDFAs;
        public int Count => tokenDFAs.Count;
        

        public void AddDFA(DFA dfa, TokenType type)
        {
            tokenDFAs.Add(new TokenDFA(dfa, type));
        }

        public bool Analysis(string buffer, char next, out Token token, out int notErrorCount, out int endCount)
        {
            notErrorCount = 0;
            endCount = 0;
            TokenDFA activeDFA = null;
            token = null;
            foreach (TokenDFA td in tokenDFAs)
            {
                if (!td.IsError())
                {
                    notErrorCount++;
                }
                if (td.IsEnd())
                {
                    endCount++;
                }
                if (!td.IsError() && td.IsEnd())
                {
                    activeDFA = td;
                }
            }

            if(activeDFA != null)
            {
                token = CreateWord(buffer, activeDFA);
            }

            if(notErrorCount == 1)
            {
                if(endCount == 1)
                {
                    if (!activeDFA.dfa.PeekNext(next))
                    {
                        token = CreateWord(buffer, activeDFA);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Next(char input)
        {
            foreach(TokenDFA w in tokenDFAs)
            {
                w.Next(input);
            }
        }

        public void Reset()
        {
            foreach (TokenDFA w in tokenDFAs)
            {
                w.dfa.Reset();
            }
        }

        private Token CreateWord(string value, TokenDFA wdfa)
        {
            string parsingType;
            if(wdfa.type == TokenType.Constant)
            {
                parsingType = ((ConstantDFA)wdfa.dfa).PasingType;
            }
            else if(wdfa.type == TokenType.Identifier)
            {
                parsingType = "identifier";
            }
            else
            {
                parsingType = value;
            }
            Token token = new Token(value, wdfa.type, parsingType);
            return token;
        }

        public TokenBox()
        {
            tokenDFAs = new List<TokenDFA>();
        }
    }
}
