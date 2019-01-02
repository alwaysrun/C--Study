#region Header
/* *
 * JsonException.cs
 *   Base class throwed by LitJSON when a parsing error occurs.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;


namespace LitJson
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public JsonException () : base ()
        {
        }

        internal JsonException (ParserToken token) :
            base (String.Format (
                    "Invalid token '{0}' in input string", token))
        {
        }

        internal JsonException (ParserToken token,
                                Exception inner_exception) :
            base (String.Format (
                    "Invalid token '{0}' in input string", token),
                inner_exception)
        {
        }

        internal JsonException (int c) :
            base (String.Format (
                    "Invalid character '{0}' in input string", (char) c))
        {
        }

        internal JsonException (int c, Exception inner_exception) :
            base (String.Format (
                    "Invalid character '{0}' in input string", (char) c),
                inner_exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public JsonException (string message) : base (message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner_exception"></param>
        public JsonException (string message, Exception inner_exception) :
            base (message, inner_exception)
        {
        }
    }
}
