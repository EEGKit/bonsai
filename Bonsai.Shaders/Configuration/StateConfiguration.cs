﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bonsai.Shaders.Configuration
{
    [XmlInclude(typeof(EnableState))]
    [XmlInclude(typeof(DisableState))]
    [XmlInclude(typeof(ViewportState))]
    [XmlInclude(typeof(LineWidthState))]
    [XmlInclude(typeof(PointSizeState))]
    [XmlInclude(typeof(DepthMaskState))]
    [XmlInclude(typeof(BlendFunctionState))]
    [XmlInclude(typeof(DepthFunctionState))]
    [XmlInclude(typeof(MemoryBarrierState))]
    public abstract class StateConfiguration
    {
        public abstract void Execute(ShaderWindow window);
    }
}
