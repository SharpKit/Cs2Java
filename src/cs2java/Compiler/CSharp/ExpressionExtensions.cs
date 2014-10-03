using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace JSharp.Compiler
{
    static class ExpressionExtensions
    {
        public static ExpressionType? ExtractCompoundAssignment(this ExpressionType x)
        {
            switch (x)
            {
                case ExpressionType.PostIncrementAssign: return ExpressionType.AddAssign;
                case ExpressionType.PostDecrementAssign: return ExpressionType.SubtractAssign;
                case ExpressionType.PreIncrementAssign: return ExpressionType.AddAssign;
                case ExpressionType.PreDecrementAssign: return ExpressionType.SubtractAssign;

                case ExpressionType.AddAssign: return ExpressionType.Add;
                case ExpressionType.SubtractAssign: return ExpressionType.Subtract;
                case ExpressionType.DivideAssign: return ExpressionType.Divide;
                case ExpressionType.ModuloAssign: return ExpressionType.Modulo;
                case ExpressionType.MultiplyAssign: return ExpressionType.Multiply;
            }
            return null;
        }
    }
}
