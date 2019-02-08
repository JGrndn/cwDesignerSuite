using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.Batch
{
    /// <summary>
    /// Allow the management of Batch execution
    /// </summary>
    public class ApplicationBatchManager
    {
        /// <summary>
        /// Arguments passed to the executable
        /// </summary>
        public Dictionary<string, string> arguments = new Dictionary<string, string>();
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBatchManager"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public ApplicationBatchManager(string[] args) 
        {
            try
            {
                loadArguments(args);
                foreach (string key in new string[] { "connection", "model", "username", "password", "id" })
                {
                    isMandatoryArgument(key);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }


        }

        private void isMandatoryArgument(string argKey)
        {
            if (!arguments.ContainsKey(argKey))
            {
                throw new cwExceptionFatal("The " + argKey + " argument is mandatory in order to run in batch mode");
            }
        }

        private void loadArguments(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Count(); ++i)
                {
                    string argKeyString = args[i];
                    if (argKeyString.StartsWith("-"))
                    {
                        string argKey = argKeyString.Substring(1);
                        string argValue = catchNextArgument(args, argKey, i);
                        arguments[argKey] = argValue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        private string catchNextArgument(string[] args, string argKey, int keyPosition)
        {
            if (keyPosition + 1 < args.Count())
            {
                return args[keyPosition + 1];
            }
            else 
            {
                throw new cwExceptionFatal("After -" + argKey  + " argument, the " + argKey + " is required");
            }
        }
    }

 
}
