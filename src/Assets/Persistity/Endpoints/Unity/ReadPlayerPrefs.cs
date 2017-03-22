﻿using System;
using System.Text;
using UnityEngine;

namespace Persistity.Endpoints.Unity
{
    public class ReadPlayerPrefs : IReceiveData
    {
        public string KeyName { get; set; }

        public ReadPlayerPrefs(string keyName)
        {
            KeyName = keyName;
        }

        public void Execute(Action<byte[]> onSuccess, Action<Exception> onError)
        {
            try
            {
                var data = PlayerPrefs.GetString(KeyName);
                var byteData = Encoding.Default.GetBytes(data);
                onSuccess(byteData);
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }
    }
}