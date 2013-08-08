using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace Zgke.Run
{
    /// <summary>  
    /// 运行一个代码  
    /// zgke@sina.com  
    /// qq :116149  
    /// </summary>  
    public class GetAssembly
    {

        /// <summary>  
        /// 根据代码获取一个Assembly  
        /// </summary>  
        /// <param name="Code">代码区域 包含USING</param>  
        /// <param name="UsingList">需要引用的DLL</param>         
        /// <returns>返回Assembly</returns>  
        public static Assembly GetCodeAssembly(string p_Code, IList<string> p_UsingList)
        {
            CodeDomProvider _CodeDom = new CSharpCodeProvider();
            CompilerParameters _CodeParamertes = new CompilerParameters();

            for (int i = 0; i != p_UsingList.Count; i++)
            {
                _CodeParamertes.ReferencedAssemblies.Add(p_UsingList[i].ToString());   //("System.dll");  
            }
            _CodeParamertes.GenerateExecutable = false;
            _CodeParamertes.GenerateInMemory = true;

            CompilerResults _CompilerResults = _CodeDom.CompileAssemblyFromSource(_CodeParamertes, p_Code);

            if (_CompilerResults.Errors.HasErrors)
            {
                string _ErrorText = "";
                foreach (CompilerError _Error in _CompilerResults.Errors)
                {
                    _ErrorText += _Error.ErrorText + "/r/n";
                }
                throw new Exception(_ErrorText);
            }
            else
            {
                return _CompilerResults.CompiledAssembly;
            }
        }

        /// <summary>  
        /// 根据WEBSERVICE地址获取一个 Assembly  
        /// </summary>  
        /// <param name="p_Url">地址</param>  
        /// <param name="p_NameSpace">命名空间</param>  
        /// <returns>返回Assembly</returns>  
        public static Assembly GetWebServiceAssembly(string p_Url, string p_NameSpace)
        {
            try
            {
                System.Net.WebClient _WebClient = new System.Net.WebClient();


                
                System.IO.Stream _WebStream = _WebClient.OpenRead(p_Url);

                ServiceDescription _ServiceDescription = ServiceDescription.Read(_WebStream);

                _WebStream.Close();
                _WebClient.Dispose();
                ServiceDescriptionImporter _ServiceDescroptImporter = new ServiceDescriptionImporter();
                _ServiceDescroptImporter.AddServiceDescription(_ServiceDescription, "", "");
                System.CodeDom.CodeNamespace _CodeNameSpace = new System.CodeDom.CodeNamespace(p_NameSpace);
                System.CodeDom.CodeCompileUnit _CodeCompileUnit = new System.CodeDom.CodeCompileUnit();
                _CodeCompileUnit.Namespaces.Add(_CodeNameSpace);
                _ServiceDescroptImporter.Import(_CodeNameSpace, _CodeCompileUnit);

                System.CodeDom.Compiler.CodeDomProvider _CodeDom = new Microsoft.CSharp.CSharpCodeProvider();
                System.CodeDom.Compiler.CompilerParameters _CodeParameters = new System.CodeDom.Compiler.CompilerParameters();
                _CodeParameters.GenerateExecutable = false;
                _CodeParameters.GenerateInMemory = true;
                _CodeParameters.ReferencedAssemblies.Add("System.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.XML.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                _CodeParameters.ReferencedAssemblies.Add("System.Data.dll");

                System.CodeDom.Compiler.CompilerResults _CompilerResults = _CodeDom.CompileAssemblyFromDom(_CodeParameters, _CodeCompileUnit);

                if (_CompilerResults.Errors.HasErrors)
                {
                    string _ErrorText = "";
                    foreach (CompilerError _Error in _CompilerResults.Errors)
                    {
                        _ErrorText += _Error.ErrorText + "/r/n";
                    }
                    throw new Exception(_ErrorText);
                }
               
                return _CompilerResults.CompiledAssembly;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }

}