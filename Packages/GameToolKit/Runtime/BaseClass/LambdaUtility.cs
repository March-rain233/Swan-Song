using System;

namespace GameToolKit.Utility
{
    /// <summary>
    /// lambda函数辅助类
    /// </summary>
    public static class LambdaUtility
    {
        /// <summary>
        /// 生成递归调用的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T, TResult> Fix<T, TResult>(Func<Func<T, TResult>, Func<T, TResult>> f)
        {
            return x => f(Fix(f))(x);
        }
        /// <summary>
        /// 生成递归调用的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T1, T2, TResult> Fix<T1, T2, TResult>(Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> f)
        {
            return (x, y) => f(Fix(f))(x, y);
        }
        public static Action<T1, T2> Fix<T1, T2>(Func<Action<T1,T2>, Action<T1, T2>> f)
        {
            return (x, y) => f(Fix(f))(x, y);
        }
        public static Action<T1> Fix<T1>(Func<Action<T1>, Action<T1>> f)
        {
            return x => f(Fix(f))(x);
        }
    }
}
