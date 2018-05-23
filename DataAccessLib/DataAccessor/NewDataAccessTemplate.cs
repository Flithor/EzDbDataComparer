/* Author: Flithor
 * ==============
 * Description: 
 * This file just a template for add 
 * new type of database support class
 * Not generated in program
 * Please strictly follow this template 
 * to create a new database support class
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccessLib.DataAccessor
{
    internal sealed class NewDataAccess : DataAccessor
    {
        //override connectionstringformat 
        #region Initialization
        //↓This method is used to splicing connection strings by fields
        //↓Argument order follows constructor arguments
        internal override string BuildConnectionString(params string[] fields)
        {
            throw new NotImplementedException();
        }
        //↓This method is used to add the timeout argument in connnetion string
        //↓usually 5 seconds
        //↓If not supported, can return "connStr"
        internal override string SetTimeOut(string connStr)
        {
            throw new NotImplementedException();
        }
        //↓This method is used to check whether the connection string can access the database
        internal override bool CheckConnection()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Query
        //↓This method is used to get target database table names list
        public override string[] QueryAllTableName()
        {
            throw new NotImplementedException();
        }

        //↓This method is used to query all selected tables data
        //↓If can, get the table schema
        public override DataSet QueryTables(string[] TableNames, Action processCallBack = null)
        {
            throw new NotImplementedException();
            //↓Please execute this action after each table successful query
            processCallBack?.DynamicInvoke();
        } 
        #endregion

    }
}
