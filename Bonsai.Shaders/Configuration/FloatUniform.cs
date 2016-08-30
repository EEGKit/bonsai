﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Shaders.Configuration
{
    public class FloatUniform : UniformConfiguration
    {
        [Description("The value used to initialize the uniform variable.")]
        public float Value { get; set; }

        internal override void SetUniform(int location)
        {
            GL.Uniform1(location, Value);
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", string.IsNullOrEmpty(Name) ? "Float" : Name, Value);
        }
    }
}