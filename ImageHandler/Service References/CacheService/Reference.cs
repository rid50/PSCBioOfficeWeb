﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageHandler.CacheService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WsqImage", Namespace="http://schemas.datacontract.org/2004/07/")]
    [System.SerializableAttribute()]
    public partial class WsqImage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private byte[] Contentk__BackingFieldField;
        
        private int PixelFormatk__BackingFieldField;
        
        private int XResk__BackingFieldField;
        
        private int XSizek__BackingFieldField;
        
        private int YResk__BackingFieldField;
        
        private int YSizek__BackingFieldField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Content>k__BackingField", IsRequired=true)]
        public byte[] Contentk__BackingField {
            get {
                return this.Contentk__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.Contentk__BackingFieldField, value) != true)) {
                    this.Contentk__BackingFieldField = value;
                    this.RaisePropertyChanged("Contentk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<PixelFormat>k__BackingField", IsRequired=true)]
        public int PixelFormatk__BackingField {
            get {
                return this.PixelFormatk__BackingFieldField;
            }
            set {
                if ((this.PixelFormatk__BackingFieldField.Equals(value) != true)) {
                    this.PixelFormatk__BackingFieldField = value;
                    this.RaisePropertyChanged("PixelFormatk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<XRes>k__BackingField", IsRequired=true)]
        public int XResk__BackingField {
            get {
                return this.XResk__BackingFieldField;
            }
            set {
                if ((this.XResk__BackingFieldField.Equals(value) != true)) {
                    this.XResk__BackingFieldField = value;
                    this.RaisePropertyChanged("XResk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<XSize>k__BackingField", IsRequired=true)]
        public int XSizek__BackingField {
            get {
                return this.XSizek__BackingFieldField;
            }
            set {
                if ((this.XSizek__BackingFieldField.Equals(value) != true)) {
                    this.XSizek__BackingFieldField = value;
                    this.RaisePropertyChanged("XSizek__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<YRes>k__BackingField", IsRequired=true)]
        public int YResk__BackingField {
            get {
                return this.YResk__BackingFieldField;
            }
            set {
                if ((this.YResk__BackingFieldField.Equals(value) != true)) {
                    this.YResk__BackingFieldField = value;
                    this.RaisePropertyChanged("YResk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<YSize>k__BackingField", IsRequired=true)]
        public int YSizek__BackingField {
            get {
                return this.YSizek__BackingFieldField;
            }
            set {
                if ((this.YSizek__BackingFieldField.Equals(value) != true)) {
                    this.YSizek__BackingFieldField = value;
                    this.RaisePropertyChanged("YSizek__BackingField");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FingerPrintDataContract", Namespace="http://schemas.datacontract.org/2004/07/CommonService")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ImageHandler.CacheService.WsqImage))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.Collections.ArrayList))]
    public partial class FingerPrintDataContract : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.ArrayList fingersCollectionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string idField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ArrayList fingersCollection {
            get {
                return this.fingersCollectionField;
            }
            set {
                if ((object.ReferenceEquals(this.fingersCollectionField, value) != true)) {
                    this.fingersCollectionField = value;
                    this.RaisePropertyChanged("fingersCollection");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                if ((object.ReferenceEquals(this.idField, value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CacheService.IMemoryCacheService")]
    public interface IMemoryCacheService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/Contains", ReplyAction="http://tempuri.org/IMemoryCacheService/ContainsResponse")]
        bool Contains(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/SetDirty", ReplyAction="http://tempuri.org/IMemoryCacheService/SetDirtyResponse")]
        void SetDirty();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/GetRawFingerCollection", ReplyAction="http://tempuri.org/IMemoryCacheService/GetRawFingerCollectionResponse")]
        byte[] GetRawFingerCollection(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/GetQualityFingerCollection", ReplyAction="http://tempuri.org/IMemoryCacheService/GetQualityFingerCollectionResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(ImageHandler.CacheService.WsqImage))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(ImageHandler.CacheService.FingerPrintDataContract))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ArrayList))]
        System.Collections.ArrayList GetQualityFingerCollection(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/GetPicture", ReplyAction="http://tempuri.org/IMemoryCacheService/GetPictureResponse")]
        byte[] GetPicture(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMemoryCacheService/Put", ReplyAction="http://tempuri.org/IMemoryCacheService/PutResponse")]
        void Put(ImageHandler.CacheService.FingerPrintDataContract fingersCollectionDataContract);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMemoryCacheServiceChannel : ImageHandler.CacheService.IMemoryCacheService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MemoryCacheServiceClient : System.ServiceModel.ClientBase<ImageHandler.CacheService.IMemoryCacheService>, ImageHandler.CacheService.IMemoryCacheService {
        
        public MemoryCacheServiceClient() {
        }
        
        public MemoryCacheServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MemoryCacheServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MemoryCacheServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MemoryCacheServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool Contains(string id) {
            return base.Channel.Contains(id);
        }
        
        public void SetDirty() {
            base.Channel.SetDirty();
        }
        
        public byte[] GetRawFingerCollection(string id) {
            return base.Channel.GetRawFingerCollection(id);
        }
        
        public System.Collections.ArrayList GetQualityFingerCollection(string id) {
            return base.Channel.GetQualityFingerCollection(id);
        }
        
        public byte[] GetPicture(string id) {
            return base.Channel.GetPicture(id);
        }
        
        public void Put(ImageHandler.CacheService.FingerPrintDataContract fingersCollectionDataContract) {
            base.Channel.Put(fingersCollectionDataContract);
        }
    }
}
