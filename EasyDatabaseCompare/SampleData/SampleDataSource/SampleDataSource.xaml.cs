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
#endif
}
