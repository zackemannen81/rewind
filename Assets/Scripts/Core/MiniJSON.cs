/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.MiniJSON
{
    public static class Json
    {
        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }

            return Parser.Parse(json);
        }

        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        private sealed class Parser : IDisposable
        {
            private const string WORD_BREAK = "{}[],:\"";

            private readonly StringReader _json;

            private Parser(string jsonString)
            {
                _json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString)
            {
                using var instance = new Parser(jsonString);
                return instance.ParseValue();
            }

            public void Dispose()
            {
                _json.Dispose();
            }

            private enum Token
            {
                None,
                CurlyOpen,
                CurlyClose,
                SquaredOpen,
                SquaredClose,
                Colon,
                Comma,
                String,
                Number,
                True,
                False,
                Null
            }

            private object ParseValue()
            {
                switch (NextToken)
                {
                    case Token.String:
                        return ParseString();
                    case Token.Number:
                        return ParseNumber();
                    case Token.CurlyOpen:
                        return ParseObject();
                    case Token.SquaredOpen:
                        return ParseArray();
                    case Token.True:
                        return true;
                    case Token.False:
                        return false;
                    case Token.Null:
                        return null;
                    default:
                        return null;
                }
            }

            private Dictionary<string, object> ParseObject()
            {
                var table = new Dictionary<string, object>();

                // consume '{'
                _json.Read();

                while (true)
                {
                    switch (NextToken)
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            continue;
                        case Token.CurlyClose:
                            _json.Read();
                            return table;
                        default:
                            var name = ParseString();
                            if (NextToken != Token.Colon)
                            {
                                return null;
                            }

                            // consume ':'
                            _json.Read();

                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            private List<object> ParseArray()
            {
                var array = new List<object>();

                // consume '['
                _json.Read();

                var parsing = true;
                while (parsing)
                {
                    var token = NextToken;
                    switch (token)
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            continue;
                        case Token.SquaredClose:
                            _json.Read();
                            parsing = false;
                            break;
                        default:
                            array.Add(ParseByToken(token));
                            break;
                    }
                }

                return array;
            }

            private object ParseByToken(Token token)
            {
                return token switch
                {
                    Token.String => ParseString(),
                    Token.Number => ParseNumber(),
                    Token.CurlyOpen => ParseObject(),
                    Token.SquaredOpen => ParseArray(),
                    Token.True => true,
                    Token.False => false,
                    Token.Null => null,
                    _ => null
                };
            }

            private string ParseString()
            {
                var builder = new StringBuilder();

                // consume opening '"'
                _json.Read();

                var parsing = true;
                while (parsing)
                {
                    if (_json.Peek() == -1)
                    {
                        break;
                    }

                    var c = NextChar;
                    switch (c)
                    {
                        case '\"':
                            parsing = false;
                            break;
                        case '\\':
                            if (_json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '\"':
                                case '\\':
                                case '/':
                                    builder.Append(c);
                                    break;
                                case 'b':
                                    builder.Append('\b');
                                    break;
                                case 'f':
                                    builder.Append('\f');
                                    break;
                                case 'n':
                                    builder.Append('\n');
                                    break;
                                case 'r':
                                    builder.Append('\r');
                                    break;
                                case 't':
                                    builder.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new char[4];
                                    for (var i = 0; i < 4; i++)
                                    {
                                        hex[i] = NextChar;
                                    }

                                    builder.Append((char)Convert.ToInt32(new string(hex), 16));
                                    break;
                            }

                            break;
                        default:
                            builder.Append(c);
                            break;
                    }
                }

                return builder.ToString();
            }

            private object ParseNumber()
            {
                var number = NextWord;

                if (number.IndexOf('.') == -1)
                {
                    if (long.TryParse(number, out var parsedInt))
                    {
                        return parsedInt;
                    }
                }

                if (double.TryParse(number, out var parsedDouble))
                {
                    return parsedDouble;
                }

                return 0;
            }

            private void EatWhitespace()
            {
                while (_json.Peek() != -1)
                {
                    if (!char.IsWhiteSpace(PeekChar))
                    {
                        break;
                    }

                    _json.Read();
                }
            }

            private char PeekChar => Convert.ToChar(_json.Peek());

            private char NextChar => Convert.ToChar(_json.Read());

            private string NextWord
            {
                get
                {
                    var builder = new StringBuilder();

                    while (_json.Peek() != -1)
                    {
                        var c = PeekChar;
                        if (WORD_BREAK.IndexOf(c) != -1 || char.IsWhiteSpace(c))
                        {
                            break;
                        }

                        builder.Append(NextChar);
                    }

                    return builder.ToString();
                }
            }

            private Token NextToken
            {
                get
                {
                    EatWhitespace();

                    if (_json.Peek() == -1)
                    {
                        return Token.None;
                    }

                    switch (PeekChar)
                    {
                        case '{':
                            return Token.CurlyOpen;
                        case '}':
                            _json.Read();
                            return Token.CurlyClose;
                        case '[':
                            return Token.SquaredOpen;
                        case ']':
                            _json.Read();
                            return Token.SquaredClose;
                        case ',':
                            _json.Read();
                            return Token.Comma;
                        case '"':
                            return Token.String;
                        case ':':
                            _json.Read();
                            return Token.Colon;
                    }

                    var word = NextWord;
                    if (word.Length == 0)
                    {
                        return Token.None;
                    }

                    return word switch
                    {
                        "false" => Token.False,
                        "true" => Token.True,
                        "null" => Token.Null,
                        _ => Token.Number
                    };
                }
            }
        }

        private sealed class Serializer
        {
            private readonly StringBuilder _builder;

            private Serializer()
            {
                _builder = new StringBuilder();
            }

            public static string Serialize(object obj)
            {
                var instance = new Serializer();
                instance.SerializeValue(obj);
                return instance._builder.ToString();
            }

            private void SerializeValue(object value)
            {
                switch (value)
                {
                    case null:
                        _builder.Append("null");
                        break;
                    case string s:
                        SerializeString(s);
                        break;
                    case bool b:
                        _builder.Append(b ? "true" : "false");
                        break;
                    case IList list:
                        SerializeArray(list);
                        break;
                    case IDictionary dictionary:
                        SerializeObject(dictionary);
                        break;
                    case char c:
                        SerializeString(c.ToString());
                        break;
                    default:
                        SerializeOther(value);
                        break;
                }
            }

            private void SerializeObject(IDictionary obj)
            {
                var first = true;
                _builder.Append('{');

                foreach (DictionaryEntry entry in obj)
                {
                    if (!first)
                    {
                        _builder.Append(',');
                    }

                    first = false;

                    SerializeString(entry.Key.ToString());
                    _builder.Append(':');
                    SerializeValue(entry.Value);
                }

                _builder.Append('}');
            }

            private void SerializeArray(IList array)
            {
                _builder.Append('[');

                var first = true;
                foreach (var value in array)
                {
                    if (!first)
                    {
                        _builder.Append(',');
                    }

                    first = false;
                    SerializeValue(value);
                }

                _builder.Append(']');
            }

            private void SerializeString(string str)
            {
                _builder.Append('"');

                foreach (var c in str)
                {
                    switch (c)
                    {
                        case '"':
                            _builder.Append("\\\"");
                            break;
                        case '\\':
                            _builder.Append("\\\\");
                            break;
                        case '\b':
                            _builder.Append("\\b");
                            break;
                        case '\f':
                            _builder.Append("\\f");
                            break;
                        case '\n':
                            _builder.Append("\\n");
                            break;
                        case '\r':
                            _builder.Append("\\r");
                            break;
                        case '\t':
                            _builder.Append("\\t");
                            break;
                        default:
                            if (c < ' ')
                            {
                                _builder.AppendFormat("\\u{0:X4}", (int)c);
                            }
                            else
                            {
                                _builder.Append(c);
                            }
                            break;
                    }
                }

                _builder.Append('"');
            }

            private void SerializeOther(object value)
            {
                // Handles numbers and fallbacks
                if (value is float or double or decimal)
                {
                    _builder.Append(Convert.ToDouble(value).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (value is int or uint or long or sbyte or byte or short or ushort or ulong)
                {
                    _builder.Append(Convert.ToInt64(value));
                }
                else
                {
                    SerializeString(value.ToString());
                }
            }
        }
    }
}
