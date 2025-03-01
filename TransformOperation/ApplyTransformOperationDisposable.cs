using System;
using UnityEngine;
namespace BB
{
	public readonly struct ApplyTransformOperationDisposable : IDisposable
	{
		readonly TransformOperation _operation;
		readonly Transform _transform;
		public ApplyTransformOperationDisposable(TransformOperation operation, Transform transform)
		{
			_operation = operation;
			_transform = transform;
		}
		public void Dispose()
		{
			_operation.Apply(_transform);
		}
	}
}