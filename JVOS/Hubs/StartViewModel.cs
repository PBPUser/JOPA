using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.Hubs
{
    public class StartViewModel : INotifyPropertyChanged
    {
        public StartViewModel()
        {
            _Entrypoints = ApplicationManager.EntryPoints;
            _Pinnedpoints = new List<IEntryPoint>();
        }

        private List<IEntryPoint> _Entrypoints;
        public List<IEntryPoint> EntryPoints
        {
            get => _Entrypoints;
            set {
                OnPropertyChanged(nameof(EntryPoints));
                _Entrypoints = value;
            }
        }

        private List<IEntryPoint> _Pinnedpoints;
        public List<IEntryPoint> PinnedPoints
        {
            get => _Pinnedpoints;
            set
            {
                OnPropertyChanged(nameof(PinnedPoints));
                _Pinnedpoints = value;
            }
        }

        private void OnPropertyChanged(string s)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(s));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
