﻿using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;
using static Warpstone.Parsers.ExpressionParsers;

namespace Warpstone.Examples.Expressions
{
    public static class ExpressionParser
    {
        private static readonly IParser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x)) as Expression).Trim();

        private static readonly IParser<Expression> Parenthesized
            = Char('(')
            .Then(Lazy(() => Exp).Trim())
            .ThenSkip(Char(')'))
            .Trim();

        private static readonly IParser<Expression> Atom
            = Or(Parenthesized, Num).Trim();

        private static readonly IParser<Expression> Exp
            = BuildExpression(Atom, new[]
            {
                RightToLeft<char, Expression>(
                    (Operator('^'), (l, r) => new PowExpression(l, r))
                ),
                LeftToRight<char, Expression>(
                    (Operator('*'), (l, r) => new MulExpression(l, r)),
                    (Operator('/'), (l, r) => new DivExpression(l, r))
                ),
                LeftToRight<char, Expression>(
                    (Operator('+'), (l, r) => new AddExpression(l, r)),
                    (Operator('-'), (l, r) => new SubExpression(l, r))
                ),
            });

        private static IParser<char> Operator(char c)
            => Char(c).Trim();

        public static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);
    }
}
