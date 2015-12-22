using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace EVEMon.Common.Service
{
    internal class CodeCompiler
    {
        private readonly CompilerParameters m_compilerParameters = new CompilerParameters();

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompiler"/> class.
        /// </summary>
        /// <param name="referenceAssemblies">The reference assemblies.</param>
        internal CodeCompiler(string[] referenceAssemblies)
        {
            m_compilerParameters.GenerateInMemory = true;
            m_compilerParameters.GenerateExecutable = false;
            m_compilerParameters.ReferencedAssemblies.Add(GetType().Assembly.Location);

            foreach (string referenceAssembly in referenceAssemblies)
            {
                m_compilerParameters.ReferencedAssemblies.Add(referenceAssembly);
            }
        }

        /// <summary>
        /// Creates the instance from the specified code text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeText">The code text.</param>
        /// <returns></returns>
        internal T CreateInstanceFrom<T>(string codeText) where T : class
        {
            Assembly assembly = Compile(codeText);

            if (assembly == null)
                return null;

            Type type = assembly.GetExportedTypes().FirstOrDefault(exportedType => exportedType.IsSubclassOf(typeof(T)));

            if (type == null)
                return null;

            return Activator.CreateInstance(type) as T;
        }

        /// <summary>
        /// Compiles the specified code text.
        /// </summary>
        /// <param name="codeText">The code text.</param>
        /// <returns></returns>
        private Assembly Compile(string codeText)
        {
            CompilerResults results;
            using (CodeDomProvider csProvider = new CSharpCodeProvider())
            {
                results = csProvider.CompileAssemblyFromSource(m_compilerParameters, codeText);
            }

            if (!results.Errors.HasErrors)
                return results.CompiledAssembly;

            results.Errors.OfType<CompilerError>().ToList().ForEach(x => EveMonClient.Trace(x.ErrorText));

            return null;
        }
    }
}