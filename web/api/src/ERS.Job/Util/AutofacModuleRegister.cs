using System.Reflection;
using Autofac;

namespace ERS.Job.Util
{
    public class AutofacModuleRegister: Autofac.Module {
        //重写Autofac管道Load方法，在这里注册注入
        protected override void Load (ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(GetAssemblyByName("ERS.Job")).Where(a => a.Namespace.EndsWith("Jobs")).AsImplementedInterfaces();
        }
        /// <summary>
        /// 根据程序集名称获取程序集
        /// </summary>
        /// <param name="AssemblyName">程序集名称</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName (String AssemblyName) {

            return Assembly.Load (AssemblyName);
        }
    }
}