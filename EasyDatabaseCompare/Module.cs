using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDatabaseCompare.Model;
using EasyDatabaseCompare.ViewModel;
using Ninject;
using Ninject.Modules;

namespace EasyDatabaseCompare
{
    class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Load<ComparisonLib.Module>();
            Bind<DataCacheModel>().ToSelf();
            Bind<MainWindow>().ToSelf();
            Bind<IWindowViewModel>().To<WindowViewModel>().InSingletonScope();
            //Bind<IDataService>().To<DataService>().InSingletonScope();
        }
    }
}
