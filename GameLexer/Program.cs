using GameLexer.DFAU;
using GameLexer.TokenU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameLexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer("test.txt");
            lexer.AddNotConstantWord("Heal", TokenType.Identifier);
            lexer.AddNotConstantWord("TakeDamage", TokenType.Identifier);
            lexer.AddNotConstantWord(";", TokenType.Delimiter);
            lexer.AddNotConstantWord(":", TokenType.Delimiter);
            List<Token> result = lexer.Analysis();
            foreach(Token token in result)
            {
                Console.WriteLine($"Line {token.line} {token}");
            }
        }
    }
}
