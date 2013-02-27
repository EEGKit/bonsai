﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

namespace Bonsai.Design
{
    public abstract class DialogTypeVisualizer
    {
        public abstract void Show(object value);

        public abstract void Load(IServiceProvider provider);

        public abstract void Unload();

        public virtual IObservable<object> Visualize(IObservable<object> source, IServiceProvider provider)
        {
            var visualizerDialog = (TypeVisualizerDialog)provider.GetService(typeof(TypeVisualizerDialog));
            if (visualizerDialog != null)
            {
                return source.ObserveOn(visualizerDialog).Do(Show);
            }

            return source;
        }
    }
}
