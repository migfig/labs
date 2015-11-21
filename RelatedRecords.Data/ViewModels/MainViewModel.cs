﻿using Common;
using RelatedRecords.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RelatedRecords.Data.ViewModels
{
    public partial class MainViewModel: BaseModel
    {
        #region properties

        private static MainViewModel _viewModel;
        public static MainViewModel ViewModel
        {
            get
            {
                if(null == _viewModel)
                {
                    _viewModel = new MainViewModel(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"), 
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatedrecords.cgt"));
                }

                return _viewModel;
            }
        }

        private readonly RRParser _parser;
        private readonly string _configurationFile;
        private IEnumerable<MethodInfo> _commandMethods;
        private readonly State _state;

        public MainViewModel(string configFile, string grammarFile)
        {
            _parser = new RRParser(grammarFile);
            _configurationFile = configFile;
            _commandMethods = GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<CommandAttribute>(false) != null);
            _state = new State(this);
        }
        
        public CConfiguration SelectedConfiguration
        {
            get { return Extensions.SelectedConfiguration; }
            set
            {
                Extensions.SelectedConfiguration = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedDataset");
            }
        }

        public CDataset SelectedDataset
        {
            get { return Extensions.SelectedDataset; }
            set
            {
                Extensions.SelectedDataset = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedDatasource");
            }
        }

        public CDatasource SelectedDatasource
        {
            get { return Extensions.SelectedDatasource; }
            set
            {
                Extensions.SelectedDatasource = value;
                OnPropertyChanged();
            }
        }

        private DatatableEx _currentTable;
        public DatatableEx CurrentTable
        {
            get { return _currentTable; }
            set
            {
                _currentTable = value;
                OnPropertyChanged();
            }
        }

        private Stack<DatatableEx> _tableNavigation = new Stack<DatatableEx>();
        public Stack<DatatableEx> TableNavigation
        {
            get { return _tableNavigation; }
        }

        private bool _isValidCommand;
        public bool IsValidCommand
        {
            get { return _isValidCommand; }
            set
            {
                _isValidCommand = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        #endregion properties

        #region methods

        public bool LoadConfiguration()
        {
            var isStateSaved = _state.SaveState();

            var cfg = XmlHelper<CConfiguration>.Load(_configurationFile);
            cfg.Deflate();
            SelectedConfiguration = cfg;

            if (isStateSaved)
            {
                _state.RestoreState();
            }

            return cfg.Dataset.Any();
        }

        public bool SaveConfiguration()
        {
            SelectedConfiguration.Inflate();
            var result = XmlHelper<CConfiguration>.Save(_configurationFile, SelectedConfiguration);
            SelectedConfiguration.Deflate();

            return result;
        }

        private string _command;
        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                OnPropertyChanged();
            }
        }

        public void ExecuteCommand()
        {
            if (string.IsNullOrWhiteSpace(Command)) return;

            var parseResults = _parser.Parse(Command);
            IsValidCommand = parseResults.isAccepted;
            if(parseResults.isAccepted)
            {
                HandleCommand(parseResults);
            }
        }

        #endregion methods
    }
}
