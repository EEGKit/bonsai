﻿using OpenTK;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Shaders
{
    /// <summary>
    /// Represents an operator that creates a 3x3 matrix containing 3D rotation
    /// and scale.
    /// </summary>
    [Description("Creates a 3x3 matrix containing 3D rotation and scale.")]
    public class CreateMatrix3 : Source<Matrix3>
    {
        /// <summary>
        /// Gets or sets the top row of the matrix.
        /// </summary>
        [TypeConverter(typeof(NumericRecordConverter))]
        [Description("The top row of the matrix.")]
        public Vector3 Row0 { get; set; }

        /// <summary>
        /// Gets or sets the second row of the matrix.
        /// </summary>
        [TypeConverter(typeof(NumericRecordConverter))]
        [Description("The second row of the matrix.")]
        public Vector3 Row1 { get; set; }

        /// <summary>
        /// Gets or sets the bottom row of the matrix.
        /// </summary>
        [TypeConverter(typeof(NumericRecordConverter))]
        [Description("The bottom row of the matrix.")]
        public Vector3 Row2 { get; set; }

        /// <summary>
        /// Generates an observable sequence that contains a single 3x3 matrix
        /// with the specified rows.
        /// </summary>
        /// <returns>
        /// A sequence containing the created <see cref="Matrix3"/> object.
        /// </returns>
        public override IObservable<Matrix3> Generate()
        {
            return Observable.Defer(() => Observable.Return(new Matrix3(Row0, Row1, Row2)));
        }

        /// <summary>
        /// Generates an observable sequence of 3x3 matrices with the specified
        /// rows, and where each <see cref="Matrix3"/> object is emitted only
        /// when an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting new matrices.
        /// </param>
        /// <returns>
        /// The sequence of created <see cref="Matrix3"/> values.
        /// </returns>
        public IObservable<Matrix3> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(input => new Matrix3(Row0, Row1, Row2));
        }
    }
}
