﻿using PythonTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonTS
{
    public interface IScript
    {
        Executor Executor { get; set; }
        string Contents { get; }
        string Name { get; }
        string FilePath { get; set; }
        bool Exists { get; }
        bool Enabled { get; set; }
        bool Enforced { get; set; }
        DateTime LastUpdated { get; }

        Executor Execute(ScriptArguments[] args = null);
        Executor ExecuteMethod(string methodName, ScriptArguments[] args = null);
        Executor Reload(ScriptArguments[] args = null);
        void Disable();
    }
}