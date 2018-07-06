using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLib;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using DataAccessLib.Entities;
using ComparisonLib.Common;

namespace ComparisonLib
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel?.Load<DataAccessLib.Module>();
            //Kernel.Bind(x =>
            //{
            //    x.FromAssemblyContaining<IDataAccessor>()
            //        .SelectAllClasses()
            //        .InheritedFrom<IDataAccessor>()
            //        .Where(c => c.Name == _config.DbType)
            //        .BindSingleInterface()
            //        .Configure((b, c) => b
            //            .InSingletonScope()
            //            .WithConstructorArgument(_config.DbConnectionStringInfo));
            //});
            Bind<IDataQuery>().To<DataQuery>().InSingletonScope();
            Bind<IDataComparer>().To<DataComparer>();
        }
    }
}
