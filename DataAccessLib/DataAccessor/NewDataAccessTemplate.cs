/* Author: Flithor
 * ==============
 * Description: 
 * This file is a demo for add new database accessor class
 * Please strictly follow this file to write new database accessor class
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DataAccessLib.Common;
using DataAccessLib.Entities;
using Ninject.Infrastructure.Language;

namespace DataAccessLib.DataAccessor
{
    public sealed class DemoDb : DataAccessorBase<DemoConnection>//←Set this DbConnection
    {
        #region Constructor
        //↓This is Constructor, normal is empty
        public DemoDb(DbConnectionStringInfo connStrInfo) : base(connStrInfo)
        {
            //You can do some other things here
        }
        #endregion

        #region ConnectionInfoSetting
        //↓Override this readonly property to set database connection string format
        //↓It will be use with DbConnectionStringInfo.ConnecgtionStringFieldValues
        public override string ConnectionStringFormat => "Data Source={0};PassWord={1}";
        //↓Override this readonly property to set database connection string field name for display on window
        //↓Use \u200B(0 width character) before word to mark this filed type (Or use ZeroWidthMark class)
        //↓Check more info in EasyDatabaseCompare/FieldsTextBlockTemplateSelector.cs
        internal override string[] ConnectionStringFieldNames { get; } =
        {
            "Data Source", "Password", ZeroWidthMark.BooleanMark + "Bool", ZeroWidthMark.SelectMark + "Select"
        };
        //↓Override it is not necessary.
        //↓But if the field has a default value,
        //↓you can override it and create an string array of the same length as ConnectionStringFieldNames
        //↓Set the default values in order by ConnectionStringFieldNames array.
        //↓If a fields not has a default values should skip by null.
        internal override string[] ConnectionStringFieldDefaultValue { get; } = { null, "DefaultValue", "True", "Item1|Item2|Item3" };
        #endregion

        #region Custom initialization
        //↓If you need to building the connection string in custom way, you can override it
        internal override string BuildConnectionString(params string[] fields)
        {
            return base.BuildConnectionString(fields);
        }

        //↓If you need to check the connection workable in custom way, you can override it
        public override bool CheckConnection()
        {
            //return base.CheckConnection();
            return true;
        }
        #endregion

        #region Query
        //↓This method is used to get target database table names list
        public override IEnumerable<string> GetDataBaseTableNames()
        {
            return DemoTables.DemoTable.Select(t => t.TableName);
        }
        //↓This method is used to query all selected tables data
        //↓If can, get the table schema
        //↓**Temporarily deprecated**
        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            throw new NotImplementedException();
            //↓Please execute this action after each table successful query(May be discarded)
            processCallBack?.DynamicInvoke();
        }
        //↓This method is used to query a specific table data
        //↓If can, get the table schema
        public override DataTable QueryTable(string tableName)
        {
            DemoTables.RandomChange();
            return DemoTables.GetTableCopy(DemoTables.DemoTable.First(t =>
                string.Equals(t.TableName, tableName, StringComparison.CurrentCultureIgnoreCase)));
        }
        #endregion
    }

    #region ignore this, just for demo
    public class DemoConnection : IDbConnection
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get; }
        public string Database { get; }
        public ConnectionState State { get; }
    }

    internal static class DemoTables
    {
        public static readonly DataTable[] DemoTable;
        static DemoTables()
        {
            DemoTable = new[]
            {
                new DataTable("TableA")
                {
                    Columns =
                    {
                        "ColumnA",
                        "ColumnB",
                        "ColumnC",
                        "ColumnD",
                        "ColumnE",
                        "ColumnF",
                        "ColumnG"
                    },
                    Rows =
                    {
                        new object[]
                        {
                            "Class aenean curae",
                            "4567 Main St., Buffalo, NY 98052",
                            100m,
                            55,
                            "http://www.adatum.com/",
                            "A. Datum Corporation",
                            @"SampleDataSource_Files\image01.png"
                        },
                        new object[]
                        {
                            "Aliquam integer cras",
                            "1234 Main St., Buffalo, NY 98052",
                            29.99m,
                            87,
                            "http://www.adventure-works.com/",
                            "Adventure Works",
                            @"SampleDataSource_Files\image02.png"
                        },
                        new object[]
                        {
                            "Duis quisque",
                            "2345 Main St., Buffalo, NY 98052",
                            249.99m,
                            34,
                            "http://www.adventure-works.com/",
                            "Adventure Works",
                            @"SampleDataSource_Files\image03.png"
                        },
                        new object[]
                        {
                            "Nam maecenas",
                            "789 Main St., Buffalo, NY 98052",
                            1000m,
                            39,
                            "http://www.alpineskihouse.com/",
                            "Alpine Ski House",
                            @"SampleDataSource_Files\image04.png"
                        },
                        new object[]
                        {
                            "Vivamus aliquam praesent aliquet",
                            "1234 Front St., Seattle, WA 12345",
                            300m,
                            49,
                            "http://www.contoso.com/",
                            "Contoso, Ltd",
                            @"SampleDataSource_Files\image05.png"
                        },
                        new object[]
                        {
                            "Donec sed nunc dis",
                            "2345 Front St., Seattle, WA 12345",
                            10.999m,
                            52,
                            "http://www.contoso.com/",
                            "Contoso Pharmaceuticals",
                            @"SampleDataSource_Files\image01.png"
                        },
                        new object[]
                        {
                            "Blandit etiam",
                            "789 Front St., Seattle, WA 12345",
                            49.99m,
                            53,
                            "http://www.fabrikam.com/",
                            "Fabrikam, Inc.",
                            @"SampleDataSource_Files\image02.png"
                        },
                        new object[]
                        {
                            "Fusce mauris lorem",
                            "4567 Main St., Seattle, WA 12345",
                            25.34m,
                            33,
                            "http://www.fourthcoffee.com/",
                            "Fourth Coffee",
                            @"SampleDataSource_Files\image03.png"
                        },
                        new object[]
                        {
                            "Amet accumsan commodo ante",
                            "1234 1st Ave, San Francisco, CA 99999",
                            100m,
                            64,
                            "http://www.graphicdesigninstitute.com/",
                            "Graphic Design Institute",
                            @"SampleDataSource_Files\image04.png"
                        },
                        new object[]
                        {
                            "Nullam est morbi bibendum",
                            "2345 1st Ave, San Francisco, CA 99999",
                            29.99m,
                            73,
                            "http://www.humongousinsurance.com/",
                            "Humongous Insurance",
                            @"SampleDataSource_Files\image05.png"
                        }
                    }
                },
                new DataTable("TableB")
                {
                    Columns =
                    {
                        "Col1",
                        "Col2",
                        "Col3",
                        "Col4"
                    },
                    Rows =
                    {
                        new object[]
                        {
                            "Madison Becker",
                            "2018/12/10",
                            "7216",
                            "8",
                        },
                        new object[]
                        {
                            "Dominique Duke",
                            "2018/09/10",
                            "8052",
                            "4",
                        },
                        new object[]
                        {
                            "Garth Bernard",
                            "2018/01/19",
                            "7888",
                            "2",
                        },
                        new object[]
                        {
                            "Hannah Holder",
                            "2018/03/28",
                            "8057",
                            "2",
                        },
                        new object[]
                        {
                            "Carson Bryan",
                            "2018/01/11",
                            "4044",
                            "10",
                        },
                        new object[]
                        {
                            "Yvonne Ross",
                            "2019/02/17",
                            "1162",
                            "6",
                        },
                        new object[]
                        {
                            "Lane Ashley",
                            "2018/11/03",
                            "2612",
                            "5",
                        },
                        new object[]
                        {
                            "Josephine Baxter",
                            "2019/06/19",
                            "5042",
                            "8",
                        },
                        new object[]
                        {
                            "Shoshana Roberts",
                            "2018/05/26",
                            "1369",
                            "4",
                        },
                        new object[]
                        {
                            "Wilma Dodson",
                            "2017/11/30",
                            "4955",
                            "3",
                        },
                        new object[]
                        {
                            "Richard Sullivan",
                            "2018/11/01",
                            "2954",
                            "4",
                        },
                        new object[]
                        {
                            "Chelsea Bailey",
                            "2018/06/21",
                            "6190",
                            "7",
                        },
                        new object[]
                        {
                            "Nash Whitley",
                            "2018/03/23",
                            "2086",
                            "7",
                        },
                    }
                }
            };
            DemoTable[0].PrimaryKey = new[] { DemoTable[0].Columns[0] };
        }

        public struct DemoTableObj
        {
            public string ColumnA;
            public string ColumnB;
            public decimal ColumnC;
            public int ColumnD;
            public string ColumnE;
            public string ColumnF;
            public string ColumnG;
        }
        public struct DemoObj2
        {
            public string Col1;
            public string Col2;
            public string Col3;
            public string Col4;
        }

        public static DataTable GetTableCopy(DataTable dt)
        {
            var copy = dt.Clone();
            foreach (DataRow dataRow in dt.Rows)
                copy.Rows.Add(dataRow.ItemArray);

            return copy;
        }

        public static void RandomChange()
        {
            foreach (var table in DemoTable)
            {
                foreach (DataRow tableRow in table.Rows)
                {
                    for (int i = 0; i < tableRow.ItemArray.Length; i++)
                    {
                        if (rndBool())
                            if (Type.GetTypeCode(tableRow.ItemArray[i].GetType()) == TypeCode.String)
                                tableRow[i] = tableRow.ItemArray[i].ToString() + rnd.Next(0, 9);
                    }
                }
                table.AcceptChanges();
            }
        }
        static Random rnd = new Random();

        private static bool rndBool()
        {
            var p = 0.01;
            return rnd.NextDouble() < p;
        }

    }
    #endregion
}
