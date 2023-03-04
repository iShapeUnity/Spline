using System;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public struct Contour {

        public NativeArray<Spline> splines;
        public readonly float length;
        private NativeArray<float> lengths;

        public Contour(NativeArray<Anchor> anchors, bool isClosed, int countPerSpline, Allocator allocator) {
            int n = anchors.Length;
            int m = isClosed ? n : n - 1;
            splines = new NativeArray<Spline>(m, allocator);
            lengths = new NativeArray<float>(m, allocator);
            float len = 0f;
            for (int i = 0; i < m; i++) {
                int j = (i + 1) % n;
                var spline = SplineBuilder.Create(anchors[i], anchors[j]);
                splines[i] = spline;
                float l = spline.Length(countPerSpline);
                lengths[i] = l;
                len += l;
            }

            this.length = len;
        }

        public void Dispose() {
            splines.Dispose();
            lengths.Dispose();
        }

        public NativeArray<float2> GetPoints(float step, float2 pos, Allocator allocator) {
            int n = splines.Length;

            int count = 1;
            for (int i = 0; i < n; i++) {
                float dl = lengths[i];
                int m = (int)(dl / step + 0.5f);
                m = Math.Max(1, m);
                count += m;
            }
            
            var result = new NativeArray<float2>(count, allocator);
            int index = 0;
            
            for (int i = 0; i < n; i++) {
                var sp = splines[i];
                float dl = lengths[i];

                int m = (int)(dl / step + 0.5f);
                m = Math.Max(1, m);

                float s = 1f / m;
                float t = 0;
                for (int j = 0; j < m; j++) {
                    result[index++] = sp.Point(t) + pos; 
                    t += s;
                }
            }
            
            result[index] = splines[n - 1].Point(1) + pos;

            return result;
        }
    }

}