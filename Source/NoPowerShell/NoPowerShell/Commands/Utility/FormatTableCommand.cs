﻿using NoPowerShell.Arguments;
using NoPowerShell.HelperClasses;

/*
Author: @_bitsadmin
Website: https://github.com/bitsadmin
License: BSD 3-Clause
*/

namespace NoPowerShell.Commands
{
    public class FormatTableCommand : PSCommand
    {
        public FormatTableCommand(string[] arguments) : base(arguments, SupportedArguments)
        {
        }

        public override CommandResult Execute(CommandResult pipeIn)
        {
            string[] properties = _arguments.Get<StringArgument>("Property").Value.Split(',');

            _results.Output = CommandResult.OutputType.Table;

            foreach (ResultRecord result in pipeIn)
            {
                // Show all columns
                if (properties[0] == string.Empty)
                    _results.Add(result);

                // Show only specific columns
                else
                {
                    ResultRecord newResult = new ResultRecord();

                    foreach (string attr in properties)
                    {
                        if (result.ContainsKey(attr))
                            newResult.Add(attr, result[attr]);
                        else
                            newResult.Add(attr, null);
                    }

                    _results.Add(newResult);
                }
            }

            ResultPrinter.OutputResults(_results);

            return _results;
        }

        public static new CaseInsensitiveList Aliases
        {
            get { return new CaseInsensitiveList() { "Format-Table", "ft" }; }
        }

        public static new ArgumentList SupportedArguments
        {
            get
            {
                return new ArgumentList()
                {
                    new StringArgument("Property", string.Empty)
                };
            }
        }

        public static new string Synopsis
        {
            get { return "Formats the output as a table."; }
        }
    }
}
