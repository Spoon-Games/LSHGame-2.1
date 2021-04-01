using System.Collections.Generic;
using System.Text;

namespace TagInterpreterR
{
    internal static class TokenBuilder
    {
        public static Token[] BuildTokens(string s)
        {
            List<Token> tokens = new List<Token>();
            StringBuilder builder = new StringBuilder();

            //Collect all tokens
            bool isString = false;
            bool isDeclaration = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (isString)
                {
                    if (c == '"')
                    {
                        isString = false;
                        AddToTokens(builder, tokens, isDeclaration, true);
                        continue;
                    }
                    else
                    {
                        builder.Append(c);
                        continue;
                    }
                }
                else if (c == '"')
                {
                    isString = true;
                    continue;
                }
                else if (c == '<')
                {
                    if (isDeclaration)
                        throw InterpreterException.InvalidCharacter('<', i);

                    AddToTokens(builder, tokens, isDeclaration, false);
                    if (IsChar(s, i + 1, '/'))
                    {
                        tokens.Add(new Token("</"));
                        i++;
                    }
                    else
                        tokens.Add(new Token("<"));
                    isDeclaration = true;
                }
                else if (c == '=' && isDeclaration)
                {

                    AddToTokens(builder, tokens, isDeclaration, false);
                    tokens.Add(new Token("="));
                }
                else if (c == '/')
                {
                    if (IsChar(s, i + 1, '>') && isDeclaration)
                    {

                        AddToTokens(builder, tokens, isDeclaration, false);
                        tokens.Add(new Token("/>"));
                        i++;
                        isDeclaration = false;
                    }
                    else
                        throw InterpreterException.InvalidCharacter('/', i);
                }
                else if (c == '>')
                {
                    if (!isDeclaration)
                        throw InterpreterException.InvalidCharacter('>', i);


                    AddToTokens(builder, tokens, isDeclaration,false);
                    tokens.Add(new Token(">"));
                    isDeclaration = false;
                }
                else if ((c == ' ' || c == '\n' || c == '\t' || c == '\r') && isDeclaration)
                {

                    AddToTokens(builder, tokens, isDeclaration,false);
                }
                else
                {
                    builder.Append(c);
                }
            }
            AddToTokens(builder, tokens, isDeclaration,false);

            //DebugTokens(tokens);

            ValidateTokens(tokens);

            return tokens.ToArray();
        }

        private static void ValidateTokens(List<Token> tokens)
        {
            //Validate all tokens and remove all unnecessary
            int state = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                switch (state)
                {
                    case 0:
                        if (token.IsString)
                        {
                            //tokens.RemoveAt(i);
                            //i--;
                        }
                        else if (token.Is("<"))
                        {
                            state = 1;
                        }
                        else if (token.Is("</"))
                            state = 7;
                        break;
                    case 1:
                        if (token.IsDeclaration)
                            state = 2;
                        else
                            state = -1;
                        break;
                    case 2:
                        if (token.IsDeclaration)
                            state = 3;
                        else if (token.Is("="))
                            state = 4;
                        else if (token.Is(">") | token.Is("/>"))
                            state = 6;
                        else
                            state = -1;
                        break;
                    case 3:
                        if (token.Is("="))
                            state = 4;
                        else state = -1; break;
                    case 4:
                        if (token.IsDeclaration)
                            state = 5;
                        else state = -1; break;
                    case 5:
                        if (token.IsDeclaration)
                            state = 3;
                        else if (token.Is(">") | token.Is("/>"))
                            state = 6;
                        else state = -1; break;
                    case 6:
                        if (token.Is("<"))
                        {
                            state = 1;
                        }
                        else if (token.Is("</"))
                            state = 7;
                        break;
                    case 7:
                        if (token.IsDeclaration)
                            state = 8;
                        else state = -1; break;
                    case 8:
                        if (token.Is(">"))
                            state = 6;
                        else state = -1; break;
                    default:
                        throw new InterpreterException("Invalid syntax");
                }
            }
            if (state == -1)
                throw new InterpreterException("Invalid syntax");
            //else if (state == 6)
            //{
            //    for (int i = tokens.Count - 1; i >= 0; i--)
            //    {
            //        if (tokens[i].Is(">") || tokens[i].Is("/>"))
            //            break;
            //        tokens.RemoveAt(i);
            //    }
            //}
        }

        #region Helper Methods
        private static void AddToTokens(StringBuilder builder, List<Token> tokens, bool isDeclaration,bool isStringDec)
        {
            if(!isStringDec)
                TrimWhiteSpace(builder,trimEnd: isDeclaration);
            if (builder.Length > 0)
            {
                tokens.Add(new Token(builder.ToString(), true, isDeclaration));
            }
            builder.Clear();
        }

        private static void TrimWhiteSpace(StringBuilder builder,bool trimEnd = true)
        {

            while (builder.Length > 0 && char.IsWhiteSpace(builder[0]))
                builder.Remove(0, 1);

            while (trimEnd && builder.Length > 0 && char.IsWhiteSpace(builder[builder.Length - 1]))
                builder.Remove(builder.Length - 1, 1);
        }

        private static bool IsChar(string s, int index, char c)
        {
            if (s.Length <= index)
                return false;
            return s[index] == c;
        }
        #endregion
    }

    internal struct Token
    {
        public string Value;
        public bool IsString;
        public bool IsDeclaration;

        public Token(string value, bool isString = false, bool isDeclaration = false)
        {
            Value = value;
            IsString = isString;
            IsDeclaration = isDeclaration;
        }

        public bool Is(string s)
        {
            return Equals(Value, s);
        }
    }
}
