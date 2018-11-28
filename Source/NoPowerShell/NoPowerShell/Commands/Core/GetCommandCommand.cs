﻿using System.Collections.Generic;
using NoPowerShell.Arguments;
using NoPowerShell.HelperClasses;
using System;
using System.Reflection;

/*
Author: @_bitsadmin
Website: https://github.com/bitsadmin
License: BSD 3-Clause
*/

namespace NoPowerShell.Commands
{
    public class GetCommandCommand : PSCommand
    {
        public GetCommandCommand(string[] userArguments) : base(userArguments, SupportedArguments)
        {
        }

        public override CommandResult Execute(CommandResult pipeIn)
        {
            Dictionary<Type, CaseInsensitiveList>  commandTypes = ReflectionHelper.GetCommands();

            // Iterate over all available cmdlets
            foreach (KeyValuePair<Type, CaseInsensitiveList> commandType in commandTypes)
            {
                // Hide TemplateCommand from list of commands
                // It is available to experiment with though
                if (commandType.Key == typeof(TemplateCommand))
                    continue;

                // Aliases
                CaseInsensitiveList aliases = commandType.Value;

                // Command name
                string commandName = aliases[0];

                // Arguments
                ArgumentList arguments = null;
                PropertyInfo argumentsProperty = commandType.Key.GetProperty("SupportedArguments", BindingFlags.Static | BindingFlags.Public);
                if (argumentsProperty != null)
                    arguments = (ArgumentList)argumentsProperty.GetValue(null, null);
                else
                    arguments = new ArgumentList();

                string[] strArgs = new string[arguments.Count];
                int i = 0;
                foreach (Argument arg in arguments)
                {
                    // Bool arguments don't require a value, they are simply a flag
                    if (arg.GetType() == typeof(BoolArgument))
                        strArgs[i] = string.Format("[-{0}]", arg.Name);
                    // String arguments can both be mandatory and optional
                    else if (arg.GetType() == typeof(StringArgument))
                    {
                        if (arg.IsOptionalArgument)
                            strArgs[i] = string.Format("[-{0} [Value]]", arg.Name);
                        else
                            strArgs[i] = string.Format("-{0} [Value]", arg.Name);
                    }
                    else if (arg.GetType() == typeof(IntegerArgument))
                        strArgs[i] = string.Format("[-{0} [Value]]", arg.Name);

                    i++;
                }

                // Synopsis
                string strSynopsis = null;
                PropertyInfo synopsisProperty = commandType.Key.GetProperty("Synopsis", BindingFlags.Static | BindingFlags.Public);
                if (synopsisProperty != null)
                    strSynopsis = (string)synopsisProperty.GetValue(null, null);

                string strArguments = string.Join(" ", strArgs);
                string strAliases = string.Join(", ", aliases.GetRange(1, aliases.Count - 1).ToArray());

                _results.Add(
                    new ResultRecord()
                    {
                        { "Command", string.Format("{0} {1}", commandName, strArguments) },
                        { "Aliases", strAliases },
                        { "Synopsis", strSynopsis }
                    }
                );
            }

            return _results;
        }

        public static new CaseInsensitiveList Aliases
        {
            get { return new CaseInsensitiveList() { "Get-Command" }; }
        }

        public static new ArgumentList SupportedArguments
        {
            get
            {
                return new ArgumentList()
                {
                };
            }
        }

        public static new string Synopsis
        {
            get { return "Shows all available commands."; }
        }
    }
}
