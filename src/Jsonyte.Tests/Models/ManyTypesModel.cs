﻿using System;
using System.Text.Json.Serialization;
using Jsonyte.Tests.Converters;

namespace Jsonyte.Tests.Models
{
    public class ManyTypesModel
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string StringValue { get; set; }

        public int IntValue { get; set; }

        public uint UintValue { get; set; }

        public short ShortValue { get; set; }

        public ushort UshortValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public Guid GuidValue { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan TimeSpanValue { get; set; }

        public byte ByteValue { get; set; }

        public char CharValue { get; set; }

        public long LongValue { get; set; }

        public ulong UlongValue { get; set; }
    }
}
