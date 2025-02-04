﻿using System.Collections.Generic;
using System.Collections.Immutable;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;

namespace Warpstone.Examples.Json
{
    public static class JsonParser
    {
        private static readonly IParser<JsonNull> Null
            = String("null").Then(Create(new JsonNull()));

        private static readonly IParser<JsonInt> Int
            = OneOrMore(Digit).Concat().Transform(x => new JsonInt(int.Parse(x)));

        private static readonly IParser<JsonDouble> Double
            = Regex(@"[0-9]*\.[0-9]+")
            .Transform(x => new JsonDouble(double.Parse(x)));

        private static readonly IParser<JsonArray> Array
            = Char('[').Then(Many(Lazy(() => Json), Char(','))).ThenSkip(Char(']'))
            .Transform(x => new JsonArray(x.ToImmutableArray()));

        private static readonly IParser<JsonBoolean> True
            = String("true").Transform(x => new JsonBoolean(true));

        private static readonly IParser<JsonBoolean> False
            = String("false").Transform(x => new JsonBoolean(false));

        private static readonly IParser<JsonBoolean> Boolean
            = Or(True, False);

        private static readonly IParser<string> StringContent
            = Or(Regex("[^\"\\n\\r]"), String("\\\""));

        private static readonly IParser<JsonString> String
            = Char('"').Then(Many(StringContent).Concat()).ThenSkip(Char('"'))
            .Transform(x => new JsonString(System.Text.RegularExpressions.Regex.Unescape(x)));

        private static readonly IParser<KeyValuePair<JsonString, JsonValue>> Field
            = OptionalWhitespaces
            .Then(String.Transform(x => x))
            .ThenSkip(OptionalWhitespaces)
            .ThenSkip(Char(':'))
            .ThenSkip(OptionalWhitespaces)
            .ThenAdd(Lazy(() => Json))
            .Transform((x, y) => new KeyValuePair<JsonString, JsonValue>(x, y));

        private static readonly IParser<JsonObject> Object
            = Char('{').Then(Many(Field, Char(','))).ThenSkip(Char('}'))
            .Transform(x => new JsonObject(x.ToImmutableArray()));

        private static readonly IParser<JsonValue> Json
            = OptionalWhitespaces.Then(Or<JsonValue>(Array, Object, String, Null, Double, Int, Boolean)).ThenSkip(OptionalWhitespaces);

        public static JsonValue Parse(string input)
            => Json.ThenEnd().Parse(input);

        private static IParser<string> Concat<T>(this IParser<IEnumerable<T>> chars)
            => chars.Transform(x => string.Join(string.Empty, x));
    }
}
