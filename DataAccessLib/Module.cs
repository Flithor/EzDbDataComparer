using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;

namespace DataAccessLib
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel?.Load<DataAccessor.Module>();
            Bind<IDataAccessorFactory>().To<DataAccessorFactory>().InSingletonScope();
        }
    }
}
