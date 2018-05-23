using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataAccessLib.DataAccessor;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Factory;

namespace DataAccessLib.DataAccessor
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x =>
                x.FromThisAssembly()
                    .SelectAllClasses()
                    //.Where(t=>typeof(IDataAccessor).IsAssignableFrom(t))
                    .InheritedFrom<DataAccessorBase>()
                    .BindSingleInterface()
                    .Configure((b, c) => b.InSingletonScope().Named(c.Name)));
        }
    }
}
