using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace DVG.Timers
{
	internal sealed class TimerDataStorage
	{
		private readonly SparseIndexMap _sparseIndexMap;

		private TimerData[] _unmanagedDataArray;
		private TimerManagedData[] _managedDataArray;

		internal int Count { get; private set; }

		internal TimerDataStorage(int initialCapacity)
		{
			_sparseIndexMap = new SparseIndexMap(initialCapacity);

			_unmanagedDataArray = new TimerData[initialCapacity];
			_managedDataArray = new TimerManagedData[initialCapacity];

			Count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int minimumCapacity)
		{
			_sparseIndexMap.EnsureCapacity(minimumCapacity);

			ArrayHelper.EnsureCapacity(ref _unmanagedDataArray, minimumCapacity);
			ArrayHelper.EnsureCapacity(ref _managedDataArray, minimumCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Timer Create(ref TimerData data, ref TimerManagedData managedData)
		{
			EnsureCapacity(Count + 1);

			_unmanagedDataArray[Count] = data;
			_managedDataArray[Count] = managedData;

			_sparseIndexMap.CreateDataAt(Count, out SparseIndex sparseIndex);

			++Count;

			return new Timer(sparseIndex.Version, sparseIndex.Index, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TimerData GetDataRef(Timer timer)
		{
			ref var denseIndex = ref _sparseIndexMap.GetDenseRefUnchecked(timer.DataSparseIndex);
			if(timer.Version != denseIndex.Version)
			{
				throw new InvalidOperationException("Timer expired, consider having \"preserve: true\".");
			}
			return ref _unmanagedDataArray[denseIndex.DataIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TimerManagedData GetManagedDataRef(Timer timer)
		{
			ref var denseIndex = ref _sparseIndexMap.GetDenseRefUnchecked(timer.DataSparseIndex);
			if(timer.Version != denseIndex.Version)
			{
				throw new InvalidOperationException("Timer expired, consider having \"preserve: true\".");
			}
			return ref _managedDataArray[denseIndex.DataIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveAtDenseIndex(int denseIndex)
		{
			--Count;

			_unmanagedDataArray[denseIndex] = _unmanagedDataArray[Count];
			_unmanagedDataArray[Count] = default;
			_managedDataArray[denseIndex] = _managedDataArray[Count];
			_managedDataArray[Count] = default;

			_sparseIndexMap.MoveDenseWithParse(Count, denseIndex);

			_sparseIndexMap.FreeDense(Count);
		}

		public void RemoveAll(NativeList<int> denseIndexList)
		{
			Span<SparseIndex> span = stackalloc SparseIndex[denseIndexList.Length];
			for (int i = 0; i < span.Length; ++i)
			{
				span[i] = _sparseIndexMap.GetSparseIndex(denseIndexList[i]);
			}

			for (int i = 0; i < span.Length; ++i)
			{
				ref var denseIndex = ref _sparseIndexMap.GetDenseRefUnchecked(span[i].Index);
				RemoveAtDenseIndex(denseIndex.DataIndex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<TimerData> GetDataSpan() => _unmanagedDataArray.AsSpan(0, Count);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<TimerManagedData> GetManagedDataSpan() => _managedDataArray.AsSpan(0, Count);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TimerData[] GetDataArray() => _unmanagedDataArray;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TimerManagedData[] GetManagedDataArray() => _managedDataArray;

		public void Clear()
		{
			_sparseIndexMap.Reset();
			_unmanagedDataArray.AsSpan().Clear();
			_managedDataArray.AsSpan().Clear();

			Count = 0;
		}
	}

	internal static class ArrayHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCapacity<T>(ref T[] array, int minimumCapacity)
        {
            if (array == null)
            {
                array = new T[minimumCapacity];
				return;
            }

			var l = array.Length;
			if (l >= minimumCapacity) return;

			while (l < minimumCapacity)
			{
				l *= 2;
			}

			Array.Resize(ref array, l);
        }
	}
}