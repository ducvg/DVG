using System;
using System.Runtime.CompilerServices;

namespace DVG.Timers
{
	internal struct SparseIndex
	{
		public int Index;
		public int Version;

		public SparseIndex(int index, int version)
		{
			Index = index;
			Version = version;
		}
	}
	
	internal sealed class SparseIndexMap
	{
		internal struct DenseIndex
		{
			public int NextFreeSparse;
			public int DataIndex;
			public int Version;
		}

		private DenseIndex[] _denseIndexLookup; //accept sparse index, gives data index
		private SparseIndex[] _sparseIndexLookup; //accept data index, gives sparse index 

		private int _freeSparseIndex = -1;

		public int Capacity => _denseIndexLookup.Length;

		public SparseIndexMap(int initialCapacity = 32)
		{
			EnsureCapacity(initialCapacity);
		}

		public void EnsureCapacity(int capacity)
		{
			int prevLength;
			int newLength;

			if (_denseIndexLookup == null)
			{
				prevLength = 0;
				newLength = capacity;

				_denseIndexLookup = new DenseIndex[newLength];
				_sparseIndexLookup = new SparseIndex[newLength];
			}
			else
			{
				prevLength = _denseIndexLookup.Length;

				if (prevLength >= capacity)
				{
					return;
				}

				newLength = prevLength;

				while (newLength < capacity)
				{
					newLength *= 2;
				}

				Array.Resize(ref _denseIndexLookup, newLength);
				Array.Resize(ref _sparseIndexLookup, newLength);
			}

			var span = _denseIndexLookup.AsSpan(prevLength);

			for (int i = 0; i < span.Length; i++)
			{
				int index = prevLength + i;

				span[i] = new DenseIndex()
				{
					NextFreeSparse = index == newLength - 1
						? _freeSparseIndex
						: index + 1,

					DataIndex = -1,
					Version = 1
				};
			}

			_freeSparseIndex = prevLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CreateDataAt(int denseIndex, out SparseIndex sparseIndex)
		{
			if (_freeSparseIndex == -1)
			{
				EnsureCapacity(Capacity * 2);
			}

			ref DenseIndex newDenseIndex = ref _denseIndexLookup[_freeSparseIndex];

			int prevFreeSparseIndex = _freeSparseIndex;
			_freeSparseIndex = newDenseIndex.NextFreeSparse;

			newDenseIndex.NextFreeSparse = -1;
			newDenseIndex.DataIndex = denseIndex;

			if (newDenseIndex.Version == 0)
			{
				newDenseIndex.Version = 1;
			}

			sparseIndex = new SparseIndex(prevFreeSparseIndex, newDenseIndex.Version);

			_sparseIndexLookup[denseIndex] = sparseIndex; //point back to newDense
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseIndex GetSparseIndex(int denseIndex)
		{
			return _sparseIndexLookup[denseIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref DenseIndex GetDenseRefUnchecked(int sparseIndex)
		{
			return ref _denseIndexLookup[sparseIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MoveDenseWithParse(int from, int to)
		{
			if (from == to) return;
			
			var sparseIndex = _sparseIndexLookup[from];
			_sparseIndexLookup[from] = default;
			_sparseIndexLookup[to] = sparseIndex;

			if (sparseIndex.Version != 0)
			{
				ref DenseIndex dense = ref _denseIndexLookup[sparseIndex.Index];
				dense.DataIndex = to;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FreeDense(int denseIndex)
		{
			var sparseIndex = _sparseIndexLookup[denseIndex];
			if (sparseIndex.Version == 0) return;

			ref DenseIndex Dense = ref _denseIndexLookup[sparseIndex.Index];

			Dense.NextFreeSparse = _freeSparseIndex;
			Dense.Version++;

			_freeSparseIndex = sparseIndex.Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			for (int i = 0; i < _denseIndexLookup.Length; i++)
			{
				_denseIndexLookup[i] = new DenseIndex()
				{
					NextFreeSparse = i + 1,
					DataIndex = -1,
					Version = 1
				};

				_sparseIndexLookup[i] = default;
			}

			_freeSparseIndex = 0;
			_denseIndexLookup[_denseIndexLookup.Length - 1].NextFreeSparse = -1;
		}
	}
}