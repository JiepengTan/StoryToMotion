﻿// MIT License

// Copyright (c) 2018 Felix Lange 

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecordAndRepeat
{
    public abstract class RecordingBase : ScriptableObject
    {
        [HideInInspector]
        public float duration = 0;

        private bool dirtyFlag = true;
        private List<IDataFrame> copiedDataFrames = null;
        public List<IDataFrame> DataFrames
        {
            get
            {
                UpdateStoredDataCopy();
                return copiedDataFrames;
            }
        }

        public abstract void Add(IDataFrame frame);
        public abstract int Count();
        protected abstract IEnumerable<IDataFrame> ConvertDataFrames();

        public IDataFrame GetDataFrame(float timeS)
        {
            UpdateStoredDataCopy();

            IDataFrame data = copiedDataFrames.FindLast(x => x.Time <= timeS);
            return data;
        }

        public void Log()
        {
            UpdateStoredDataCopy();

            Debug.Log(String.Format("{0} - {1} seconds - {2} samples", name, duration, copiedDataFrames.Count));
            copiedDataFrames.ForEach(frame => Debug.Log(frame));
        }

        public void OnValidate()
        {
            dirtyFlag = true;
        }

        private void UpdateStoredDataCopy()
        {
            if (dirtyFlag || copiedDataFrames == null)
            {
                // Debug.Log($"Update copied data: {this.name}");
                copiedDataFrames = ConvertDataFrames().ToList();
                dirtyFlag = false;
            }
        }
    }
}