﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Reactive.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml;

namespace Bonsai.Expressions
{
    [XmlType("GateInterval", Namespace = Constants.XmlNamespace)]
    public class GateIntervalBuilder : CombinatorBuilder
    {
        [XmlIgnore]
        public TimeSpan Interval { get; set; }

        [Browsable(false)]
        [XmlElement("Interval")]
        public string IntervalXml
        {
            get { return XmlConvert.ToString(Interval); }
            set { Interval = XmlConvert.ToTimeSpan(value); }
        }

        protected override IObservable<TSource> Combine<TSource>(IObservable<TSource> source)
        {
            return source.Gate(Interval, HighResolutionScheduler.ThreadPool);
        }
    }
}
