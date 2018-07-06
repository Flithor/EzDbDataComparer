//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ComparisonLib;
//using EasyDatabaseCompare.Model;

//namespace EasyDatabaseCompare.Service
//{
//    public interface IDataService
//    {
//        bool CheckConnection(string dbType, string[] fields);
//    }
//    public class DataService : IDataService
//    {
//        public DataService(IDataQuery querier)
//        {
//            Querier = querier;
//        }

//        private IDataQuery Querier { get; }

//        public bool CheckConnection(string dbType, string[] fields)
//        {
//            return Querier.CreateDataAccessor(dbType, fields);
//        }
//    }
//}
