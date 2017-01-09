namespace Contracts
{
    public partial class GeocodeLocation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double AltitudeField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LatitudeField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LongitudeField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Altitude
        {
            get
            {
                return this.AltitudeField;
            }
            set
            {
                if ((this.AltitudeField.Equals(value) != true))
                {
                    this.AltitudeField = value;
                    this.RaisePropertyChanged("Altitude");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude
        {
            get
            {
                return this.LatitudeField;
            }
            set
            {
                if ((this.LatitudeField.Equals(value) != true))
                {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude
        {
            get
            {
                return this.LongitudeField;
            }
            set
            {
                if ((this.LongitudeField.Equals(value) != true))
                {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CalculationMethodField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CalculationMethod
        {
            get
            {
                return this.CalculationMethodField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CalculationMethodField, value) != true))
                {
                    this.CalculationMethodField = value;
                    this.RaisePropertyChanged("CalculationMethod");
                }
            }
        }
    }
}
