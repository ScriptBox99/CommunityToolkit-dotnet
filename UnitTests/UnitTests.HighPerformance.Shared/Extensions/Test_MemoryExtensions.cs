﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_MemoryExtensions
    {
        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_Empty()
        {
            Memory<byte> m1 = default;
            Memory<byte> mc1 = m1.Cast<byte, byte>();

            Assert.IsTrue(mc1.IsEmpty);

            Memory<byte> m2 = default;
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.IsTrue(mc2.IsEmpty);

            Memory<short> m3 = default;
            Memory<Guid> mc3 = m3.Cast<short, Guid>();

            Assert.IsTrue(mc3.IsEmpty);

            Memory<byte> m4 = new byte[12].AsMemory(12);
            Memory<int> mc4 = m4.Cast<byte, int>();

            Assert.IsTrue(mc4.IsEmpty);

            Memory<byte> m5 = new byte[12].AsMemory().Slice(4).Slice(8);
            Memory<int> mc5 = m5.Cast<byte, int>();

            Assert.IsTrue(mc5.IsEmpty);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_TooShort()
        {
            Memory<byte> m1 = new byte[3];
            Memory<int> mc1 = m1.Cast<byte, int>();

            Assert.IsTrue(mc1.IsEmpty);

            Memory<byte> m2 = new byte[13];
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.AreEqual(mc2.Length, 3);

            Memory<byte> m3 = new byte[16].AsMemory(5);
            Memory<float> mc3 = m3.Cast<byte, float>();

            Assert.AreEqual(mc3.Length, 2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByte()
        {
            Memory<byte> memoryOfBytes = new byte[128];
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToByte()
        {
            Memory<float> memoryOfFloats = new float[128];
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToShort()
        {
            Memory<float> memoryOfFloats = new float[128];
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByteAndBack()
        {
            var data = new byte[128];
            Memory<byte> memoryOfBytes = data;
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            Assert.IsTrue(MemoryMarshal.TryGetArray<byte>(memoryBack, out var segment));
            Assert.AreSame(segment.Array!, data);
            Assert.AreEqual(segment.Offset, 0);
            Assert.AreEqual(segment.Count, data.Length);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_TooShort_WithSlice()
        {
            Memory<byte> m1 = new byte[8].AsMemory().Slice(4, 3);
            Memory<int> mc1 = m1.Cast<byte, int>();

            Assert.IsTrue(mc1.IsEmpty);

            Memory<byte> m2 = new byte[20].AsMemory().Slice(4, 13);
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.AreEqual(mc2.Length, 3);

            Memory<byte> m3 = new byte[16].AsMemory().Slice(5);
            Memory<float> mc3 = m3.Cast<byte, float>();

            Assert.AreEqual(mc3.Length, 2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByte_WithSlice()
        {
            Memory<byte> memoryOfBytes = new byte[512].AsMemory().Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToByte_WithSlice()
        {
            Memory<float> memoryOfFloats = new float[512].AsMemory().Slice(128, 128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToShort_WithSlice()
        {
            Memory<float> memoryOfFloats = new float[512].AsMemory().Slice(128, 128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByteAndBack_WithSlice()
        {
            var data = new byte[512];
            Memory<byte> memoryOfBytes = data.AsMemory().Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            Assert.IsTrue(MemoryMarshal.TryGetArray<byte>(memoryBack, out var segment));
            Assert.AreSame(segment.Array!, data);
            Assert.AreEqual(segment.Offset, 128);
            Assert.AreEqual(segment.Count, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByte()
        {
            Memory<byte> memoryOfBytes = new ArrayMemoryManager<byte>(128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToByte()
        {
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToShort()
        {
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByteAndBack()
        {
            var data = new ArrayMemoryManager<byte>(128);
            Memory<byte> memoryOfBytes = data;
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            Assert.IsTrue(MemoryMarshal.TryGetMemoryManager<byte, ArrayMemoryManager<byte>>(memoryBack, out var manager, out var start, out var length));
            Assert.AreSame(manager!, data);
            Assert.AreEqual(start, 0);
            Assert.AreEqual(length, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByte_WithSlice()
        {
            Memory<byte> memoryOfBytes = new ArrayMemoryManager<byte>(512).Memory.Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToByte_WithSlice()
        {
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(512).Memory.Slice(128, 128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToShort_WithSlice()
        {
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(512).Memory.Slice(128, 128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByteAndBack_WithSlice()
        {
            var data = new ArrayMemoryManager<byte>(512);
            Memory<byte> memoryOfBytes = data.Memory.Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            Assert.IsTrue(MemoryMarshal.TryGetMemoryManager<byte, ArrayMemoryManager<byte>>(memoryBack, out var manager, out var start, out var length));
            Assert.AreSame(manager!, data);
            Assert.AreEqual(start, 128);
            Assert.AreEqual(length, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        [DataRow(64, 0, 0)]
        [DataRow(64, 4, 0)]
        [DataRow(64, 0, 4)]
        [DataRow(64, 4, 4)]
        [DataRow(64, 4, 0)]
        [DataRow(256, 16, 0)]
        [DataRow(256, 4, 16)]
        [DataRow(256, 64, 0)]
        [DataRow(256, 64, 8)]
        public unsafe void Test_MemoryExtensions_FromArray_CastFromByte_Pin(int size, int preOffset, int postOffset)
        {
            var data = new byte[size];
            Memory<byte> memoryOfBytes = data.AsMemory(preOffset);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>().Slice(postOffset);

            using var handle = memoryOfFloats.Pin();

            void* p1 = handle.Pointer;
            void* p2 = Unsafe.AsPointer(ref data[preOffset + (postOffset * sizeof(float))]);

            Assert.IsTrue(p1 == p2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        [DataRow(64, 0, 0)]
        [DataRow(64, 4, 0)]
        [DataRow(64, 0, 4)]
        [DataRow(64, 4, 4)]
        [DataRow(64, 4, 0)]
        [DataRow(256, 16, 0)]
        [DataRow(256, 4, 16)]
        [DataRow(256, 64, 0)]
        [DataRow(256, 64, 8)]
        public unsafe void Test_MemoryExtensions_FromMemoryManager_CastFromByte_Pin(int size, int preOffset, int postOffset)
        {
            var data = new ArrayMemoryManager<byte>(size);
            Memory<byte> memoryOfBytes = data.Memory.Slice(preOffset);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>().Slice(postOffset);

            using var handle = memoryOfFloats.Pin();

            void* p1 = handle.Pointer;
            void* p2 = Unsafe.AsPointer(ref data.GetSpan()[preOffset + (postOffset * sizeof(float))]);

            Assert.IsTrue(p1 == p2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_EmptyMemoryStream()
        {
            Memory<byte> memory = default;

            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsTrue(stream.CanWrite);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_MemoryStream()
        {
            Memory<byte> memory = new byte[1024];

            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsTrue(stream.CanWrite);
        }

        private sealed class ArrayMemoryManager<T> : MemoryManager<T>
            where T : unmanaged
        {
            private readonly T[] array;

            public ArrayMemoryManager(int size)
            {
                this.array = new T[size];
            }

            public override Span<T> GetSpan()
            {
                return this.array;
            }

            public override unsafe MemoryHandle Pin(int elementIndex = 0)
            {
                GCHandle handle = GCHandle.Alloc(this.array, GCHandleType.Pinned);
                ref T r0 = ref this.array[elementIndex];
                void* p = Unsafe.AsPointer(ref r0);

                return new MemoryHandle(p, handle);
            }

            public override void Unpin()
            {
            }

            protected override void Dispose(bool disposing)
            {
            }

            public static implicit operator Memory<T>(ArrayMemoryManager<T> memoryManager)
            {
                return memoryManager.Memory;
            }
        }
    }
}
