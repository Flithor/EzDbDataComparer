using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib;

namespace DataModelsLib.Service
{
    public interface IDataService
    {

    }
    public class DataService : IDataService
    {
        public DataService(IDataQuery querier, IDataComparer comparator)
        {
            Querier = querier;
            Comparator = comparator;
        }

        private IDataQuery Querier { get; }
        private IDataComparer Comparator { get; }
    }
}
