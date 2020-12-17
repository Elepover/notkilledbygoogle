using System;

namespace NotKilledByGoogle.Bot
{
    public static class Utils
    {
        /// <summary>
        /// Throw an exception if the passed value is <see langword="null"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="exceptionToThrow">Exception to throw if <see cref="value"/> is <see langword="null"/>.</param>
        /// <param name="valueMemberName"><see cref="value"/>'s member name in caller.</param>
        /// <typeparam name="T">Value's corresponding type. Can be automatically inferred.</typeparam>
        /// <returns>The <see cref="value"/> itself if not <see langword="null"/>.</returns>
        /// <exception cref="Exception">Passed <see cref="value"/> is <see langword="null"/>.</exception>
        public static T ThrowIfNull<T>(T? value, Exception? exceptionToThrow = null, string valueMemberName = "")
        {
            if (value is null) throw exceptionToThrow ?? new ArgumentNullException(valueMemberName + " cannot be null."); 
            return value;
        }
    }
}
