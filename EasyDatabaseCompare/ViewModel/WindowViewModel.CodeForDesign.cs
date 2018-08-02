#if DEBUG
using ComparisonLib;
using EasyDatabaseCompare.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDatabaseCompare.ViewModel
{
    public partial class WindowViewModel
    {
        #region DesignCode
        public WindowViewModel() : this(new DataCacheModel(), new StandardKernel(new Module()).Get<IDataQuery>(), new DataComparer())
        {
            //SelectedDbType = "MsSqlServer";
            //CustomConnectionStringMode = true;
            //Fields[0].Value = "Server=127.0.0.1,51433;Database=V2895;User Id=sghtis;Password=h8848";
            //CheckConnectionCommand.Execute(null);
            //BlackListMode = true;
            //QuerySourceCommand.Execute(null);
            //TargetData = SourceData;
            //HideUnchangedTables = false;
            //StartComparer();
        }
        #endregion
    }
}
#endif