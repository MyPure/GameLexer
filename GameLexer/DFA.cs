using GameLexer.TokenU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLexer.DFAU
{
    abstract class DFA
    {
        public enum DFAState
        {
            Running,
            End,
            Error
        }
        public char state;
        protected char startState;
        public abstract DFAState State { get; }
        public abstract bool Next(char input, bool peek);
        public abstract bool PeekNext(char input);
        public abstract void Reset();
    }
    class ConstantDFA : DFA
    {
        public override DFAState State
        {
            get
            {
                if(state == '0')
                {
                    return DFAState.Error;
                }
                if(state >= '1' && state <= '4')
                {
                    return DFAState.End;
                }
                else
                {
                    return DFAState.Running;
                }
            }
        }

        public DFAState IsMatch(string input)
        {
            state = 'A';
            for (int i = 0; i < input.Length; i++)
            {
                if (!Next(input[i], false))
                {
                    return DFAState.Error;
                }
            }
            return State;
        }

        public string PasingType
        {
            get
            {
                if (state == '1' || state == '4')
                {
                    return "int";
                }
                else if (state == '2')
                {
                    return "float";
                }
                else if (state == '3')
                {
                    return "string";
                }
                else
                {
                    return "";
                }
            }
        }

        public override bool Next(char input,bool peek)
        {
            char tempState = state;
            switch (tempState)
            {
                case 'A':
                    if (input >= '1' && input <= '9')
                    {
                        tempState = '1';
                    }
                    else if (input == '0')
                    {
                        tempState = '4';
                    }
                    else if (input == '"')
                    {
                        tempState = 'B';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                case 'B':
                    if (input != '"')
                    {
                        tempState = 'B';
                    }
                    else if (input == '"')
                    {
                        tempState = '3';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                case 'C':
                    if (input >= '0' && input <= '9')
                    {
                        tempState = '2';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                case '1':
                    if (input >= '0' && input <= '9')
                    {
                        tempState = '1';
                    }
                    else if (input == '.')
                    {
                        tempState = 'C';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                case '2':
                    if (input >= '0' && input <= '9')
                    {
                        tempState = '2';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                case '4':
                    if (input == '.')
                    {
                        tempState = 'C';
                    }
                    else
                    {
                        tempState = '0';
                    }
                    break;
                default:
                    tempState = '0';
                    break;
            }
            if (!peek)
            {
                state = tempState;
            }
            if(tempState >= '1' && tempState <= '4')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool PeekNext(char input)
        {
            return Next(input, true);
        }

        public override void Reset()
        {
            state = startState;
        }

        public ConstantDFA()
        {
            startState = state = 'A';
        }
    }

    class NotConstantDFA : DFA
    {
        public override DFAState State
        {
            get
            {
                if (state == '0')
                {
                    return DFAState.Error;
                }
                if (state == 'e')
                {
                    return DFAState.End;
                }
                else
                {
                    return DFAState.Running;
                }
            }
        }

        public override bool Next(char input, bool peek)
        {
            string next;
            if(input == '$')
            {
                next = buffer.ToString() + '\\' + input;
            }
            else
            {
                next = buffer.ToString() + input;
            }
            bool isMatch = Regex.IsMatch(token, "^" + next);
            if (!peek)
            {
                buffer.Append(input);
            }
            if (isMatch)
            {
                if(token == next)
                {
                    state = 'e';
                }
                else
                {
                    state = 'r';
                }
            }
            else
            {
                state = '0';
            }
            return isMatch;
        }

        public override bool PeekNext(char input)
        {
            return Next(input, true);
        }

        public override void Reset()
        {
            state = startState;
            buffer.Clear();
        }

        private string token;
        private StringBuilder buffer;
        public NotConstantDFA(string token)
        {
            this.token = token;
            buffer = new StringBuilder();
            startState = state = 'r';
        }
    }

    class TokenDFA
    {
        public DFA dfa;
        public TokenType type;
        
        public TokenDFA(DFA dfa, TokenType type)
        {
            this.dfa = dfa;
            this.type = type;
        }
        public bool IsEnd()
        {
            return dfa.State == DFA.DFAState.End;
        }
        public bool IsError()
        {
            return dfa.State == DFA.DFAState.Error;
        }
        public bool Next(char input)
        {
            return dfa.Next(input, false);
        }
        public bool PeekNext(char input)
        {
            return dfa.PeekNext(input);
        }
    }
}
