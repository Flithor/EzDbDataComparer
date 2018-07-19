//   *********  请勿修改此文件   *********
//   此文件由设计工具再生成。更改
//   此文件可能会导致错误。
namespace Expression.Blend.SampleData.SampleDataSource
{
    using System; 
    using System.ComponentModel;

// 若要在生产应用程序中显著减小示例数据涉及面，则可以设置
// DISABLE_SAMPLE_DATA 条件编译常量并在运行时禁用示例数据。
#if DISABLE_SAMPLE_DATA
    internal class SampleDataSource { }
#else

    public class SampleDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SampleDataSource()
        {
            try
            {
                Uri resourceUri = new Uri("/EasyDatabaseCompare;component/SampleData/SampleDataSource/SampleDataSource.xaml", UriKind.RelativeOrAbsolute);
                System.Windows.Application.LoadComponent(this, resourceUri);
            }
            catch
            {
            }
        }

        private FilteredComparerResultOverview _FilteredComparerResultOverview = new FilteredComparerResultOverview();

        public FilteredComparerResultOverview FilteredComparerResultOverview
        {
            get
            {
                return this._FilteredComparerResultOverview;
            }
        }

        private SelectedDetail _SelectedDetail = new SelectedDetail();

        public SelectedDetail SelectedDetail
        {
            get
            {
                return this._SelectedDetail;
            }
        }

        private bool _HideUnchangedTables = false;

        public bool HideUnchangedTables
        {
            get
            {
                return this._HideUnchangedTables;
            }

            set
            {
                if (this._HideUnchangedTables != value)
                {
                    this._HideUnchangedTables = value;
                    this.OnPropertyChanged("HideUnchangedTables");
                }
            }
        }

        private bool _HideEmptyTables = false;

        public bool HideEmptyTables
        {
            get
            {
                return this._HideEmptyTables;
            }

            set
            {
                if (this._HideEmptyTables != value)
                {
                    this._HideEmptyTables = value;
                    this.OnPropertyChanged("HideEmptyTables");
                }
            }
        }

        private bool _ShowSameColumn = false;

        public bool ShowSameColumn
        {
            get
            {
                return this._ShowSameColumn;
            }

            set
            {
                if (this._ShowSameColumn != value)
                {
                    this._ShowSameColumn = value;
                    this.OnPropertyChanged("ShowSameColumn");
                }
            }
        }

        private bool _ShowChangedColumn = false;

        public bool ShowChangedColumn
        {
            get
            {
                return this._ShowChangedColumn;
            }

            set
            {
                if (this._ShowChangedColumn != value)
                {
                    this._ShowChangedColumn = value;
                    this.OnPropertyChanged("ShowChangedColumn");
                }
            }
        }
    }

    public class FilteredComparerResultOverview : System.Collections.ObjectModel.ObservableCollection<FilteredComparerResultOverviewItem>
    { 
    }

    public class FilteredComparerResultOverviewItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Table_Name = string.Empty;

        public string Table_Name
        {
            get
            {
                return this._Table_Name;
            }

            set
            {
                if (this._Table_Name != value)
                {
                    this._Table_Name = value;
                    this.OnPropertyChanged("Table_Name");
                }
            }
        }

        private double _Same = 0;

        public double Same
        {
            get
            {
                return this._Same;
            }

            set
            {
                if (this._Same != value)
                {
                    this._Same = value;
                    this.OnPropertyChanged("Same");
                }
            }
        }

        private double _Changed = 0;

        public double Changed
        {
            get
            {
                return this._Changed;
            }

            set
            {
                if (this._Changed != value)
                {
                    this._Changed = value;
                    this.OnPropertyChanged("Changed");
                }
            }
        }

        private double _Inserted = 0;

        public double Inserted
        {
            get
            {
                return this._Inserted;
            }

            set
            {
                if (this._Inserted != value)
                {
                    this._Inserted = value;
                    this.OnPropertyChanged("Inserted");
                }
            }
        }

        private double _Deleted = 0;

        public double Deleted
        {
            get
            {
                return this._Deleted;
            }

            set
            {
                if (this._Deleted != value)
                {
                    this._Deleted = value;
                    this.OnPropertyChanged("Deleted");
                }
            }
        }
    }

    public class SelectedDetail : System.Collections.ObjectModel.ObservableCollection<SelectedDetailItem>
    { 
    }

    public class SelectedDetailItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _ColumnA = string.Empty;

        public string ColumnA
        {
            get
            {
                return this._ColumnA;
            }

            set
            {
                if (this._ColumnA != value)
                {
                    this._ColumnA = value;
                    this.OnPropertyChanged("ColumnA");
                }
            }
        }

        private string _ColumnB = string.Empty;

        public string ColumnB
        {
            get
            {
                return this._ColumnB;
            }

            set
            {
                if (this._ColumnB != value)
                {
                    this._ColumnB = value;
                    this.OnPropertyChanged("ColumnB");
                }
            }
        }

        private string _ColumnC = string.Empty;

        public string ColumnC
        {
            get
            {
                return this._ColumnC;
            }

            set
            {
                if (this._ColumnC != value)
                {
                    this._ColumnC = value;
                    this.OnPropertyChanged("ColumnC");
                }
            }
        }

        private double _ColumnD = 0;

        public double ColumnD
        {
            get
            {
                return this._ColumnD;
            }

            set
            {
                if (this._ColumnD != value)
                {
                    this._ColumnD = value;
                    this.OnPropertyChanged("ColumnD");
                }
            }
        }

        private string _ColumnE = string.Empty;

        public string ColumnE
        {
            get
            {
                return this._ColumnE;
            }

            set
            {
                if (this._ColumnE != value)
                {
                    this._ColumnE = value;
                    this.OnPropertyChanged("ColumnE");
                }
            }
        }

        private string _ColumnF = string.Empty;

        public string ColumnF
        {
            get
            {
                return this._ColumnF;
            }

            set
            {
                if (this._ColumnF != value)
                {
                    this._ColumnF = value;
                    this.OnPropertyChanged("ColumnF");
                }
            }
        }

        private System.Windows.Media.ImageSource _ColumnG = null;

        public System.Windows.Media.ImageSource ColumnG
        {
            get
            {
                return this._ColumnG;
            }

            set
            {
                if (this._ColumnG != value)
                {
                    this._ColumnG = value;
                    this.OnPropertyChanged("ColumnG");
                }
            }
        }
    }
#endif
}
