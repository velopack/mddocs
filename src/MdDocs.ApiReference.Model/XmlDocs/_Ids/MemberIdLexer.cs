﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grynwald.MdDocs.ApiReference.Model.XmlDocs
{
    /// <summary>
    /// Lexer for XML docs member ids
    /// </summary>
    /// <remarks>
    /// A lexer for member ids in XML documentation docs generated by the C# compiler as documented 
    /// here https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file
    /// Used by <see cref="MemberIdParser"/>
    /// </remarks>
    internal class MemberIdLexer
    {
        private readonly string m_Text;
        private int m_Position;


        private char Current => m_Position >= m_Text.Length ? '\0' : m_Text[m_Position];

        private char Next => m_Position + 1 >= m_Text.Length ? '\0' : m_Text[m_Position + 1];


        public MemberIdLexer(string input)
        {
            m_Text = input;
        }


        public IReadOnlyList<MemberIdToken> GetTokens()
        {
            m_Position = 0;
            return EnumerateTokens().ToArray();
        }

        private IEnumerable<MemberIdToken> EnumerateTokens()
        {
            //iterate over the input text
            while (Current != '\0')
            {
                switch (Current)
                {
                    // number => read as number token, names can contain digits but never start with a digit
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        yield return ReadNumberToken();
                        break;

                    case 'N' when m_Position == 0:
                    case 'T' when m_Position == 0:
                    case 'F' when m_Position == 0:
                    case 'E' when m_Position == 0:
                    case 'P' when m_Position == 0:
                    case 'M' when m_Position == 0:
                        yield return new MemberIdToken(MemberIdTokenKind.IdentifierType, Current);
                        m_Position++;
                        break;

                    case '.':
                        yield return new MemberIdToken(MemberIdTokenKind.Dot, ".");
                        m_Position++;
                        break;

                    // for backtick, look ahead one character to detect a double-backtick token
                    case '`':
                        if (Next == '`')
                        {
                            yield return new MemberIdToken(MemberIdTokenKind.DoubleBacktick, "``");
                            m_Position += 2;
                        }
                        else
                        {
                            yield return new MemberIdToken(MemberIdTokenKind.Backtick, "`");
                            m_Position++;
                        }
                        break;

                    case '(':
                        yield return new MemberIdToken(MemberIdTokenKind.OpenParenthesis, "(");
                        m_Position++;
                        break;

                    case ')':
                        yield return new MemberIdToken(MemberIdTokenKind.CloseParenthesis, ")");
                        m_Position++;
                        break;

                    case '{':
                        yield return new MemberIdToken(MemberIdTokenKind.OpenBrace, "{");
                        m_Position++;
                        break;

                    case '}':
                        yield return new MemberIdToken(MemberIdTokenKind.CloseBrace, "}");
                        m_Position++;
                        break;

                    case ',':
                        yield return new MemberIdToken(MemberIdTokenKind.Comma, ",");
                        m_Position++;
                        break;

                    case '[':
                        yield return new MemberIdToken(MemberIdTokenKind.OpenSquareBracket, "[");
                        m_Position++;
                        break;

                    case ']':
                        yield return new MemberIdToken(MemberIdTokenKind.CloseSquareBracket, "]");
                        m_Position++;
                        break;

                    case '~':
                        yield return new MemberIdToken(MemberIdTokenKind.Tilde, "~");
                        m_Position++;
                        break;

                    case ':':
                        yield return new MemberIdToken(MemberIdTokenKind.Colon, ":");
                        m_Position++;
                        break;

                    default:
                        yield return ReadNameToken();
                        break;
                }
            }

            // return EOF token to signal the end of the text
            yield return new MemberIdToken(MemberIdTokenKind.Eof, "");
        }

        private MemberIdToken ReadNameToken()
        {
            var resultBuilder = new StringBuilder();

            var startPosition = m_Position;
            while (Current != '\0')
            {
                if (Current == '.' ||
                    Current == '`' ||
                    Current == '(' ||
                    Current == ')' ||
                    Current == '{' ||
                    Current == '}' ||
                    Current == '[' ||
                    Current == ']' ||
                    Current == ',' ||
                    Current == '~')
                {
                    break;
                }

                if (Current == '#')
                {
                    resultBuilder.Append('.');
                }
                else
                {
                    resultBuilder.Append(Current);
                }

                m_Position++;
            }

            // names cannot be empty
            if (resultBuilder.Length == 0)
                throw new MemberIdLexerException($"Failed to read name at position '{startPosition}'");

            return new MemberIdToken(MemberIdTokenKind.Name, resultBuilder.ToString());
        }

        private MemberIdToken ReadNumberToken()
        {
            var startPosition = m_Position;
            while (Current != '\0' && Char.IsDigit(Current))
            {
                m_Position++;
            }

            // numbers must contain at least one digit
            if (startPosition == m_Position)
                throw new MemberIdLexerException($"Failed to read number at position {startPosition}");

            return new MemberIdToken(MemberIdTokenKind.Number, m_Text.Substring(startPosition, m_Position - startPosition));
        }
    }
}
