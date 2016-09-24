// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Elmah;
//using Microsoft.WindowsAzure.ServiceRuntime;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;

namespace NuGetGallery.Infrastructure
{
    public interface ITableEntity
    {
    }

    public class EntityProperty
    {

        public string StringValue { get; set; }
    }

    public class ErrorEntity // : ITableEntity
    {
        public Error Error { get; set; }

        //ITableEntity.
        string ETag
        {
            get;
            set;
        }

        string PartitionKey
        {
            get;
            set;
        }

        string RowKey
        {
            get;
            set;
        }

        DateTimeOffset Timestamp
        {
            get;
            set;
        }

        //public long LogicalIndex
        //{
        //    get
        //    {
        //        return AzureEntityList<ErrorEntity>.GetLogicalIndex(this);
        //    }
        //}

        public ErrorEntity() { }

        public ErrorEntity(Error error)
        {
            Error = error;
        }

        void ReadEntity(IDictionary<string, EntityProperty> properties, Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            // This can occasionally fail because someone didn't finish creating the entity yet.

            EntityProperty value;
            if (properties.TryGetValue("SerializedError", out value))
            {
                Error = ErrorXml.DecodeString(value.StringValue);
            }
            else
            {
                Error = new Error
                {
                    ApplicationName = "TableErrorLog",
                    StatusCode = 999,
                    HostName = Environment.MachineName,
                    Time = DateTime.UtcNow,
                    Type = typeof(Exception).FullName,
                    Detail = "Error Log Entry is Corrupted/Missing in Table Store"
                };

                return;
            }

            if (properties.TryGetValue("Detail", out value))
            {
                Error.Detail = value.StringValue;
            }

            if (properties.TryGetValue("WebHostHtmlMessage", out value))
            {
                Error.WebHostHtmlMessage = value.StringValue;
            }
        }

        IDictionary<string, object> // EntityProperty> 
            WriteEntity(object // OperationContext 
                operationContext)
        {
            // Table storage has a limitation on property lengths - 64KiB.
            // Strings will be encoded as UTF-16, apparently?

            const int MaxChars = 32 * 1000;

            var detail = Error.Detail;
            if (detail.Length > MaxChars)
            {
                detail = detail.Substring(0, MaxChars);
            }

            var htmlMessage = Error.WebHostHtmlMessage;
            if (htmlMessage.Length > MaxChars)
            {
                htmlMessage = htmlMessage.Substring(0, MaxChars);
            }

            Error.Detail = null;
            Error.WebHostHtmlMessage = null;
            string serializedError = ErrorXml.EncodeString(Error);

            if (serializedError.Length > MaxChars)
            {
                serializedError = ErrorXml.EncodeString(
                    new Error
                    {
                        ApplicationName = "TableErrorLog",
                        StatusCode = 888,
                        HostName = Environment.MachineName,
                        Time = DateTime.UtcNow,
                        Detail = "Error Log Entry Will Not Fit In Table Store: " + serializedError.Substring(0, 4000)
                    });
            }

            return new Dictionary<string, object> // EntityProperty>
                {
                    { "SerializedError", null }    // EntityProperty.GeneratePropertyForString(serializedError) },
                    //{ "Detail", EntityProperty.GeneratePropertyForString(detail) },
                    //{ "WebHostHtmlMessage", EntityProperty.GeneratePropertyForString(htmlMessage) },
                };
        }
    }

    //public class ErrorEntity : ITableEntity
    //{
    //    public Exception Error { get; set; }
    //}

    public class TableErrorLog : ErrorLog
    {
        public const string TableName = "ElmahErrors";

        private readonly string _connectionString;
        private readonly AzureEntityList<ErrorEntity> _entityList;

        public TableErrorLog(IDictionary config)
        {
            _connectionString = (string)config["connectionString"]
                ; // ?? RoleEnvironment.GetConfigurationSettingValue((string)config["connectionStringName"]);
            _entityList = new AzureEntityList<ErrorEntity>(_connectionString, TableName);
        }

        public TableErrorLog(string connectionString)
        {
            _connectionString = connectionString;
            _entityList = new AzureEntityList<ErrorEntity>(connectionString, TableName);
        }

        public TableErrorLog()
        {
            _entityList = new AzureEntityList<ErrorEntity>(_connectionString, TableName);
        }

        public override ErrorLogEntry GetError(string id)
        {
            long pos = Int64.Parse(id, CultureInfo.InvariantCulture);
            var error = _entityList[pos];
            Debug.Assert(id == pos.ToString(CultureInfo.InvariantCulture));
            return new ErrorLogEntry(this, id, error.Error);
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            // A little math is required since the AzureEntityList is in ascending order
            // And we want to retrieve entries in descending order
            long queryOffset = _entityList.LongCount - ((pageIndex + 1) * pageSize);
            if (queryOffset < 0)
            {
                pageSize += (int)queryOffset;
                queryOffset = 0;
            }

            // And since that range was in ascending, flip it to descending.
            //var results = _entityList.GetRange(queryOffset, pageSize).Reverse();
            //foreach (var error in results)
            //{
            //    string id = error.LogicalIndex.ToString(CultureInfo.InvariantCulture);
            //    errorEntryList.Add(new ErrorLogEntry(this, id, error.Error));
            //}

            return 0; // _entityList.Count;
        }

        public override string Log(Error error)
        {
            var entity = new ErrorEntity(error);
            long pos = _entityList.Add(entity);
            return pos.ToString(CultureInfo.InvariantCulture);
        }
    }
}