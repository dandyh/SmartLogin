﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPFLogin
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="SmarterHomeDB")]
	public partial class UserTableDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    #endregion
		
		public UserTableDataContext() : 
				base(global::WPFLogin.Properties.Settings.Default.SmarterHomeDBConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public UserTableDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserTableDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserTableDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserTableDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<User> Users
		{
			get
			{
				return this.GetTable<User>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[User]")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _username;
		
		private string _password;
		
		private string _name;
		
		private string _photofile;
		
		private string _age;
		
		private string _gender;
		
		private string _speakerguid;
		
		private string _speakerphrase;
		
		private string _familyid;
		
		private System.Nullable<int> _defaulthometemprature;
		
		private string _faceguid;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnusernameChanging(string value);
    partial void OnusernameChanged();
    partial void OnpasswordChanging(string value);
    partial void OnpasswordChanged();
    partial void OnnameChanging(string value);
    partial void OnnameChanged();
    partial void OnphotofileChanging(string value);
    partial void OnphotofileChanged();
    partial void OnageChanging(string value);
    partial void OnageChanged();
    partial void OngenderChanging(string value);
    partial void OngenderChanged();
    partial void OnspeakerguidChanging(string value);
    partial void OnspeakerguidChanged();
    partial void OnspeakerphraseChanging(string value);
    partial void OnspeakerphraseChanged();
    partial void OnfamilyidChanging(string value);
    partial void OnfamilyidChanged();
    partial void OndefaulthometempratureChanging(System.Nullable<int> value);
    partial void OndefaulthometempratureChanged();
    partial void OnfaceguidChanging(string value);
    partial void OnfaceguidChanged();
    #endregion
		
		public User()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_username", DbType="VarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string username
		{
			get
			{
				return this._username;
			}
			set
			{
				if ((this._username != value))
				{
					this.OnusernameChanging(value);
					this.SendPropertyChanging();
					this._username = value;
					this.SendPropertyChanged("username");
					this.OnusernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_password", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if ((this._password != value))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="VarChar(50)")]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if ((this._name != value))
				{
					this.OnnameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("name");
					this.OnnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_photofile", DbType="VarChar(MAX)")]
		public string photofile
		{
			get
			{
				return this._photofile;
			}
			set
			{
				if ((this._photofile != value))
				{
					this.OnphotofileChanging(value);
					this.SendPropertyChanging();
					this._photofile = value;
					this.SendPropertyChanged("photofile");
					this.OnphotofileChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_age", DbType="VarChar(50)")]
		public string age
		{
			get
			{
				return this._age;
			}
			set
			{
				if ((this._age != value))
				{
					this.OnageChanging(value);
					this.SendPropertyChanging();
					this._age = value;
					this.SendPropertyChanged("age");
					this.OnageChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_gender", DbType="VarChar(10)")]
		public string gender
		{
			get
			{
				return this._gender;
			}
			set
			{
				if ((this._gender != value))
				{
					this.OngenderChanging(value);
					this.SendPropertyChanging();
					this._gender = value;
					this.SendPropertyChanged("gender");
					this.OngenderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_speakerguid", DbType="VarChar(50)")]
		public string speakerguid
		{
			get
			{
				return this._speakerguid;
			}
			set
			{
				if ((this._speakerguid != value))
				{
					this.OnspeakerguidChanging(value);
					this.SendPropertyChanging();
					this._speakerguid = value;
					this.SendPropertyChanged("speakerguid");
					this.OnspeakerguidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_speakerphrase", DbType="VarChar(MAX)")]
		public string speakerphrase
		{
			get
			{
				return this._speakerphrase;
			}
			set
			{
				if ((this._speakerphrase != value))
				{
					this.OnspeakerphraseChanging(value);
					this.SendPropertyChanging();
					this._speakerphrase = value;
					this.SendPropertyChanged("speakerphrase");
					this.OnspeakerphraseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_familyid", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string familyid
		{
			get
			{
				return this._familyid;
			}
			set
			{
				if ((this._familyid != value))
				{
					this.OnfamilyidChanging(value);
					this.SendPropertyChanging();
					this._familyid = value;
					this.SendPropertyChanged("familyid");
					this.OnfamilyidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_defaulthometemprature", DbType="Int")]
		public System.Nullable<int> defaulthometemprature
		{
			get
			{
				return this._defaulthometemprature;
			}
			set
			{
				if ((this._defaulthometemprature != value))
				{
					this.OndefaulthometempratureChanging(value);
					this.SendPropertyChanging();
					this._defaulthometemprature = value;
					this.SendPropertyChanged("defaulthometemprature");
					this.OndefaulthometempratureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_faceguid", DbType="VarChar(50)")]
		public string faceguid
		{
			get
			{
				return this._faceguid;
			}
			set
			{
				if ((this._faceguid != value))
				{
					this.OnfaceguidChanging(value);
					this.SendPropertyChanging();
					this._faceguid = value;
					this.SendPropertyChanged("faceguid");
					this.OnfaceguidChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
