﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FrameWork
{
    public interface IObjectDatabase
    {
        bool AddObject(DataObject dataObject);

        void SaveObject(DataObject dataObject);

        void DeleteObject(DataObject dataObject);

        TObject FindObjectByKey<TObject>(object key)
            where TObject : DataObject;

        TObject SelectObject<TObject>(string whereExpression)
            where TObject : DataObject;

        TObject SelectObject<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject;

        IList<TObject> SelectObjects<TObject>(string whereExpression)
            where TObject : DataObject;

        IList<TObject> SelectObjects<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject;

        IList<TObject> SelectAllObjects<TObject>()
            where TObject : DataObject;

        IList<TObject> SelectAllObjects<TObject>(IsolationLevel isolation)
            where TObject : DataObject;

        int GetObjectCount<TObject>()
            where TObject : DataObject;

        int GetObjectCount<TObject>(string whereExpression)
            where TObject : DataObject;

        void RegisterDataObject(Type dataObjectType);

        bool UpdateInCache<TObject>(object key)
            where TObject : DataObject;

        void FillObjectRelations(DataObject dataObject);

        string Escape(string rawInput);

        bool ExecuteNonQuery(string rawQuery);
    }
}
