using System;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public readonly struct Contour {

        private readonly Spline[] splines;
        private readonly float[] lengths;

        public Contour(NativeArray<Anchor> anchors, bool isClosed, int stepCount = 20) {
            int n = anchors.Length;
            int m = isClosed ? n : n - 1;
            splines = new Spline[m];
            lengths = new float[m];
            for (int i = 0; i < m; i++) {
                int j = (i + 1) % n;
                var spline = SplineBuilder.Create(anchors[i], anchors[j]);
                splines[i] = spline;
                lengths[i] = spline.Length(stepCount);
            }
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