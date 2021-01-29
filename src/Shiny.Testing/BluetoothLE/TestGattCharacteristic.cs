﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Shiny.BluetoothLE;


namespace Shiny.Testing.BluetoothLE
{
    public class TestGattCharacteristic : IGattCharacteristic
    {
        public TestGattCharacteristic(string uuid, IGattService service, CharacteristicProperties? props = null)
        {
            this.Uuid = uuid;
            this.Service = service;
            this.Properties = props ?? CharacteristicProperties.Notify | CharacteristicProperties.Write | CharacteristicProperties.Read;
        }


        public IGattService Service { get; }
        public string Uuid { get; }
        public CharacteristicProperties Properties { get; }
        public bool IsNotifying { get; private set; }
        public List<IGattDescriptor> Descriptors { get; set; } = new List<IGattDescriptor>();
        public IObservable<IGattDescriptor> DiscoverDescriptors() => this.Descriptors.ToObservable();


        public IObservable<Unit> EnableNotifications(bool enable, bool useIndicationsIfAvailable)
        {
            this.IsNotifying = enable;
            return Observable.Return(Unit.Default);
        }


        public Subject<byte[]> NotificationSubject { get; } = new Subject<byte[]>();
        public IObservable<CharacteristicGattResult> WhenNotificationReceived()
            => this.NotificationSubject.Select(data => new CharacteristicGattResult(this, data, CharacteristicResultType.Notification));


        public byte[] ReadValue { get; set; }
        public IObservable<CharacteristicGattResult> Read()
            => Observable.Return(new CharacteristicGattResult(this, this.ReadValue, CharacteristicResultType.Read));


        public byte[] LastWriteValue { get; private set; }
        public IObservable<CharacteristicGattResult> Write(byte[] value, bool withResponse = true)
        {
            if (value is null)
                throw new ArgumentException("Write value cannot be null", nameof(value));

            if (value.Length > this.Service.Peripheral.MtuSize)
                throw new ArgumentException($"Write length of $'{value.Length}' exceeds MTU size '{this.Service.Peripheral.MtuSize}'");

            this.LastWriteValue = value;
            var type = withResponse ? CharacteristicResultType.Write : CharacteristicResultType.WriteWithoutResponse;
            return Observable.Return(new CharacteristicGattResult(this, value, type));
        }
    }
}
